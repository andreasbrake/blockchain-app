pragma solidity ^0.4.23;

contract Thimble {
    struct DocumentVersion
    {
        string id;
        uint version;
        uint timestamp;
    }

    struct Document
    {
        bool isValue;
        string name;
        DocumentVersion currentDocument;
        uint historySize;
        mapping (uint => DocumentVersion) history;
    }

    struct Schema
    {
        bool isValue;
        string name;
        string[] documentNames;
        mapping (string => Document) documents;
    }

    struct Module
    {
        bool isValue;
        string name;
        string[] schemaNames;
        mapping (string => Schema) schemas;
    }

    string public clientName;
    string[] public moduleNames;
    mapping (string => Module) modules;

    event CreationEvent(
        string client
    );
    event DebugLog(
        string message
    );
    event DocumentEvent(
        string module,
        string schema,
        string name,
        string documentId,
        uint historySize
    );

    constructor (string client) public { 
        clientName = client;
        emit CreationEvent(client);
    }

    function saveDocument(string module, string schema, string name, string documentId) public {
        if (!modules[module].isValue) {
            moduleNames.push(module);
            modules[module] = Module ({
                isValue: true,
                name: module,
                schemaNames: new string[](0)
            });
        }
        
        if (!modules[module].schemas[schema].isValue) {
            modules[module].schemaNames.push(schema);
            modules[module].schemas[schema] = Schema ({
                isValue: true,
                name: schema,
                documentNames: new string[](0)
            });
        }
        
        Document storage doc = modules[module].schemas[schema].documents[name];
        if (!doc.isValue) {
            modules[module].schemas[schema].documentNames.push(name);
            modules[module].schemas[schema].documents[name] = Document ({
                isValue: true,
                name: name,
                currentDocument: DocumentVersion({
                    id: documentId,
                    version: 0,
                    timestamp: block.timestamp
                }),
                historySize: 0
            });
            doc = modules[module].schemas[schema].documents[name];
        }
        else {
            doc.history[doc.historySize] = modules[module].schemas[schema].documents[name].currentDocument;
            doc.historySize++;
            doc.currentDocument = DocumentVersion({
                id: documentId,
                version: doc.historySize,
                timestamp: block.timestamp
            });
        }

        emit DocumentEvent(module, schema, name, documentId, doc.historySize);
    }

    function getModules () view public returns (string modulesNameList) {
        for (uint i = 0; i < moduleNames.length; i++) {
            if (i == 0) {
                modulesNameList = moduleNames[i];
            }
            else {
                modulesNameList = strConcat(modulesNameList, ",", moduleNames[i]);
            }
        }
        return modulesNameList;
    }
    function getSchemas (string module) view public returns (string schemaNameList) {
        Module memory m = modules[module];
        for (uint i = 0; i < m.schemaNames.length; i++) {
            if (i == 0) {
                schemaNameList = m.schemaNames[i];
            }
            else {
                schemaNameList = strConcat(schemaNameList, ",", m.schemaNames[i]);
            }
        }
        return schemaNameList;
    }
    function getDocuments (string module, string schema) view public returns (string documentNameList) {
        Schema memory s = modules[module].schemas[schema];
        for (uint i = 0; i < s.documentNames.length; i++) {
            if (i == 0) {
                documentNameList = s.documentNames[i];
            }
            else {
                documentNameList = strConcat(documentNameList, ",", s.documentNames[i]);
            }
        }
        return documentNameList;
    }

    function getDocument(string module, string schema, string name) view public returns (string documentId) {
        return modules[module].schemas[schema].documents[name].currentDocument.id;
    }
    function getDocumentVersionCount(string module, string schema, string name) view public returns (uint versionCount) {
        return modules[module].schemas[schema].documents[name].historySize;
    }

    function getDocumentByVersion(string module, string schema, string name, uint version) view public returns (string documentId) {
        Document storage doc = modules[module].schemas[schema].documents[name];
        if (version == doc.historySize) {
            return doc.currentDocument.id;
        }
        return doc.history[version].id;
    }
    function getDocumentByTimestamp(string module, string schema, string name, uint timestamp) view public returns (string documentId) {
        Document storage doc = modules[module].schemas[schema].documents[name];
        for (uint i = 0; i < doc.historySize; i++) {
            if (doc.history[i].timestamp == timestamp) {
                return doc.history[i].id;
            }
        }
    }

    function getVersionByTimestamp(string module, string schema, string name, uint timestamp) view public returns (uint version) {
        Document storage doc = modules[module].schemas[schema].documents[name];

        if (doc.currentDocument.timestamp == timestamp) {
            return doc.historySize;
        }

        for (uint i = 0; i < doc.historySize; i++) {
            if (doc.history[i].timestamp == timestamp) {
                return i;
            }
        }
    }
    function getVersionByDocumentId(string module, string schema, string name, string documentId) view public returns (uint version) {
        Document storage doc = modules[module].schemas[schema].documents[name];

        if (keccak256(bytes(doc.currentDocument.id)) == keccak256(bytes(documentId))) {
            return doc.historySize;
        }

        for (uint i = 0; i < doc.historySize; i++) {
            if (keccak256(bytes(doc.history[i].id)) == keccak256(bytes(documentId))) {
                return i;
            }
        }
    }

    function getTimestampByVersion(string module, string schema, string name, uint version) view public returns (uint timestamp) {
        Document storage doc = modules[module].schemas[schema].documents[name];
        if (version == doc.historySize) {
            return doc.currentDocument.timestamp;
        }
        return doc.history[version].timestamp;
    }
    function getTimestampByDocumentId(string module, string schema, string name, string documentId) view public returns (uint timestamp) {
        Document storage doc = modules[module].schemas[schema].documents[name];

        if (keccak256(bytes(doc.currentDocument.id)) == keccak256(bytes(documentId))) {
            return doc.currentDocument.timestamp;
        }

        for (uint i = 0; i < doc.historySize; i++) {
            if (keccak256(bytes(doc.history[i].id)) == keccak256(bytes(documentId))) {
                return doc.history[i].timestamp;
            }
        }
    }
    
    function strConcat(string _a, string _b, string _c) pure internal returns (string){
        bytes memory _ba = bytes(_a);
        bytes memory _bb = bytes(_b);
        bytes memory _bc = bytes(_c);
        string memory abc = new string(_ba.length + _bb.length + _bc.length);
        bytes memory babc = bytes(abc);
        uint k = 0;
        for (uint i = 0; i < _ba.length; i++) babc[k++] = _ba[i];
        for (i = 0; i < _bb.length; i++) babc[k++] = _bb[i];
        for (i = 0; i < _bc.length; i++) babc[k++] = _bc[i];
        return string(babc);
    }
}