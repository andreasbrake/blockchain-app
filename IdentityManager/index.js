const fs = require('fs')
const Web3 = require('web3')
const uuid = require('uuid/v1')
const amqp = require('amqplib/callback_api');
const deployer = require('./deployer')

const abi = JSON.parse(fs.readFileSync('./Contracts/out/Vault_sol_Vault.abi', {'encoding':'utf8', 'flag':'r'}));

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

console.log(` [#] Starting Identity-Manager with MQ ${$MQ_HOST} and RPC ${$RPC_HOST}`)

var web3 = new Web3(new Web3.providers.HttpProvider($RPC_HOST));

web3.eth.defaultAccount = $admin_account;

findAddress((address) => {
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

function findAddress (cb) {
  web3.personal.unlockAccount($admin_account, "");

  let filter = web3.eth.filter({
    fromBlock: 0,
    toBlock: 'latest',
    topics: [web3.sha3('VaultCreationEvent(string)')]
  })
  
  filter.get((error, results) => {
    let mapped = results.map(r => r.address)[0]
    cb(mapped)
  })
}

var CREATE_RESPONDER = (requestId, message) => {};
var GET_RESPONDER = (requestId, data) => {};
function IDENTITY_CREATE_REQUEST(err, ch) {
  var ex = 'identity_request';

  ch.assertExchange(ex, 'direct', { durable: false });
  ch.assertQueue(`IDENTITY-MANAGER-CREATE-REQUEST-HANDLER-${uuid()}`, { exclusive: true }, function(err, q) {
    if(err) {
      console.error(` [!] Error asserting queue ${err.toString()}`)
    }
    console.log(" [*] Waiting for messages in identity_request/create %s.", q.queue);

    ch.bindQueue(q.queue, ex, 'create');
    ch.consume(q.queue, function(msg) {
      let content = msg.content.toString()
      console.log(" [x] %s", content);
      let request = JSON.parse(content)

      let requestId = request.REQUEST_ID
      let userId = request.USER_ID
      let publickey = request.PUBLIC_KEY

      console.log(` [*] Registering user vault for ${userId}`);

      if(retriever(userId)) {
        CREATE_RESPONDER(requestId, { STATUS: "ERROR", ERROR: "USER_EXISTS" })
        return
      }
      try {
        creator(userId, publickey)
        CREATE_RESPONDER(requestId, { STATUS: "SUCCESS" })
      }
      catch(e) {
        CREATE_RESPONDER(requestId, { STATUS: "ERROR", ERROR: e.toString() })
      }
    }, { noAck: false });
  });
}
function IDENTITY_CREATE_RESPONSE(err, ch) {
  var ex = 'identity_response';
  ch.assertExchange(ex, 'direct', {durable: false});
  CREATE_RESPONDER = (requestId, message) => {
    ch.publish(ex, requestId, new Buffer(JSON.stringify(message)));
    console.log(" [/] Sent to identity_response message %s", JSON.stringify(message));
  }
}

function IDENTITY_GET_REQUEST(err, ch) {
  var ex = 'identity_request';

  ch.assertExchange(ex, 'direct', { durable: false });
  ch.assertQueue(`IDENTITY-MANAGER-GET-REQUEST-HANDLER-${uuid()}`, { exclusive: true }, function(err, q) {
    if(err) {
      console.error(` [!] Error asserting queue ${err.toString()}`)
    }
    console.log(" [*] Waiting for messages in identity_request/get %s.", q.queue);

    ch.bindQueue(q.queue, ex, 'get');
    ch.consume(q.queue, function(msg) {
      let content = msg.content.toString()
      console.log(" [x] %s", content);
      let request = JSON.parse(content)

      let requestId = request.REQUEST_ID
      let userId = request.USER_ID
      
      console.log(` [x] Getting user key for ${userId}`);
      
      try
      {
        let result = retriever(userId)
        
        console.log(` [/] Resolved key for ${userId}`);
  
        GET_RESPONDER(requestId, {
          STATUS: 'SUCCESS',
          PUBLIC_KEY: result
        });
      }
      catch(e)
      {
        GET_RESPONDER(requestId, {
          STATUS: 'ERROR',
          ERROR: e
        });
      }
    }, { noAck: false });
  });
}
function IDENTITY_GET_RESPONSE(err, ch) {
  var ex = 'identity_response';
  ch.assertExchange(ex, 'direct', {durable: false});
  GET_RESPONDER = (requestId, data) => {
    ch.publish(ex, requestId, new Buffer(JSON.stringify(data)));
    console.log(" [/] Sent to identity_response message %s", JSON.stringify(data));
  }
}


function retriever(userId) {
  let contract = web3.eth.contract(abi).at($contract_address)
  web3.personal.unlockAccount($admin_account, "")
  return contract.resolvePublickKey(userId, { gas: 500000 })
}

function creator(userId, publicKey) {
  let contract = web3.eth.contract(abi).at($contract_address)
  web3.personal.unlockAccount($admin_account, "")
  contract.createVault(userId, publicKey, { gas: 500000 })
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
    conn.createChannel(IDENTITY_CREATE_REQUEST);
    conn.createChannel(IDENTITY_CREATE_RESPONSE);
    conn.createChannel(IDENTITY_GET_REQUEST);
    conn.createChannel(IDENTITY_GET_RESPONSE);
  });
}

try {
  connect()
}
catch(e) {
  console.error("ERROR!!")
}