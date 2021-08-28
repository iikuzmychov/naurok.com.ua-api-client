using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class EnumValueOrDefaultFromStringJsonConverter<T> : JsonConverter<T>
        where T : struct, Enum
    {
        public T DefaultValue { get; }
        public override bool CanWrite => false;

        public EnumValueOrDefaultFromStringJsonConverter(T defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value!;

            if (Enum.TryParse(value, true, out T result))
                return result;
            else
                return DefaultValue;
        }
    }
}
