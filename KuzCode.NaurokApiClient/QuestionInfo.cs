using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Тип вопроса
    /// </summary>
    public enum QuestionType
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        Unknown,

        /// <summary>
        /// Викторина (только один правильный вариант ответа)
        /// </summary>
        Quiz,

        /// <summary>
        /// Мультивикторина (несколько правильных ответов)
        /// </summary>
        Multiquiz
    }

    /// <summary>
    /// Информация о вопросе
    /// </summary>
    public record QuestionInfo
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; init; }

        /// <summary>
        /// Идентификатор оригинального вопроса (<see langword="null"/> - вопрос является оригиналом)
        /// </summary>
        [JsonProperty("clone_id")]
        public long? SourceQuestionId { get; init; }

        /// <summary>
        /// HTML-текст
        /// </summary>
        [JsonProperty("content")]
        public string? HtmlText { get; init; }
        
        /// <summary>
        /// Тип
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(QuestionTypeJsonConverter))]
        public QuestionType Type { get; init; }

        /// <summary>
        /// Количество баллов за выбор полностью правильного ответа
        /// </summary>
        [JsonProperty("point")]
        public int Point { get; init; }

        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        [JsonProperty("image")]
        public string? ImageUrl { get; init; }
    }
}
