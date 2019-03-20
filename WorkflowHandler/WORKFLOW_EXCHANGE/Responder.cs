using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WorkflowHandler.WORKFLOW_EXCHANGE
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
            
            this.channel.ExchangeDeclare(exchange: "workflow_response",
                                         type: "direct");
        }

        public void Send(string actionId, JObject message)
        {
            Console.WriteLine($" [*] Sending message to exchange `workflow_response` with actionId {actionId}");

            byte[] parts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            this.channel.BasicPublish(exchange: "workflow_response",
                                      routingKey: actionId,
                                      basicProperties: null,
                                      body: parts);
        }

        public void EndRequest(string actionId)
        {
            Console.WriteLine($" [*] Ending communication to {actionId}");
            
            this.channel.BasicPublish(exchange: "workflow_response",
                                      routingKey: actionId,
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