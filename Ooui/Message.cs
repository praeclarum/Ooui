using System;

namespace Ooui
{
    public class Message
    {
        public DateTime CreatedTime = DateTime.UtcNow;
        public MessageType MessageType = MessageType.Nop;
        public string TargetId = "";
        public string Member = "";
        public string Value = "";

        public void SetValue (object value)
        {
            switch (value) {
                case null:
                    Value = "null";
                    break;
                case String s:
                    Value = EncodeString (s);
                    break;
                default:
                    Value = String.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", value);
                    break;
            }
        }

        public static string EncodeString (string s)
        {
            return s;
        }
        public static string DecodeString (string s)
        {
            return s;
        }
    }

    public enum MessageType
    {
        Nop,
        Create,
        SetProperty,
    }

    public enum ValueType
    {
        String
    }
}
