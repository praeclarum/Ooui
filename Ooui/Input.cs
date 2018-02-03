using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ooui
{
    public class Input : FormControl
    {
        public InputType Type {
            get => GetAttribute ("type", InputType.Text);
            set => SetAttributeProperty ("type", value);
        }

        public string Value {
            get => GetStringAttribute ("value", "");
            set => SetAttributeProperty ("value", value ?? "");
        }

        public double NumberValue {
            get {
                double v;
                double.TryParse (Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentUICulture, out v);
                return v;
            }
            set {
                Value = value.ToString (System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        public event TargetEventHandler Change {
            add => AddEventListener ("change", value);
            remove => RemoveEventListener ("change", value);
        }

        public string Placeholder {
            get => GetStringAttribute ("placeholder", "");
            set => SetAttributeProperty ("placeholder", value ?? "");
        }

        public bool IsChecked {
            get => GetBooleanAttribute ("checked");
            set {
                if (SetBooleanAttributeProperty ("checked", value)) {
                    TriggerEvent ("change");
                }
            }
        }

        public double Minimum {
            get => GetAttribute ("min", 0.0);
            set => SetAttributeProperty ("min", value);
        }

        public double Maximum {
            get => GetAttribute ("max", 100.0);
            set => SetAttributeProperty ("max", value);
        }

        public double Step {
            get => GetAttribute ("step", 1.0);
            set => SetAttributeProperty ("step", value);
        }

        public Input ()
            : base ("input")
        {
            // Subscribe to the change event so we always get up-to-date values
            Change += (s, e) => {};
        }

        public Input (InputType type)
            : this ()
        {
            Type = type;
        }

        protected override bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId == Id && message.MessageType == MessageType.Event && (message.Key == "change" || message.Key == "input")) {
                // Don't need to notify here because the base implementation will fire the event
                if (Type == InputType.Checkbox) {
                    UpdateBooleanAttributeProperty ("checked", message.Value != null ? Convert.ToBoolean (message.Value) : false, "IsChecked");
                }
                else {
                    UpdateAttributeProperty ("value", message.Value != null ? Convert.ToString (message.Value) : "", "Value");
                }
            }
            return base.TriggerEventFromMessage (message);
        }
    }

    [JsonConverter (typeof (StringEnumConverter))]
    public enum InputType
    {
        [EnumMember (Value = "text")]
        Text,
        [EnumMember (Value = "date")]
        Date,
        [EnumMember (Value = "week")]
        Week,
        [EnumMember (Value = "datetime")]
        Datetime,
        [EnumMember (Value = "datetimelocal")]
        DatetimeLocal,
        [EnumMember (Value = "time")]
        Time,
        [EnumMember (Value = "month")]
        Month,
        [EnumMember (Value = "range")]
        Range,
        [EnumMember (Value = "number")]
        Number,
        [EnumMember (Value = "hidden")]
        Hidden,
        [EnumMember (Value = "search")]
        Search,
        [EnumMember (Value = "email")]
        Email,
        [EnumMember (Value = "tel")]
        Tel,
        [EnumMember (Value = "url")]
        Url,
        [EnumMember (Value = "password")]
        Password,
        [EnumMember (Value = "color")]
        Color,
        [EnumMember (Value = "checkbox")]
        Checkbox,
        [EnumMember (Value = "radio")]
        Radio,
        [EnumMember (Value = "file")]
        File,
        [EnumMember (Value = "submit")]
        Submit,
        [EnumMember (Value = "reset")]
        Reset,
        [EnumMember (Value = "image")]
        Image,
        [EnumMember (Value = "button")]
        Button,        
    }
}
