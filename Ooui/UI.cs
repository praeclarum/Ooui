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
    public static class UI
    {
        static CancellationTokenSource serverCts;

        static readonly Dictionary<string, Func<Element>> publishedPaths =
            new Dictionary<string, Func<Element>> ();

        static readonly byte[] clientJsBytes;

        static string host = "*";
        public static string Host {
            get => host;
            set {
                if (!string.IsNullOrWhiteSpace (value) && host != value) {
                    host = value;
                    Restart ();
                }
            }
        }
        static int port = 8080;
        public static int Port {
            get => port;
            set {
                if (port != value) {
                    port = value;
                    Restart ();
                }
            }
        }

        static UI ()
        {
            var asm = typeof(UI).Assembly;
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

        public static void Publish (string path, Func<Element> elementCtor)
        {
            Console.WriteLine ($"PUBLISH {path}");
            lock (publishedPaths) publishedPaths[path] = elementCtor;
            Start ();
        }

        public static void Publish (string path, Element element)
        {
            Publish (path, () => element);
        }

        static void Start ()
        {
            if (serverCts != null) return;
            serverCts = new CancellationTokenSource ();
            var token = serverCts.Token;
            var listenerPrefix = $"http://{host}:{port}/";
            Task.Run (() => RunAsync (listenerPrefix, token), token);
        }

        static void Stop ()
        {
            var scts = serverCts;
            if (scts == null) return;
            serverCts = null;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine ($"Stopping...");
            Console.ResetColor ();

            scts.Cancel ();
        }

        static void Restart ()
        {
            if (serverCts == null) return;
            Stop ();
            Start ();
        }

        static async Task RunAsync (string listenerPrefix, CancellationToken token)
        {
            HttpListener listener = null;

            var started = false;
            while (!started && !token.IsCancellationRequested) {
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
                    await Task.Delay (wait * 1000).ConfigureAwait (false);
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

        static void ProcessRequest (HttpListenerContext listenerContext, CancellationToken token)
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
            else {
                var found = false;
                lock (publishedPaths) found = publishedPaths.TryGetValue (path, out ctor);
                if (found) {
                    WriteElementHtml (path, response);
                }
                else {
                    response.StatusCode = 404;
                    response.Close ();
                }
            }
        }

        static void WriteElementHtml (string elementPath, HttpListenerResponse response)
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

        static async void ProcessWebSocketRequest (HttpListenerContext listenerContext, CancellationToken serverToken)
        {
            //
            // Find the element
            //
            var url = listenerContext.Request.Url;
            var path = url.LocalPath;

            Func<Element> ctor;
            var found = false;
            lock (publishedPaths) found = publishedPaths.TryGetValue (path, out ctor);
            if (!found) {
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
                webSocketContext = await listenerContext.AcceptWebSocketAsync (subProtocol: "ooui").ConfigureAwait (false);
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
            // Create a new session and let it handle everything from here
            //
            try {
                var session = new Session (webSocket, element);
                await session.RunAsync (serverToken).ConfigureAwait (false);
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely) {
                // The remote party closed the WebSocket connection without completing the close handshake.
            }
            catch (Exception ex) {
                Error ("Web socket failed", ex);
            }
            finally {
                webSocket?.Dispose ();
            }
        }

        static async Task SendMessageAsync (WebSocket webSocket, Message message, EventTarget target, HashSet<string> createdIds, CancellationToken token)
        {
            //
            // Make sure all the referenced objects have been created
            //
            if (message.MessageType == MessageType.Create) {
                createdIds.Add (message.TargetId);
            }
            else {
                if (!createdIds.Contains (message.TargetId)) {
                    createdIds.Add (message.TargetId);
                    await SendStateMessagesAsync (webSocket, target.GetElementById (message.TargetId), createdIds, token).ConfigureAwait (false);
                }
                if (message.Value is Array a) {
                    for (var i = 0; i < a.Length; i++) {
                        // Console.WriteLine ($"A{i} = {a.GetValue(i)}");
                        if (a.GetValue (i) is EventTarget e && !createdIds.Contains (e.Id)) {
                            createdIds.Add (e.Id);
                            await SendStateMessagesAsync (webSocket, e, createdIds, token).ConfigureAwait (false);
                        }
                    }
                }
            }

            //
            // Now actually send this message
            //
            if (token.IsCancellationRequested)
                return;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject (message);
            var outputBuffer = new ArraySegment<byte> (Encoding.UTF8.GetBytes (json));
            await webSocket.SendAsync (outputBuffer, WebSocketMessageType.Text, true, token).ConfigureAwait (false);
        }

        static async Task SendStateMessagesAsync (WebSocket webSocket, EventTarget target, HashSet<string> createdIds, CancellationToken token)
        {
            if (target == null) return;

            foreach (var m in target.StateMessages) {
                if (token.IsCancellationRequested) return;
                await SendMessageAsync (webSocket, m, target, createdIds, token).ConfigureAwait (false);
            }
        }

        static void Error (string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine ("{0}: {1}", message, ex);
            Console.ResetColor ();
        }

        class Session
        {
            readonly WebSocket webSocket;
            readonly Element element;

            public Session (WebSocket webSocket, Element element)
            {
                this.webSocket = webSocket;
                this.element = element;
            }

            public async Task RunAsync (CancellationToken serverToken)
            {
                //
                // Create a new session cancellation token that will trigger
                // automatically if the server shutsdown or the session shutsdown.
                //
                var sessionCts = new CancellationTokenSource ();
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource (serverToken, sessionCts.Token);
                var token = linkedCts.Token;

                //
                // Keep a list of all the elements for which we've transmitted the initial state
                //
                var createdIds = new HashSet<string> {
                    "window",
                    "document",
                    "document.body",
                };

                //
                // Preparse handlers for the element
                //
                Action<Message> onElementMessage = null;
                onElementMessage = async m => {
                    if (webSocket == null) return;
                    try {
                        await SendMessageAsync (webSocket, m, element, createdIds, token).ConfigureAwait (false);
                    }
                    catch (Exception ex) {                        
                        if (webSocket.State == WebSocketState.Aborted) {
                            Error ("WebSocket is aborted, cancelling session", ex);
                            element.MessageSent -= onElementMessage;
                            sessionCts.Cancel ();
                        }
                        else {
                            Error ("Failed to handle element message", ex);
                        }
                    }
                };

                //
                // Start watching for changes in the element
                //
                element.MessageSent += onElementMessage;

                try {
                    //
                    // Add it to the document body
                    //
                    await SendMessageAsync (webSocket, new Message {
                        TargetId = "document.body",
                        MessageType = MessageType.Call,
                        Key = "appendChild",
                        Value = new[] { element },
                    }, element, createdIds, token).ConfigureAwait (false);

                    //
                    // Listen for events
                    //
                    var receiveBuffer = new byte[1024];

                    while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested) {
                        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), token).ConfigureAwait (false);

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

                            try {
                                // Console.WriteLine ("RECEIVED: {0}", receivedString);
                                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message> (receivedString);
                                element.Receive (message);
                            }
                            catch (Exception ex) {
                                Error ("Failed to process received message", ex);
                            }
                        }
                    }
                }
                finally {
                    element.MessageSent -= onElementMessage;
                }
            }
        }
    }
}
