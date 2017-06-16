using System;

namespace Ooui
{
    public class Heading : Element
    {
        public Heading (int level = 1)
            : base ("h" + level)
        {
        }

        public Heading (int level, string text)
            : this (level)
        {
            Text = text;
        }

        public Heading (string text)
            : this ()
        {
            Text = text;
        }
    }
}
