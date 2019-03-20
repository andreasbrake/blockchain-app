using System;
using System.Threading;
using DataAggregator.Conecctor;
using DataAggregator.Data;
using Newtonsoft.Json.Linq;

namespace DataAggregator
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

            Console.WriteLine($" [#] Starting Data-Aggregator listening to bus at {MQ_ENDPOINT}");

            using(Requestor rq = new Requestor(MQ_ENDPOINT))
            using(Responder re = new Responder(MQ_ENDPOINT))
            using(IPFS_EXCHANGE.Requestor drq = new IPFS_EXCHANGE.Requestor(MQ_ENDPOINT))
            using(IPFS_EXCHANGE.Responder dre = new IPFS_EXCHANGE.Responder(MQ_ENDPOINT))
            {
                rq.AddListener(new Aggregator(re, drq, dre).RequestHandler);

                while(true) {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
