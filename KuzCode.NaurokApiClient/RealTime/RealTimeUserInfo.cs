using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient.RealTime
{
    /// <summary>
    /// Информация об пользователе, который использует клиента режиме реального времени
    /// </summary>
    public record RealTimeUserInfo
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        [JsonProperty("client_id")]
        public string? ClientId { get; init; }

        /// <summary>
        /// Идентификатор домашнего задания
        /// </summary>
        [JsonProperty("room")]
        public long HomeworkId { get; init; }

        /// <summary>
        /// Идентификатор сессии (<see langword="null"/> - участник является администратором)
        /// </summary>
        [JsonProperty("session")]
        [JsonConverter(typeof(ValueOrDefaultJsonConverter<long?>), "admin")]
        public long? SessionId { get; init; }

        /// <summary>
        /// Имя участника (<see langword="null"/> - участник является администратором)
        /// </summary>
        [JsonProperty("name")]
        [JsonConverter(typeof(FalseToDefaultJsonConverter<string>))]
        public string? Name { get; init; }

        /// <summary>
        /// Является ли администратором
        /// </summary>
        public bool IsAdmin => SessionId is null && Name is null;
    }
}
