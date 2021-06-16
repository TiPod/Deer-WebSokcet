using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deer.WebSockets
{
    public abstract class DeerWebSocket
    {
        public string Id { get; private set; }

        protected WebSocket webSocket;

        protected TaskCompletionSource<object> tcs;

        internal async Task ProcessRequestAsync(IDeerWebSocketConnetionInternalManager connetionInternalManager, WebSocket socket, DeerWebSocketOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            tcs = new TaskCompletionSource<object>();
            webSocket = socket;
            Id = Guid.NewGuid().ToString("N");
            await connetionInternalManager.AddAsync(this);

            await OnConnectedAsync(cancellationToken);
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
                tcs.SetResult(default);
            }
            await connetionInternalManager.RemoveAsync(this);
            await tcs.Task;
        }

        public virtual Task OnConnectedAsync(CancellationToken cancellationToken)
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