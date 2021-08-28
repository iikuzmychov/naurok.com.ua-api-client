namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Вопрос
    /// </summary>
    public record Question
    {
        /// <summary>
        /// Информация
        /// </summary>
        public QuestionInfo? Info { get; init; }

        /// <summary>
        /// Варианты ответов
        /// </summary>
        public AnswerOption[]? AnswerOptions { get; init; }

        public Question() { }

        public Question(QuestionInfo? info, AnswerOption[]? answerOptions)
        {
            Info          = info;
            AnswerOptions = answerOptions;
        }
    }
}
