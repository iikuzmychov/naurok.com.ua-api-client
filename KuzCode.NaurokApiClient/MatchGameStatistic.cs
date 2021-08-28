using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Статистика игры на сопоставление вопросов с ответами
    /// </summary>
    public record MatchGameStatistic
    {
        /// <summary>
        /// Лучшая статистика пользователя среди всех попыток прохождения игры
        /// </summary>
        [JsonProperty("personal")]
        public MatchGameUserStatistic? BestUserStatistic { get; init; }

        /// <summary>
        /// Позиция пользователя в рейтинге. Начинается с еденицы
        /// </summary>
        [JsonProperty("position")]        
        public int UserRatingPosition { get; init; }

        /// <summary>
        /// Рейтинг пользователей по возрастанию длительности прохождения
        /// </summary>
        [JsonProperty("rating")]
        public MatchGameUserStatistic[]? UsersRating { get; init; }
    }
}
