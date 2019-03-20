const fs = require('fs')
const Web3 = require('web3')

const abi = JSON.parse(fs.readFileSync('./Contracts/out/Messenger_sol_Messenger.abi', {'encoding':'utf8', 'flag':'r'}));
const bytecode = "0x" + fs.readFileSync('./Contracts/out/Messenger_sol_Messenger.bin', {'encoding':'utf8', 'flag':'r'})

const $admin_account = "0xed9d02e382b34818e88b88a309c7fe71e65f419d"

var web3

var contract_address

function deploy(cb) {
  web3.eth.defaultAccount = $admin_account;
  web3.personal.unlockAccount($admin_account, "")
  var simpleContract = web3.eth.contract(abi);
  simpleContract.new({
      from: $admin_account, 
      data: bytecode,
      gas: 0xFFFFFF
    }, function(e, contract) {
      if (e) {
        console.log(" [!] err creating contract", e);
      } else if (!contract.address) {
        console.log(" [*] Contract transaction send: TransactionHash: " + contract.transactionHash + " waiting to be mined...");
      } else {
        console.log(" [/] Contract mined! Address: " + contract.address);
        fs.writeFile("./Contracts/Thimble_Address.txt", contract.address, (err) => {
          if(err) console.log(` [!] Error saving contract to disk ${err.toString()}`)
          else cb(contract.address)
        })
      }
    });
}

function lookupAddress (cb) {
  unlockAccount()

  let filter = web3.eth.filter({
    fromBlock: 0,
    toBlock: 'latest',
    topics: [web3.sha3('MessengerCreated(address)')]
  })
  
  filter.get((error, results) => {
    let mapped = results.map(r => ({
      address: r.address,
      bytes: web3.eth.getTransaction(r.transactionHash).input
    })).filter(r => 
      r.bytes === bytecode
    )
  
    cb(mapped[0] ? mapped[0].address : undefined)
  })
}
function findAddress (cb) {
  lookupAddress((address) => {
    if(!address) {
      console.log(' [*] Contract not yet deployed')
      deploy(cb)
    } else {
      console.log(' [/] Found contract address', address)
      cb(address)
    }
  })
}

function getContract(cb) {
  if (contract_address) return cb(contract_address)
  findAddress((contract_addr) => {
    contract_address = contract_addr
    let contract = web3.eth.contract(abi).at(contract_addr)
    cb(contract)
  })
}

function unlockAccount () {
  web3.personal.unlockAccount($admin_account, "")
}

function hash (input) {
  return web3.sha3(input)
}

function filter (obj) {
  return web3.eth.filter(Object.assign({
    fromBlock: 0,
    toBlock: 'latest',
    address: contract_address,
    topics: []
  }, obj))
}

function init (rpc_address) {
  web3 = new Web3(new Web3.providers.HttpProvider(rpc_address));
  web3.eth.defaultAccount = $admin_account;
}

module.exports = {
  init,
  getContract,
  unlockAccount,
  filter,
  hash
}
