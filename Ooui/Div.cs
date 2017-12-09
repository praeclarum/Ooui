using System;
using System.Collections.Generic;

namespace Ooui
{
    public class Div : Element
    {
        public Div ()
            : base ("div")
        {
        }

        public Div (params Element[] children)
            : this ()
        {
            foreach (var c in children) {
                AppendChild (c);
            }
        }

        public Div (IEnumerable<Element> children)
            : this ()
        {
            foreach (var c in children) {
                AppendChild (c);
            }
        }
    }
}
