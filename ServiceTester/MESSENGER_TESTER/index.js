const fs = require('fs')
const NodeRSA = require('node-rsa')

var connection

function sendMessage () {
  let message = 'Hi there chum'

  let userId1 = 'andreassbrake@gmail.com'
  let userId2 = 'andreas@andreasbrake.ca'
  
  let prvkey = fs.readFileSync('./DIRECTORY_TESTER/test/user').toString()
  let pubkey = fs.readFileSync('./DIRECTORY_TESTER/test/user.pub.pem').toString()
  
  let key = new NodeRSA(pubkey)

  const data1 = key.encrypt(Buffer.from(message), 'base64');
  const data2 = key.encrypt(Buffer.from(message), 'base64');

  let data = {
    TO_USER: userId1,
    FROM_USER: userId2,
    TO_DATA: data1,
    FROM_DATA: data2
  }

  connection.create_request(
    'message_request', 
    'message_response', 
    'send', 
    data,
    (response) => {
      console.log('responsed', response)
    }
  )
}

function getMessages () {
  let userId1 = 'andreassbrake@gmail.com'

  let data = {
    USER_KEY: userId1,
    FROM_BLOCK: 0
  }

  let total = -1
  let received = []
  connection.create_request(
    'message_request', 
    'message_response', 
    'get_recent_sent', 
    data,
    (response, end) => {
      if (response.STATUS === 'SUCCESS') {
        total = response.MESSAGE_COUNT
      } else {
        received.push(response.MESSAGE)
        readMessage(response.MESSAGE)
      }
      if (total >= 0 && total === received.length) {
        end()
      }
    }
  )
}

function readMessage (message) {
  let prvkey = fs.readFileSync('./DIRECTORY_TESTER/test/user').toString()
  let key = new NodeRSA(prvkey)
  let data = key.decrypt(message, 'utf8');
  console.log(data)
}


function test () {
  sendMessage()
}

function init (conn) {
  connection = conn
}

module.exports = {
  init: init,
  test: test
}