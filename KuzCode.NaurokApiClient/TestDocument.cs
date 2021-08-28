namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Документ теста
    /// </summary>
    public record TestDocument
    {
        /// <summary>
        /// Информация
        /// </summary>
        public TestDocumentInfo? Info { get; init; }

        /// <summary>
        /// Список вопросов
        /// </summary>
        public Question[]? Questions { get; init; }

        public TestDocument() { }

        public TestDocument(TestDocumentInfo? info, Question[]? questions)
        {
            Info      = info;
            Questions = questions;
        }
    }
}
