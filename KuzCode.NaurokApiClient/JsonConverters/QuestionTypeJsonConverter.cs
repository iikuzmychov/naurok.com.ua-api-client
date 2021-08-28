using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class QuestionTypeJsonConverter : JsonConverter<QuestionType>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, QuestionType value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override QuestionType ReadJson(JsonReader reader, Type objectType, QuestionType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value!;

            if (Enum.TryParse(value, true, out QuestionType result))
                return result;
            else
                return QuestionType.Unknown;
        }
    }
}
