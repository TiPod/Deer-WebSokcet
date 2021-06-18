using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Deer.WebSockets;

using Microsoft.AspNetCore.Http;

namespace WebsocketServerDemo
{
    public class TestDeerWebsocket : DeerWebSocket
    {
        public override async Task OnConnectedAsync(HttpContext Context, CancellationToken cancellationToken)
        {

            await this.CloseAsync(WebSocketCloseStatus.NormalClosure);
            await base.OnConnectedAsync(Context, cancellationToken);
        }

        public string Name { get; private set; }

        public override async Task ReceiveAsync(string message, CancellationToken cancellationToken)
        {
            Name = message;


            Console.WriteLine(message);
            await SendAsync(message);
        }

    }
}