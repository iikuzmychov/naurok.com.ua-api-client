using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class EnumValueOrDefaultFromIntJsonConverter<T> : JsonConverter<T>
        where T : struct, Enum
    {
        public T DefaultValue { get; }
        public override bool CanWrite => false;

        public EnumValueOrDefaultFromIntJsonConverter(T defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = (int)(long)reader.Value!;

            if (Enum.IsDefined(typeof(T), value))
                return (T)(object)value;
            else
                return DefaultValue;
        }
    }
}
