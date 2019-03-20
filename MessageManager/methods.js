const mq_connector = require('./mq_connect')
const ipfs_connector = require('./ipfs_connect')
const contractWrapper = require('./contract')
const NodeRSA = require('node-rsa')

var contract;
var unlock;

// Channel handlers
{
  function MESSAGE_SEND_REQUEST (request, RESPONDER) {
    let requestId = request.REQUEST_ID
    
    let toUser = request.TO_USER
    let fromUser = request.FROM_USER

    let toData = request.TO_DATA
    let fromData = request.FROM_DATA

    try
    {
      saveDocument(toData, (toHash) => {
        saveDocument(fromData, (fromHash) => {
          sendMessage(toUser, fromUser, toHash, fromHash)

          RESPONDER(requestId, {
            STATUS: 'SUCCESS',
            TO_HASH: toHash,
            FROM_HASH: fromHash
          });

        })
      })
    }
    catch(e)
    {
      RESPONDER(requestId, {
        STATUS: 'ERROR',
        ERROR: e
      });
    }
  }

  function MESSAGE_GET_RECENT_RECEIVED_REQUEST (request, RESPONDER) {
    let requestId = request.REQUEST_ID
    let userKey = request.USER_KEY
    let fromBlock = request.FROM_BLOCK

    try
    {
      getReceivedMessagesSince(userKey, fromBlock, (messages) => {
        RESPONDER(requestId, {
          STATUS: 'SUCCESS',
          MESSAGE_COUNT: messages.length
        });
  
        for(let i=0; i < messages.length; i++) {
          RESPONDER(requestId, {
            MESSAGE: messages[i]
          });
          // getDocument(messages[i], (doc) => {
          //   RESPONDER(requestId, {
          //     MESSAGE: doc[0].content.toString()
          //   });
          // })
        }
      })
    }
    catch(e)
    {
      RESPONDER(requestId, {
        STATUS: 'ERROR',
        ERROR: e
      });
    }
  }

  function MESSAGE_GET_RECENT_SENT_REQUEST (request, RESPONDER) {
    let requestId = request.REQUEST_ID
    let userKey = request.USER_KEY
    let fromBlock = request.FROM_BLOCK

    try
    {
      getSentMessagesSince(userKey, fromBlock, (messages) => {
        RESPONDER(requestId, {
          STATUS: 'SUCCESS',
          MESSAGE_COUNT: messages.length
        });
  
        for(let i=0; i < messages.length; i++) {
          RESPONDER(requestId, {
            MESSAGE: messages[i]
          });
          // getDocument(messages[i], (doc) => {
          //   RESPONDER(requestId, {
          //     MESSAGE: doc[0].content.toString()
          //   });
          // })
        }
      })
    }
    catch(e)
    {
      RESPONDER(requestId, {
        STATUS: 'ERROR',
        ERROR: e
      });
    }
  }

  function MESSAGE_GET_REQUEST (request, RESPONDER) {
    let requestId = request.REQUEST_ID
    let messageId = request.MESSAGE_ID

    try
    {
      getDocument(messageId, (doc) => {
        if (doc.length === 0) {
          RESPONDER(requestId, {
            STATUS: 'ERROR',
            ERROR: 'Message not found'
          });
        } else {
          RESPONDER(requestId, {
            STATUS: 'SUCCESS',
            MESSAGE: doc[0].content.toString()
          });
        }
      })
    }
    catch(e)
    {
      RESPONDER(requestId, {
        STATUS: 'ERROR',
        ERROR: e
      });
    }
  }
}

// Internal methods
{
  function saveDocument(doc, cb) {
    ipfs_connector.set(doc, cb)
  }
  function getDocument(docId, cb) {
    ipfs_connector.get(docId, cb)
  }
  function sendMessage(toUser, fromUser, toHash, fromHash) {
    contractWrapper.unlockAccount()
    return contract.sendMessage(toUser, fromUser, toHash, fromHash, { gas: 500000 })
  }
  function getReceivedMessagesSince(userId, fromBlock, cb) {
    contractWrapper.unlockAccount()

    let event = contractWrapper.hash('MessageSent(string,string,string,string)')
    let userhash = contractWrapper.hash(userId)

    let filter = contractWrapper.filter({
      fromBlock: fromBlock,
      topics: [
        event,
        userhash
      ]
    })
    
    filter.get((err, results) => {
      cb(results.map(r => {
        let rawdata = r.data.substring(2)
        let firstStr = rawdata.substring(192, 320)
        let secondStr = rawdata.substring(384, 512)
        return hex2a(firstStr)
      }))
    })
  }
  function getSentMessagesSince(userId, fromBlock, cb) {
    contractWrapper.unlockAccount()

    let event = contractWrapper.hash('MessageSent(string,string,string,string)')
    let userhash = contractWrapper.hash(userId)

    let filter = contractWrapper.filter({
      fromBlock: fromBlock,
      topics: [
        event,
        null,
        userhash
      ]
    })
    
    filter.get((err, results) => {
      cb(results.map(r => {
        let rawdata = r.data
        let firstStr = rawdata.substring(192, 320)
        let secondStr = rawdata.substring(384, 512)
        return secondStr
      }))
    })
  }

  function hex2a(hex) {
    var str = '';
    for (var i = 0; (i < hex.length && hex.substr(i, 2) !== '00'); i += 2)
      str += String.fromCharCode(parseInt(hex.substr(i, 2), 16));
    return str;
  }
}

// Initialization methods
{
  function mq_init (mq_host) {
    mq_connector.connect(
      'MESSAGE-MANAGER',
      'message_request',
      'message_response',
      mq_host, 
      [
        { topic: 'send', handler: MESSAGE_SEND_REQUEST },
        { topic: 'get_recent', handler: MESSAGE_GET_RECENT_RECEIVED_REQUEST },
        { topic: 'get_recent_sent', handler: MESSAGE_GET_RECENT_SENT_REQUEST },
        { topic: 'get', handler: MESSAGE_GET_REQUEST }
      ]
    )
  }
  
  function bc_init (host) {
    contractWrapper.init(host)
    contractWrapper.getContract((contractInstance) => {
      contract = contractInstance
    })
  }
  
  function ipfs_init (host) {
    ipfs_connector.init(host)
  }
}

module.exports = {
  mq_init,
  bc_init,
  ipfs_init
}
