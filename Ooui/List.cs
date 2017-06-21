using System;

namespace Ooui
{
    public class List : Element
    {
        public List (bool ordered = false)
            : base (ordered ? "ol" : "ul")
        {
        }
    }
}
