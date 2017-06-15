using System;

namespace Ooui
{
    public abstract class Element : Node
    {
        string className = "";
        public string ClassName {
            get => className;
            set => SetProperty (ref className, value, "className");
        }

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
        }
    }
}
