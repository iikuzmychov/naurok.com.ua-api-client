namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Сессия выполнения теста
    /// </summary>
    public record TestPassingSession
    {
        /// <summary>
        /// Информация
        /// </summary>
        public TestPassingSessionInfo? Info { get; init; }

        /// <summary>
        /// Настройки
        /// </summary>
        public TestPasingSessionSettings? Settings { get; init; }

        /// <summary>
        /// Документ теста
        /// </summary>
        public TestDocument? TestDocument { get; init; }

        /// <summary>
        /// Информация о домашнем задании
        /// </summary>
        public HomeworkInfo? HomeworkInfo { get; init; }

        public TestPassingSession() { }

        public TestPassingSession(TestPassingSessionInfo? info, TestPasingSessionSettings? settings,
            TestDocument? testDocument, HomeworkInfo? homeworkInfo)
        {
            Info         = info;
            Settings     = settings;
            TestDocument = testDocument;
            HomeworkInfo = homeworkInfo;
        }
    }
}
