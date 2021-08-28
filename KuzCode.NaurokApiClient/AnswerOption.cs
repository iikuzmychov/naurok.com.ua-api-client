using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Вариант ответа (на вопрос)
    /// </summary>
    public record AnswerOption
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; init; }

        /// <summary>
        /// HTML-текст
        /// </summary>
        [JsonProperty("value")]
        public string? HtmlText { get; init; }

        /// <summary>
        /// Является ли вариант ответа правильным ответом (<see langword="null"/> - неизвестно)
        /// </summary>
        [JsonProperty("correct")]
        [JsonConverter(typeof(NullableBooleanJsonConverter))]
        public bool? IsCorrectAnswer { get; init; }

        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        [JsonProperty("image")]
        public string? ImageUrl { get; init; }

        public AnswerOption() { }

        public AnswerOption(string? htmlText)
        {
            HtmlText = htmlText;
        }
    }
}
