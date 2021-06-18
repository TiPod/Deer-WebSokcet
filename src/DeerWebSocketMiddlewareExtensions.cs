using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deer.WebSockets
{
    public static class DeerWebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseDeerWebSockets(this IApplicationBuilder app)
        {

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            app.UseWebSockets();
            return app.UseMiddleware<DeerWebSocketMiddleware>(Array.Empty<object>());
        }

        public static IApplicationBuilder UseDeerWebSockets(this IApplicationBuilder app, DeerWebSocketOptions options)
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
            return app.UseMiddleware<DeerWebSocketMiddleware>(new object[1] {
                Options.Create(options)
            });
        }
    }
}