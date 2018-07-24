using System;

namespace Ooui.Html {
    public abstract class FormControl : Element
    {
        public string Name {
            get => GetStringAttribute ("name", "");
            set => SetAttributeProperty ("name", value);
        }

        public bool IsDisabled {
            get => GetBooleanAttribute ("disabled");
            set => SetBooleanAttributeProperty ("disabled", value);
        }

        public FormControl (string tagName)
            : base (tagName)
        {
        }
    }
}
