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
    public class EchoSocketHandler : BaseSocketHandler
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
 
        public EchoSocketHandler() { }
        
        public override async Task Invoke(HttpContext context)
        {
            CancellationToken ct = context.RequestAborted;
            WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = Guid.NewGuid().ToString();
    
            _sockets.TryAdd(socketId, currentSocket);
    
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }
    
                var response = await ReceiveStringAsync(currentSocket, ct);
                if(string.IsNullOrEmpty(response))
                {
                    if(currentSocket.State != WebSocketState.Open)
                    {
                        break;
                    }
    
                    continue;
                }
    
                foreach (var socket in _sockets)
                {
                    if(socket.Value.State != WebSocketState.Open)
                    {
                        continue;
                    }
    
                    await SendStringAsync(socket.Value, response, ct);
                }
            }
    
            WebSocket dummy;
            _sockets.TryRemove(socketId, out dummy);
    
            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentSocket.Dispose();
        }
    }
}