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
    public class DeerWebSocketConnectionManager<TDeerWebSocket> : IDeerWebSocketConnetionInternalManager<TDeerWebSocket>, IDeerWebSocketConnectionManager<TDeerWebSocket> where TDeerWebSocket : DeerWebSocket
    {
        private ConcurrentDictionary<string, TDeerWebSocket> _connections = new ConcurrentDictionary<string, TDeerWebSocket>(StringComparer.OrdinalIgnoreCase);

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private Task<TResult> SemaphoreLockAsync<TResult>(Func<ConcurrentDictionary<string, TDeerWebSocket>, Task<TResult>> func)
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

        private Task SemaphoreLockAsync(Func<ConcurrentDictionary<string, TDeerWebSocket>, Task> func)
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



        public Task AddAsync(TDeerWebSocket webSocket)
        {

            return SemaphoreLockAsync(connections =>
            {
                connections[webSocket.Id] = webSocket;
                return Task.CompletedTask;

            });
        }

        public Task RemoveAsync(TDeerWebSocket webSocket)
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
                connections.TryGetValue(id, out TDeerWebSocket websocket);

                return Task.FromResult(websocket as TDeerWebSocket);
            });
        }

        public Task<IEnumerable<TDeerWebSocket>> GetAsync(Func<TDeerWebSocket, bool> predicate)
        {
            return SemaphoreLockAsync(connections =>
            {
                return Task.FromResult(connections.Select(p => p.Value).Where(predicate));
            });
        }

        public Task<IEnumerable<TDeerWebSocket>> GetAllAsync()
        {
            return SemaphoreLockAsync(connections =>
            {
                return Task.FromResult(connections.Select(p => p.Value));
            });
        }


    }
}
