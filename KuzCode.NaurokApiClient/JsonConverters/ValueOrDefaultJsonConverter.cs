using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class ValueOrDefaultJsonConverter<T> : JsonConverter<T>
    {
        public object DefaultValue { get; }
        public override bool CanWrite => false;

        public ValueOrDefaultJsonConverter(object defaultValue)
        {
            DefaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
        }

        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value?.Equals(DefaultValue) ?? false)
                return default;
            else
                return (T?)reader.Value;
        }
    }
}
