using System;

namespace Ooui
{
    public class Button : Element
    {
        string name = "";
        public string Name {
            get => name;
            set => SetProperty (ref name, value);
        }
    }
}
