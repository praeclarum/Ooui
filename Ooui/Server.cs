using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;

namespace Ooui
{
    public class Server
    {
        readonly Dictionary<string, Func<Element>> publishedPaths =
            new Dictionary<string, Func<Element>> ();

        readonly static byte[] clientJsBytes;

        static Server ()
        {
            var asm = typeof(Server).Assembly;
            // System.Console.WriteLine("ASM = {0}", asm);
            // foreach (var n in asm.GetManifestResourceNames()) {
            //     System.Console.WriteLine("  {0}", n);
            // }
            using (var s = asm.GetManifestResourceStream ("Ooui.Client.js")) {
                using (var r = new StreamReader (s)) {
                    clientJsBytes = Encoding.UTF8.GetBytes (r.ReadToEnd ());
                }
            }
        }

        public Task RunAsync (string listenerPrefix)
        {
            return RunAsync (listenerPrefix, CancellationToken.None);
        }

        public async Task RunAsync (string listenerPrefix, CancellationToken token)
        {
            HttpListener listener = null;

            var started = false;
            while (!started) {
                try {
                    listener = new HttpListener ();
                    listener.Prefixes.Add (listenerPrefix);
                    listener.Start ();
                    started = true;
                }
                catch (System.Net.Sockets.SocketException ex) when
                    (ex.SocketErrorCode == System.Net.Sockets.SocketError.AddressAlreadyInUse) {
                    var wait = 5;
                    Console.WriteLine ($"{listenerPrefix} is in use, trying again in {wait} seconds...");
                    await Task.Delay (wait * 1000);
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine ($"Listening at {listenerPrefix}...");
            Console.ResetColor ();

            while (!token.IsCancellationRequested) {
                var listenerContext = await listener.GetContextAsync ().ConfigureAwait (false);
                if (listenerContext.Request.IsWebSocketRequest) {
                    ProcessWebSocketRequest (listenerContext, token);
                }
                else {
                    ProcessRequest (listenerContext, token);
                }
            }
        }

        public void Publish (string path, Func<Element> elementCtor)
        {
            System.Console.WriteLine($"PUBLISH {path}");
            publishedPaths[path] = elementCtor;
        }

        public void Publish (string path, Element element)
        {
            Publish (path, () => element);
        }

        void ProcessRequest (HttpListenerContext listenerContext, CancellationToken token)
        {
            var url = listenerContext.Request.Url;
            var path = url.LocalPath;

            Console.WriteLine ($"{listenerContext.Request.HttpMethod} {url.LocalPath}");

            var response = listenerContext.Response;

            Func<Element> ctor;

            if (path == "/client.js") {
                response.ContentLength64 = clientJsBytes.LongLength;
                response.ContentType = "application/javascript";
                response.ContentEncoding = Encoding.UTF8;
                response.AddHeader ("Cache-Control", "public, max-age=3600");
                using (var s = response.OutputStream) {
                    s.Write (clientJsBytes, 0, clientJsBytes.Length);
                }
            }
            else if (publishedPaths.TryGetValue (path, out ctor)) {
                WriteElementHtml (path, response);
            }
            else {
                response.StatusCode = 404;
                response.Close ();
            }
        }

        void WriteElementHtml (string elementPath, HttpListenerResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            var html = Encoding.UTF8.GetBytes ($@"<html>
<head><title>{elementPath}</title></head>
<body>
<script>rootElementPath = ""{elementPath}"";</script>
<script src=""/client.js""> </script></body>
</html>");
            response.ContentLength64 = html.LongLength;
            using (var s = response.OutputStream) {
                s.Write (html, 0, html.Length);
            }
            response.Close ();
        }

        async void ProcessWebSocketRequest (HttpListenerContext listenerContext, CancellationToken token)
        {
            //
            // Find the element
            //
            var url = listenerContext.Request.Url;
            var path = url.LocalPath;

            Func<Element> ctor;
            if (!publishedPaths.TryGetValue (path, out ctor)) {
                listenerContext.Response.StatusCode = 404;
                listenerContext.Response.Close ();
                return;
            }

            Element element = null;
            try {
                element = ctor ();
            }
            catch (Exception ex) {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Error ("Failed to create element", ex);
                return;
            }

            //
            // Connect the web socket
            //
            WebSocketContext webSocketContext = null;
            WebSocket webSocket = null;
            try {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: "ooui-1.0").ConfigureAwait (false);
                webSocket = webSocketContext.WebSocket;
                Console.WriteLine ("WEBSOCKET {0}", listenerContext.Request.Url.LocalPath);
            }
            catch (Exception ex) {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Error ("Failed to accept WebSocket", ex);
                return;
            }


            //
            // Preparse handlers for the element
            //
            Action<Message> onElementMessage = async m => {
                if (webSocket == null) return;
                try {
                    await SendMessageAsync (webSocket, m, token);
                }
                catch (Exception ex) {
                    Error ("Failed to handled element message", ex);
                }
            };

            //
            // Communicate!
            //
            try {
                //
                // Send message history, start sending updates, and add it to the body
                //
                foreach (var m in element.AllMessages) {
                    if (webSocket.State == WebSocketState.Open) {
                        await SendMessageAsync (webSocket, m, token);
                    }
                }
                element.MessageLogged += onElementMessage;
                await SendMessageAsync (webSocket, new Message {
                    TargetId = "document.body",
                    MessageType = MessageType.Call,
                    Key = "appendChild",
                    Value = new[] { "\u2999" + element.Id },
                }, token);

                //
                // Listen for events
                //
                var receiveBuffer = new byte[1024];

                while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested) {
                    var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), token);

                    if (receiveResult.MessageType == WebSocketMessageType.Close) {
                        await webSocket.CloseAsync (WebSocketCloseStatus.NormalClosure, "", token).ConfigureAwait (false);
                    }
                    else if (receiveResult.MessageType == WebSocketMessageType.Binary) {
                        await webSocket.CloseAsync (WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame", token).ConfigureAwait (false);
                    }
                    else {
                        var size = receiveResult.Count;
                        while (!receiveResult.EndOfMessage) {
                            if (size >= receiveBuffer.Length) {
                                await webSocket.CloseAsync (WebSocketCloseStatus.MessageTooBig, "Message too big", token).ConfigureAwait (false);
                                return;
                            }
                            receiveResult = await webSocket.ReceiveAsync (new ArraySegment<byte>(receiveBuffer, size, receiveBuffer.Length - size), token).ConfigureAwait (false);
                            size += receiveResult.Count;
                        }
                        var receivedString = Encoding.UTF8.GetString (receiveBuffer, 0, size);
                        Console.WriteLine ("RECEIVED: {0}", receivedString);

                        // var outputBuffer = new ArraySegment<byte> (Encoding.UTF8.GetBytes ($"You said: {receivedString}"));
                        // await webSocket.SendAsync (outputBuffer, WebSocketMessageType.Text, true, token).ConfigureAwait (false);
                    }
                }
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely) {
                // The remote party closed the WebSocket connection without completing the close handshake.
            }
            catch (Exception ex) {
                Error ("Failed to process web socket", ex);
            }
            finally {
                element.MessageLogged -= onElementMessage;
                webSocket?.Dispose ();
            }
        }

        Task SendMessageAsync (WebSocket webSocket, Message message, CancellationToken token)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject (message);
            var outputBuffer = new ArraySegment<byte> (Encoding.UTF8.GetBytes (json));
            return webSocket.SendAsync (outputBuffer, WebSocketMessageType.Text, true, token);
        }

        void Error (string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine ("{0}: {1}", message, ex);
            Console.ResetColor ();
        }
    }
}
