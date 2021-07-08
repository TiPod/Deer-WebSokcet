using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deer.WebSockets
{
    public static class DeerWebsocketServiceCollectionExtensions
    {
        public static IServiceCollection AddDeerWebSockets<TDeerWebsocket>(this IServiceCollection services) where TDeerWebsocket : DeerWebSocket
        {

            return AddDeerWebSockets<TDeerWebsocket>(services, null);
        }

        public static IServiceCollection AddDeerWebSockets<TDeerWebsocket>(this IServiceCollection services, Action<DeerWebSocketOptions<TDeerWebsocket>> configure) where TDeerWebsocket : DeerWebSocket
        {

            services.AddScoped<DeerWebSocket, TDeerWebsocket>();
            services.AddScoped<TDeerWebsocket>();
            services.AddSingleton<IDeerWebSocketConnetionInternalManager<TDeerWebsocket>, DeerWebSocketConnectionManager<TDeerWebsocket>>();
            services.AddSingleton<IDeerWebSocketConnectionManager<TDeerWebsocket>, DeerWebSocketConnectionManager<TDeerWebsocket>>(seviceProvider => (DeerWebSocketConnectionManager<TDeerWebsocket>)seviceProvider.GetService(typeof(IDeerWebSocketConnetionInternalManager<TDeerWebsocket>)));
            services.AddOptions();
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.Configure<DeerWebSocketOptions<TDeerWebsocket>>(configuration.GetSection(nameof(DeerWebSocketOptions)));
            if (configure != null)
            {
                services.Configure(configure);
            }
            return services;

        }
    }
}