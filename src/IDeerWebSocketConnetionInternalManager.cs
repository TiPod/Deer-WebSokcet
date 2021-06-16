using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deer.WebSockets
{
    internal interface IDeerWebSocketConnetionInternalManager
    {

        Task AddAsync(DeerWebSocket webSocket);

        Task RemoveAsync(DeerWebSocket webSocket);


    }
}
