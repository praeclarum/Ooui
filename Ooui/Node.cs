using System;
using System.Collections;
using System.Collections.Generic;

namespace Ooui
{
    public abstract class Node : EventTarget
    {
        readonly List<Node> children = new List<Node> ();

        public IReadOnlyList<Node> Children {
            get {
                lock (children) {
                    return new ReadOnlyList<Node> (children);
                }
            }
        }

        public Node FirstChild {
            get {
                lock (children) {
                    if (children.Count > 0)
                        return children[0];
                    return null;
                }
            }
        }

        public virtual string Text {
            get {
                var sb = new System.Text.StringBuilder ();
                foreach (var c in Children) {
                    sb.Append (c.Text);
                }
                return sb.ToString ();
            }
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

        public Node InsertBefore (Node newChild, Node referenceChild)
        {
            if (newChild == null)
                return null;
            lock (children) {
                if (referenceChild == null) {
                    children.Add (newChild);
                }
                else {
                    var index = children.IndexOf (referenceChild);
                    if (index < 0) {
                        throw new ArgumentException ("Reference must be a child of this element", nameof (referenceChild));
                    }
                    children.Insert (index, newChild);
                }
            }
            newChild.MessageSent += HandleChildMessageSent;
            Call ("insertBefore", newChild, referenceChild);
            OnChildInsertedBefore (newChild, referenceChild);
            return newChild;
        }

        public Node RemoveChild (Node child)
        {
            if (child == null)
                return null;
            lock (children) {
                if (!children.Remove (child)) {
                    throw new ArgumentException ("Child not contained in this element", nameof (child));
                }
            }
            child.MessageSent -= HandleChildMessageSent;
            Call ("removeChild", child);
            OnChildRemoved (child);
            return child;
        }

        protected virtual void OnChildInsertedBefore (Node newChild, Node referenceChild)
        {
        }

        protected virtual void OnChildRemoved (Node child)
        {
        }

        protected void ReplaceAll (Node newNode)
        {
            var toRemove = new List<Node> ();
            lock (children) {
                toRemove.AddRange (children);
                children.Clear ();
            }
            foreach (var child in toRemove) {
                child.MessageSent -= HandleChildMessageSent;
                Call ("removeChild", child);
                // TODO: Should call OnChildRemoved?
            }
            InsertBefore (newNode, null);
        }

        public void ReplaceChildrenWith (params Node[] nodes)
        {
            ReplaceChildren(nodes);
        }

        public void ReplaceChildren (IEnumerable<Node> replaceWith)
        {
            var wanted = new List<Node> (16);
            foreach (var node in replaceWith) {
                if (node != null) {
                    wanted.Add (node);
                }
            }

            var current = new List<Node> ();

            lock (children) {
                current.AddRange (children);
                children.Clear ();
                children.AddRange (wanted);
            }

            var previous = current.ToArray ();

            void RemoveChild (Node child)
            {
                child.MessageSent -= HandleChildMessageSent;
                Call ("removeChild", child);
                // TODO: Should call OnChildRemoved?
                current.Remove (child);
            }

            void InsertAt (int i, Node child)
            {
                var existing = i < current.Count ? current[i] : null;
                if (existing != null && existing.PersistentId == child.PersistentId) {
                    return;
                } else {
                    //var next = i + 1 < current.Count ? current[i + 1] : null;
                    Call ("insertBefore", child, existing);
                    // TODO: Should call OnInsertBefore?

                    if (current.Remove (child)) {
                        current.Insert (i, child);
                        return;
                    } else {
                        child.MessageSent += HandleChildMessageSent;
                        current.Insert (i, child);
                        return;
                    }
                }
            }

            var insertionPoint = 0;

            for (var iter = 0; iter < wanted.Count; ++iter) {
                InsertAt (insertionPoint, wanted[iter]);
                ++insertionPoint;
            }

            for (var iter = current.Count - 1; iter >= insertionPoint; --iter) {
                RemoveChild (current[iter]);
            }

        }

        void HandleChildMessageSent (Message message)
        {
            Send (message);
        }

        protected override bool SaveStateMessageIfNeeded (Message message)
        {
            if (message.TargetId == Id) {
                var handled = false;
                switch (message.MessageType) {
                    case MessageType.Call when message.Key == "insertBefore":
                        AddStateMessage (message);
                        break;
                    case MessageType.Call when message.Key == "removeChild" && message.Value is Array ma && ma.Length == 1:
                        UpdateStateMessages (state => {
                            var mchild = ma.GetValue (0);
                            Node nextChild = null;
                            for (var i = 0; i < state.Count;) {
                                var x = state[i];
                                if (x.Key == "insertBefore" && x.Value is Array xa && xa.Length == 2 && ReferenceEquals (xa.GetValue (0), mchild)) {
                                    // Remove any inserts for this node
                                    nextChild = xa.GetValue (1) as Node;
                                    state.RemoveAt (i);
                                }
                                else if (x.Key == "insertBefore" && x.Value is Array ya && ya.Length == 2 && ReferenceEquals (ya.GetValue (1), mchild)) {
                                    // Replace inserts that reference this node
                                    state[i] = Message.Call (Id, "insertBefore", ya.GetValue (0), nextChild);
                                    i++;
                                }
                                else {
                                    i++;
                                }
                            }
                        });
                        break;
                }
                if (!handled) {
                    base.SaveStateMessageIfNeeded (message);
                }
                return true;
            }
            else {
                var ch = Children;
                for (var i = 0; i < ch.Count; i++) {
                    if (ch[i].SaveStateMessageIfNeeded (message))
                        return true;
                }
                return false;
            }
        }

        protected override bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId == Id) {
                if (base.TriggerEventFromMessage (message))
                    return true;
            }
            else {
                var ch = Children;
                for (var i = 0; i < ch.Count; i++) {
                    if (ch[i].TriggerEventFromMessage (message))
                        return true;
                }
            }
            return false;
        }

#if !NO_XML

        public virtual string OuterHtml {
            get {
                using (var stream = new System.IO.MemoryStream ()) {
                    var settings = new System.Xml.XmlWriterSettings {
                        OmitXmlDeclaration = true,
                        ConformanceLevel = System.Xml.ConformanceLevel.Fragment,
                        CloseOutput = false,
                    };
                    using (var w = System.Xml.XmlWriter.Create (stream, settings)) {
                        WriteOuterHtml (w);
                    }
                    stream.Position = 0;
                    return new System.IO.StreamReader (stream).ReadToEnd ();
                }
            }
        }

        public abstract void WriteOuterHtml (System.Xml.XmlWriter w);

#endif
    }

    class ReadOnlyList<T> : IReadOnlyList<T>
    {
        readonly List<T> list;

        public ReadOnlyList (List<T> items)
        {
            list = new List<T> (items);
        }

        T IReadOnlyList<T>.this[int index] => list[index];

        int IReadOnlyCollection<T>.Count => list.Count;

        IEnumerator<T> IEnumerable<T>.GetEnumerator ()
        {
            return ((IEnumerable<T>)list).GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return ((IEnumerable)list).GetEnumerator ();
        }
    }
}
