using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var timestamp = (long)reader.Value!;

            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(timestamp)
                .ToLocalTime();

            return date;
        }
    }
}
