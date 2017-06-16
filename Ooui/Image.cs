using System;

namespace Ooui
{
    public class Image : Element
    {
        string src = "";
        public string Source {
            get => src;
            set => SetProperty (ref src, value ?? "", "src");
        }

        public Image ()
            : base ("img")
        {
        }
    }
}
