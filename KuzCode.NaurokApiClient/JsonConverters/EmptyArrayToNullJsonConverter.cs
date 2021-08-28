using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class EmptyArrayToNullJsonConverter<T> : JsonConverter<T[]>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, T[]? value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override T[]? ReadJson(JsonReader reader, Type objectType, T[]? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            while (reader.TokenType != JsonToken.EndArray)
                reader.Read();

            return existingValue is null || existingValue.Length == 0 ? null : existingValue;
        }
    }
}
