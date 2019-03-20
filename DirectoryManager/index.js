const contractWrapper = require('./contract')
const methods = require('./methods')

var $MQ_HOST = 'amqp://10.0.75.2';
var $RPC_HOST = 'http://demo.thimble.xyz:22000'; //'http://10.0.75.2:22000';

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

contractWrapper.init($RPC_HOST)

methods.mq_init($MQ_HOST)

contractWrapper.getContract((contract) => {
  methods.bc_init(contract, contractWrapper.unlockAccount)
})
