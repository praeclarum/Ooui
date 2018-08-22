﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using Ooui.Html;

namespace Ooui
{
    public class WebSocketSession : Session
    {
        readonly WebSocket webSocket;
        readonly Action<Message> handleElementMessageSent;

        readonly CancellationTokenSource sessionCts = new CancellationTokenSource ();
        readonly CancellationTokenSource linkedCts;
        readonly CancellationToken token;

        readonly System.Timers.Timer sendThrottle;
        DateTime lastTransmitTime = DateTime.MinValue;
        readonly TimeSpan throttleInterval = TimeSpan.FromSeconds (1.0 / UI.MaxFps);

        public WebSocketSession (WebSocket webSocket, Element element, bool disposeElementAfterSession, double initialWidth, double initialHeight, Action<string, Exception> errorLogger, CancellationToken serverToken)
            : base (element, disposeElementAfterSession, initialWidth, initialHeight, errorLogger)
        {
            this.webSocket = webSocket;

            //
            // Create a new session cancellation token that will trigger
            // automatically if the server shutsdown or the session shutsdown.
            //
            linkedCts = CancellationTokenSource.CreateLinkedTokenSource (serverToken, sessionCts.Token);
            token = linkedCts.Token;

            //
            // Preparse handlers for the element
            //
            handleElementMessageSent = QueueMessage;

            //
            // Create a timer to use as a throttle when sending messages
            //
            sendThrottle = new System.Timers.Timer (throttleInterval.TotalMilliseconds);
            sendThrottle.Elapsed += (s, e) => {
                // System.Console.WriteLine ("TICK SEND THROTTLE FOR {0}", element);
                if ((e.SignalTime - lastTransmitTime) >= throttleInterval) {
                    sendThrottle.Enabled = false;
                    lastTransmitTime = e.SignalTime;
                    TransmitQueuedMessages ();
                }
            };
        }

        public async Task RunAsync ()
        {
            //
            // Start watching for changes in the element
            //
            element.MessageSent += handleElementMessageSent;

            try {
                //
                // Add it to the document body
                //
                if (element.WantsFullScreen) {
                    element.Style.Width = initialWidth;
                    element.Style.Height = initialHeight;
                }
                QueueMessage (Message.Call ("document.body", "appendChild", element));

                //
                // Start the Read Loop
                //
                var receiveBuffer = new byte[64 * 1024];

                while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested) {
                    var receiveResult = await webSocket.ReceiveAsync (new ArraySegment<byte> (receiveBuffer), token).ConfigureAwait (false);

                    if (receiveResult.MessageType == WebSocketMessageType.Close) {
                        await webSocket.CloseAsync (WebSocketCloseStatus.NormalClosure, "", token).ConfigureAwait (false);
                        sessionCts.Cancel ();
                    }
                    else if (receiveResult.MessageType == WebSocketMessageType.Binary) {
                        await webSocket.CloseAsync (WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame", token).ConfigureAwait (false);
                        sessionCts.Cancel ();
                    }
                    else {
                        var size = receiveResult.Count;
                        while (!receiveResult.EndOfMessage) {
                            if (size >= receiveBuffer.Length) {
                                await webSocket.CloseAsync (WebSocketCloseStatus.MessageTooBig, "Message too big", token).ConfigureAwait (false);
                                return;
                            }
                            receiveResult = await webSocket.ReceiveAsync (new ArraySegment<byte> (receiveBuffer, size, receiveBuffer.Length - size), token).ConfigureAwait (false);
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
                element.MessageSent -= handleElementMessageSent;

                if (disposeElementAfterSession && (element is IDisposable disposable)) {
                    try {
                        disposable.Dispose ();
                    }
                    catch (Exception ex) {
                        Error ("Failed to dispose of element", ex);
                    }
                }
            }
        }

        protected override void QueueMessage (Message message)
        {
            base.QueueMessage (message);
            sendThrottle.Enabled = true;
        }

        async void TransmitQueuedMessages ()
        {
            try {
                //
                // Dequeue as many messages as we can
                //
                var messagesToSend = new List<Message> ();
                System.Runtime.CompilerServices.ConfiguredTaskAwaitable task;
                lock (queuedMessages) {
                    messagesToSend.AddRange (queuedMessages);
                    queuedMessages.Clear ();

                    if (messagesToSend.Count == 0)
                        return;

                    //
                    // Now actually send this message
                    // Do this while locked to make sure SendAsync is called in the right order
                    //
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject (messagesToSend);
                    var outputBuffer = new ArraySegment<byte> (Encoding.UTF8.GetBytes (json));
                    //Console.WriteLine ("TRANSMIT " + json);
                    task = webSocket.SendAsync (outputBuffer, WebSocketMessageType.Text, true, token).ConfigureAwait (false);
                }
                await task;
            }
            catch (Exception ex) {
                Error ("Failed to send queued messages, aborting session", ex);
                element.MessageSent -= handleElementMessageSent;
                sessionCts.Cancel ();
            }
        }
    }
}
