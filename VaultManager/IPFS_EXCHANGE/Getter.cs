using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VaultManager.IPFS_EXCHANGE
{
    class Getter: IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public Getter(string hostname)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = hostname };

            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            
            this.channel.ExchangeDeclare(exchange: "data_get_request",
                                         type: "direct");

            this.channel.ExchangeDeclare(exchange: "data_get_response",
                                         type: "direct");
        }

        public void GetDocument(string module, string schema, string name, JObject data, Action<JObject> responseHandler)
        {
            Guid requestId = Guid.NewGuid();
            
            AddListener(requestId.ToString(), responseHandler);

            Console.WriteLine($" [*] Sending message to exchange `data_get_request` with requestId {requestId}");

            byte[] parts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
                REQUEST_ID = requestId,
                MODULE = module,
                SCHEMA = schema,
                NAME = name
            }));

            this.channel.BasicPublish(exchange: "data_get_request",
                                      routingKey: "",
                                      basicProperties: null,
                                      body: parts);
        }

        public void AddListener(string requestId, Action<JObject> messageHandler)
        {
            this.channel.QueueBind(queue: this.channel.QueueDeclare($"VAULT-MANAGER-DATA-GET-QUEUE-{requestId}").QueueName,
                                    exchange: "data_get_response",
                                    routingKey: requestId);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) => 
            {
                string body = Encoding.UTF8.GetString(ea.Body);
                JObject response = JObject.Parse(body);
                messageHandler(response);
            };

            channel.BasicConsume(queue: $"VAULT-MANAGER-DATA-GET-QUEUE-{requestId}",
                                autoAck: true,
                                consumer: consumer);

            Console.WriteLine($" [*] Added listener to queue `VAULT-MANAGER-DATA-GET-QUEUE-{requestId}`");
     }


        void IDisposable.Dispose() 
        {
            this.connection.Dispose();
            this.channel.Dispose();
        }
    }
}
