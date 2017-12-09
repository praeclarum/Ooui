using System;

namespace Ooui
{
    public class Select : FormControl
    {
        string val = "";
        public string Value {
            get => val;
            set => SetProperty (ref val, value ?? "", "value");
        }

        public event TargetEventHandler Changed {
            add => AddEventListener ("change", value);
            remove => RemoveEventListener ("change", value);
        }

        public event TargetEventHandler Inputted {
            add => AddEventListener ("input", value);
            remove => RemoveEventListener ("input", value);
        }

        public Select ()
            : base ("select")
        {
            // Subscribe to the change event so we always get up-to-date values
            Changed += (s, e) => { };
        }

        public void AddOption (string label, string value)
        {
            AppendChild (new Option { Label = label, Value = value });
        }

        protected override void OnChildInsertedBefore (Node newChild, Node referenceChild)
        {
            base.OnChildInsertedBefore (newChild, referenceChild);
            if (string.IsNullOrEmpty (val) && newChild is Option o && !string.IsNullOrEmpty (o.Value)) {
                val = o.Value;
            }
        }

        protected override bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId == Id && message.MessageType == MessageType.Event && (message.Key == "change" || message.Key == "input")) {
                val = message.Value != null ? Convert.ToString (message.Value) : "";
            }
            return base.TriggerEventFromMessage (message);
        }
    }
}
