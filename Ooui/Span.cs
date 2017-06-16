using System;

namespace Ooui
{
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
