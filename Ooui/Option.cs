using System;

namespace Ooui
{
    public class Option : Element
    {
        string val = "";
        public string Value {
            get => val;
            set => SetProperty (ref val, value ?? "", "value");
        }

        string label = "";
        public string Label {
            get => label;
            set => SetProperty (ref label, value ?? "", "label");
        }

        bool defaultSelected = false;
        public bool DefaultSelected {
            get => defaultSelected;
            set => SetProperty (ref defaultSelected, value, "defaultSelected");
        }

        public Option ()
            : base ("option")
        {
        }
    }
}
