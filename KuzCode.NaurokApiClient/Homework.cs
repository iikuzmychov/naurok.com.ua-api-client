namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Домашнее задание
    /// </summary>
    public record Homework
    {
        /// <summary>
        /// Информация
        /// </summary>
        public HomeworkInfo? Info { get; init; }

        /// <summary>
        /// Документ теста
        /// </summary>
        public TestDocument? TestDocument { get; init; }

        public Homework() { }

        public Homework(HomeworkInfo? info, TestDocument? testDocument)
        {
            Info         = info;
            TestDocument = testDocument;
        }
    }
}
