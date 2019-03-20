const uuid = require('uuid/v1');

var GETTER = (module, schema, name, params, cb) => { console.log(" [!] Calling empty method"); };
var RESPONSE_HANDLER = (versionRequestId, cb) => {};

function Get(module, schema, name, params, cb) {
  GETTER(module, schema, name, params, cb);
}

function VERSION_GET_REQUESTOR(err, ch) {
  var ex = 'version_get_request';
  ch.assertExchange(ex, 'direct', {durable: false});
  GETTER = (module, schema, name, params, cb) => {
    let versionRequestId = uuid();
    RESPONSE_HANDLER(versionRequestId, cb)
    let request = {
      REQUEST_ID: versionRequestId,
      MODULE: module,
      SCHEMA: schema,
      NAME: name,
      PARAMS: params || {}
    }
    ch.publish(ex, '', new Buffer(JSON.stringify(request)));
    console.log(" [/] Sent to version_get_request message %s", JSON.stringify(request));
  }
}

function VERSION_GET_RESPONSE_HANDLER(err, ch) {
  var ex = 'version_get_response';

  ch.assertExchange(ex, 'direct', { durable: false });

  RESPONSE_HANDLER = (versionRequestId, cb) => {
    ch.assertQueue(`IPFS-VERSION-GET-RESPONSE-HANDLER-${versionRequestId}`, { exclusive: true }, function(err, q) {
      console.log(` [*] Waiting for messages in version_get_response ${q.queue} for versionRequestId ${versionRequestId}.`);
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
  REQUESTOR: VERSION_GET_REQUESTOR,
  RESPONSE_HANDLER: VERSION_GET_RESPONSE_HANDLER,
  Get: Get
}