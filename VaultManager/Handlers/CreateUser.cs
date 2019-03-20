using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;

namespace VaultManager.Handlers
{
    class CreateUser
    {
        private readonly WORKFLOW_EXCHANGE.Responder _responder;
        private readonly IPFS_EXCHANGE.Saver _dataRequestSaver;
        private readonly IDENTITY_EXCHANGE.Creator _identityCreator;

        public CreateUser(
            WORKFLOW_EXCHANGE.Responder responder, 
            IPFS_EXCHANGE.Saver dataRequestSaver, 
            IDENTITY_EXCHANGE.Creator identityCreator
        )
        {
            this._responder = responder;
            this._dataRequestSaver = dataRequestSaver;
            this._identityCreator = identityCreator;
        }

        public void RequestHandler(string requstId, JObject parameters)
        {
            Console.WriteLine($" [X] Request received for data with request id {requstId}");
            
            string username = parameters["USER_NAME"].ToString();
            string publickey = parameters["PUBLIC_KEY"].ToString();


            Action<JObject> vaultSaveResponse = (res) => {
                if(res["STATUS"].ToString() == "SUCCESS")
                {
                    Console.WriteLine($" [/] Vault save success for requst id {requstId}");
                }
                else if(res["STATUS"].ToString() == "ERROR")
                {
                    Console.WriteLine($" [!] Vault save error for requst id {requstId}: {res["ERROR"].ToString()}");
                }
                else 
                {
                    Console.WriteLine($" [!] Unknown vault save response requst id {requstId}: {res["STATUS"].ToString()}");
                }
            };

            Action<JObject> identitySaveResponse = (res) => {
                if(res["STATUS"].ToString() == "SUCCESS")
                {
                    Console.WriteLine($" [/] Identity save success for requst id {requstId}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        USER_ID = username
                    }));
                }
                else if(res["STATUS"].ToString() == "ERROR")
                {
                    Console.WriteLine($" [!] Identity save error for requst id {requstId}: {res["ERROR"].ToString()}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        STATUS = "ERROR",
                        ERROR = res["ERROR"].ToString()
                    }));
                }
                else 
                {
                    Console.WriteLine($" [!] Unknown identity save response requst id {requstId}: {res["STATUS"].ToString()}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        STATUS = "ERROR",
                        ERROR = $"UNKNOWN RESPONSE {res["STATUS"].ToString()}"
                    }));
                }
            };

            _dataRequestSaver.UpdateVault(
                "VAULT", 
                "USER", 
                username, 
                false,
                JObject.FromObject(new {
                    Username = username,
                    DateCreated = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds
                }), 
                vaultSaveResponse
            );

            _identityCreator.CreateUser(
                username, 
                publickey, 
                identitySaveResponse
            );
        }
    }
}