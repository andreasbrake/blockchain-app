using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Models.Configuration;
using BlockchainAppAPI.Models.Search;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace BlockchainAppAPI.Logic.Data
{
    public class DataManager: IDataManager
    {
        private readonly IConnection connection;
        private readonly IModel requestChannel;
        private readonly IModel responseChannel;
        private readonly string responseQueue;

        private SystemContext _dbContext;

        public DataManager(SystemContext context)
        {
            this._dbContext = context;
            
            ConnectionFactory factory = new ConnectionFactory() { HostName = "10.0.75.2" };

            this.connection = factory.CreateConnection();

            this.requestChannel = connection.CreateModel();
            this.requestChannel.ExchangeDeclare(exchange: "aggregator_request",
                                                type: "direct");

                                 
            this.responseChannel = connection.CreateModel();     
            this.responseChannel.ExchangeDeclare(exchange: "aggregator_response",
                                                 type: "direct");

            this.responseQueue = this.responseChannel.QueueDeclare($"PATENT-LISTENER-{Guid.NewGuid().ToString()}").QueueName;

            Console.WriteLine($"Listening for RabbitMQ data on queue `aggregator_response.{this.responseQueue}`");
        }

        public async Task PushToStore(string key, JObject data)
        {
            using(ConnectionMultiplexer multiplex = ConnectionMultiplexer.Connect("10.0.75.2:6379"))
            {
                ISubscriber subscription = multiplex.GetSubscriber();
                IDatabase dataSource = multiplex.GetDatabase(1);
                
                HashEntry[] oldHash = await dataSource.HashGetAllAsync(key);
                JObject baseData = JObject.FromObject(
                    oldHash.ToDictionary(
                        he => he.Name,
                        he => he.Value.ToString()
                    )
                );

                baseData.Merge(data);

                HashEntry[] hash = baseData.ToObject<Dictionary<string, string>>().Select(kv =>
                    new HashEntry(kv.Key, kv.Value)
                ).ToArray();

                await dataSource.HashSetAsync(key, hash);
                await subscription.PublishAsync(key, JsonConvert.SerializeObject(data));
            }
        }

        public async Task<BaseDataModel> LookupObject(string moduleName, string objectName, string objectId)
        {
            List<ObjectField> fields = await this._dbContext.ObjectFields.Where(f =>
                f.Object.Module.Name == moduleName && f.Object.Name == objectName
            ).ToListAsync();

            string fieldLookup = $@"
                SELECT 
                    [{objectName}Id] as [ObjectId], 
                    [DateCreated], 
                    [CreatedBy], 
                    [DateModified], 
                    [ModifiedBy], 
                    '{{{{' + { 
                        String.Join(
                            " + ',' + ", 
                            fields.Select(f => 
                                String.Format(@"'""{0}"":' + ISNULL('""' + REPLACE(REPLACE(CONVERT(NVARCHAR(MAX), {0}), '\', '\\'), '""', '\""') + '""', 'null')", f.Name) 
                            )
                        ) 
                    } + '}}}}' as [DataBlob] 
                FROM [dbo].[{moduleName}_{objectName}] 
                WHERE [{objectName}Id] = @OID";

            return await this._dbContext.Data
                .FromSql(fieldLookup, new SqlParameter("OID", objectId))
                .OrderByDescending(o => o.DateModified)
                .FirstOrDefaultAsync();
        }

        public async Task RequestData(string moduleName, string objectName, string objectId)
        {
            // BaseDataModel response = await this.LookupObject(moduleName, objectName, objectId);
            // JObject data = response.Data;
            JObject parameters = JObject.FromObject(new
            {
                moduleName = moduleName,
                objectName = objectName,
                objectId = objectId
            });

            string requestId = RequestDataFromAggregator(
                parameters, 
                async (data) => 
                {
                    await this.PushToStore($"{moduleName}-{objectName}-{objectId}", data);
                }
            );

            await this.PushToStore($"{moduleName}-{objectName}-{objectId}", JObject.FromObject(new
            {
                CACHE_STATUS = "DATA_REQUEST",
                REQUEST_ID = requestId
            }));
        }

        private string RequestDataFromAggregator(JObject parameters, Action<JObject> handler)
        {
            string requestId = Guid.NewGuid().ToString();

            this.responseChannel.QueueBind(queue: this.responseQueue,
                                           exchange: "aggregator_response",
                                           routingKey: requestId);

            EventingBasicConsumer consumer = new EventingBasicConsumer(responseChannel);

            consumer.Received += (model, ea) => 
            {
                string response = Encoding.Unicode.GetString(ea.Body);
                
                if(response == "END")
                {
                    this.responseChannel.QueueUnbind(queue: this.responseQueue,
                                                    exchange: "aggregator_response",
                                                    routingKey: requestId);
                    handler(JObject.FromObject(new 
                    {
                        CACHE_STATUS = "DONE"
                    }));
                }
                else
                {
                    handler(JObject.Parse(response));
                }
            };

            this.responseChannel.BasicConsume(queue: this.responseQueue,
                                              autoAck: true,
                                              consumer: consumer);

            byte[] parts = Encoding.Unicode.GetBytes(
                JsonConvert.SerializeObject(
                    JObject.FromObject(new
                    {
                        RequestId = requestId,
                        Parameters = parameters
                    })
                )
            );

            this.requestChannel.BasicPublish(exchange: "aggregator_request",
                                             routingKey: "PATENT",
                                             basicProperties: null,
                                             body: parts);

            return requestId;
        }
    }
}