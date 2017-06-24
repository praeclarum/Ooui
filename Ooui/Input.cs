using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ooui
{
    public class Input : FormControl
    {
        InputType typ = InputType.Text;
        public InputType Type {
            get => typ;
            set => SetProperty (ref typ, value, "type");
        }

        string val = "";
        public string Value {
            get => val;
            set => SetProperty (ref val, value ?? "", "value");
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

        public event EventHandler Changed {
            add => AddEventListener ("change", value);
            remove => RemoveEventListener ("change", value);
        }

        public event EventHandler Inputted {
            add => AddEventListener ("input", value);
            remove => RemoveEventListener ("input", value);
        }

        bool isChecked = false;
        public bool IsChecked {
            get => isChecked;
            set => SetProperty (ref isChecked, value, "checked");
        }

        double minimum = 0;
        public double Minimum {
            get => minimum;
            set => SetProperty (ref minimum, value, "min");
        }

        double maximum = 100;
        public double Maximum {
            get => maximum;
            set => SetProperty (ref maximum, value, "max");
        }

        double step = 1;
        public double Step {
            get => step;
            set => SetProperty (ref step, value, "step");
        }

        public Input ()
            : base ("input")
        {
            // Subscribe to the change event so we always get up-to-date values
            Changed += (s, e) => {};
        }

        public Input (InputType type)
            : this ()
        {
            Type = type;
        }

        protected override bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId == Id && message.MessageType == MessageType.Event && message.Key == "change") {
                // Don't need to notify here because the base implementation will fire the event
                if (Type == InputType.Checkbox) {
                    isChecked = message.Value != null ? Convert.ToBoolean (message.Value) : false;
                }
                else {
                    val = message.Value != null ? Convert.ToString (message.Value) : "";
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
