using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;

namespace VaultManager.Handlers
{
    class GetVault
    {
        private readonly WORKFLOW_EXCHANGE.Responder _responder;
        private readonly IPFS_EXCHANGE.Getter _dataRequestGetter;

        public GetVault(
            WORKFLOW_EXCHANGE.Responder responder, 
            IPFS_EXCHANGE.Getter dataRequestGetter
        )
        {
            this._responder = responder;
            this._dataRequestGetter = dataRequestGetter;
        }

        public void RequestHandler(string requstId, JObject parameters)
        {
            Console.WriteLine($" [X] Request received for data with request id {requstId}");
            
            string username = parameters["USER_NAME"].ToString();

            Action<JObject> vaultDocumentResponse = (res) => {
                if(res["STATUS"].ToString() == "SUCCESS")
                {
                    Console.WriteLine($" [/] Vault get success for requst id {requstId}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        VAULT = res["VAULT"].ToString()
                    }));
                }
                else if(res["STATUS"].ToString() == "ERROR")
                {
                    Console.WriteLine($" [!] Vault get error for requst id {requstId}: {res["ERROR"].ToString()}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        STATUS = "ERROR",
                        ERROR = res["ERROR"].ToString()
                    }));
                }
                else 
                {
                    Console.WriteLine($" [!] Unknown vault get  response requst id {requstId}: {res["STATUS"].ToString()}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        STATUS = "ERROR",
                        ERROR = $"UNKNOWN RESPONSE {res["STATUS"].ToString()}"
                    }));
                }
            };

            _dataRequestGetter.GetDocument(
                "VAULT", 
                "USER", 
                username,
                JObject.FromObject(new {
                    Username = username,
                    DateCreated = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds
                }), 
                vaultDocumentResponse
            );
        }
    }
}