using System;

namespace Ooui
{
    public class Button : FormControl
    {
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
