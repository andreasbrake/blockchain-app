const mq_connector = require('./mq_connect')
const NodeRSA = require('node-rsa')

var contract;
var unlock;

// Channel handlers
{
  function DIRECTORY_CREATE_REQUEST(request, RESPONDER) {
    let requestId = request.REQUEST_ID
    let userId = request.USER_ID
    let publickey = request.PUBLIC_KEY
    let dateSent = request.DATE_SEND
    let signature = request.SIGNATURE

    let rsaKey = new NodeRSA(publickey)
    let data = Buffer.from(`${userId}-${dateSent}`)
    let signatureVerified = rsaKey.verify(data, signature, 'buffer', 'base64')

    if (!signatureVerified) {
      console.log(` [!] Requestor failed signature verification`);
      RESPONDER(requestId, { STATUS: "ERROR", ERROR: "SIG_FAIL" })
      return
    }

    console.log(` [*] Creating directory entry for ${userId}`);

    if(resolve(userId).PUBLIC_KEY) {
      RESPONDER(requestId, { STATUS: "ERROR", ERROR: "USER_EXISTS" })
      return
    }
    try {
      createEntry(userId, publickey)
      RESPONDER(requestId, { STATUS: "SUCCESS" })
    }
    catch(e) {
      RESPONDER(requestId, { STATUS: "ERROR", ERROR: e.toString() })
    }
  }
  
  function DIRECTORY_VERIFY_REQUEST(request, RESPONDER) {
    let requestId = request.REQUEST_ID
    let userId = request.USER_ID

    console.log(` [*] Verifying directory entry for ${userId}`);

    try {
      verifyEntry(userId)
      RESPONDER(requestId, { STATUS: "SUCCESS" })
    }
    catch(e) {
      RESPONDER(requestId, { STATUS: "ERROR", ERROR: e.toString() })
    }
  }
  
  function DIRECTORY_GET_REQUEST(request, RESPONDER) {
    let requestId = request.REQUEST_ID
    let userId = request.USER_ID
    
    console.log(` [x] Getting key for ${userId}`);
    
    try
    {
      let result = resolve(userId)
      
      console.log(` [/] Resolved key for ${userId}`);

      RESPONDER(requestId, {
        STATUS: 'SUCCESS',
        ENTRY: result
      });
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
  function resolve(userId) {
    unlock()
    let response = contract.resolve(userId, { gas: 500000 })
    return {
      USER_ID: response[0],
      PUBLIC_KEY: response[1],
      PUBLIC_KEY_HASH: response[2],
      IS_ENTERPRISE: response[3],
      IS_VERIFIED: response[4]
    }
  }
  
  function verifyEntry(userId) {
    unlock()
    contract.verifyEntry(userId, { gas: 500000 })
  }
  
  function createEntry(userId, publicKey) {
    unlock()
    contract.registerEntry(userId, publicKey, { gas: 1500000 })
  }
}

// Initialization methods
{
  function mq_init (mq_host) {
    mq_connector.connect(
      'DIRECTORY-MANAGER',
      'directory_request',
      'directory_response',
      mq_host, 
      [
        { topic: 'create', handler: DIRECTORY_CREATE_REQUEST },
        { topic: 'verify', handler: DIRECTORY_VERIFY_REQUEST },
        { topic: 'get', handler: DIRECTORY_GET_REQUEST }
      ]
    )
  }
  
  function bc_init (contractInstance, unlockMethod) {
    contract = contractInstance
    unlock = unlockMethod
  }
}

module.exports = {
  mq_init: mq_init,
  bc_init: bc_init
}
