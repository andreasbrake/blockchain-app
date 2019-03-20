var ipfs = null
var VERSION_GET = null;
var GET_RESPONDER = (requestId, data) => {};

function init(ipfsInstance, getter) {
  ipfs = ipfsInstance
  VERSION_GET = getter
}

function DATA_GET_REQUEST(err, ch) {
  var ex = 'data_get_request';

  ch.assertExchange(ex, 'direct', { durable: false });
  ch.assertQueue('IPFS-GET-REQUEST-HANDLER', { exclusive: true }, function(err, q) {
    console.log(" [*] Waiting for messages in data_get_request %s.", q.queue);
    ch.bindQueue(q.queue, ex, '');
    ch.consume(q.queue, function(msg) {
      let content = msg.content.toString()
      console.log(" [x] %s", content);
      let request = JSON.parse(content)

      let requestId = request.REQUEST_ID
      let module = request.MODULE
      let schema = request.SCHEMA
      let name = request.NAME

      console.log(` [x] Getting document at ${module}-${schema}-${name}`);

      resolveDocument(module, schema, name, (docId) => {
        console.log(` [/] Resolved document to id ${docId}`);
        getDocument(docId)
          .then((doc) => {
            console.log(` [/] Got document`)
            GET_RESPONDER(requestId, doc)
          })
      })
    }, { noAck: false });
  });
}

function DATA_GET_RESPONSE(err, ch) {
  var ex = 'data_get_response';
  ch.assertExchange(ex, 'direct', {durable: false});
  GET_RESPONDER = (requestId, data) => {
    ch.publish(ex, requestId, new Buffer(JSON.stringify(data)));
    console.log(" [/] Sent to data_get_response message %s", JSON.stringify(data));
  }
}

function resolveDocument(module, schema, name, cb) {
  VERSION_GET.Get(module, schema, name, { RETURN_TYPE: 'DOCUMENT_ID' }, (response) => {
    let documentId = response.DOCUMENT_ID;
    cb(documentId);
  })
}

function getDocument(docId) {
  return ipfs.dag.get(docId)
    .then((obj) => {
      return obj.value
    })
}

module.exports = {
  init: init,
  REQUEST: DATA_GET_REQUEST,
  RESPONSE: DATA_GET_RESPONSE
}