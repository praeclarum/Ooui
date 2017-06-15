using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooui
{
    public abstract class EventTarget
    {
        readonly List<Message> stateMessages = new List<Message> ();

        readonly Dictionary<string, List<EventHandler>> eventListeners =
            new Dictionary<string, List<EventHandler>> ();

        public string Id { get; private set; } = GenerateId ();

        public string TagName { get; private set; }

        public event Action<Message> MessageSent;

        public IEnumerable<Message> StateMessages => stateMessages;

        protected EventTarget (string tagName)
        {
            TagName = tagName;
            
            Send (new Message {
                MessageType = MessageType.Create,
                TargetId = Id,
                Key = TagName,
            });
        }

        public void AddEventListener (string eventType, EventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            var sendListen = false;

            List<EventHandler> handlers;
            if (!eventListeners.TryGetValue (eventType, out handlers)) {
                handlers = new List<EventHandler> ();
                eventListeners[eventType] = handlers;
                sendListen = true;
            }
            handlers.Add (handler);

            if (sendListen)
                Send (Message.Listen (Id, eventType));
        }

        public void RemoveEventListener (string eventType, EventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            List<EventHandler> handlers;
            if (eventListeners.TryGetValue (eventType, out handlers)) {
                handlers.Remove (handler);
            }
        }

        protected bool SetProperty<T> (ref T backingStore, T newValue, string attributeName, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (!backingStore.Equals (newValue)) {
                backingStore = newValue;
                SendSet (attributeName, newValue);
                return true;
            }
            return false;
        }

        static long idCounter = 0;
        static string GenerateId ()
        {
            var id = System.Threading.Interlocked.Increment (ref idCounter);
            return "n" + id;
        }

        public virtual void Send (Message message)
        {
            SaveStateMessageIfNeeded (message);
            MessageSent?.Invoke (message);
        }

        protected void SendCall (string methodName, params object[] args)
        {
            Send (new Message {
                MessageType = MessageType.Call,
                TargetId = Id,
                Key = methodName,
                Value = args,
            });
        }

        protected void SendSet (string attributeName, object value)
        {
            Send (new Message {
                MessageType = MessageType.Set,
                TargetId = Id,
                Key = attributeName,
                Value = value,
            });
        }

        public virtual void Receive (Message message)
        {
            if (message == null)
                return;
            if (message.TargetId != Id)
                return;
            SaveStateMessageIfNeeded (message);
            TriggerEventFromMessage (message);
        }

        protected void SaveStateMessage (Message message)
        {
            stateMessages.Add (message);
        }

        protected void ReplaceStateMessage (Message old, Message message)
        {
            if (old != null) {
                stateMessages.Remove (old);
            }
            stateMessages.Add (message);
        }

        protected virtual void SaveStateMessageIfNeeded (Message message)
        {
            switch (message.MessageType) {
                case MessageType.Create:
                    SaveStateMessage (message);
                    break;
                case MessageType.Set:
                    {
                        var old = stateMessages.FirstOrDefault (
                            x => x.MessageType == MessageType.Set &&
                                x.Key == message.Key);
                        ReplaceStateMessage (old, message);
                    }
                    break;
                case MessageType.Listen:
                    SaveStateMessage (message);
                    break;
            }
        }

        protected virtual void TriggerEventFromMessage (Message message)
        {
            List<EventHandler> handlers;
            if (eventListeners.TryGetValue (message.Key, out handlers)) {
                var args = EventArgs.Empty;
                foreach (var h in handlers) {
                    h.Invoke (this, args);
                }
            }
        }
    }
}
