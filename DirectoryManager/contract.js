const fs = require('fs')
const Web3 = require('web3')

const abi = JSON.parse(fs.readFileSync('./Contracts/out/Directory_sol_Directory.abi', {'encoding':'utf8', 'flag':'r'}));
const bytecode = "0x" + fs.readFileSync('./Contracts/out/Directory_sol_Directory.bin', {'encoding':'utf8', 'flag':'r'})

const $admin_account = "0xed9d02e382b34818e88b88a309c7fe71e65f419d"

var web3

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


function unlockAccount () {
  web3.personal.unlockAccount($admin_account, "")
}

function lookupAddress (cb) {
  unlockAccount()

  let filter = web3.eth.filter({
    fromBlock: 0,
    toBlock: 'latest',
    topics: [
      web3.sha3('DirectoryCreated(address)'),
      `0x000000000000000000000000${$admin_account.substring(2)}`
    ]
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
  findAddress((contract_address) => {
    let contract = web3.eth.contract(abi).at(contract_address)
    cb(contract)
  })
}

function init (rpc_address) {
  web3 = new Web3(new Web3.providers.HttpProvider(rpc_address));
  web3.eth.defaultAccount = $admin_account;
}

module.exports = {
  init: init,
  getContract: getContract,
  unlockAccount: unlockAccount
}
