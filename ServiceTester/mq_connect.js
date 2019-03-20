const uuid = require('uuid/v1')
const amqp = require('amqplib/callback_api');

var CONNECTION

function create_listener (exchange, topic, cb) {
  CONNECTION.createChannel((err, channel) => {
    channel.assertExchange(exchange, 'direct', { durable: false });
    channel.assertQueue(`${exchange.toUpperCase()}-${topic.toUpperCase()}-REQUEST-HANDLER-${uuid()}`, { exclusive: true }, function(err, q) {
      if(err) {
        console.error(` [!] Error asserting queue ${err.toString()}`)
      }
      console.log(` [*] Waiting for messages on ${exchange}/${topic} ${q.queue}.`);
  
      channel.bindQueue(q.queue, exchange, topic);
      channel.consume(q.queue, function(msg) {
        let content = msg.content.toString()
        console.log(" [x] %s", content);
        let request = JSON.parse(content)
  
        cb(request)
      }, { noAck: false });
    });
  });
}

function create_responder (exchange, cb) {
  CONNECTION.createChannel((err, channel) => {
    channel.assertExchange(exchange, 'direct', {durable: false});
    cb((requestId, message) => {
      channel.publish(exchange, requestId, new Buffer(JSON.stringify(message)));
      console.log(` [/] Responded on ${exchange} message`, message);
    })
  })
}

function create_request (requestExchange, responseExchange, topic, data, cb) {
  CONNECTION.createChannel((err, channel) => {
    channel.assertExchange(requestExchange, 'direct', { durable: false });

    let requestId = uuid()

    data.REQUEST_ID = requestId

    create_request_listener(responseExchange, requestId, cb)

    channel.publish(requestExchange, topic, new Buffer(JSON.stringify(data)));

    console.log(` [/] Published to ${requestExchange}/${topic} message`, data);
  });
}

function create_request_listener (exchange, requestId, cb) {
  CONNECTION.createChannel((err, channel) => {
    channel.assertExchange(exchange, 'direct', { durable: false });
    channel.assertQueue(`${exchange.toUpperCase()}-${requestId.toUpperCase()}-REQUEST-HANDLER-${uuid()}`, { exclusive: true }, function(err, q) {
      if(err) {
        console.error(` [!] Error asserting queue ${err.toString()}`)
      }
      console.log(` [*] Waiting for responses on ${exchange}/${requestId} ${q.queue}.`);
      
      channel.bindQueue(q.queue, exchange, requestId);
      let unbind = () => {
        channel.unbindQueue(q.queue, exchange, requestId);
      }
      channel.consume(q.queue, function(msg) {
        let content = msg.content.toString()
        console.log(" [x] %s", content);
        let request = JSON.parse(content)
  
        cb(request, unbind)
      }, { noAck: false });
    });
  });
}

var attempts = 0
function connectMQ (hostAddress, cb) {
  amqp.connect(hostAddress, function(err, conn) {
    if (err || !conn) {
      console.error('MQ CONNECTION ERROR', err)

      if(attempts < 15) {
        setTimeout(function() {
          attempts ++
          connectMQ(hostAddress, cb)
        }, 2000)
      }
      return;
    }
    console.log(" [*] Connected to AMQP server");

    CONNECTION = conn
    
    cb()
  });
}

function connect (hostAddress, cb) {
  try {
    connectMQ(hostAddress, cb)
  }
  catch(e) {
    console.error("ERROR!!")
  }
}

module.exports = {
  connect,
  create_listener,
  create_responder,
  create_request,
  create_request_listener
}
