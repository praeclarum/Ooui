using System;

namespace Ooui
{
    public class TextInput : Input
    {
        public event TargetEventHandler Input {
            add => AddEventListener ("input", value);
            remove => RemoveEventListener ("input", value);
        }

        public TextInput ()
            : base (InputType.Text)
        {
        }
    }
}
