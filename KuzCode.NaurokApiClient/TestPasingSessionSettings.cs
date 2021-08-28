using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Настройки сессии выполнения теста
    /// </summary>
    public record TestPasingSessionSettings
    {
        /// <summary>
        /// Показывать ли после выбора варианта ответа правильный
        /// </summary>
        [JsonProperty("show_answer")]
        public bool ShowRigthAnswers { get; init; }

        /// <summary>
        /// Показывать ли анализ после окончания тестирования. Анализ состоит из перечня, где указанны выбранные и правильные варианты ответов
        /// </summary>
        [JsonProperty("show_review")]
        public bool ShowReview { get; init; }

        /// <summary>
        /// Показывать ли таблицу лидеров после окончания тестирования
        /// </summary>
        [JsonProperty("show_leaderbord")]
        public bool ShowLeaderboard { get; init; }

        /// <summary>
        /// Показывать ли мемы перед началом выполнения
        /// </summary>
        [JsonProperty("show_memes")]
        public bool ShowMemes { get; init; }

        /// <summary>
        /// Перемешивать ли варианты ответов на вопросы
        /// </summary>
        [JsonProperty("shuffle_options")]
        public bool ShuffleAnswerOptions { get; init; }

        /// <summary>
        /// Перемешивать ли вопросы
        /// </summary>
        [JsonProperty("shuffle_question")]
        public bool ShuffleQuestions { get; init; }
    }
}
