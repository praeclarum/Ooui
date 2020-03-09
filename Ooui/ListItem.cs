using System;

namespace Ooui
{
    public class ListItem : Element
    {
        public ListItem ()
            : base ("li")
        {
        }

        public ListItem (string text)
            : this ()
        {
            Text = text;
        }
    }
}
