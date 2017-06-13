using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ooui
{
    public class Message
    {
        [JsonProperty("t")]
        [JsonConverter (typeof (MillisecondEpochConverter))]
        public DateTime CreatedTime = DateTime.UtcNow;

        [JsonProperty("m")]
        [JsonConverter (typeof (StringEnumConverter))]
        public MessageType MessageType = MessageType.Nop;

        [JsonProperty("id")]
        public string TargetId = "";

        [JsonProperty("k")]
        public string Key = "";

        [JsonProperty("v")]
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
                    Value = JsonConvert.SerializeObject (value);
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
        Set,
        Call,
    }

    class MillisecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue (((DateTime)value - epoch).TotalMilliseconds.ToString (System.Globalization.CultureInfo.InvariantCulture));
        }

        public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            return epoch.AddMilliseconds ((double)reader.Value);
        }
    }
}
