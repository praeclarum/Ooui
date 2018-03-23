using System;
using System.Linq;

namespace Ooui
{
    public class Select : FormControl
    {
        bool gotValue = false;

        public string Value {
            get {
                if (gotValue) return GetStringAttribute ("value", "");
                return GetDefaultValue ();
            }
            set {
                gotValue = true;
                SetAttributeProperty ("value", value ?? "");
            }
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
            Change += (s, e) => { gotValue = true; };
        }

        public void AddOption (string label, string value)
        {
            AppendChild (new Option { Text = label, Value = value });
        }

        string GetDefaultValue ()
        {
            var options = Children.OfType<Option> ();
            var r = options.FirstOrDefault (x => x.DefaultSelected);
            if (r != null)
                return r.Value;
            r = options.FirstOrDefault ();
            return r?.Value ?? "";
        }

        protected override bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId == Id && message.MessageType == MessageType.Event && (message.Key == "change" || message.Key == "input" || message.Key == "keyup")) {
                gotValue = true;
                UpdateAttributeProperty ("value", message.Value != null ? Convert.ToString (message.Value) : "", "Value");
            }
            return base.TriggerEventFromMessage (message);
        }
    }
}
