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

        public event EventHandler Clicked {
            add => AddEventListener ("click", value);
            remove => RemoveEventListener ("click", value);
        }

        public event EventHandler DoubleClicked {
            add => AddEventListener ("dblclick", value);
            remove => RemoveEventListener ("dblclick", value);
        }

        public event EventHandler KeyDown {
            add => AddEventListener ("keydown", value);
            remove => RemoveEventListener ("keydown", value);
        }

        public event EventHandler KeyPressed {
            add => AddEventListener ("keypress", value);
            remove => RemoveEventListener ("keypress", value);
        }

        public event EventHandler KeyUp {
            add => AddEventListener ("keyup", value);
            remove => RemoveEventListener ("keyup", value);
        }

        public event EventHandler MouseDown {
            add => AddEventListener ("mousedown", value);
            remove => RemoveEventListener ("mousedown", value);
        }

        public event EventHandler MouseEntered {
            add => AddEventListener ("mouseenter", value);
            remove => RemoveEventListener ("mouseenter", value);
        }

        public event EventHandler MouseLeft {
            add => AddEventListener ("mouseleave", value);
            remove => RemoveEventListener ("mouseleave", value);
        }

        public event EventHandler MouseMove {
            add => AddEventListener ("mousemove", value);
            remove => RemoveEventListener ("mousemove", value);
        }

        public event EventHandler MouseOut {
            add => AddEventListener ("mouseout", value);
            remove => RemoveEventListener ("mouseout", value);
        }

        public event EventHandler MouseOver {
            add => AddEventListener ("mouseover", value);
            remove => RemoveEventListener ("mouseover", value);
        }

        public event EventHandler MouseUp {
            add => AddEventListener ("mouseup", value);
            remove => RemoveEventListener ("mouseup", value);
        }

        public event EventHandler Wheeled {
            add => AddEventListener ("wheel", value);
            remove => RemoveEventListener ("wheel", value);
        }

        protected Element (string tagName)
            : base (tagName)
        {
            Style.PropertyChanged += HandleStylePropertyChanged;
        }

        void HandleStylePropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            SendSet ("style." + Style.GetJsName (e.PropertyName), Style[e.PropertyName]);
        }
    }
}
