const fs = require('fs')
const Web3 = require('web3')
const uuid = require('uuid/v1')
const amqp = require('amqplib/callback_api');
const deployer = require('./deployer')

const abi = JSON.parse(fs.readFileSync('./Contracts/out/Thimble_sol_Thimble.abi', {'encoding':'utf8', 'flag':'r'}));

const $admin_account = "0xed9d02e382b34818e88b88a309c7fe71e65f419d"
var $contract_address = null;

var $MQ_HOST = 'amqp://10.0.75.2';
var $RPC_HOST = 'http://10.0.75.2:22000';

for(var i=2; i < process.argv.length; i++)
{
  var arg = process.argv[i];
  if(arg === "--rpc" && process.argv[i + 1] !== undefined)
  {
    $RPC_HOST = process.argv[i + 1]
  }
  if(arg === "--mq" && process.argv[i + 1] !== undefined)
  {
    $MQ_HOST = process.argv[i + 1]
  }
}

console.log(` [#] Starting Version-Manager with MQ ${$MQ_HOST} and RPC ${$RPC_HOST}`)

var web3 = new Web3(new Web3.providers.HttpProvider($RPC_HOST));

web3.eth.defaultAccount = $admin_account;

findAddress('demo', (address) => {
  $contract_address = address
  if(!$contract_address) {
    console.log(' [*] Contract not yet deployed')
    deployer.deploy(web3, (err, address2) => {
      if(err) {
        console.log(` [!] Error deploying contract ${err.toString()}`)
      }
      else {
        $contract_address = address2
        console.log(' [/] Deplpoyed contract to address ', address2)
      }
    })
  } else {
    console.log(' [/] Found contract address', address)
  }
})

function findAddress (client, cb) {
  web3.personal.unlockAccount($admin_account, "");

  let filter = web3.eth.filter({
    fromBlock: 0,
    toBlock: 'latest',
    topics: [web3.sha3('CreationEvent(string)')]
  })
  
  filter.get((error, results) => {
    let mapped = results.map(r => ({
        address: r.address,
        data: web3.toAscii(r.data.substring(130)).replace(/\u0000/g,'')
      }))
      .filter(r => 
        r.data === client
      )[0]

    cb(mapped ? mapped.address : undefined)
  })
}

var SAVE_RESPONDER = (requestId, message) => {};
var GET_RESPONDER = (requestId, data) => {};
function VERSION_SAVE_REQUEST(err, ch) {
  var ex = 'version_save_request';

  ch.assertExchange(ex, 'direct', { durable: false });
  ch.assertQueue(`VERSION-MANAGER-SAVE-REQUEST-HANDLER-${uuid()}`, { exclusive: true }, function(err, q) {
    if(err) {
      console.error(` [!] Error asserting queue ${err.toString()}`)
    }
    console.log(" [*] Waiting for messages in data_save_request %s.", q.queue);
    ch.bindQueue(q.queue, ex, '');
    ch.consume(q.queue, function(msg) {
      let content = msg.content.toString()
      console.log(" [x] %s", content);
      let request = JSON.parse(content)

      let requestId = request.REQUEST_ID
      let module = request.MODULE.toUpperCase()
      let schema = request.SCHEMA.toUpperCase()
      let name = request.NAME.toUpperCase()
      let documentId = request.DOCUMENT_ID
      let isUpdate = request.IS_UPDATE

      if(!isUpdate) {
        console.log(` [*] Saving new document at ${module}-${schema}-${name}`);
      }
      else {
        console.log(` [*] Saving document at ${module}-${schema}-${name}`);
      }
      
      if(!isUpdate && retriever(module, schema, name)) {
        SAVE_RESPONDER(requestId, { STATUS: "ERROR", ERROR: "NAME_IN_USE" })
        return
      }

      try {
        saver(module, schema, name, documentId)
        SAVE_RESPONDER(requestId, { STATUS: "SUCCESS" })
      }
      catch(e) {
        SAVE_RESPONDER(requestId, { STATUS: "ERROR", ERROR: e.toString() })
      }
    }, { noAck: false });
  });
}
function VERSION_SAVE_RESPONSE(err, ch) {
  var ex = 'version_save_response';
  ch.assertExchange(ex, 'direct', {durable: false});
  SAVE_RESPONDER = (requestId, message) => {
    ch.publish(ex, requestId, new Buffer(JSON.stringify(message)));
    console.log(" [/] Sent to version_save_response message %s", JSON.stringify(message));
  }
}

function VERSION_GET_REQUEST(err, ch) {
  var ex = 'version_get_request';

  ch.assertExchange(ex, 'direct', { durable: false });
  ch.assertQueue(`VERSION-MANAGER-GET-REQUEST-HANDLER-${uuid()}`, { exclusive: true }, function(err, q) {
    if(err) {
      console.error(` [!] Error asserting queue ${err.toString()}`)
    }
    console.log(" [*] Waiting for messages in version_get_request %s.", q.queue);
    ch.bindQueue(q.queue, ex, '');
    ch.consume(q.queue, function(msg) {
      let content = msg.content.toString()
      console.log(" [x] %s", content);
      let request = JSON.parse(content)

      let requestId = request.REQUEST_ID
      let module = request.MODULE.toUpperCase()
      let schema = request.SCHEMA.toUpperCase()
      let name = request.NAME.toUpperCase()
      
      let params = request.PARAMS || {}
      let returnType = params.RETURN_TYPE || 'DOCUMENT_ID' // Default null resolves document id
      let documentId = params.DOCUMENT_ID
      let version = params.VERSION
      let timestamp = params.TIMESTAMP
      
      console.log(` [x] Getting document version at ${module}-${schema}-${name} and returning ${request.RETURN_TYPE ? returnType : 'DEFAULT (DOCUMENT_ID)'}`);
      
      try
      {
        let result = retriever(module, schema, name, returnType, documentId, version, timestamp)
        
        console.log(` [/] Resolved request to ${returnType}: ${result}`);
  
        GET_RESPONDER(requestId, {
          [returnType]: result
        });
      }
      catch(e)
      {
        GET_RESPONDER(requestId, {
          ERROR: e
        });
      }
    }, { noAck: false });
  });
}
function VERSION_GET_RESPONSE(err, ch) {
  var ex = 'version_get_response';
  ch.assertExchange(ex, 'direct', {durable: false});
  GET_RESPONDER = (requestId, data) => {
    ch.publish(ex, requestId, new Buffer(JSON.stringify(data)));
    console.log(" [/] Sent to version_get_response message %s", JSON.stringify(data));
  }
}


function retriever(module, schema, name) {
  return retriever(module, schema, name, null, null, null, null)
}
function retriever(module, schema, name, returnType, documentId, version, timestamp) {
  let contract = web3.eth.contract(abi).at($contract_address)
  web3.personal.unlockAccount($admin_account, "")

  switch((returnType || 'DOCUMENT_ID').toUpperCase())
  {
    case 'VERSION_COUNT': {
      return contract.getDocumentVersionCount(module, schema, name, { gas: 500000 })
    }
    case 'VERSION': {
      if(documentId) {
        return contract.getVersionByDocumentId(module, schema, name, documentId.toUpperCase(), { gas: 500000 })
      } else if(timestamp) {
        return contract.getVersionByTimestamp(module, schema, name, timestamp, { gas: 500000 })
      } else {
        throw 'NEED DOCUMENT_ID OR TIMESTAMP TO RETURN VERSION'
      }
    }
    case 'TIMESTAMP': {
      if(documentId) {
        return contract.getTimestampByDocumentId(module, schema, name, documentId.toUpperCase(), { gas: 500000 })
      } else if(version) {
        return contract.getTimestampByVersion(module, schema, name, version, { gas: 500000 })
      } else {
        throw 'NEED DOCUMENT_ID OR VERSION TO RETURN TIMESTAMP'
      }
    }
    default: {
      if(timestamp) {
        return contract.getDocumentByTimestamp(module, schema, name, timestamp, { gas: 500000 })
      } else if(version) {
        return contract.getTimestampByVersion(module, schema, name, version, { gas: 500000 })
      } else {
        return contract.getDocument(module, schema, name, { gas: 500000 })
      }
    }
  }
}

function saver(module, schema, name, documentId) {
  let contract = web3.eth.contract(abi).at($contract_address)
  web3.personal.unlockAccount($admin_account, "")
  contract.saveDocument(module, schema, name, documentId, { gas: 500000 })
}

var attempts = 0
function connect () {
  amqp.connect($MQ_HOST, function(err, conn) {
    if (err || !conn) {
      console.error('MQ CONNECTION ERROR', err)

      if(attempts < 15) {
        setTimeout(function() {
          attempts ++
          connect()
        }, 2000)
      }
      return;
    }
    console.log(" [*] Connected to AMQP server");
    conn.createChannel(VERSION_SAVE_REQUEST);
    conn.createChannel(VERSION_SAVE_RESPONSE);
    conn.createChannel(VERSION_GET_REQUEST);
    conn.createChannel(VERSION_GET_RESPONSE);
  });
}

try {
  connect()
}
catch(e) {
  console.error("ERROR!!")
}