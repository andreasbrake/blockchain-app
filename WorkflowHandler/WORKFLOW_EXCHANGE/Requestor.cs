using System;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkflowHandler.WORKFLOW_EXCHANGE
{
    class Requestor: IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        private int _connection_attempts;
        public Requestor(string hostname)
        {
            this._connection_attempts = 0;
            this.connection = connect(hostname);

            this.channel = connection.CreateModel();
            
            this.channel.ExchangeDeclare(exchange: "workflow_request",
                                         type: "direct");
        }
        private IConnection connect(string hostname)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = hostname };
                return factory.CreateConnection();
            }
            catch(Exception e)
            {
                Console.WriteLine($" [!] Error connecting to MQ. Retrying in 2 seconds...");

                if(_connection_attempts < 15)
                {
                    Thread.Sleep(2000);
                    return connect(hostname);
                }
                else
                {
                    throw e;
                }
            }
        }

        public void AddListener(string action, Action<string, JObject> messageHandler)
        {
            string queueId = Guid.NewGuid().ToString();

            this.channel.QueueBind(queue: this.channel.QueueDeclare($"WORKFLOW-HANDLER-QUEUE-{action}-{queueId}").QueueName,
                                   exchange: "workflow_request",
                                   routingKey: action);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) => 
            {
                string body = Encoding.UTF8.GetString(ea.Body);
                JObject request = JObject.Parse(body);
                string id = request["ActionId"].ToString();
                JObject parameters = (JObject)request["Parameters"];
                messageHandler(id, parameters);
            };

            channel.BasicConsume(queue: $"WORKFLOW-HANDLER-QUEUE-{action}-{queueId}",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine($" [*] Added listener to queue `WORKFLOW-HANDLER-QUEUE-{action}-{queueId}`");
        }

        void IDisposable.Dispose() 
        {
            this.connection.Dispose();
            this.channel.Dispose();
        }
    }
}
