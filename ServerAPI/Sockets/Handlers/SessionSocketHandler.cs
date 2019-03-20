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
using StackExchange.Redis;

namespace BlockchainAppAPI.Sockets
{
    //[Authorize]
    public class SessionSocketHandler : BaseSocketHandler
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _sockets = new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();
        private string _userId;
 
        public SessionSocketHandler(string userId) 
        { 
            this._userId = userId;
        }
        
        public override async Task Invoke(HttpContext context)
        {
            using(ConnectionMultiplexer _multiplex = ConnectionMultiplexer.Connect("10.0.75.2:6379"))
            {
                CancellationToken ct = context.RequestAborted;
                WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();
                
                if(!_sockets.ContainsKey(this._userId)) {
                    _sockets.TryAdd(this._userId, new ConcurrentDictionary<string, WebSocket>());
                }
                string socketId = Guid.NewGuid().ToString();
                _sockets[this._userId].TryAdd(socketId, currentSocket);

                // Pub/sub subscription
                ISubscriber subscription = _multiplex.GetSubscriber();
                // Database connection
                IDatabase cache = _multiplex.GetDatabase(2);

                // Return initial value on connection
                JObject currentValue = ParseHash(await cache.HashGetAllAsync($"session-{this._userId}"));
                if(currentSocket.State == WebSocketState.Open)
                {
                    await SendStringAsync(currentSocket, JsonConvert.SerializeObject(currentValue), ct);
                }
                

                // listent to session value changes
                await subscription.SubscribeAsync($"session-{this._userId}", subscriptionHandler(subscription, currentSocket, ct));
        
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
        
                    string response = await ReceiveStringAsync(currentSocket, ct);
                    if(string.IsNullOrEmpty(response))
                    {
                        if(currentSocket.State != WebSocketState.Open)
                        {
                            break;
                        }
        
                        continue;
                    }

                    JObject newValues = JObject.Parse(response);

                    // TODO: Prevent publishing of specific state information

                    // Publish incremental update
                    await subscription.PublishAsync($"session-{this._userId}", response);
                    
                    HashEntry[] oldValue = await cache.HashGetAllAsync($"session-{this._userId}");
                    JObject baseSession = ParseHash(oldValue);

                    baseSession.Merge(newValues);

                    // Commit full merge
                    await cache.HashSetAsync($"session-{this._userId}", ParseToHash(baseSession));
                }
        
                _sockets[this._userId].TryRemove(socketId, out WebSocket dummy);
                await subscription.UnsubscribeAllAsync();
                await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
                currentSocket.Dispose();

                if(_sockets[this._userId].Values.Count == 0) {
                    _sockets.TryRemove(this._userId, out ConcurrentDictionary<string, WebSocket> x);
                }
            }
        }

        private static Action<RedisChannel, RedisValue> subscriptionHandler(ISubscriber subscription, WebSocket socket, CancellationToken ct) 
        {
            return async (chanel, message) => {
                Console.WriteLine("Session update: " + (string)message);

                if(socket.State == WebSocketState.Open)
                {
                    await SendStringAsync(socket, message, ct);
                }
                else
                {
                    await subscription.UnsubscribeAllAsync();
                }
            };
        }
    }
}