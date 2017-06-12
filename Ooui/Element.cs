using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooui
{
    public class Element
    {
        public string Id { get; private set; } = GenerateId ();

        public HtmlMapping Mapping { get; private set; }

        readonly List<Message> messages = new List<Message> ();
        readonly List<Message> stateMessages = new List<Message> ();

        public Element ()
        {
            Mapping = HtmlMapping.Get (GetType ());
            LogCreate ();
        }

        protected void Log (Message message)
        {
            messages.Add (message);

            switch (message.MessageType) {
                case MessageType.Create:
                    stateMessages.Add (message);
                    break;
                case MessageType.SetProperty:
                    {
                        var old = stateMessages.FirstOrDefault (
                            x => x.MessageType == MessageType.SetProperty &&
                                x.Member == message.Member);
                        if (old != null) {
                            stateMessages.Remove (old);
                        }
                        stateMessages.Add (message);
                    }
                    break;
            }
        }

        protected void LogCreate ()
        {
            Log (new Message {
                MessageType = MessageType.Create,
                TargetId = Id,
                Value = Mapping.TagName,
            });
        }

        protected void LogSetProperty (string propertyName, object value)
        {
            var m = new Message {
                MessageType = MessageType.SetProperty,
                TargetId = Id,
                Member = Mapping.GetMemberPath (propertyName),
            };
            m.SetValue (value);
            Log (m);
        }

        protected bool SetProperty<T> (ref T backingStore, T newValue, string propertyName = "")
        {
            if (!backingStore.Equals (newValue)) {
                backingStore = newValue;
                LogSetProperty (propertyName, newValue);
                return true;
            }
            return false;
        }

        const string IdChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        static string GenerateId ()
        {
            var rand = new Random();
            var chars = new char[8];
            for (var i = 0; i < chars.Length; i++) {
                chars[i] = IdChars[rand.Next(0, IdChars.Length)];
            }
            return new string(chars);
        }
    }
}
