using KuzCode.NaurokApiClient.JsonConverters;
using Newtonsoft.Json;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Информация о документе теста
    /// </summary>
    public record TestDocumentInfo
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; init; }

        /// <summary>
        /// Название
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; init; }

        /// <summary>
        /// Описание
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; init; }

        /// <summary>
        /// Уникальная идентифицирующая часть веб-адреса
        /// </summary>
        [JsonProperty("slug")]
        public string? Slug { get; init; }

        /// <summary>
        /// Предмет
        /// </summary>
        [JsonProperty("subject_id")]
        [JsonConverter(typeof(EnumValueOrDefaultFromIntJsonConverter<Subject>), Subject.Unknown)]
        public Subject Subject { get; init; }

        /// <summary>
        /// Класс
        /// </summary>
        [JsonProperty("grade_id")]
        [JsonConverter(typeof(EnumValueOrDefaultFromIntJsonConverter<Grade>), Grade.Unknown)]
        public Grade Grade { get; init; }

        /// <summary>
        /// Является ли приватным (<see langword="null"/> - неизвестно)
        /// </summary>
        [JsonProperty("private")]
        [JsonConverter(typeof(NullableBooleanJsonConverter))]
        public bool? IsPrivate { get; init; }

        /// <summary>
        /// Разрешено ли клонирование (<see langword="null"/> - неизвестно)
        /// </summary>
        [JsonProperty("enable_cloning")]
        [JsonConverter(typeof(NullableBooleanJsonConverter))]
        public bool? IsCloningEnable { get; init; }

        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        [JsonProperty("image")]
        public string? ImageUrl { get; init; }
    }
}
