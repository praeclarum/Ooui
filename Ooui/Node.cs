using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooui
{
    public abstract class Node
    {
        public string Id { get; private set; } = GenerateId ();

        public Mapping Mapping { get; private set; }

        readonly List<Node> children = new List<Node> ();
        readonly List<Message> messages = new List<Message> ();
        readonly List<Message> stateMessages = new List<Message> ();

        public event Action<Message> MessageLogged;

        public IEnumerable<Message> AllMessages =>
            messages
            .Concat (from c in children from m in c.AllMessages select m)
            .OrderBy (x => x.Id);

        public Node ()
        {
            Mapping = Mapping.Get (GetType ());
            LogCreate ();
        }

        public Node AppendChild (Node newChild)
        {
            return InsertBefore (newChild, null);
        }

        public Node ParentNode { get; private set; }

        public Node InsertBefore (Node newChild, Node referenceChild)
        {
            if (referenceChild == null) {
                children.Add (newChild);                
            }
            else {
                var index = children.IndexOf (referenceChild);
                if (index < 0) {
                    throw new ArgumentException ("Reference must be a child of this element", nameof(referenceChild));
                }
                children.Insert (index, newChild);
            }
            newChild.ParentNode = this;
            LogCall ("insertBefore", newChild, referenceChild);
            return newChild;
        }

        public Node RemoveChild (Node child)
        {
            if (!children.Remove (child)) {
                throw new ArgumentException ("Child not contained in this element", nameof(child));
            }
            child.ParentNode = null;
            LogCall ("removeChild", child);
            return child;
        }

        protected void Log (Message message)
        {
            messages.Add (message);

            switch (message.MessageType) {
                case MessageType.Create:
                    stateMessages.Add (message);
                    break;
                case MessageType.Set:
                    {
                        var old = stateMessages.FirstOrDefault (
                            x => x.MessageType == MessageType.Set &&
                                x.Key == message.Key);
                        if (old != null) {
                            stateMessages.Remove (old);
                        }
                        stateMessages.Add (message);
                    }
                    break;
            }

            MessageLogged?.Invoke (message);
        }

        protected void LogCreate ()
        {
            Log (new Message {
                MessageType = MessageType.Create,
                TargetId = Id,
                Key = Mapping.TagName,
            });
        }

        protected void LogCall (string methodName, params object[] args)
        {
            Log (new Message {
                MessageType = MessageType.Call,
                TargetId = Id,
                Key = methodName,
            });
        }

        protected void LogSet (string attributeName, object value)
        {
            Log (new Message {
                MessageType = MessageType.Set,
                TargetId = Id,
                Key = attributeName,
                Value = value,
            });
        }

        protected bool SetProperty<T> (ref T backingStore, T newValue, string attributeName, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (!backingStore.Equals (newValue)) {
                backingStore = newValue;
                LogSet (attributeName, newValue);
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
    }
}
