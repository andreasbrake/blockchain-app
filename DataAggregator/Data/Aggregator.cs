using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DataAggregator.Conecctor;
using System.Threading;

namespace DataAggregator.Data
{
    class Aggregator
    {
        private readonly Responder _responder;
        private readonly IPFS_EXCHANGE.Requestor _dataRequestor;
        private readonly IPFS_EXCHANGE.Responder _dataResponder;

        public Aggregator(Responder responder, IPFS_EXCHANGE.Requestor dataRequestor, IPFS_EXCHANGE.Responder dataResponder)
        {
            this._responder = responder;
            this._dataRequestor = dataRequestor;
            this._dataResponder = dataResponder;
        }

        public void RequestHandler(string inputRequestId, JObject parameters)
        {
            string requestId = inputRequestId;
            Console.WriteLine($" [X] Request received for data with request id {requestId}");

            string module = parameters["moduleName"].ToString();
            string schema = parameters["objectName"].ToString();
            string name = parameters["objectId"].ToString();


            this._responder.Send(requestId, JObject.FromObject(new {
                CACHE_STATUS = "GETTING_DOCUMENT",
                CACHE_ERROR = ""
            }));

            Action<JObject> callback = (res) => {
                this._responder.Send(requestId, res);
                this._responder.EndRequest(requestId);
            };

            Console.WriteLine($" [*] Getting latest document at {module}-{schema}-{name}");

            _dataRequestor.GetCurrentDocument(
                module, 
                schema, 
                name,
                _dataResponder.AddListener(callback)
            );
        }
    }
}