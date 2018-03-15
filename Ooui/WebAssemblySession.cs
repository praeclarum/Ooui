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
            lock (queuedMessages) {
                QueueMessageLocked (message);
                var max = 1;
                var i = 0;
                while (i < queuedMessages.Count) {
                    TransmitQueuedMessagesLocked (queuedMessages, i, max);
                    i += max;
                }
                // WebAssembly.Runtime.InvokeJS("console.log('TRANSMITTED'," + queuedMessages.Count + ")");
                queuedMessages.Clear ();
            }
        }

        void TransmitQueuedMessagesLocked (List<Message> messagesToSend, int startIndex, int max)
        {
            if (messagesToSend.Count == 0)
                return;

            //
            // Now actually send the messages
            //
            var sb = new System.IO.StringWriter ();
            sb.Write ("__oouiReceiveMessages(\"");
            sb.Write (id);
            sb.Write ("\",");
            sb.Write ("[");
            var head = "";
            int n = 0;
            for (var i = startIndex; i < messagesToSend.Count && n < max; i++, n++) {
                sb.Write (head);
                messagesToSend[i].WriteJson (sb);
                head = ",";
            }
            sb.Write ("])");
            var jsonp = sb.ToString ();
            // WebAssembly.Runtime.InvokeJS("console.log('TRANSMIT',"+n+")");
            WebAssembly.Runtime.InvokeJS (jsonp);
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
