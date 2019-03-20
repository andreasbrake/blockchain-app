pragma solidity ^0.4.23;

contract Vault {
    struct VaultUser
    {
        bool isValue;
        string userid;
        string publickey;
    }

    string[] public userNames;
    mapping (string => VaultUser) userVaults;
    
    event VaultCreationEvent(
        string msg
    );

    event UserCreationEvent(
        string userid
    );

    constructor () public { 
        emit VaultCreationEvent("CreatedVault");
    }

    function createVault(string userId, string publicKey) public {
        if (!userVaults[userId].isValue) {
            userNames.push(userId);
            userVaults[userId] = VaultUser ({
                isValue: true,
                userid: userId,
                publickey: publicKey
            });
        }
        emit UserCreationEvent(userId);
    }

    function resolvePublickKey(string userId) view public returns (string){
        return userVaults[userId].publickey;
    }
}