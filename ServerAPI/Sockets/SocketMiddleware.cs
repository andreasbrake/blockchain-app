using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Logic.Data;

namespace BlockchainAppAPI.Sockets
{
    //[Authorize]
    public class SocketMiddleware
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly RequestDelegate _next;
        private string _userId;
        private IDataManager _dm;
        
        public SocketMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IDataManager dm)
        {
            _next = next;
            _userId = httpContextAccessor.CurrentUser() ?? "guest";
            _dm = dm;
        }
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            BaseSocketHandler socket = new EchoSocketHandler();
            switch(context.Request.Path)
            {
                case "/data":
                    socket = new DataSocketHandler(_dm);
                    break;
                case "/session":
                    socket = new SessionSocketHandler(_userId);
                    break;
            }
            await socket.Invoke(context);
        }
    }
}