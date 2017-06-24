using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooui
{
    [Newtonsoft.Json.JsonConverter (typeof (EventTargetJsonConverter))]
    public abstract class EventTarget
    {
        readonly List<Message> stateMessages = new List<Message> ();

        readonly Dictionary<string, List<EventHandler>> eventListeners =
            new Dictionary<string, List<EventHandler>> ();

        public string Id { get; private set; } = GenerateId ();

        public string TagName { get; private set; }

        public event Action<Message> MessageSent;

        public IReadOnlyList<Message> StateMessages {
            get {
                lock (stateMessages) {
                    return new List<Message> (stateMessages).AsReadOnly ();
                }
            }
        }

        protected EventTarget (string tagName)
        {
            TagName = tagName;

            Send (new Message {
                MessageType = MessageType.Create,
                TargetId = Id,
                Key = TagName,
            });
        }

        public override string ToString() => $"<{TagName} id=\"{Id}\" />";

        public virtual EventTarget GetElementById (string id)
        {
            if (id == Id) return this;
            return null;
        }

        public void AddEventListener (string eventType, EventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            var sendListen = false;

            List<EventHandler> handlers;
            lock (eventListeners) {
                if (!eventListeners.TryGetValue (eventType, out handlers)) {
                    handlers = new List<EventHandler> ();
                    eventListeners[eventType] = handlers;
                    sendListen = true;
                }
                handlers.Add (handler);
            }

            if (sendListen)
                Send (new Message {
                    MessageType = MessageType.Listen,
                    TargetId = Id,
                    Key = eventType,
                });
        }

        public void RemoveEventListener (string eventType, EventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            List<EventHandler> handlers;
            lock (eventListeners) {
                if (eventListeners.TryGetValue (eventType, out handlers)) {
                    handlers.Remove (handler);
                }
            }
        }

        protected bool SetProperty<T> (ref T backingStore, T newValue, string attributeName, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals (backingStore, newValue))
                return false;
            backingStore = newValue;
            SendSet (attributeName, newValue);
            OnPropertyChanged (propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged (string propertyName)
        {
        }

        public const char IdPrefix = '\u2999';

        static long idCounter = 0;
        static string GenerateId ()
        {
            var id = System.Threading.Interlocked.Increment (ref idCounter);
            return $"{IdPrefix}{id}";
        }

        public void Send (Message message)
        {
            if (message == null)
                return;
            if (message.TargetId == Id)
                SaveStateMessageIfNeeded (message);
            MessageSent?.Invoke (message);
        }

        protected void SendCall (string methodName, params object[] args)
        {
            Send (Message.Call (Id, methodName, args));
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

        public void Receive (Message message)
        {
            if (message == null)
                return;
            if (message.TargetId == Id)
                SaveStateMessageIfNeeded (message);
            TriggerEventFromMessage (message);
        }

        protected void AddStateMessage (Message message)
        {
            lock (stateMessages) stateMessages.Add (message);
        }

        protected void UpdateStateMessages (Action<List<Message>> updater)
        {
            lock (stateMessages) updater (stateMessages);
        }

        protected virtual void SaveStateMessageIfNeeded (Message message)
        {
            switch (message.MessageType) {
                case MessageType.Create:
                    AddStateMessage (message);
                    break;
                case MessageType.Set:
                    UpdateStateMessages (state => {
                        state.RemoveAll (x => x.MessageType == MessageType.Set && x.Key == message.Key);
                        state.Add (message);
                    });
                    break;
                case MessageType.Listen:
                    AddStateMessage (message);
                    break;
            }
        }

        protected virtual bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId != Id)
                return false;
            List<EventHandler> handlers = null;
            lock (eventListeners) {
                List<EventHandler> hs;
                if (eventListeners.TryGetValue (message.Key, out hs)) {
                    handlers = new List<EventHandler> (hs);
                }
            }
            if (handlers == null) return true;
            var args = EventArgs.Empty;
            foreach (var h in handlers) {
                h.Invoke (this, args);
            }
            return true;
        }
    }

    class EventTargetJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson (Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue (((EventTarget)value).Id);
        }

        public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotSupportedException ();
        }

        public override bool CanConvert (Type objectType)
        {
            return typeof (EventTarget).IsAssignableFrom (objectType);
        }
    }
}
