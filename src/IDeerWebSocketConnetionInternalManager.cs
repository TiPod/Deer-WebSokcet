using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deer.WebSockets
{
    internal interface IDeerWebSocketConnetionInternalManager<TDeerWebsocket> where TDeerWebsocket : DeerWebSocket
    {

        Task AddAsync(TDeerWebsocket webSocket);

        Task RemoveAsync(TDeerWebsocket webSocket);


    }
}
