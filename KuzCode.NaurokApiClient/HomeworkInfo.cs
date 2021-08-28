using KuzCode.NaurokApiClient.JsonConverters;
using KuzCode.NaurokApiClient.RealTime;
using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Режим домашнего задания
    /// </summary>
    public enum HomeworkMode
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        Unknown,

        /// <summary>
        /// Индивидуальное задание
        /// </summary>
        Individual,
    }

    /// <summary>
    /// Информация о домашнем задании
    /// </summary>
    public record HomeworkInfo
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; init; }

        /// <summary>
        /// Игровой код, необходимый для начала выполнения
        /// </summary>
        [JsonProperty("gamecode")]
        public int Code { get; init; }

        /// <summary>
        /// Название
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; init; }

        /// <summary>
        /// Идентификатор автора
        /// </summary>
        [JsonProperty("account_id")]
        public long AuthorId { get; init; }

        /// <summary>
        /// Режим
        /// </summary>
        [JsonProperty("mode")]
        [JsonConverter(typeof(EnumValueOrDefaultFromStringJsonConverter<HomeworkMode>), HomeworkMode.Unknown)]
        public HomeworkMode Mode { get; init; }

        /// <summary>
        /// Стадия тестирования в режиме реального времени
        /// </summary>
        [JsonProperty("active")]
        [JsonConverter(typeof(EnumValueOrDefaultFromIntJsonConverter<RealTimeTestingStage>), RealTimeTestingStage.Unknown)]
        public RealTimeTestingStage RealTimeStage { get; init; }

        /// <summary>
        /// Неизвестно что значущий тип
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; init; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        [JsonProperty("created_at")]
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime CreatedDateTime { get; init; }

        /// <summary>
        /// Дата и время крайнего срока окончания выполнения
        /// </summary>
        [JsonProperty("deadline")]
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime DeadlineDateTime { get; init; }

        /// <summary>
        /// Дата и время последнего обновления настроек
        /// </summary>
        [JsonProperty("updated_at")]
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime LastUpdateDateTime { get; init; }

        /// <summary>
        /// Ограничение выполнения по продолжительности (<see cref="TimeSpan.Zero"/> - нет ограничения)
        /// </summary>
        [JsonProperty("duration")]
        [JsonConverter(typeof(SecondsToTimeSpanJsonConverter))]
        public TimeSpan DurationLimit { get; init; }

        /// <summary>
        /// Ограниченно ли количество выполнений одной попыткой
        /// </summary>
        [JsonProperty("available_attempts")]
        public bool OneAttemptsOnly { get; init; }

        /// <summary>
        /// Показывать ли после выбора варианта ответа правильный
        /// </summary>
        [JsonProperty("show_timer")]
        public bool IsDurationLimitActive { get; init; }

        /// <summary>
        /// Разрешить ли просмотр флеш-карт после оконочания тестирования
        /// </summary>
        [JsonProperty("show_flashcard")]
        public bool AllowViewingFlashCards { get; init; }

        /// <summary>
        /// Разрешить ли играть в игру на сопоставление вопросов с ответами после окончания тестирования
        /// </summary>
        [JsonProperty("show_match")]
        public bool AllowMatchGame { get; init; }
    }
}
