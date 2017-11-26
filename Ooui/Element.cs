using System;
using System.ComponentModel;

namespace Ooui
{
    public abstract class Element : Node
    {
        string className = "";
        public string ClassName {
            get => className;
            set => SetProperty (ref className, value, "className");
        }

        public Style Style { get; private set; } = new Style ();

        string title = "";
        public string Title {
            get => title;
            set => SetProperty (ref title, value, "title");
        }

        bool hidden = false;
        public bool IsHidden {
            get => hidden;
            set => SetProperty (ref hidden, value, "hidden");
        }

        public event TargetEventHandler Clicked {
            add => AddEventListener ("click", value);
            remove => RemoveEventListener ("click", value);
        }

        public event TargetEventHandler DoubleClicked {
            add => AddEventListener ("dblclick", value);
            remove => RemoveEventListener ("dblclick", value);
        }

        public event TargetEventHandler KeyDown {
            add => AddEventListener ("keydown", value);
            remove => RemoveEventListener ("keydown", value);
        }

        public event TargetEventHandler KeyPressed {
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

        public event TargetEventHandler MouseEntered {
            add => AddEventListener ("mouseenter", value);
            remove => RemoveEventListener ("mouseenter", value);
        }

        public event TargetEventHandler MouseLeft {
            add => AddEventListener ("mouseleave", value);
            remove => RemoveEventListener ("mouseleave", value);
        }

        public event TargetEventHandler MouseMoved {
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

        public event TargetEventHandler Wheeled {
            add => AddEventListener ("wheel", value);
            remove => RemoveEventListener ("wheel", value);
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

        public void SetAttribute (string attributeName, string value)
        {
            Send (new Message {
                MessageType = MessageType.SetAttribute,
                TargetId = Id,
                Key = attributeName,
                Value = value,
            });
        }

        void HandleStylePropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            SendSet ("style." + Style.GetJsName (e.PropertyName), Style[e.PropertyName]);
        }
    }
}
