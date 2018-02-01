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

        static readonly ConcurrentDictionary<string, ActiveSession> activeSessions =
            new ConcurrentDictionary<string, ActiveSession> ();

        public static string BeginSession (HttpContext context, Element element)
        {
            var id = Guid.NewGuid ().ToString ("N");

            var s = new ActiveSession {
                Element = element,
                LastConnectTimeUtc = DateTime.UtcNow,
            };

            if (!activeSessions.TryAdd (id, s)) {
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
            var toClear = activeSessions.Where (x => (DateTime.UtcNow - x.Value.LastConnectTimeUtc) > SessionTimeout).ToList ();
            foreach (var c in toClear) {
                activeSessions.TryRemove (c.Key, out var _);
            }

            //
            // Find the pending session
            //
            if (!activeSessions.TryGetValue (id, out var activeSession)) {
                BadRequest ("Unknown `id`");
                return;
            }
            activeSession.LastConnectTimeUtc = DateTime.UtcNow;

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
            var webSocket = await context.WebSockets.AcceptWebSocketAsync ("ooui");
            var session = new Ooui.UI.Session (webSocket, activeSession.Element, w, h, token);
            await session.RunAsync ().ConfigureAwait (false);
        }

        class ActiveSession
        {
            public Element Element;
            public DateTime LastConnectTimeUtc;
        }
    }
}
