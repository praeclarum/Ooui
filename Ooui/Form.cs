using System;

namespace Ooui
{
    public class Form : Element
    {
        public event EventHandler Submitted {
            add => AddEventListener ("submit", value);
            remove => RemoveEventListener ("submit", value);
        }

        public Form ()
            : base ("form")
        {
        }
    }
}
