using System;

namespace Ooui
{
    public class TextArea : FormControl
    {
        public event EventHandler Changed {
            add => AddEventListener ("change", value);
            remove => RemoveEventListener ("change", value);
        }

        string val = "";
        public string Value {
            get => val;
            set => SetProperty (ref val, value ?? "", "value");
        }

        int rows = 2;
        public int Rows {
            get => rows;
            set => SetProperty (ref rows, value, "rows");
        }

        int cols = 20;
        public int Columns {
            get => cols;
            set => SetProperty (ref cols, value, "cols");
        }

        public TextArea ()
            : base ("textarea")
        {
        }

        public TextArea (string text)
            : this ()
        {
            Text = text;
        }
    }
}
