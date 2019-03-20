const amqp = require('amqplib/callback_api');

const DATA_GET = require('./handlers/data_get');
const DATA_SAVE = require('./handlers/data_save');
const VERSION_GET = require('./handlers/version_get');
const VERSION_SAVE = require('./handlers/version_save');
const IPFS = require('./handlers/ipfs')

var $IPFS_HOST = '10.0.75.2'
var $MQ_HOST = 'amqp://10.0.75.2'

for(var i=2; i < process.argv.length; i++)
{
  var arg = process.argv[i];
  if(arg === "--ipfs" && process.argv[i + 1] !== undefined)
  {
    $IPFS_HOST = process.argv[i + 1]
  }
  if(arg === "--mq" && process.argv[i + 1] !== undefined)
  {
    $MQ_HOST = process.argv[i + 1]
  }
}

console.log(` [#] Starting IPFS-Manager with MQ ${$MQ_HOST} and IPFS ${$IPFS_HOST}`);

IPFS.init($IPFS_HOST)
DATA_GET.init(IPFS.getInstance(), VERSION_GET)
DATA_SAVE.init(IPFS.getInstance(), VERSION_SAVE)

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
    conn.createChannel(DATA_SAVE.REQUEST);
    conn.createChannel(DATA_SAVE.RESPONSE);
  
    conn.createChannel(DATA_GET.REQUEST);
    conn.createChannel(DATA_GET.RESPONSE);
  
    conn.createChannel(VERSION_GET.REQUESTOR);
    conn.createChannel(VERSION_GET.RESPONSE_HANDLER);
  
    conn.createChannel(VERSION_SAVE.REQUESTOR);
    conn.createChannel(VERSION_SAVE.RESPONSE_HANDLER);
  });
}

try {
  connect()
}
catch(e) {
  console.error("ERROR!!")
}
