pragma solidity ^0.4.24;

contract Directory {

    struct DirectoryEntry
    {
        bool isValue;
        string userid;
        string publickey;
        bytes32 keyhash;
        bool isEnterprise;
        bool verified;
    }

    address manager;

    string[] public userIds;
    mapping (string => DirectoryEntry) lookup;
    mapping (string => DirectoryEntry) reverseLookup;

    event DirectoryCreated(
        address indexed manager
    );

    event Registered(
        string indexed useridix,
        string userid,
        bytes32 keyhash,
        bool isEnterprise
    );

    event Verified(
        string indexed userid
    );

    constructor () public { 
        manager = msg.sender;
        emit DirectoryCreated(manager);
    }

    function registerEntry(string userEmail, string publicKey) public {
        require(msg.sender == manager, "Cannot register unless manager.");

        // TODO: Verify userid is formatted as email address
        
        if (!lookup[userEmail].isValue) {
            userIds.push(userEmail);
            lookup[userEmail] = DirectoryEntry ({
                isValue: true,
                userid: userEmail,
                publickey: publicKey,
                keyhash: keccak256(bytes(publicKey)),
                isEnterprise: false,
                verified: false
            });
            reverseLookup[publicKey] = lookup[userEmail];
        }
        emit Registered(userEmail, userEmail, lookup[userEmail].keyhash, false);
    }

    function registerEnterpriseEntry(string enterpriseName, string publicKey) public {
        require(msg.sender == manager, "Cannot register unless manager.");
        
        if (!lookup[enterpriseName].isValue) {
            userIds.push(enterpriseName);
            lookup[enterpriseName] = DirectoryEntry ({
                isValue: true,
                userid: enterpriseName,
                publickey: publicKey,
                keyhash: keccak256(bytes(publicKey)),
                isEnterprise: true,
                verified: false
            });
            reverseLookup[publicKey] = lookup[enterpriseName];
        }
        emit Registered(enterpriseName, enterpriseName, lookup[enterpriseName].keyhash, true);
    }

    function verifyEntry(string userId) public {
        require(msg.sender == manager, "Cannot register unless manager.");
        lookup[userId].verified = true;
        reverseLookup[userId].verified = true;
    }

    function resolve(string userId) view public returns (string, string, bytes32, bool, bool){
        return (lookup[userId].userid, lookup[userId].publickey, lookup[userId].keyhash, lookup[userId].isEnterprise, lookup[userId].verified);
    }
    function resolveHash(string userId) view public returns (bytes32){
        return lookup[userId].keyhash;
    }
    function reverseResolve(string publickey) view public returns (string){
        return reverseLookup[publickey].userid;
    }
}
