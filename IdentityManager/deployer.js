const fs = require('fs')

admin_account = "0xed9d02e382b34818e88b88a309c7fe71e65f419d"

if(!admin_account) throw("did not connect correctly, no address found")

const abi = JSON.parse(fs.readFileSync('./Contracts/out/Vault_sol_Vault.abi', {'encoding':'utf8', 'flag':'r'}));
const bytecode = "0x" + fs.readFileSync('./Contracts/out/Vault_sol_Vault.bin', {'encoding':'utf8', 'flag':'r'})

function deploy(web3, cb) {
  web3.eth.defaultAccount = admin_account;
  web3.personal.unlockAccount(admin_account, "")
  var simpleContract = web3.eth.contract(abi);
  simpleContract.new("demo", {
      from: admin_account, 
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
          cb(err, contract.address)
        })
      }
    });
}

function test(web3, address) {
  console.log('testing contract');

  var contract = web3.eth.contract(abi).at(address)
  contract.createModule("DEMO");
}

module.exports = {
  deploy: deploy
}