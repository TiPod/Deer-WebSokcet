using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Deer.WebSockets
{
    public abstract class DeerWebSocket
    {
        public string Id { get; protected set; }

        public HttpContext Context { get; private set; }

        protected WebSocket webSocket;

        protected TaskCompletionSource<object> tcs;



        internal async Task ProcessRequestAsync(HttpContext context, DeerWebSocketOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            tcs = new TaskCompletionSource<object>();
            Context = context;

            var connetionInternalManager = context.RequestServices.GetRequiredService<IDeerWebSocketConnetionInternalManager>();
            webSocket = await context.WebSockets.AcceptWebSocketAsync();

            Id = Guid.NewGuid().ToString("N");
            await OnConnectedAsync(context, cancellationToken);
            await connetionInternalManager.AddAsync(this);
            try
            {
                var revBuffers = new List<byte>();
                var buffer = new byte[options.ReceiveBufferSize];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                while (!result.CloseStatus.HasValue)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    //追加获取的字节
                    revBuffers.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                    if (result.EndOfMessage)
                    {
                        var revMsg = Encoding.UTF8.GetString(revBuffers.ToArray());
                        await ReceiveAsync(revMsg, cancellationToken);
                        revBuffers.Clear();
                    }

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                }
                await CloseAsync(result.CloseStatus.Value, cancellationToken);
            }
            catch (Exception)
            {
                await CloseAsync(WebSocketCloseStatus.InternalServerError, cancellationToken);
            }
            finally
            {
                await connetionInternalManager.RemoveAsync(this);
                tcs.SetResult(default);
            }

            await tcs.Task;
        }

        public virtual Task OnConnectedAsync(HttpContext Context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public abstract Task ReceiveAsync(string message, CancellationToken cancellationToken);

        public virtual async Task SendAsync(string message, CancellationToken cancellationToken = default)
        {
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, cancellationToken);
        }

        public virtual async Task CloseAsync(WebSocketCloseStatus closeStatus, CancellationToken cancellationToken = default)
        {
            if (webSocket.State == WebSocketState.Open)
                await webSocket.CloseAsync(closeStatus, nameof(closeStatus), cancellationToken);
            await OnCloseedAsync(closeStatus, cancellationToken);
        }

        public virtual Task OnCloseedAsync(WebSocketCloseStatus closeStatus, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
    }
}