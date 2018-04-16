using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Ooui.AspNetCore
{
    public static class WebSocketHandler
    {
        public static string WebSocketPath { get; set; } = "/ooui.ws";

        public static TimeSpan SessionTimeout { get; set; } = TimeSpan.FromMinutes (1);

        static readonly ConcurrentDictionary<string, PendingSession> pendingSessions =
            new ConcurrentDictionary<string, PendingSession> ();

        public static string BeginSession (HttpContext context, Element element, bool disposeElementAfterSession, ILogger logger)
        {
            var id = Guid.NewGuid ().ToString ("N");

            var s = new PendingSession {
                Element = element,
                CreateTimeUtc = DateTime.UtcNow,
                DisposeElementAfterSession = disposeElementAfterSession,
                Logger = logger,
            };

            if (!pendingSessions.TryAdd (id, s)) {
                throw new Exception ("Failed to schedule pending session");
            }

            return id;
        }

        public static async Task HandleWebSocketRequestAsync (HttpContext context)
        {
            void BadRequest (string message)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "text/plain; charset=utf-8";
                using (var sw = new System.IO.StreamWriter (context.Response.Body)) {
                    sw.WriteLine (message);
                }
            }

            //
            // Make sure we get a good ID
            //
            if (!context.Request.Query.TryGetValue ("id", out var idValues)) {
                BadRequest ("Missing `id`");
                return;
            }

            var id = idValues.LastOrDefault ();
            if (id == null || id.Length != 32) {
                BadRequest ("Invalid `id`");
                return;
            }

            //
            // Clear old sessions
            //
            var toClear = pendingSessions.Where (x => (DateTime.UtcNow - x.Value.CreateTimeUtc) > SessionTimeout).ToList ();
            foreach (var c in toClear) {
                pendingSessions.TryRemove (c.Key, out var _);
            }

            //
            // Find the pending session
            //
            if (!pendingSessions.TryRemove (id, out var activeSession)) {
                BadRequest ("Unknown `id`");
                return;
            }

            //
            // Set the element's dimensions
            //
            if (!context.Request.Query.TryGetValue ("w", out var wValues) || wValues.Count < 1) {
                BadRequest ("Missing `w`");
                return;
            }
            if (!context.Request.Query.TryGetValue ("h", out var hValues) || hValues.Count < 1) {
                BadRequest ("Missing `h`");
                return;
            }
            var icult = System.Globalization.CultureInfo.InvariantCulture;
            if (!double.TryParse (wValues.Last (), System.Globalization.NumberStyles.Any, icult, out var w))
                w = 640;
            if (!double.TryParse (hValues.Last (), System.Globalization.NumberStyles.Any, icult, out var h))
                h = 480;

            //
            // OK, Run
            //
            var token = CancellationToken.None;
            System.Net.WebSockets.WebSocket webSocket = null;

            //
            // Create a new session and let it handle everything from here
            //
            try {
                webSocket = await context.WebSockets.AcceptWebSocketAsync ("ooui").ConfigureAwait (false);
                var session = new Ooui.WebSocketSession (webSocket, activeSession.Element, activeSession.DisposeElementAfterSession, w, h, token);
                await session.RunAsync ().ConfigureAwait (false);
            }
            catch (System.Net.WebSockets.WebSocketException ex) when (ex.WebSocketErrorCode == System.Net.WebSockets.WebSocketError.ConnectionClosedPrematurely) {
                // The remote party closed the WebSocket connection without completing the close handshake.
            }
            catch (Exception ex) {
                context.Abort ();
                activeSession?.Logger?.LogWarning (ex, "Web socket session failed");
            }
            finally {
                webSocket?.Dispose ();
            }
        }

        class PendingSession
        {
            public Element Element;
            public DateTime CreateTimeUtc;
            public bool DisposeElementAfterSession;
            public ILogger Logger;
        }
    }
}
