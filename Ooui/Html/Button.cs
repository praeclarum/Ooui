using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ooui.Html {
    public class Button : FormControl
    {
        public ButtonType Type {
            get => GetAttribute ("type", ButtonType.Submit);
            set => SetAttributeProperty ("type", value);
        }

        public Button ()
            : base ("button")
        {
        }

        public Button (string text)
            : this ()
        {
            Text = text;
        }
    }

    [JsonConverter (typeof (StringEnumConverter))]
    public enum ButtonType
    {
        [EnumMember (Value = "submit")]
        Submit,
        [EnumMember (Value = "reset")]
        Reset,
        [EnumMember (Value = "button")]
        Button,
    }
}
