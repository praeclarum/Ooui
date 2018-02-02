using System;

namespace Ooui
{
    public class Anchor : Element
    {
        public string HRef {
            get => GetStringAttribute ("href", "");
            set => SetAttributeProperty ("href", value);
        }

        public string Target {
            get => GetStringAttribute ("target", "");
            set => SetAttributeProperty ("target", value);
        }

        public Anchor ()
            : base ("a")
        {
        }
    }
}
