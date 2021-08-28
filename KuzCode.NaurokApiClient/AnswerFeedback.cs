using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Обратный ответ на сохранение ответа на вопрос
    /// </summary>
    public record AnswerFeedback
    {
        /// <summary>
        /// На сколько процентов правильный ответ
        /// </summary>
        /// <remarks>В зависимости от <see cref="SceneName"/> и <see cref="Message"/>, 0 может также обозначать отсутсвие информации</remarks>
        [JsonProperty("correct")]
        public int CorrectnessPercentage { get; init; }

        /// <summary>
        /// Массив идентификаторов опций, которые являются правильными ответами (<see langword="null"/> - неизвестно)
        /// </summary>
        [JsonProperty("correct_options")]
        [JsonConverter(typeof(EmptyArrayToNullJsonConverter<long>))]
        public long[]? CorrectAnswerOptionsId { get; init; }

        /// <summary>
        /// Сообщение
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; init; } = "";

        /// <summary>
        /// Название сцены, которую необходимо показать
        /// </summary>
        /// <remarks>Возможные значения: <b>"failed"</b> - неправильный или невозможный ответ, <b>"success"</b> - правильный ответ, <b>"saved"</b> - ответ сохранён</remarks>
        [JsonProperty("message_scene")]
        public string SceneName { get; init; } = "";

        /// <summary>
        /// Идентификатор гифки, которую необходимо показать
        /// </summary>
        [JsonProperty("message_gif")]
        public int GifId { get; init; }
    }
}
