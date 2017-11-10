using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ooui.AspNetCore
{
    public static class WebSocketHandler
    {
        public static string WebSocketPath { get; set; } = "/ooui.ws";

        public static TimeSpan SessionTimeout { get; set; } = TimeSpan.FromMinutes (5);

        static readonly ConcurrentDictionary<string, PendingSession> pendingSessions =
            new ConcurrentDictionary<string, PendingSession> ();

        public static string BeginSession (HttpContext context, Element element)
        {
            var id = Guid.NewGuid ().ToString ("N");

            var s = new PendingSession {
                Element = element,
                RequestTimeUtc = DateTime.UtcNow,
            };

            if (!pendingSessions.TryAdd (id, s)) {
                throw new Exception ("Failed to schedule pending session");
            }

            return id;
        }

        public static async Task HandleWebSocketRequestAsync (HttpContext context)
        {
            //
            // Make sure we get a good ID
            //
            if (!context.Request.Query.TryGetValue ("id", out var idValues)) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var id = idValues.FirstOrDefault ();
            if (id == null || id.Length != 32) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            //
            // Find the pending session
            //
            if (!pendingSessions.TryRemove (id, out var pendingSession)) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            //
            // Reject the session if it's old
            //
            if ((DateTime.UtcNow - pendingSession.RequestTimeUtc) > SessionTimeout) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            //
            // OK, Run
            //
            var token = CancellationToken.None;
            var webSocket = await context.WebSockets.AcceptWebSocketAsync ("ooui");
            var session = new Ooui.UI.Session (webSocket, pendingSession.Element, token);
            await session.RunAsync ().ConfigureAwait (false);
        }

        class PendingSession
        {
            public Element Element;
            public DateTime RequestTimeUtc;
        }
    }
}
