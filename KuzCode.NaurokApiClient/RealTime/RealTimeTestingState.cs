namespace KuzCode.NaurokApiClient.RealTime
{
    /// <summary>
    /// Стадия тестирования в режиме реального времени
    /// </summary>
    public enum RealTimeTestingStage
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        Unknown,

        /// <summary>
        /// Тестирование НЕ в режиме реального времени
        /// </summary>
        NotRealtime = 0,

        /// <summary>
        /// Ожидание участников
        /// </summary>
        ParticipantsExpectation = 1,

        /// <summary>
        /// Начато
        /// </summary>
        Started = 2,
    }
}
