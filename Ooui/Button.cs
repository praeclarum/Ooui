using System;

namespace Ooui
{
    public class Button : FormControl
    {
        string val = "";
        public string Value {
            get => val;
            set => SetProperty (ref val, value, "value");
        }

        public event EventHandler Clicked {
            add => AddEventListener ("click", value);
            remove => RemoveEventListener ("click", value);
        }

        public Button ()
            : base ("button")
        {
        }

        public Button (string text)
            : this ()
        {
            Text = text;
        }
    }
}
