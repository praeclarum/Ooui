using System;

namespace Ooui
{
    public class Form : Element
    {
        string action = "";
        public string Action {
            get => action;
            set => SetProperty (ref action, value ?? "", "action");
        }

        public event EventHandler Submitted {
            add => AddEventListener ("submit", value);
            remove => RemoveEventListener ("submit", value);
        }

        public event EventHandler Reset {
            add => AddEventListener ("reset", value);
            remove => RemoveEventListener ("reset", value);
        }

        public Form ()
            : base ("form")
        {
        }
    }
}
