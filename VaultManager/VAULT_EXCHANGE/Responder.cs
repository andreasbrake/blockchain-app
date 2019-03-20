using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace VaultManager.WORKFLOW_EXCHANGE
{
    class Responder: IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public Responder(string hostname)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = hostname };

            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            
            this.channel.ExchangeDeclare(exchange: "vault_response",
                                         type: "direct");
        }

        public void Send(string requestId, JObject message)
        {
            Console.WriteLine($" [*] Sending message to exchange `vault_response` with requestId {requestId}");

            byte[] parts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            this.channel.BasicPublish(exchange: "vault_response",
                                      routingKey: requestId,
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