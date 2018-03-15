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
                JsonConvert.WriteJsonValue (w, Value);
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

        public static Message FromJson (string json)
        {
            var m = new Message ();

            var i = 0;
            var e = json.Length;

            while (i < e) {
                while (i < e && (json[i]==',' || json[i]=='{' || char.IsWhiteSpace (json[i])))
                    i++;
                if (i >= e)
                    throw new Exception ("JSON Unexpected end");
                var n = e - i;
                if (json[i] == '}')
                    break;
                if (n > 4 && json[i] == '\"' && json[i+2] == '\"' && json[i+3] == ':') {
                    switch (json[i + 1]) {
                        case 'm':
                            if (json[i + 4] == '\"' && json[i + 5] == 'e') {
                                m.MessageType = MessageType.Event;
                            }
                            i += 5;
                            while (i < e && json[i] != '\"') i++;
                            i++;
                            break;
                        case 'k': {
                                i += 5;
                                var se = i;
                                while (se < e && json[se] != '\"')
                                    se++;
                                m.Key = json.Substring (i, se - i);
                                i = se + 1;
                            }
                            break;
                        case 'v':
                            m.Value = JsonConvert.ReadJsonValue (json, i + 4);
                            break;
                    }
                }
                else if (n > 5 && json[i] == '\"' && json[i + 3] == '\"' && json[i + 4] == ':' && json[i+5] == '\"') {
                    switch (json[i + 1]) {
                        case 'i': {
                                i += 6;
                                var se = i;
                                while (se < e && json[se] != '\"')
                                    se++;
                                m.TargetId = json.Substring (i, se - i);
                                i = se + 1;
                            }
                            break;
                    }
                }
                else {
                    throw new Exception ("JSON Expected property");
                }
            }

            return m;
        }

        public override string ToString ()
        {
            return ToJson ();
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
