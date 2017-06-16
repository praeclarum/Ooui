using System;

namespace Ooui
{
    public class TextArea : Element
    {
        public event EventHandler Changed {
            add => AddEventListener ("change", value);
            remove => RemoveEventListener ("change", value);
        }

        string text = "";
        public override string Text {
            get => text;
            set => SetProperty (ref text, value, "value");
        }

        int rows = 2;
        public int Rows {
            get => rows;
            set => SetProperty (ref rows, value, "rows");
        }

        int cols = 2;
        public int Columns {
            get => cols;
            set => SetProperty (ref cols, value, "cols");
        }

        public TextArea ()
            : base ("textarea")
        {
        }
    }
}
