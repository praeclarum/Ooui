using System;

namespace Ooui.Html {
    public class Span : Element
    {
        public Span ()
            : base ("span")
        {
        }

        public Span (string text)
            : this ()
        {
            Text = text;
        }
    }
}
