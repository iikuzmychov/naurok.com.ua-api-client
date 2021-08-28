using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KuzCode.NaurokApiClient
{
    internal static class JTokenExtensions
    {
        public static T? ToObject<T>(this JToken token, JsonConverter[] converters)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));

            var serializerSettings = new JsonSerializerSettings();

            foreach (var converter in converters)
                serializerSettings.Converters.Add(converter);

            var serializer         = JsonSerializer.Create(serializerSettings);
            var deserializedObject = token.ToObject<T>(serializer);

            return deserializedObject;
        }
    }
}
