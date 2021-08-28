using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Информация о сессии выполнения теста
    /// </summary>
    public record TestPassingSessionInfo
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; init; }

        /// <summary>
        /// Идентификатор пользователя (<see langword="null"/> - пользователь не авторизован)
        /// </summary>
        [JsonProperty("account_id")]
        public int? UserId { get; init; }

        /// <summary>
        /// Универсальный уникальный идентификатор
        /// </summary>
        [JsonProperty("uuid")]
        public string Uuid { get; init; } = "";

        /// <summary>
        /// Дата и время начала выполнения
        /// </summary>
        [JsonProperty("start_at")]
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime StartDateTime { get; init; }

        /// <summary>
        /// Количество данных ответов
        /// </summary>
        [JsonProperty("answers")]
        public int GivenAnswersCount { get; init; }

        /// <summary>
        /// Идентификатор последнего вопроса, на который был дан ответ
        /// </summary>
        [JsonProperty("latest_question")]
        public int? LatestAnsweredQuestionId { get; init; }
    }
}
