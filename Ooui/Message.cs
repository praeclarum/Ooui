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
        public object Value = "";
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
