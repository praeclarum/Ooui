using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooui
{
    public abstract class Node : EventTarget
    {
        readonly List<Node> children = new List<Node> ();

        public IEnumerable<Node> Children => children;

        public virtual string Text {
            get { return String.Join ("", from c in children select c.Text); }
            set {
                ReplaceAll (new TextNode (value ?? ""));
            }
        }

        protected Node (string tagName)
            : base (tagName)
        {            
        }

        public override EventTarget GetElementById (string id)
        {
            if (id == Id) return this;
            foreach (var c in Children) {
                if (c is Element e) {
                    var r = e.GetElementById (id);
                    if (r != null)
                        return r;
                }
            }
            return null;
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
            SendCall ("insertBefore", newChild, referenceChild);
            return newChild;
        }

        public Node RemoveChild (Node child)
        {
            if (!children.Remove (child)) {
                throw new ArgumentException ("Child not contained in this element", nameof(child));
            }
            child.ParentNode = null;
            SendCall ("removeChild", child);
            return child;
        }

        protected void ReplaceAll (Node newNode)
        {
            var toRemove = new List<Node> (children);
            foreach (var c in toRemove)
                RemoveChild (c);
            InsertBefore (newNode, null);
        }

        protected override void SaveStateMessageIfNeeded (Message message)
        {
            switch (message.MessageType) {
                case MessageType.Call when 
                    message.Key == "insertBefore" ||
                    message.Key == "removeChild":
                    SaveStateMessage (message);
                    break;
                default:
                    base.SaveStateMessageIfNeeded (message);
                    break;
            }
        }
    }
}
