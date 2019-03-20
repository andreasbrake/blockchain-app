using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;

namespace WorkflowHandler.Handlers
{
    class Submission
    {
        private readonly WORKFLOW_EXCHANGE.Responder _responder;
        private readonly IPFS_EXCHANGE.Requestor _dataRequestHandler;
        private readonly IPFS_EXCHANGE.Responder _dataResponseHandler;

        public Submission(
            WORKFLOW_EXCHANGE.Responder responder, 
            IPFS_EXCHANGE.Requestor dataRequestHandler, 
            IPFS_EXCHANGE.Responder dataResponseHandler
        )
        {
            this._responder = responder;
            this._dataRequestHandler = dataRequestHandler;
            this._dataResponseHandler = dataResponseHandler;
        }

        public void RequestHandler(string requstActionId, JObject parameters)
        {
            string actionId = requstActionId;

            Console.WriteLine($" [X] Request received for data with request id {actionId}");
            
            string module = parameters["moduleName"].ToString();
            string schema = parameters["objectName"].ToString();
            string name = parameters["objectId"].ToString();
            string draft = parameters["draftId"].ToString();
            JObject data = (JObject)parameters["data"];

            if(name.ToUpper() == "NEW") {
                name = (data["Name"]?.ToString() ?? Guid.NewGuid().ToString()).ToUpper();
            }

            this._responder.Send(actionId, JObject.FromObject(new {
                WORKFLOW_STATUS = "COMMITING_DOCUMENT",
                WORKFLOW_ERROR = ""
            }));

            Action<JObject> callback = (res) => {
                if(res["STATUS"].ToString() == "SUCCESS")
                {
                    Console.WriteLine($" [/] Workflow success for actionId id {actionId}");
                    this._responder.Send(actionId, JObject.FromObject(new {
                        WORKFLOW_STATUS = "SAVED_DOCUMENT",
                        DRAFT_FINAL_NAME = name
                    }));
                }
                else if(res["STATUS"].ToString() == "ERROR")
                {
                    Console.WriteLine($" [!] Workflow error for actionId id {actionId}: {res["ERROR"].ToString()}");
                    this._responder.Send(actionId, JObject.FromObject(new {
                        WORKFLOW_STATUS = "ERROR",
                        WORKFLOW_ERROR = res["ERROR"].ToString()
                    }));
                }
                else 
                {
                    Console.WriteLine($" [!] Unknown workflow response actionId id {actionId}: {res["STATUS"].ToString()}");
                    this._responder.Send(actionId, JObject.FromObject(new {
                        WORKFLOW_STATUS = "ERROR",
                        WORKFLOW_ERROR = $"UNKNOWN DATA RESPONSE {res["STATUS"].ToString()}"
                    }));
                }
                this._responder.EndRequest(actionId);
            };

            _dataRequestHandler.CommitData(
                module, 
                schema, 
                name, 
                parameters["objectId"].ToString().ToUpper() != "NEW",
                data, 
                _dataResponseHandler.AddListener(callback)
            );
        }
    }
}