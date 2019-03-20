const ipfsAPI = require('ipfs-api')

ipfsInstance = null

function init (host) {
  ipfsInstance = ipfsAPI({host: host, port: '5001', protocol: 'http'})
}

function set (data, cb) {
  ipfsInstance.files.add(Buffer.from(data), (err, file) => {
    if (err) throw err
    cb(file[0].hash)
  })
}

function get (docId, cb) {
  ipfsInstance.files.get(docId, (err, file) => {
    if (err) throw `Error resolving file with id ${docId}`
    cb(file)
  })
}

module.exports = {
  init,
  set,
  get,
  getInstance: function () {
    return ipfsInstance
  }
}
