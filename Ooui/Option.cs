using System;

namespace Ooui
{
    public class Option : Element
    {
        public string Value {
            get => GetStringAttribute ("value", "");
            set => SetAttributeProperty ("value", value ?? "");
        }

        public string Label {
            get => GetStringAttribute ("label", "");
            set => SetAttributeProperty ("label", value ?? "");
        }

        public bool DefaultSelected {
            get => GetBooleanAttribute ("selected");
            set => SetBooleanAttributeProperty ("selected", value);
        }

        public Option ()
            : base ("option")
        {
        }
    }
}
