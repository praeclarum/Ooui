using System;

namespace Ooui
{
    public class Button : FormControl
    {
        string typ = "submit";
        public string Type {
            get => typ;
            set => SetProperty (ref typ, value, "type");
        }

        string val = "";
        public string Value {
            get => val;
            set => SetProperty (ref val, value, "value");
        }

        public event EventHandler Clicked {
            add => AddEventListener ("onclick", value);
            remove => RemoveEventListener ("onclick", value);
        }

        public Button ()
        {
        }

        public Button (string text)
        {
            Text = text;
        }
    }
}
