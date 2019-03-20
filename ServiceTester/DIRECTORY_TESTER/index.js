const fs = require('fs')
const NodeRSA = require('node-rsa')

var connection

function createNewUser () {
  let userId = 'andreassbrake@gmail.com'
  let date = new Date().getTime();
  let prvkey = fs.readFileSync('./DIRECTORY_TESTER/test/user').toString()
  let pubkey = fs.readFileSync('./DIRECTORY_TESTER/test/user.pub.pem').toString()
  
  let key = new NodeRSA(prvkey)
  const signature = key.sign(Buffer.from(`${userId}-${date}`), 'base64');

  let data = {
    USER_ID: userId,
    PUBLIC_KEY: pubkey,
    DATE_SEND: date,
    SIGNATURE: signature
  }

  connection.create_request(
    'directory_request', 
    'directory_response', 
    'create', 
    data,
    (response) => {
      console.log('responsed', response)
    }
  )
}

function verifyUser () {
  let userId = 'andreassbrake@gmail.com'

  let data = {
    USER_ID: userId
  }

  connection.create_request(
    'directory_request', 
    'directory_response', 
    'verify', 
    data,
    (response) => {
      console.log('responsed', response)
    }
  )
}

function test () {
  verifyUser()
}

function init (conn) {
  connection = conn
}

module.exports = {
  init: init,
  test: test
}