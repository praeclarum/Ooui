using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;

namespace Ooui
{
    public class Server
    {
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
                var listenerContext = await listener.GetContextAsync ();
                if (listenerContext.Request.IsWebSocketRequest) {
                    ProcessWebSocketRequest (listenerContext, token);
                }
                else {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close ();
                }
            }
        }

        async void ProcessWebSocketRequest (HttpListenerContext listenerContext, CancellationToken token)
        {
            WebSocketContext webSocketContext = null;
            try {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
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
                        await webSocket.CloseAsync (WebSocketCloseStatus.NormalClosure, "", token);
                    }
                    else if (receiveResult.MessageType == WebSocketMessageType.Binary) {
                        await webSocket.CloseAsync (WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame", token);
                    }
                    else {
                        var size = receiveResult.Count;
                        while (!receiveResult.EndOfMessage) {
                            if (size >= receiveBuffer.Length) {
                                await webSocket.CloseAsync (WebSocketCloseStatus.MessageTooBig, "Message too big", token);
                                return;
                            }
                            receiveResult = await webSocket.ReceiveAsync (new ArraySegment<byte>(receiveBuffer, size, receiveBuffer.Length - size), token);
                            size += receiveResult.Count;
                        }
                        var receivedString = Encoding.UTF8.GetString (receiveBuffer, 0, size);
                        Console.WriteLine ("RECEIVED: {0}", receivedString);

                        var outputBuffer = new ArraySegment<byte> (Encoding.UTF8.GetBytes ($"You said: {receivedString}"));
                        await webSocket.SendAsync (outputBuffer, WebSocketMessageType.Text, true, token);
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
