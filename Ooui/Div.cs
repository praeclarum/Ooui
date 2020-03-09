using System;
using System.Collections.Generic;

namespace Ooui
{
    public class Div : Element
    {
        protected override bool HtmlNeedsFullEndElement => true;

        public Div ()
            : base ("div")
        {
        }

        public Div (params Node[] children)
            : this ()
        {
            foreach (var c in children) {
                AppendChild (c);
            }
        }

        public Div (IEnumerable<Node> children)
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
