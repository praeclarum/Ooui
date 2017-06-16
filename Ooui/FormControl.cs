using System;

namespace Ooui
{
    public abstract class FormControl : Element
    {
        string name = "";
        public string Name {
            get => name;
            set => SetProperty (ref name, value, "name");
        }

        bool isDisabled = false;
        public bool IsDisabled {
            get => isDisabled;
            set => SetProperty (ref isDisabled, value, "disabled");
        }

        public Form Form {
            get {
                var p = ParentNode;
                while (p != null && !(p is Form)) {
                    p = p.ParentNode;
                }
                return p as Form;
            }
        }

        public FormControl (string tagName)
            : base (tagName)
        {
        }
    }
}
