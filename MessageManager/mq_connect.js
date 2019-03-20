const uuid = require('uuid/v1')
const amqp = require('amqplib/callback_api');

var managerName
var requestExchange
var responseExchange

var RESPONDER = (requestId, message) => {};

function init_listener (channel, topic, cb) {
  channel.assertExchange(requestExchange, 'direct', { durable: false });
  channel.assertQueue(`${managerName}-${topic.toUpperCase()}-REQUEST-HANDLER-${uuid()}`, { exclusive: true }, function(err, q) {
    if(err) {
      console.error(` [!] Error asserting queue ${err.toString()}`)
    }
    console.log(` [*] Waiting for messages in ${requestExchange}/${topic} ${q.queue}.`);

    channel.bindQueue(q.queue, requestExchange, topic);
    channel.consume(q.queue, function(msg) {
      let content = msg.content.toString()
      let request = JSON.parse(content)
      console.log(" [x] ", request);

      cb(request, RESPONDER)
    }, { noAck: false });
  });
}

function RESPONSE_CHANNEL(err, channel) {
  channel.assertExchange(responseExchange, 'direct', {durable: false});
  RESPONDER = (requestId, message) => {
    channel.publish(responseExchange, requestId, new Buffer(JSON.stringify(message)));
    console.log(` [/] Sent to ${responseExchange} message`, message);
  }
}

var attempts = 0
function connectMQ (hostAddress, channels) {
  amqp.connect(hostAddress, function(err, conn) {
    if (err || !conn) {
      console.error('MQ CONNECTION ERROR', err)

      if(attempts < 15) {
        setTimeout(function() {
          attempts ++
          connectMQ(hostAddress, channels)
        }, 2000)
      }
      return;
    }
    console.log(" [*] Connected to AMQP server");

    conn.createChannel(RESPONSE_CHANNEL);
    
    for(let i=0; i < channels.length; i++) {
      conn.createChannel((err, ch) => {
        init_listener(ch, channels[i].topic, channels[i].handler)
      });
    }
  });
}

function connect (name, request, response, hostAddress, channels) {
  managerName = name
  requestExchange = request
  responseExchange = response
  
  try {
    connectMQ(hostAddress, channels)
  }
  catch(e) {
    console.error("ERROR!!")
  }
}

module.exports = {
  connect: connect
}
