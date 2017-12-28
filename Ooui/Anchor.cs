using System;

namespace Ooui
{
    public class Anchor : Element
    {
        string href = "";
        public string HRef {
            get => href;
            set => SetProperty (ref href, value ?? "", "href");
        }

        public Anchor ()
            : base ("a")
        {
        }
    }
}
