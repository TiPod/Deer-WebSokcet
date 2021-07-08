using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Deer.WebSockets
{
    public class DeerWebSocketOptions<TDeerWebSocket> : DeerWebSocketOptions where TDeerWebSocket : DeerWebSocket
    {

    }

    public class DeerWebSocketOptions : IOptions<DeerWebSocketOptions>
    {

        public string Path { get; set; } = "/*";

        public int ReceiveBufferSize { get; set; } = 4 * 1024;

        public int SendBufferSize { get; set; } = 4 * 1024;

        public DeerWebSocketOptions Value => this;
    }



}
