using System;

namespace Ooui
{
    public class Text : Node
    {
        string data = "";
        public string Data {
            get => data;
            set => SetProperty (ref data, value ?? "", "data");
        }

        public Text ()
        {            
        }

        public Text (string text)
        {
            Data = text;
        }
    }
}
