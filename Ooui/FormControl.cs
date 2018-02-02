using System;

namespace Ooui
{
    public abstract class FormControl : Element
    {
        public string Name {
            get => GetStringAttribute ("name", "");
            set => SetAttributeProperty ("name", value);
        }

        bool isDisabled = false;
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
