const mq_connector = require('./mq_connect')

var $MQ_HOST = 'amqp://10.0.75.2';

for(var i=2; i < process.argv.length; i++)
{
  var arg = process.argv[i];
  if(arg === "--mq" && process.argv[i + 1] !== undefined)
  {
    $MQ_HOST = process.argv[i + 1]
  }
}

mq_connector.connect($MQ_HOST, () => {
  // const DIRECTORY_TESTER = require('./DIRECTORY_TESTER')
  // DIRECTORY_TESTER.init(mq_connector)
  // DIRECTORY_TESTER.test()

  const MESSENGER_TESTER = require('./MESSENGER_TESTER')
  MESSENGER_TESTER.init(mq_connector)
  MESSENGER_TESTER.test()
})
