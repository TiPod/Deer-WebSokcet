using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using static System.Net.Mime.MediaTypeNames;

namespace Deer.WebSockets
{
    public class DeerWebSocketMiddleware<TDeerWebSocket> where TDeerWebSocket : DeerWebSocket
    {
        private readonly RequestDelegate _next;
        private readonly DeerWebSocketOptions _options;
        private readonly PathString _requestPath;
        private readonly bool allAnyPath = false;

        public DeerWebSocketMiddleware(RequestDelegate next, IOptions<DeerWebSocketOptions<TDeerWebSocket>> options)

        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            _next = next;
            _options = options.Value;
            _requestPath = _options.Path;
            allAnyPath = _requestPath == PathString.FromUriComponent("/*");


        }

        public async Task Invoke(HttpContext context)
        {

            if (allAnyPath || string.Compare(context.Request.Path, _requestPath, true) == 0)
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var serviceProvider = context.RequestServices;
                    var connetionInternalManager = serviceProvider.GetRequiredService<IDeerWebSocketConnetionInternalManager<TDeerWebSocket>>();
                    var webSocket = serviceProvider.GetRequiredService<TDeerWebSocket>();
                    var socket = await context.WebSockets.AcceptWebSocketAsync();
                    await webSocket.HandleAceeptWebSocketAsync(socket, _options);
                    await connetionInternalManager.AddAsync(webSocket);
                    try
                    {
                        await webSocket.ProcessRequestAsync();
                    }
                    finally
                    {
                        await connetionInternalManager.RemoveAsync(webSocket);
                    }
                    return;
                }
            }
            await _next(context);



        }
    }
}