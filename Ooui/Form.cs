using System;

namespace Ooui
{
    public class Form : Element
    {
        string action = "";
        public string Action {
            get => GetStringAttribute ("action", "");
            set => SetAttributeProperty ("action", value ?? "");
        }

		public string Method {
            get => GetStringAttribute ("method", "GET");
            set => SetAttributeProperty ("method", value ?? "");
		}

		public string EncodingType {
            get => GetStringAttribute ("enctype", "application/x-www-form-urlencoded");
            set => SetAttributeProperty ("enctype", value ?? "");
		}

		public event TargetEventHandler Submit {
            add => AddEventListener ("submit", value);
            remove => RemoveEventListener ("submit", value);
        }

        public event TargetEventHandler Reset {
            add => AddEventListener ("reset", value);
            remove => RemoveEventListener ("reset", value);
        }

        public Form ()
            : base ("form")
        {
        }
    }
}
