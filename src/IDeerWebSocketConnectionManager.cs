using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deer.WebSockets
{
    public interface IDeerWebSocketConnectionManager<TDeerWebsocket>
    {
        Task<TDeerWebsocket> GetAsync(string id);

        Task<IEnumerable<TDeerWebsocket>> Get(Func<TDeerWebsocket, bool> predicate);

        Task<IEnumerable<TDeerWebsocket>> GetAll();

    }
}
