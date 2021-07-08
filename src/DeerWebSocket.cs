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


        private int _receiveBufferSize;

        private int _sendBufferSize;

        protected WebSocket webSocket;

        private CancellationTokenSource _cancellationTokenSource;


        private WebSocketCloseStatus _webSocketCloseStatus = WebSocketCloseStatus.Empty;

        internal void Initialize(DeerWebSocketOptions options)
        {
            _receiveBufferSize = options?.ReceiveBufferSize ?? 4 * 1024;
            _sendBufferSize = options?.SendBufferSize ?? 4 * 1024;
        }
        internal async Task HandleAceeptWebSocketAsync(HttpContext context, CancellationToken cancellationToken = default)
        {

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Id = Guid.NewGuid().ToString("N");
            Context = context;
            webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await OnConnectedAsync();
        }








        internal async Task ProcessRequestAsync()
        {

            var revBuffers = new List<byte>();
            var buffer = new byte[_receiveBufferSize];
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                while (!result.CloseStatus.HasValue)
                {
                    //追加获取的字节
                    revBuffers.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                    if (result.EndOfMessage)
                    {
                        var revMsg = Encoding.UTF8.GetString(revBuffers.ToArray());
                        await ReceiveAsync(revMsg, _cancellationTokenSource.Token);
                        revBuffers.Clear();
                    }

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                }
                _webSocketCloseStatus = result.CloseStatus.Value;
            }
            catch (OperationCanceledException) //操作被取消
            {
                _webSocketCloseStatus = WebSocketCloseStatus.NormalClosure;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _webSocketCloseStatus = WebSocketCloseStatus.InternalServerError;
            }
            finally
            {
                await webSocket.CloseAsync(_webSocketCloseStatus, _webSocketCloseStatus.ToString(), _cancellationTokenSource.Token);
                await OnCloseedAsync(_webSocketCloseStatus);
            }
        }

        public virtual Task OnConnectedAsync()
        {
            return Task.CompletedTask;
        }

        public abstract Task ReceiveAsync(string message, CancellationToken cancellationToken);

        public virtual async Task SendAsync(string message, CancellationToken cancellationToken = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, cts.Token);
        }

        public virtual Task CloseAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.Empty)
        {
            _webSocketCloseStatus = closeStatus;
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        public virtual Task OnCloseedAsync(WebSocketCloseStatus closeStatus)
        {
            return Task.CompletedTask;
        }


    }
}