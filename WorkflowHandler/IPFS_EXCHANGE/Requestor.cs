using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkflowHandler.IPFS_EXCHANGE
{
    class Requestor: IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public Requestor(string hostname)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = hostname };

            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            
            this.channel.ExchangeDeclare(exchange: "data_save_request",
                                         type: "direct");
        }

        public void CommitData(string module, string schema, string name, bool isUpdate, JObject data, Action<string> responseHandler)
        {
            Guid requestId = Guid.NewGuid();
            
            responseHandler(requestId.ToString());

            Console.WriteLine($" [*] Sending message to exchange `data_save_request` with requestId {requestId}");

            byte[] parts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
                REQUEST_ID = requestId,
                MODULE = module,
                SCHEMA = schema,
                NAME = name,
                IS_UPDATE = isUpdate,
                DATA = data
            }));

            this.channel.BasicPublish(exchange: "data_save_request",
                                      routingKey: "",
                                      basicProperties: null,
                                      body: parts);
        }


        void IDisposable.Dispose() 
        {
            this.connection.Dispose();
            this.channel.Dispose();
        }
    }
}
