using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ooui
{
    public class Message
    {
        [JsonProperty("mid")]
        public long Id = GenerateId ();

        [JsonProperty("m")]        
        public MessageType MessageType = MessageType.Nop;

        [JsonProperty("id")]
        public string TargetId = "";

        [JsonProperty("k")]
        public string Key = "";

        object v = null;
        [JsonProperty("v")]
        public object Value {
            get => v;
            set => v = FixupValue (value);
        }

        public static Message Event (string targetId, string eventType) => new Message {
            MessageType = MessageType.Event,
            TargetId = targetId,
            Key = eventType,
        };

        static object FixupValue (object v)
        {
            if (v is Array a) {
                var na = new object[a.Length];
                for (var i = 0; i < a.Length; i++) {
                    na[i] = FixupValue (a.GetValue (i));
                }
                return na;
            }
            else if (v is EventTarget t) {
                return "\u2999" + t.Id;
            }
            return v;
        }

        static long idCounter = 0;
        static long GenerateId ()
        {
            return System.Threading.Interlocked.Increment (ref idCounter);
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
        [EnumMember(Value = "call")]
        Call,
        [EnumMember(Value = "listen")]
        Listen,
        [EnumMember(Value = "event")]
        Event,
    }
}
