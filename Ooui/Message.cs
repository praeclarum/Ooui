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
        [EnumMember(Value = "call")]
        Call,
        [EnumMember(Value = "listen")]
        Listen,
        [EnumMember(Value = "event")]
        Event,
    }
}
