using System;

namespace Ooui.Html {
    public class List : Element
    {
        public List (bool ordered = false)
            : base (ordered ? "ol" : "ul")
        {
        }
    }
}
