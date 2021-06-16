using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Builder;

namespace Deer.WebSockets
{
    public class DeerWebSocketOptions
    {

        public string Path { get; set; } = "/*";

        public int ReceiveBufferSize { get; set; } = 4 * 1024;
    }

}
