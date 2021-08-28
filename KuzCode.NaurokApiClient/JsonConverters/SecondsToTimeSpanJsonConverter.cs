using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class SecondsToTimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var milliseconds = (long)reader.Value!;
            var timeSpan     = TimeSpan.FromSeconds(milliseconds);

            return timeSpan;
        }
    }
}
