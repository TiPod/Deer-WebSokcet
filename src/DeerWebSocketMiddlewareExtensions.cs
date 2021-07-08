using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deer.WebSockets
{
    public static class DeerWebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseDeerWebSockets<TDeerWebSocket>(this IApplicationBuilder app) where TDeerWebSocket:DeerWebSocket
        {

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            app.UseWebSockets();
            return app.UseMiddleware<DeerWebSocketMiddleware<TDeerWebSocket>>(Array.Empty<object>());
        }

        public static IApplicationBuilder UseDeerWebSockets<TDeerWebSocket>(this IApplicationBuilder app, DeerWebSocketOptions options) where TDeerWebSocket : DeerWebSocket
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            app.UseWebSockets();
            return app.UseMiddleware<DeerWebSocketMiddleware<TDeerWebSocket>>(new object[1] {
                Options.Create(options)
            });
        }
    }
}