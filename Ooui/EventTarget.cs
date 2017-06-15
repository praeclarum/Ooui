using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooui
{
    public abstract class EventTarget
    {
        public string Id { get; private set; } = GenerateId ();

        public Mapping Mapping { get; private set; }

        readonly List<Message> stateMessages = new List<Message> ();

        public event Action<Message> MessageSent;

        public IEnumerable<Message> StateMessages => stateMessages;

        public EventTarget ()
        {
            Mapping = Mapping.Get (GetType ());
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
            SaveStateMessage (message);
            MessageSent?.Invoke (message);
        }

        protected void SendCreate ()
        {
            Send (new Message {
                MessageType = MessageType.Create,
                TargetId = Id,
                Key = Mapping.TagName,
            });
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
            }
        }

        protected virtual void TriggerEventFromMessage (Message message)
        {
        }
    }
}
