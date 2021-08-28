using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class NullableBooleanJsonConverter : JsonConverter<bool?>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, bool? value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override bool? ReadJson(JsonReader reader, Type objectType, bool? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;

            if (value is null)
                return null;

            return value switch
            {
                "0" => false,
                "1" => true,
                _ => throw new ArgumentException()
            };
        }
    }
}
