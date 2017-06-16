using System;

namespace Ooui
{
    public class Label : Element
    {
        Element htmlFor = null;
        public Element For {
            get => htmlFor;
            set => SetProperty (ref htmlFor, value, "htmlFor");
        }

        public Label ()
            : base ("label")
        {
        }
    }
}
