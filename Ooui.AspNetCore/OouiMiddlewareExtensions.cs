using System;
using Ooui.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class OouiMiddlewareExtensions
    {
        public static void UseOoui (this IApplicationBuilder app, string jsPath = "/ooui.js", string webSocketPath = "/ooui.ws", TimeSpan? sessionTimeout = null)
        {
            if (string.IsNullOrWhiteSpace (webSocketPath))
                throw new ArgumentException ("A path to be used for Ooui web sockets must be specified", nameof (webSocketPath));
            
            if (string.IsNullOrWhiteSpace (jsPath))
                throw new ArgumentException ("A path to be used for Ooui JavaScript must be specified", nameof (jsPath));

            WebSocketHandler.WebSocketPath = webSocketPath;

            if (sessionTimeout.HasValue) {
                WebSocketHandler.SessionTimeout = sessionTimeout.Value;
            }

            var webSocketOptions = new WebSocketOptions () {
                KeepAliveInterval = WebSocketHandler.SessionTimeout,
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets (webSocketOptions);

            app.Use (async (context, next) =>
            {
                if (context.Request.Path == jsPath) {
                    var response = context.Response;
                    var clientJsBytes = Ooui.UI.ClientJsBytes;
                    var clientJsEtag = Ooui.UI.ClientJsEtag;
                    if (context.Request.Headers.TryGetValue ("If-None-Match", out var inms) && inms.Count > 0 && inms[0] == clientJsEtag) {
                        response.StatusCode = 304;
                    }
                    else {
                        response.StatusCode = 200;
                        response.ContentLength = clientJsBytes.Length;
                        response.ContentType = "application/javascript; charset=utf-8";
                        response.Headers.Add ("Cache-Control", "public, max-age=60");
                        response.Headers.Add ("Etag", clientJsEtag);
                        using (var s = response.Body) {
                            await s.WriteAsync (clientJsBytes, 0, clientJsBytes.Length).ConfigureAwait (false);
                        }
                    }
                }
                else if (context.Request.Path == WebSocketHandler.WebSocketPath) {
                    if (context.WebSockets.IsWebSocketRequest) {
                        await WebSocketHandler.HandleWebSocketRequestAsync (context).ConfigureAwait (false);
                    }
                    else {
                        context.Response.StatusCode = 400;
                    }
                }
                else {
                    await next ().ConfigureAwait (false);
                }
            });
        }
    }
}
