using System;
using System.Threading;
using VaultManager.WORKFLOW_EXCHANGE;
using VaultManager.Handlers;
using Newtonsoft.Json.Linq;

namespace VaultManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using(WORKFLOW_EXCHANGE.Requestor rq = new WORKFLOW_EXCHANGE.Requestor("10.0.75.2"))
            using(WORKFLOW_EXCHANGE.Responder re = new WORKFLOW_EXCHANGE.Responder("10.0.75.2"))
            using(IPFS_EXCHANGE.Saver ipfs_s = new IPFS_EXCHANGE.Saver("10.0.75.2"))
            using(IPFS_EXCHANGE.Getter ipfs_g = new IPFS_EXCHANGE.Getter("10.0.75.2"))
            using(IDENTITY_EXCHANGE.Creator id_c = new IDENTITY_EXCHANGE.Creator("10.0.75.2"))
            using(IDENTITY_EXCHANGE.Getter id_g = new IDENTITY_EXCHANGE.Getter("10.0.75.2"))
            {
                // Create a user vaulr
                rq.AddListener("create", new CreateUser(re, ipfs_s, id_c).RequestHandler);
                // Get an existing user public key
                rq.AddListener("resolve", new GetKey(re, id_g).RequestHandler);
                // Get an existing user vault
                rq.AddListener("get", new GetVault(re, ipfs_g).RequestHandler);


                while(true) {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
