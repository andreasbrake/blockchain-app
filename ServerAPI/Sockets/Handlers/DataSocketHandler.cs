using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.Logic.Data;
using StackExchange.Redis;

namespace BlockchainAppAPI.Sockets
{
    //[Authorize]
    public class DataSocketHandler : BaseSocketHandler
    {
        private static readonly string[] STATIC_PROPERTIES = new []
        {
            "CACHE_STATUS"
        };

        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private IDataManager _dm;
 
        public DataSocketHandler(IDataManager dm)
        { 
            this._dm = dm;
        }
        
        public override async Task Invoke(HttpContext context)
        {
            using(ConnectionMultiplexer _multiplex = ConnectionMultiplexer.Connect("10.0.75.2:6379"))
            {
                CancellationToken ct = context.RequestAborted;
                WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();
                var socketId = Guid.NewGuid().ToString();
        
                _sockets.TryAdd(socketId, currentSocket);

                // Database connection
                IDatabase dataStore = _multiplex.GetDatabase(1);
                // Pub/sub subscription
                ISubscriber subscription = _multiplex.GetSubscriber();

                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    /* e.g.
                    {
                        "Action": "SUBSCRIBE",
                        "Module":"patent",
                        "Object":"application",
                        "Id":"F19F133B-FCA1-4348-84DF-471DC74E1981",
                        "Query": {
                            ### RESULT SET FILTER ###
                        }
                    }
                    */
                    JObject response = await ReceiveInputAsync(currentSocket, ct); 

                    if(response == null)
                    {
                        if(currentSocket.State != WebSocketState.Open)
                        {
                            break;
                        }
        
                        continue;
                    }

                    string moduleName = response["Module"].ToString();
                    string objectName = response["Object"].ToString();
                    string objectId = response["Id"].ToString();
                    string key = $"{moduleName}-{objectName}-{objectId}";

                    JObject query = (JObject)response["Query"];

                    // SUBSCRIBE action
                    if(response["Action"].ToString() == "SUBSCRIBE") 
                    {
                        await subscription.UnsubscribeAsync(key);
                        await subscription.SubscribeAsync(key, async (channel, message) =>
                        {
                            Console.WriteLine("Got notification: " + (string)message);
                            if(currentSocket.State == WebSocketState.Open)
                            {
                                JObject parts = JObject.Parse(message);
                                await SendData(parts, query, currentSocket, ct);
                            }
                            else
                            {
                                await subscription.UnsubscribeAllAsync();
                            }
                        });

                        JObject initialData = ParseHash(await dataStore.HashGetAllAsync(key));
                        if(initialData.Values().Count() > 0)
                        {
                            await SendData(initialData, query, currentSocket, ct);
                        }

                        await this._dm.RequestData(moduleName, objectName, objectId);
                    }
                    // UNSUBSCRIBE action
                    else
                    {
                        await subscription.UnsubscribeAsync(key);
                    }
                }
        
                _sockets.TryRemove(socketId, out WebSocket dummy);
                await subscription.UnsubscribeAllAsync();
                await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
                currentSocket.Dispose();
            }
        }

        private static Task SendData(JObject data, JObject query, WebSocket socket, CancellationToken ct)
        {
            string response = JsonConvert.SerializeObject(
                JObject.FromObject(
                    data.ToObject<Dictionary<string, object>>().Where(s =>
                        Array.IndexOf(STATIC_PROPERTIES, s.Key) >= 0 || query[s.Key] != null
                    ).ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value
                    )
                )
            );

            if(response == "{}") return Task.CompletedTask;

            return SendStringAsync(socket, response, ct);
        }
    }
}