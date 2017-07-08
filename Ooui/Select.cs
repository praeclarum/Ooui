using System;

namespace Ooui
{
    public class Select : FormControl
    {
        string val = "";
        public string Value {
            get => val;
            set => SetProperty (ref val, value ?? "", "value");
        }

        public event TargetEventHandler Changed {
            add => AddEventListener ("change", value);
            remove => RemoveEventListener ("change", value);
        }

        public Select ()
            : base ("select")
        {
        }
    }
}
