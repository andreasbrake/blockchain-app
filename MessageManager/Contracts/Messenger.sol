pragma solidity ^0.4.24;

contract Messenger {
    address manager;

    event MessengerCreated(
        address manager
    );

    event MessageSent(
        string indexed userkeyTo,
        string indexed userkeyFrom,
        string toMessage,
        string fromMessage
    );

    constructor () public { 
        manager = msg.sender;
        emit MessengerCreated(manager);
    }

    function sendMessage(string userkeyTo, string userkeyFrom, string toMessage, string fromMessage) public {
        emit MessageSent(userkeyTo, userkeyFrom, toMessage, fromMessage);
    }
}
