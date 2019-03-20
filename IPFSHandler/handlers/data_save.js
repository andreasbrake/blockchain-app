var ipfs = null;
var VERSION_SAVE = null;
var SAVE_RESPONDER = (saveRequestId, message) => {};

function init(ipfsInstance, saver) {
  ipfs = ipfsInstance
  VERSION_SAVE = saver
}

function DATA_SAVE_REQUEST(err, ch) {
  var ex = 'data_save_request';

  ch.assertExchange(ex, 'direct', { durable: false });
  ch.assertQueue('IPFS-SAVE-REQUEST-HANDLER', { exclusive: true }, function(err, q) {
    console.log(" [*] Waiting for messages in data_save_request %s.", q.queue);
    ch.bindQueue(q.queue, ex, '');
    ch.consume(q.queue, saveRequestHandler, { noAck: false });
  });
}

function DATA_SAVE_RESPONSE(err, ch) {
  var ex = 'data_save_response';
  ch.assertExchange(ex, 'direct', {durable: false});
  SAVE_RESPONDER = (requestId, message) => {
    ch.publish(ex, requestId, new Buffer(JSON.stringify(message)));
    console.log(` [/] Sent to data_save_response message with requestId ${requestId} ${JSON.stringify(message)}"`);
  }
}

function saveRequestHandler(msg) {
  let message = msg.content;
  let content = message.toString()
  console.log(" [x] %s", content);
  let request = JSON.parse(content)

  var requestId = request.REQUEST_ID;
  let module = request.MODULE
  let schema = request.SCHEMA
  let name = request.NAME
  let isUpdate = request.IS_UPDATE
  let data = request.DATA

  console.log(` [*] Saving document at ${module}-${schema}-${name}`);

  let versionSaveRespose = (res) => {
    if(res.STATUS === 'SUCCESS') {
      console.log(` [/] Registered document version`);
      SAVE_RESPONDER(requestId, { STATUS: 'SUCCESS', ACTION: 'SAVED_DOCUMENT' })
    }
    else {
      if(res.STATUS === 'ERROR') {
        console.log(` [/] Error saving document ${res.ERROR}`);
        SAVE_RESPONDER(requestId, { STATUS: 'ERROR', ERROR: res.ERROR })
      }
      else {
        console.log(` [/] Error saving document unknown response ${res.ERROR}`);
        SAVE_RESPONDER(requestId, { STATUS: 'ERROR', ERROR: `UNKNOWN SAVE RESPONSE ${res.STATUS}` })
      }
    }
  }
  
  putDocument(data)
    .then(function(docId) {
      console.log(` [/] Saved document with id ${docId}`);

      VERSION_SAVE.Save(module, schema, name, docId, isUpdate, versionSaveRespose)
    });
}

function putDocument(doc) {
  return ipfs.object.put({ Data: new Buffer(JSON.stringify(doc)), Links: [] })
    .then(function(node) {
      doc['BLOB_HASH'] = node.toJSON().multihash;
      console.log(` [*] Injecting block hash ${doc['BLOB_HASH']}`);
      return ipfs.dag.put(doc, { format: 'dag-cbor', hashAlg: 'sha3-512' })
    })
    .then(function(cid) {
      let docId = cid.toBaseEncodedString();
      return docId;
    })
}

module.exports = {
  init: init,
  REQUEST: DATA_SAVE_REQUEST,
  RESPONSE: DATA_SAVE_RESPONSE
}