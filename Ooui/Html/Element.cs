using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Ooui.Html {
    public abstract class Element : Node
    {
        readonly Dictionary<string, object> attributes = new Dictionary<string, object> ();

        Document document = null;

        public string ClassName {
            get => GetStringAttribute ("class", "");
            set => SetAttributeProperty ("class", value);
        }

        public Style Style { get; private set; } = new Style ();

        public string Title {
            get => GetStringAttribute ("title", "");
            set => SetAttributeProperty ("title", value);
        }

        public bool IsHidden {
            get => GetBooleanAttribute ("hidden");
            set => SetBooleanAttributeProperty ("hidden", value);
        }

        public event TargetEventHandler Click {
            add => AddEventListener ("click", value);
            remove => RemoveEventListener ("click", value);
        }

        public event TargetEventHandler DoubleClick {
            add => AddEventListener ("dblclick", value);
            remove => RemoveEventListener ("dblclick", value);
        }

        public event TargetEventHandler KeyDown {
            add => AddEventListener ("keydown", value);
            remove => RemoveEventListener ("keydown", value);
        }

        public event TargetEventHandler KeyPress {
            add => AddEventListener ("keypress", value);
            remove => RemoveEventListener ("keypress", value);
        }

        public event TargetEventHandler KeyUp {
            add => AddEventListener ("keyup", value);
            remove => RemoveEventListener ("keyup", value);
        }

        public event TargetEventHandler MouseDown {
            add => AddEventListener ("mousedown", value);
            remove => RemoveEventListener ("mousedown", value);
        }

        public event TargetEventHandler MouseEnter {
            add => AddEventListener ("mouseenter", value);
            remove => RemoveEventListener ("mouseenter", value);
        }

        public event TargetEventHandler MouseLeave {
            add => AddEventListener ("mouseleave", value);
            remove => RemoveEventListener ("mouseleave", value);
        }

        public event TargetEventHandler MouseMove {
            add => AddEventListener ("mousemove", value);
            remove => RemoveEventListener ("mousemove", value);
        }

        public event TargetEventHandler MouseOut {
            add => AddEventListener ("mouseout", value);
            remove => RemoveEventListener ("mouseout", value);
        }

        public event TargetEventHandler MouseOver {
            add => AddEventListener ("mouseover", value);
            remove => RemoveEventListener ("mouseover", value);
        }

        public event TargetEventHandler MouseUp {
            add => AddEventListener ("mouseup", value);
            remove => RemoveEventListener ("mouseup", value);
        }

        public event TargetEventHandler Wheel {
            add => AddEventListener ("wheel", value);
            remove => RemoveEventListener ("wheel", value);
        }

        public Document Document {
            get {
                if (document == null) {
                    if (Interlocked.CompareExchange (ref document, new Document (), null) == null) {
                        document.MessageSent += Document_MessageSent;
                    }
                }
                return document;
            }
        }

        /// <summary>
        /// A signal to Ooui that this element should take up the
        /// entire browser window.
        /// </summary>
        public virtual bool WantsFullScreen => false;

        protected Element (string tagName)
            : base (tagName)
        {
            Style.PropertyChanged += HandleStylePropertyChanged;
        }

        protected bool SetAttributeProperty (string attributeName, object newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            var old = GetAttribute (attributeName);
            if (old != null && old.Equals (newValue))
                return false;
            SetAttribute (attributeName, newValue);
            OnPropertyChanged (propertyName);
            return true;
        }

        protected bool SetBooleanAttributeProperty (string attributeName, bool newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            var old = GetAttribute (attributeName) != null;
            if (old == newValue)
                return false;
            if (newValue)
                SetAttribute (attributeName, string.Empty);
            else
                RemoveAttribute (attributeName);
            OnPropertyChanged (propertyName);
            return true;
        }

        protected bool UpdateAttributeProperty (string attributeName, object newValue, string propertyName)
        {
            lock (attributes) {
                if (attributes.TryGetValue (attributeName, out var oldValue)) {
                    if (newValue != null && newValue.Equals (oldValue))
                        return false;
                }
                attributes[attributeName] = newValue;
            }
            OnPropertyChanged (propertyName);
            return true;
        }

        protected bool UpdateBooleanAttributeProperty (string attributeName, bool newValue, string propertyName)
        {
            lock (attributes) {
                var oldValue = attributes.ContainsKey (attributeName);
                if (newValue == oldValue)
                    return false;
                if (newValue) {
                    attributes[attributeName] = "";
                }
                else {
                    attributes.Remove (attributeName);
                }
            }
            OnPropertyChanged (propertyName);
            return true;
        }

        public void SetAttribute (string attributeName, object value)
        {
            lock (attributes) {
                attributes[attributeName] = value;
            }
            Send (new Message {
                MessageType = MessageType.SetAttribute,
                TargetId = Id,
                Key = attributeName,
                Value = value,
            });
        }

        public object GetAttribute (string attributeName)
        {
            lock (attributes) {
                attributes.TryGetValue (attributeName, out var v);
                return v;
            }
        }

        public T GetAttribute<T> (string attributeName, T defaultValue)
        {
            lock (attributes) {
                attributes.TryGetValue (attributeName, out var v);
                if (v is T) {
                    return (T)v;
                }
                else {
                    return defaultValue;
                }
            }
        }

        public bool GetBooleanAttribute (string attributeName)
        {
            lock (attributes) {
                return attributes.TryGetValue (attributeName, out var _);
            }
        }

        public string GetStringAttribute (string attributeName, string defaultValue)
        {
            lock (attributes) {
                if (attributes.TryGetValue (attributeName, out var v)) {
                    if (v == null) return "null";
                    else return v.ToString ();
                }
                else {
                    return defaultValue;
                }
            }
        }

        public void RemoveAttribute (string attributeName)
        {
            bool removed;
            lock (attributes) {
                removed = attributes.Remove (attributeName);
            }
            if (removed) {
                Send (new Message {
                    MessageType = MessageType.RemoveAttribute,
                    TargetId = Id,
                    Key = attributeName,
                });
            }
        }

        public void SetCapture (bool retargetToElement)
        {
            Call ("setCapture", retargetToElement);
        }

        public void Focus ()
        {
            Call ("focus");
        }

        void HandleStylePropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            SendSet ("style." + Style.GetJsName (e.PropertyName), Style[e.PropertyName]);
        }

        void Document_MessageSent (Message message)
        {
            Send (message);
        }

        protected virtual bool HtmlNeedsFullEndElement => false;

#if !NO_XML

        public override void WriteOuterHtml (System.Xml.XmlWriter w)
        {
            w.WriteStartElement (TagName);
            w.WriteAttributeString ("id", Id);
            var style = Style.ToString ();
            if (style.Length > 0) {
                w.WriteAttributeString ("style", style);
            }
            lock (attributes) {
                foreach (var a in attributes) {
                    var value = (a.Value == null) ? "null" : Convert.ToString (a.Value, System.Globalization.CultureInfo.InvariantCulture);
                    w.WriteAttributeString (a.Key, value);
                }
            }
            WriteInnerHtml (w);
            if (HtmlNeedsFullEndElement) {
                w.WriteFullEndElement ();
            }
            else {
                w.WriteEndElement ();
            }
        }

        public virtual void WriteInnerHtml (System.Xml.XmlWriter w)
        {
            var children = Children;
            foreach (var c in children) {
                c.WriteOuterHtml (w);
            }
        }

#endif
    }
}
