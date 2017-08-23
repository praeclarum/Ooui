using System;

namespace Ooui
{
    public class Form : Element
    {
        string action = "";
        public string Action {
            get => action;
            set => SetProperty (ref action, value ?? "", "action");
        }

		string method = "GET";
		public string Method {
			get => method;
			set => SetProperty (ref method, value ?? "", "method");
		}

		string enctype = "application/x-www-form-urlencoded";
		public string EncodingType {
			get => enctype;
			set => SetProperty (ref enctype, value ?? "", "enctype");
		}

		public event TargetEventHandler Submitted {
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
