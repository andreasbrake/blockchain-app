const ipfsAPI = require('ipfs-api')

var ipfsInstance = null
function init (host) {
  ipfsInstance = ipfsAPI({host: host, port: '5001', protocol: 'http'})
}


module.exports = {
  init: init,
  getInstance: function () {
    return ipfsInstance
  }
}