using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Флеш-карта (карточка "вопрос-ответ")
    /// </summary>
    public record FlashCard
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; init; }

        /// <summary>
        /// HTML-текст вопроса
        /// </summary>
        [JsonProperty("question.text")]
        public string? QuestionHtmlText { get; init; }

        /// <summary>
        /// Ссылка на изображение вопроса
        /// </summary>
        [JsonProperty("question.image")]
        public string? QuestionImageUrl { get; init; }

        /// <summary>
        /// Тип вопроса
        /// </summary>
        [JsonProperty("question.type")]
        public QuestionType QuestionType { get; init; }

        /// <summary>
        /// HTML-текст правильного варианта ответа
        /// </summary>
        [JsonProperty("answer.text")]
        public string? AnswerHtmlText { get; init; }

        /// <summary>
        /// Ссылки на изображения ответов
        /// </summary>
        [JsonProperty("answer.images")]
        public string[]? AnswerImageUrl { get; init; }

        public FlashCard() { }

        public FlashCard(string questionHtmlText, string answerHtmlText)
        {
            QuestionHtmlText = questionHtmlText;
            AnswerHtmlText   = answerHtmlText;
        }
    }
}
