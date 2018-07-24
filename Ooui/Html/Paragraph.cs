using System;

namespace Ooui.Html {
    public class Paragraph : Element
    {
        public Paragraph ()
            : base ("p")
        {
        }

        public Paragraph (string text)
            : this ()
        {
            Text = text;
        }
    }
}
