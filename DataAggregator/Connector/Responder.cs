using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DataAggregator.Conecctor
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
            
            this.channel.ExchangeDeclare(exchange: "aggregator_response",
                                         type: "direct");
        }

        public void Send(string requestId, JObject message)
        {
            Console.WriteLine($"Sending message to exchange `aggregator_response` with requestId {requestId}");

            byte[] parts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            this.channel.BasicPublish(exchange: "aggregator_response",
                                      routingKey: requestId,
                                      basicProperties: null,
                                      body: parts);
        }

        public void EndRequest(string requestId)
        {
            Console.WriteLine($"Ending communication to {requestId}");
            
            this.channel.BasicPublish(exchange: "aggregator_response",
                                      routingKey: requestId,
                                      basicProperties: null,
                                      body: Encoding.UTF8.GetBytes("END"));
        }

        void IDisposable.Dispose() 
        {
            this.connection.Dispose();
            this.channel.Dispose();
        }
    }
}