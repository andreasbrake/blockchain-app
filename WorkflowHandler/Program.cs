using System;
using System.Threading;
using WorkflowHandler.WORKFLOW_EXCHANGE;
using WorkflowHandler.Handlers;
using Newtonsoft.Json.Linq;

namespace WorkflowHandler
{
    class Program
    {
        public static string MQ_ENDPOINT = "10.0.75.2";
        static void Main(string[] args)
        {
            for(int i=0; i < args.Length; i++)
            {
                switch(args[i])
                {
                    case "--mq":
                        MQ_ENDPOINT = args.Length > i ? args[i + 1] : null;
                        break;
                }
            }

            Console.WriteLine($" [#] Starting Workflow-Manager listening to bus at {MQ_ENDPOINT}");

            using(WORKFLOW_EXCHANGE.Requestor rq = new WORKFLOW_EXCHANGE.Requestor(MQ_ENDPOINT))
            using(WORKFLOW_EXCHANGE.Responder re = new WORKFLOW_EXCHANGE.Responder(MQ_ENDPOINT))
            using(IPFS_EXCHANGE.Requestor drq = new IPFS_EXCHANGE.Requestor(MQ_ENDPOINT))
            using(IPFS_EXCHANGE.Responder dre = new IPFS_EXCHANGE.Responder(MQ_ENDPOINT))
            {
                // Create a new object
                rq.AddListener("CREATE", new Submission(re, drq, dre).RequestHandler);
                // Update an existing object
                rq.AddListener("SAVE_EDIT", new SaveEdit(re, drq, dre).RequestHandler);
                // Create or update data
                rq.AddListener("SUBMIT", new Submission(re, drq, dre).RequestHandler);


                while(true) {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
