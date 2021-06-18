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
            services.AddScoped<DeerWebSocket, TDeerWebsocket>();
            services.AddSingleton<IDeerWebSocketConnetionInternalManager, DeerWebSocketConnectionManager<TDeerWebsocket>>();
            services.AddSingleton<IDeerWebSocketConnectionManager<TDeerWebsocket>, DeerWebSocketConnectionManager<TDeerWebsocket>>(seviceProvider => (DeerWebSocketConnectionManager<TDeerWebsocket>)seviceProvider.GetService(typeof(IDeerWebSocketConnetionInternalManager)));

            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.AddOptions();
            services.Configure<DeerWebSocketOptions>(configuration.GetSection(nameof(DeerWebSocketOptions)));

            return services;
        }

        public static IServiceCollection AddDeerWebSockets<TDeerWebsocket>(this IServiceCollection services, Action<DeerWebSocketOptions> configure) where TDeerWebsocket : DeerWebSocket
        {

            services.Configure(configure);
            return AddDeerWebSockets<TDeerWebsocket>(services);
        }
    }
}