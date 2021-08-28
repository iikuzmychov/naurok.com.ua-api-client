using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Карточка игры на сопоставление вопросов с ответами
    /// </summary>
    public record MatchGameCard
    {
        /// <summary>
        /// Идентификатор вопроса, с которым связанна карточка
        /// </summary>
        [JsonProperty("id")]
        public long QuestionId { get; init; }

        /// <summary>
        /// HTML-текст (является текстом вопроса или ответа)
        /// </summary>
        [JsonProperty("name")]
        public string? HtmlText { get; init; }

        /// <summary>
        /// Ссылка на изображение (является ссылкой на изображение вопроса или ответа)
        /// </summary>
        [JsonProperty("image")]
        [JsonConverter(typeof(FalseToDefaultJsonConverter<string>))]
        public string? ImageUrl { get; init; }
    }
}
