using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;

namespace VaultManager.Handlers
{
    class GetKey
    {
        private readonly WORKFLOW_EXCHANGE.Responder _responder;
        private readonly IDENTITY_EXCHANGE.Getter _identityGetter;

        public GetKey(
            WORKFLOW_EXCHANGE.Responder responder, 
            IDENTITY_EXCHANGE.Getter identityGetter
        )
        {
            this._responder = responder;
            this._identityGetter = identityGetter;
        }

        public void RequestHandler(string requstId, JObject parameters)
        {
            Console.WriteLine($" [X] Request received for data with request id {requstId}");
            
            string username = parameters["USER_NAME"].ToString();

            Action<JObject> identitySaveResponse = (res) => {
                if(res["STATUS"].ToString() == "SUCCESS")
                {
                    Console.WriteLine($" [/] Identity resolution success for requst id {requstId}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        PUBLIC_KEY = res["PUBLIC_KEY"].ToString()
                    }));
                }
                else if(res["STATUS"].ToString() == "ERROR")
                {
                    Console.WriteLine($" [!] Identity resolution error for requst id {requstId}: {res["ERROR"].ToString()}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        STATUS = "ERROR",
                        ERROR = res["ERROR"].ToString()
                    }));
                }
                else 
                {
                    Console.WriteLine($" [!] Unknown identity resolution response requst id {requstId}: {res["STATUS"].ToString()}");
                    this._responder.Send(requstId, JObject.FromObject(new {
                        STATUS = "ERROR",
                        ERROR = $"UNKNOWN RESPONSE {res["STATUS"].ToString()}"
                    }));
                }
            };

            _identityGetter.GetUser(
                username, 
                identitySaveResponse
            );
        }
    }
}