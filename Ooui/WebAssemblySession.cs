using System;
using System.Collections.Generic;

namespace Ooui
{
    public class WebAssemblySession : Session
    {
        readonly string id;
        readonly Action<Message> handleElementMessageSent;

        public WebAssemblySession (string id, Element element, double initialWidth, double initialHeight)
            : base (element, initialWidth, initialHeight)
        {
            this.id = id;
            handleElementMessageSent = QueueMessage;
        }

        protected override void QueueMessage (Message message)
        {
            WebAssembly.Runtime.InvokeJS ("console.log('q 0')");
            base.QueueMessage (message);
            WebAssembly.Runtime.InvokeJS ("console.log('q 1')");
            TransmitQueuedMessages ();
            WebAssembly.Runtime.InvokeJS ("console.log('q end')");
        }

        void TransmitQueuedMessages ()
        {
            WebAssembly.Runtime.InvokeJS ("console.log('t 0')");
            //
            // Dequeue as many messages as we can
            //
            var messagesToSend = new List<Message> ();
            lock (queuedMessages) {
                messagesToSend.AddRange (queuedMessages);
                queuedMessages.Clear ();
            }

            WebAssembly.Runtime.InvokeJS ("console.log('t 1')");

            if (messagesToSend.Count == 0)
                return;

            WebAssembly.Runtime.InvokeJS ("console.log('t 2')");

            //
            // Now actually send the messages
            //
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject (messagesToSend);
            WebAssembly.Runtime.InvokeJS ("alert(" + messagesToSend.Count + ")");
            WebAssembly.Runtime.InvokeJS ("console.log('t end')");
        }

        public void ReceiveMessageJson (string json)
        {
            try {
                Info ("RECEIVED: " + json);
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message> (json);
                element.Receive (message);
            }
            catch (Exception ex) {
                Error ("Failed to process received message", ex);
            }
        }

        public void StartSession ()
        {
            WebAssembly.Runtime.InvokeJS ("console.log('was start session 0')");
            //
            // Start watching for changes in the element
            //
            element.MessageSent += handleElementMessageSent;

            //
            // Add it to the document body
            //
            if (element.WantsFullScreen) {
                element.Style.Width = initialWidth;
                element.Style.Height = initialHeight;
            }
            WebAssembly.Runtime.InvokeJS ("console.log('was start session 1')");
            QueueMessage (Message.Call ("document.body", "appendChild", element));
            WebAssembly.Runtime.InvokeJS ("console.log('was start session end')");
        }

        public void StopSession ()
        {
            element.MessageSent -= handleElementMessageSent;
        }
    }
}

namespace WebAssembly
{
    public sealed class Runtime
    {
        [System.Runtime.CompilerServices.MethodImplAttribute ((System.Runtime.CompilerServices.MethodImplOptions)4096)]
        static extern string InvokeJS (string str, out int exceptional_result);

        public static string InvokeJS (string str)
        {
            return InvokeJS (str, out var _);
        }
    }
}
