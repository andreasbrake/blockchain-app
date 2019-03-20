using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VaultManager.IDENTITY_EXCHANGE
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
            
            this.channel.ExchangeDeclare(exchange: "identity_request",
                                         type: "direct");

            this.channel.ExchangeDeclare(exchange: "identity_response",
                                         type: "direct");
        }

        public void GetUser(string userId, Action<JObject> responseHandler)
        {
            Guid requestId = Guid.NewGuid();
            
            AddListener(requestId.ToString(), responseHandler);

            Console.WriteLine($" [*] Sending message to exchange `identity_request` with requestId {requestId}");

            byte[] parts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
                REQUEST_ID = requestId,
                USER_ID = userId
            }));

            this.channel.BasicPublish(exchange: "identity_request",
                                      routingKey: "get",
                                      basicProperties: null,
                                      body: parts);
        }

        public void AddListener(string requestId, Action<JObject> messageHandler)
        {
            this.channel.QueueBind(queue: this.channel.QueueDeclare($"VAULT-MANAGER-IDENTITY-GET-QUEUE-{requestId}").QueueName,
                                exchange: "identity_response",
                                routingKey: requestId);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) => 
            {
                string body = Encoding.UTF8.GetString(ea.Body);
                JObject response = JObject.Parse(body);
                messageHandler(response);
            };

            channel.BasicConsume(queue: $"VAULT-MANAGER-IDENTITY-GET-QUEUE-{requestId}",
                                autoAck: true,
                                consumer: consumer);

            Console.WriteLine($" [*] Added listener to queue `VAULT-MANAGER-IDENTITY-GET-QUEUE-{requestId}`");
        }


        void IDisposable.Dispose() 
        {
            this.connection.Dispose();
            this.channel.Dispose();
        }
    }
}
