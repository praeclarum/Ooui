using System;

namespace Ooui
{
    public class Select : FormControl
    {
        public string Value {
            get => GetStringAttribute ("value", "");
            set => SetAttributeProperty ("value", value ?? "");
        }

        public event TargetEventHandler Change {
            add => AddEventListener ("change", value);
            remove => RemoveEventListener ("change", value);
        }

        public event TargetEventHandler Input {
            add => AddEventListener ("input", value);
            remove => RemoveEventListener ("input", value);
        }

        public Select ()
            : base ("select")
        {
            // Subscribe to the change event so we always get up-to-date values
            Change += (s, e) => { };
        }

        public void AddOption (string label, string value)
        {
            AppendChild (new Option { Label = label, Value = value });
        }

        protected override void OnChildInsertedBefore (Node newChild, Node referenceChild)
        {
            base.OnChildInsertedBefore (newChild, referenceChild);
            var val = Value;
            if (string.IsNullOrEmpty (val) && newChild is Option o && !string.IsNullOrEmpty (o.Value)) {
                val = o.Value;
            }
        }

        protected override bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId == Id && message.MessageType == MessageType.Event && (message.Key == "change" || message.Key == "input")) {
                SetAttribute ("value", message.Value != null ? Convert.ToString (message.Value) : "");
                OnPropertyChanged ("Value");
            }
            return base.TriggerEventFromMessage (message);
        }
    }
}
