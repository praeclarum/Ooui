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
            var json = Newtonsoft.Json.JsonConvert.SerializeObject (messagesToSend);
            SendMessagesJson (json);
        }

        protected override void QueueMessage (Message message)
        {
            base.QueueMessage (message);
            TransmitQueuedMessages ();
        }

        void SendMessagesJson (string json)
        {
            Info ("SEND: " + json);
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
