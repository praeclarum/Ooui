using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ooui
{
    public class Message
    {
        [JsonProperty("m")]        
        public MessageType MessageType = MessageType.Nop;

        [JsonProperty("id")]
        public string TargetId = "";

        [JsonProperty("k")]
        public string Key = "";

        [JsonProperty("v", NullValueHandling = NullValueHandling.Ignore)]
        public object Value = null;

        [JsonProperty("rid", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultId = null;

        public static Message Call (string targetId, string method, params object[] args) => new Message {
            MessageType = MessageType.Call,
            TargetId = targetId,
            Key = method,
            Value = args,
        };

        public static Message Event (string targetId, string eventType, object value = null) => new Message {
            MessageType = MessageType.Event,
            TargetId = targetId,
            Key = eventType,
            Value = value,
        };

        static void WriteJsonString (System.IO.TextWriter w, string s)
        {
            w.Write ('\"');
            for (var i = 0; i < s.Length; i++) {
                var c = s[0];
                if (c == '\r') {
                    w.Write ("\\r");
                }
                else if (c == '\n') {
                    w.Write ("\\n");
                }
                else if (c == '\t') {
                    w.Write ("\\t");
                }
                else if (c == '\b') {
                    w.Write ("\\b");
                }
                else if (c == '\\') {
                    w.Write ("\\");
                }
                else {
                    w.Write (c);
                }
            }
            w.Write ('\"');
        }

        static void WriteJsonValue (System.IO.TextWriter w, object value)
        {
            if (value == null) {
                w.Write ("null");
                return;
            }
            var s = value as string;
            if (s != null) {
                WriteJsonString (w, s);
                return;
            }

            var a = value as Array;
            if (a != null) {
                w.Write ('[');
                var head = "";
                foreach (var o in a) {
                    w.Write (head);
                    WriteJsonValue (w, o);
                    head = ",";
                }
                w.Write (']');
                return;
            }

            var e = value as EventTarget;
            if (e != null) {
                w.Write ('\"');
                w.Write (e.Id);
                w.Write ('\"');
                return;
            }

            if (value is Color) {
                WriteJsonString (w, ((Color)value).ToString ());
                return;
            }

            var icult = System.Globalization.CultureInfo.InvariantCulture;

            if (value is double) {
                w.Write (((double)value).ToString (icult));
            }

            if (value is int) {
                w.Write (((int)value).ToString (icult));
            }

            if (value is float) {
                w.Write (((float)value).ToString (icult));
            }

            WriteJsonString (w, Convert.ToString (value, icult));
        }

        public void WriteJson (System.IO.TextWriter w)
        {
            w.Write ('{');
            switch (MessageType) {
                case MessageType.Call: w.Write ("\"m\":\"call\",\"id\":\""); break;
                case MessageType.Create: w.Write ("\"m\":\"create\",\"id\":\""); break;
                case MessageType.Event: w.Write ("\"m\":\"event\",\"id\":\""); break;
                case MessageType.Listen: w.Write ("\"m\":\"listen\",\"id\":\""); break;
                case MessageType.Nop: w.Write ("\"m\":\"nop\",\"id\":\""); break;
                case MessageType.RemoveAttribute: w.Write ("\"m\":\"remAttr\",\"id\":\""); break;
                case MessageType.Set: w.Write ("\"m\":\"set\",\"id\":\""); break;
                case MessageType.SetAttribute: w.Write ("\"m\":\"setAttr\",\"id\":\""); break;
            }
            w.Write (TargetId);
            w.Write ("\",\"k\":\"");
            w.Write (Key);
            if (Value != null) {
                w.Write ("\",\"v\":");
                WriteJsonValue (w, Value);
                w.Write ('}');
            }
            else {
                w.Write ("\"}");
            }
        }

        public string ToJson ()
        {
            using (var sw = new System.IO.StringWriter ()) {
                WriteJson (sw);
                return sw.ToString ();
            }
        }
    }

    [JsonConverter (typeof (StringEnumConverter))]
    public enum MessageType
    {
        [EnumMember(Value = "nop")]
        Nop,
        [EnumMember(Value = "create")]
        Create,
        [EnumMember(Value = "set")]
        Set,
        [EnumMember (Value = "setAttr")]
        SetAttribute,
        [EnumMember(Value = "remAttr")]
        RemoveAttribute,
        [EnumMember(Value = "call")]
        Call,
        [EnumMember(Value = "listen")]
        Listen,
        [EnumMember(Value = "event")]
        Event,
    }
}
