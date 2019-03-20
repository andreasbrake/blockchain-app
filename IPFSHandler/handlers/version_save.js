const uuid = require('uuid/v1');

var SAVER = (module, schema, name, documentId, isUpdate, cb) => { console.log(" [!] Calling empty method"); };
var RESPONSE_HANDLER = (versionRequestId, cb) => {};

function Save(module, schema, name, documentId, isUpdate, cb) {
  SAVER(module, schema, name, documentId, isUpdate, cb);
}

function VERSION_SAVE_REQUESTOR(err, ch) {
  var ex = 'version_save_request';
  ch.assertExchange(ex, 'direct', {durable: false});
  SAVER = (module, schema, name, documentId, isUpdate, cb) => {
    console.log(" [*] Saving document to blockchain");
    let versionRequestId = uuid();
    RESPONSE_HANDLER(versionRequestId, cb)
    let versionRequest = {
      REQUEST_ID: versionRequestId,
      MODULE: module,
      SCHEMA: schema,
      NAME: name,
      DOCUMENT_ID: documentId,
      IS_UPDATE: isUpdate
    }
    ch.publish(ex, '', new Buffer(JSON.stringify(versionRequest)));
    console.log(" [/] Sent to version_save_request message %s", JSON.stringify(versionRequest));
  }
}

function VERSION_SAVE_RESPONSE_HANDLER(err, ch) {
  var ex = 'version_save_response';

  ch.assertExchange(ex, 'direct', { durable: false });

  RESPONSE_HANDLER = (versionRequestId, cb) => {
    ch.assertQueue(`IPFS-VERSION-SAVE-RESPONSE-HANDLER-${versionRequestId}`, { exclusive: true }, function(err, q) {
      console.log(` [*] Waiting for messages in version_save_response ${q.queue} for requestId ${versionRequestId}.`);
      ch.bindQueue(q.queue, ex, versionRequestId);
      ch.consume(q.queue, function(msg) {
        let content = msg.content.toString()
        cb(JSON.parse(content));
        ch.unbindQueue(q.queue, ex, versionRequestId);
      }, { noAck: false });
    });
  }
}

module.exports = {
  REQUESTOR: VERSION_SAVE_REQUESTOR,
  RESPONSE_HANDLER: VERSION_SAVE_RESPONSE_HANDLER,
  Save: Save
}