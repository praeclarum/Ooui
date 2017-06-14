using System;

namespace Ooui
{
    public class TextNode : Node
    {
        string text = "";
        public override string Text {
            get => text;
            set => SetProperty (ref text, value ?? "", "data");
        }

        public TextNode ()
        {
        }

        public TextNode (string text)
        {
            Text = text;
        }
    }
}
