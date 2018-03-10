using System;
using System.Collections.Generic;
using System.Text;

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
            base.QueueMessage (message);
            TransmitQueuedMessages ();
        }

        void TransmitQueuedMessages ()
        {
            //
            // Dequeue as many messages as we can
            //
            var messagesToSend = new List<Message> ();
            lock (queuedMessages) {
                messagesToSend.AddRange (queuedMessages);
                queuedMessages.Clear ();
            }

            if (messagesToSend.Count == 0)
                return;

            //
            // Now actually send the messages
            //
            var sb = new StringBuilder ();
            var head = "";
            sb.Append ("[");
            foreach (var m in messagesToSend) {
                sb.Append (head);
                sb.Append (m.ToJson ());
                head = ",";
            }
            sb.Append ("]");
            var json = sb.ToString ();
            WebAssembly.Runtime.InvokeJS ("__oouiReceiveMessages(\"" + id + "\", " + json + ")");
        }

        public void ReceiveMessageJson (string json)
        {
            try {
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message> (json);
                element.Receive (message);
            }
            catch (Exception ex) {
                Error ("Failed to process received message", ex);
            }
        }

        public void StartSession ()
        {
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
            QueueMessage (Message.Call ("document.body", "appendChild", element));
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
