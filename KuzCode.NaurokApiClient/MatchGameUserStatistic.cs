using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Статистика пользователя в игре на сопоставление вопросов с ответами
    /// </summary>
    public record MatchGameUserStatistic
    {
        /// <summary>
        /// Неизвестно что идентифицирующий идентификатор пользователя
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; init; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; init; }

        /// <summary>
        /// Длительнось игры. Чем длительность меньше - тем быстрее игрок справился
        /// </summary>
        [JsonProperty("score")]
        [JsonConverter(typeof(MillisecondsToTimeSpanJsonConverter))]
        public TimeSpan GameDuration { get; init; }

        /// <summary>
        /// Количество попыток прохождения игры
        /// </summary>
        [JsonProperty("plays")]
        public int AttemptsCount { get; init; }
    }
}
