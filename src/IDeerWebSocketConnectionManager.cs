using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deer.WebSockets
{
    public interface IDeerWebSocketConnectionManager<TDeerWebsocket> where TDeerWebsocket : DeerWebSocket
    {
        Task<TDeerWebsocket> GetAsync(string id);

        Task<IEnumerable<TDeerWebsocket>> GetAsync(Func<TDeerWebsocket, bool> predicate);

        Task<IEnumerable<TDeerWebsocket>> GetAllAsync();

    }
}
