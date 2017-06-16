using System;

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
        }

        public Input (InputType type)
            : this ()
        {
            Type = type;
        }
    }

    public enum InputType
    {
        Text,
        Date,
        Week,
        Datetime,
        DatetimeLocal,
        Time,
        Month,
        Range,
        Number,
        Hidden,
        Search,
        Email,
        Tel,
        Url,
        Password,
        Color,
        Checkbox,
        Radio,
        File,
        Submit,
        Reset,
        Image,
        Button,        
    }
}
