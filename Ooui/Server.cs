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

        public Task RunAsync (string listenerPrefix)
        {
            return RunAsync (listenerPrefix, CancellationToken.None);
        }

        public async Task RunAsync (string listenerPrefix, CancellationToken token)
        {
            var listener = new HttpListener ();
            listener.Prefixes.Add (listenerPrefix);
            listener.Start ();
            Console.WriteLine ($"Listening at {listenerPrefix}...");

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

            Func<Element> ctor;
            if (publishedPaths.TryGetValue (path, out ctor)) {
                var element = ctor ();
                RegisterElement (element);
                WriteElementHtml (element, listenerContext.Response);
            }
            else {
                listenerContext.Response.StatusCode = 404;
                listenerContext.Response.Close ();
            }
        }

        void RegisterElement (Element element)
        {
        }

        void WriteElementHtml (Element element, HttpListenerResponse response)
        {
            response.StatusCode = 200;
            using (var s = response.OutputStream) {
                using (var w = new StreamWriter (s, Encoding.UTF8)) {
                    w.WriteLine ($"Hello {element}");
                }
            }
            response.Close ();
        }

        async void ProcessWebSocketRequest (HttpListenerContext listenerContext, CancellationToken token)
        {
            WebSocketContext webSocketContext = null;
            try {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null).ConfigureAwait (false);
                Console.WriteLine ("Accepted WebSocket: {0}", webSocketContext);
            }
            catch (Exception e) {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Console.WriteLine ("Failed to accept WebSocket: {0}", e);
                return;
            }

            WebSocket webSocket = null;

            try {
                webSocket = webSocketContext.WebSocket;

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

                        var outputBuffer = new ArraySegment<byte> (Encoding.UTF8.GetBytes ($"You said: {receivedString}"));
                        await webSocket.SendAsync (outputBuffer, WebSocketMessageType.Text, true, token).ConfigureAwait (false);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine ("Exception: {0}", e);
            }
            finally {
                webSocket?.Dispose();
            }
        }
    }
}
