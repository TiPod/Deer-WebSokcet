using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deer.WebSockets
{
    public class DeerWebSocketConnectionManager<TDeerWebSocket> : IDeerWebSocketConnetionInternalManager, IDeerWebSocketConnectionManager<TDeerWebSocket> where TDeerWebSocket : DeerWebSocket
    {
        private ConcurrentDictionary<string, DeerWebSocket> _connections = new ConcurrentDictionary<string, DeerWebSocket>(StringComparer.OrdinalIgnoreCase);

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private Task<TResult> SemaphoreLockAsync<TResult>(Func<ConcurrentDictionary<string, DeerWebSocket>, Task<TResult>> func)
        {
            semaphore.Wait();
            try
            {
                return func(_connections);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private Task SemaphoreLockAsync(Func<ConcurrentDictionary<string, DeerWebSocket>, Task> func)
        {
            semaphore.Wait();
            try
            {
                return func(_connections);
            }
            finally
            {
                semaphore.Release();
            }
        }



        public Task AddAsync(DeerWebSocket webSocket)
        {

            return SemaphoreLockAsync(connections =>
            {
                connections[webSocket.Id] = webSocket;
                return Task.CompletedTask;
            });
        }

        public Task RemoveAsync(DeerWebSocket webSocket)
        {
            return SemaphoreLockAsync(connections =>
            {
                connections.TryRemove(webSocket.Id, out _);
                return Task.CompletedTask;
            });
        }
        public Task<TDeerWebSocket> GetAsync(string id)
        {
            return SemaphoreLockAsync(connections =>
            {
                connections.TryGetValue(id, out DeerWebSocket websocket);

                return Task.FromResult(websocket as TDeerWebSocket);
            });
        }

        public Task<IEnumerable<TDeerWebSocket>> GetAsync(Func<TDeerWebSocket, bool> predicate)
        {
            return SemaphoreLockAsync(connections =>
            {
                return Task.FromResult(connections.Select(p => p.Value as TDeerWebSocket).Where(predicate));
            });
        }

        public Task<IEnumerable<TDeerWebSocket>> GetAllAsync()
        {
            return SemaphoreLockAsync(connections =>
            {
                return Task.FromResult(connections.Select(p => p.Value as TDeerWebSocket));
            });
        }


    }
}
