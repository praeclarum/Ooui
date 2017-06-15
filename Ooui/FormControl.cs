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

        public FormControl (string tagName)
            : base (tagName)
        {
        }
    }
}
