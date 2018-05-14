using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace Ooui
{
    [Newtonsoft.Json.JsonConverter (typeof (EventTargetJsonConverter))]
    public abstract class EventTarget : INotifyPropertyChanged
    {
        readonly List<Message> stateMessages = new List<Message> ();

        readonly Dictionary<string, List<TargetEventHandler>> targetEventListeners =
            new Dictionary<string, List<TargetEventHandler>> ();

        readonly Dictionary<string, List<DOMEventHandler>> domEventListeners =
            new Dictionary<string, List<DOMEventHandler>> ();

        public string Id { get; protected set; } = GenerateId ();

        public string TagName { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<Message> MessageSent;

        public IReadOnlyList<Message> StateMessages {
            get {
                lock (stateMessages) {
                    return new ReadOnlyList<Message> (stateMessages);
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

        public void AddEventListener (string eventType, TargetEventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            var sendListen = false;

            List<TargetEventHandler> handlers;
            lock (targetEventListeners) {
                if (!targetEventListeners.TryGetValue (eventType, out handlers)) {
                    handlers = new List<TargetEventHandler> ();
                    targetEventListeners[eventType] = handlers;
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

        public void RemoveEventListener (string eventType, TargetEventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            List<TargetEventHandler> handlers;
            lock (targetEventListeners) {
                if (targetEventListeners.TryGetValue (eventType, out handlers)) {
                    handlers.Remove (handler);
                }
            }
        }

        public void AddEventListener (string eventType, DOMEventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            var sendListen = false;

            List<DOMEventHandler> handlers;
            lock (domEventListeners) {
                if (!domEventListeners.TryGetValue (eventType, out handlers)) {
                    handlers = new List<DOMEventHandler> ();
                    domEventListeners[eventType] = handlers;
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

        public void RemoveEventListener (string eventType, DOMEventHandler handler)
        {
            if (eventType == null) return;
            if (handler == null) return;

            List<DOMEventHandler> handlers;
            lock (domEventListeners) {
                if (domEventListeners.TryGetValue (eventType, out handlers)) {
                    handlers.Remove (handler);
                }
            }
        }

        protected bool SetProperty<T> (ref T backingStore, T newValue, string jsPropertyName, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals (backingStore, newValue))
                return false;
            backingStore = newValue;
            SendSet (jsPropertyName, newValue);
            OnPropertyChanged (propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged (string propertyName)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
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

        public void Call (string methodName, params object[] args)
        {
            Send (Message.Call (Id, methodName, args));
        }

        protected void SendSet (string jsPropertyName, object value)
        {
            Send (new Message {
                MessageType = MessageType.Set,
                TargetId = Id,
                Key = jsPropertyName,
                Value = value,
            });
        }

        public void Receive (Message message)
        {
            if (message == null)
                return;
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

        protected virtual bool SaveStateMessageIfNeeded (Message message)
        {
            if (message.TargetId != Id)
                return false;

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
                case MessageType.SetAttribute:
                    UpdateStateMessages (state => {
                        state.RemoveAll (x => x.MessageType == MessageType.SetAttribute && x.Key == message.Key);
                        state.Add (message);
                    });
                    break;
                case MessageType.RemoveAttribute:
                    this.UpdateStateMessages (state => {
                        state.RemoveAll (x => x.MessageType == MessageType.SetAttribute && x.Key == message.Key);
                    });
                    return true;
                case MessageType.Listen:
                    AddStateMessage (message);
                    break;
            }

            return true;
        }

        protected virtual bool TriggerEvent (string name)
        {
            List<TargetEventHandler> handlers = null;
            lock (targetEventListeners) {
                List<TargetEventHandler> hs;
                if (targetEventListeners.TryGetValue (name, out hs)) {
                    handlers = new List<TargetEventHandler> (hs);
                }
            }
            if (handlers != null) {
                var args = new TargetEventArgs ();
                foreach (var h in handlers) {
                    h.Invoke (this, args);
                }
            }
            return true;
        }

        protected virtual bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId != Id)
                return false;

            List<Delegate> handlers = new List<Delegate>();
            lock (targetEventListeners) {
                List<TargetEventHandler> hs;
                if (targetEventListeners.TryGetValue (message.Key, out hs)) {
                    handlers.AddRange(hs);
                }
            }
            lock (domEventListeners) {
                List<DOMEventHandler> hs;
                if (domEventListeners.TryGetValue (message.Key, out hs)) {
                    handlers.AddRange(hs);
                }
            }
            if (handlers != null && handlers.Count > 0) {
                var tArgs = new TargetEventArgs ();
                var domArgs = new DOMEventArgs();
                if (message.Value is Newtonsoft.Json.Linq.JObject o)
                {
                    domArgs.Data = o;
                    try
                    {
                        tArgs.OffsetX = (double)o["offsetX"];
                        tArgs.OffsetY = (double)o["offsetY"];
                    }
                    catch { }
                }
                foreach (var h in handlers) {
                    if (h is TargetEventHandler targetEventHandler)
                    {
                        targetEventHandler.Invoke(this, tArgs);
                    }
                    else if(h is DOMEventHandler domEventHandler)
                    {
                        domEventHandler.Invoke(this, domArgs);
                    }
                    else
                    {
                        h.DynamicInvoke(this, message.Value);
                    }
                }
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
            return typeof (EventTarget).GetTypeInfo ().IsAssignableFrom (objectType.GetTypeInfo ());
        }
    }

    public delegate void TargetEventHandler (object sender, TargetEventArgs e);
    public delegate void DOMEventHandler (object sender, DOMEventArgs e);

    public class TargetEventArgs : EventArgs
    {
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
    }

    public class DOMEventArgs : EventArgs
    {
        public Newtonsoft.Json.Linq.JObject Data { get; set; }
    }
}
