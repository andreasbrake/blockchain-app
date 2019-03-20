using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DataAggregator.IPFS_EXCHANGE
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
            
            this.channel.ExchangeDeclare(exchange: "data_get_response",
                                         type: "direct");
        }

        public Action<string> AddListener(Action<JObject> messageHandler)
        {
            return (requestId) => {
                this.channel.QueueBind(queue: this.channel.QueueDeclare($"DATA-AGGREGATOR-DATA-QUEUE-{requestId}").QueueName,
                                    exchange: "data_get_response",
                                    routingKey: requestId);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) => 
                {
                    string body = Encoding.UTF8.GetString(ea.Body);
                    JObject response = JObject.Parse(body);
                    messageHandler(response);
                };

                channel.BasicConsume(queue: $"DATA-AGGREGATOR-DATA-QUEUE-{requestId}",
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine($" [*] Added listener to queue `DATA-AGGREGATOR-DATA-QUEUE-{requestId}`");
            };
        }

        void IDisposable.Dispose() 
        {
            this.connection.Dispose();
            this.channel.Dispose();
        }
    }
}