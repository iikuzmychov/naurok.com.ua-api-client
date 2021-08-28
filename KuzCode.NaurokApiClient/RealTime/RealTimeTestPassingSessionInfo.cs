using Newtonsoft.Json;
using System;

namespace KuzCode.NaurokApiClient.RealTime
{
    /// <summary>
    /// Информациия о сессии выполнения теста в режиме реального времени
    /// </summary>
    public record RealTimeTestPassingSessionInfo
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; init; }

        /// <summary>
        /// Универсальный уникальный идентификатор
        /// </summary>
        [JsonProperty("uuid")]
        public string Uuid { get; init; } = "";

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [JsonProperty("name")]
        public string UserName { get; init; } = "";

        /// <summary>
        /// Дата и время окончания выполнения
        /// </summary>
        [JsonProperty("finished_at")]
        public DateTime? FinishedDateTime { get; init; }

        /// <summary>
        /// Количество правильных ответов
        /// </summary>
        [JsonProperty("correctCount")]
        public int CorrectAnswersCount { get; init; }

        /// <summary>
        /// Процент правильных ответов
        /// </summary>
        [JsonProperty("correctPercentage")]
        public int CorrectAnswersPercentage { get; init; }

        /// <summary>
        /// Количество частично правильных ответов
        /// </summary>
        [JsonProperty("partialCount")]
        public int PartialCorrectAnswersCount { get; init; }

        /// <summary>
        /// Процент частично правильных ответов
        /// </summary>
        [JsonProperty("partialPercentage")]
        public int PartialCorrectAnswersPercentage { get; init; }

        /// <summary>
        /// Количество НЕправильных ответов
        /// </summary>
        [JsonProperty("incorrectCount")]
        public int IncorrectAnswersCount { get; init; }

        /// <summary>
        /// Процент НЕправильных ответов
        /// </summary>
        [JsonProperty("incorrectPercentage")]
        public int IncorrectAnswersPercentage { get; init; }

        /// <summary>
        /// Количество пропущенных вопросов
        /// </summary>
        [JsonProperty("skipCount")]
        public int SkippedQuestionsCount { get; init; }

        /// <summary>
        /// Процент пропущенных вопросов
        /// </summary>
        [JsonProperty("skipPercentage")]
        public int SkippedQuestionsPercentage { get; init; }

        /// <summary>
        /// Оценка по 12-ти бальной системе
        /// </summary>
        [JsonProperty("mark")]
        public int Mark { get; init; }

        /// <summary>
        /// Количество набранных баллов
        /// </summary>
        [JsonProperty("points")]
        public int Points { get; init; }

        /// <summary>
        /// Процент точности<para/>
        /// Равняется процентному соотношению <see cref="Mark"/> к 12-ти
        /// </summary>
        [JsonProperty("result")]
        public double AccuracyPercentage { get; init; }
    }
}
