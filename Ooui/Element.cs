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
