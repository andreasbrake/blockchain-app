using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace BlockchainAppAPI.Sockets
{
    public abstract class BaseSocketHandler
    {
        public abstract Task Invoke(HttpContext context);
        
        protected static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        protected static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();
    
                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);
    
                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }
    
                // Encoding UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        protected static async Task<JObject> ReceiveInputAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            string response = await ReceiveStringAsync(socket, ct);
            if(String.IsNullOrEmpty(response))
            {
                return null;
            }
            return JObject.Parse(response);
        }

        protected static JObject ParseHash(HashEntry[] hashEntry)
        {
            return JObject.FromObject(
                hashEntry.ToDictionary(
                    he => he.Name,
                    he => he.Value.ToString()
                )
            );
        }

        protected static HashEntry[] ParseToHash(JObject obj)
        {
            return obj.ToObject<Dictionary<string, string>>().Select(kv =>
                new HashEntry(kv.Key, kv.Value)
            ).ToArray();
        }
    }
}