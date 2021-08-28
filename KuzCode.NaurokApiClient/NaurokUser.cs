using KuzCode.NaurokApiClient.JsonConverters;
using MAD.JsonConverters.NestedJsonConverterNS;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Класс для взаимодействия с API сайта "На Урок", доступным любому пользователю, включая неавторизованных
    /// </summary>
    public class NaurokUser
    {
        protected HttpClient HttpClient { get; }

        /// <summary>
        /// Конструктор со специальным обработчиком
        /// </summary>
        /// <param name="handler">Специальный обработчик</param>
        /// <exception cref="ArgumentNullException"/>
        public NaurokUser(HttpClientHandler handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            HttpClient             = new HttpClient(handler);
            HttpClient.BaseAddress = new Uri(Naurok.BaseWebsiteUrlAddress);
        }

        public NaurokUser() : this(new()) { }

        protected static Question[] DeserializeQuestions(JArray questionsJson)
        {
            return questionsJson
                .Select(questionJson =>
                {
                    var options      = questionJson["options"]!.ToObject<AnswerOption[]>();
                    var questionInfo = questionJson.ToObject<QuestionInfo>();
                    var question     = new Question(questionInfo, options);

                    return question;
                })
                .ToArray();
        }

        /// <summary>
        /// Получить сессию выполнения теста
        /// </summary>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <returns>Сессия выполнения теста или <see langword="null"/>, если сессии не существует</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<TestPassingSession?> GetSessionAsync(long sessionId)
        {
            if (sessionId < 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId), "The identifier must be 0 or greater than 0");

            var response     = (await HttpClient.GetAsync($@"api2/test/sessions/{sessionId}")).EnsureSuccessStatusCode();
            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());

            if (responseJson.Type == JTokenType.Boolean)
            {
                if ((bool)responseJson == false)
                    return null;
                else
                    throw new NotImplementedException();
            }

            var questions    = DeserializeQuestions((responseJson["questions"]! as JArray)!);
            var testDocument = new TestDocument(null, questions);
            var settings     = responseJson["settings"]!.ToObject<TestPasingSessionSettings>()!;
            var homeworkInfo = responseJson["settings"]!["id"] is null ? null : responseJson["settings"]!.ToObject<HomeworkInfo>()!;
            var sessionInfo  = responseJson["session"]!.ToObject<TestPassingSessionInfo>()!;
            var session      = new TestPassingSession(sessionInfo, settings, testDocument, homeworkInfo);

            return session;
        }

        /// <summary>
        /// Завершить сессию выполнения теста
        /// </summary>
        /// <remarks>Возможной причиной возникновения исключения <see cref="HttpRequestException"/> может быть неверно указанный идентификатор сессии выполнения теста</remarks>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <returns>Информация о сессии выполнения теста</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<TestPassingSessionInfo> EndSessionAsync(long sessionId)
        {
            if (sessionId < 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId), "The identifier must be 0 or greater than 0");

            var response     = (await HttpClient.PutAsync($@"api2/test/sessions/end/{sessionId}", null)).EnsureSuccessStatusCode();
            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());

            if (responseJson.Type == JTokenType.Null)
                throw new ArgumentException("Session is not exists");

            var session = responseJson["session"]!.ToObject<TestPassingSessionInfo>()!;

            return session;
        }

        /// <summary>
        /// Сохранить ответ на вопрос
        /// </summary>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <param name="questionId">Идентификатор вопроса</param>
        /// <param name="questionType">Тип вопроса</param>
        /// <param name="questionPoint">Количество баллов за выбор полностью правильного ответа на вопрос</param>
        /// <param name="answersId">Массив идентификаторов опций, которые являются выбранными ответами</param>
        /// <param name="isHomework">Привязанна ли сессия к домашнему заданию</param>
        /// <param name="customUserAgent">
        /// Пользовательское значение для заголовка "User-Agent"<para/>
        /// Если текущее значение заголовка не совпадает с переданным при подключении к прохождению теста, то любой ответ засчитается как неправильный
        /// </param>
        /// <returns>Обратный ответ на сохранение ответа на вопрос</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<AnswerFeedback> SaveAnswerAsync(long sessionId, long questionId, QuestionType questionType,
            int questionPoint, long[] answersId, bool isHomework, string? customUserAgent = null)
        {
            if (sessionId < 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId), "The identifier must be 0 or greater than 0");

            if (questionId < 0)
                throw new ArgumentOutOfRangeException(nameof(questionId), "The identifier must be 0 or greater than 0");

            if (questionType == QuestionType.Unknown)
                throw new ArgumentException("Question type can`t equals " + nameof(QuestionType.Unknown), nameof(questionType));

            if (questionPoint <= 0)
                throw new ArgumentOutOfRangeException(nameof(questionPoint), "The question point must be greater than 0");

            if (answersId is null)
                throw new ArgumentNullException(nameof(answersId));

            if (answersId.Any(id => id < 0))
                throw new ArgumentException("Each answer identifier must be 0 or greater than 0", nameof(answersId));

            var requestJson = JToken.FromObject(new
            {
                session_id  = sessionId,
                question_id = questionId,
                type        = (int)questionType,
                point       = questionPoint,
                answer      = answersId,
                homework    = isHomework,
                show_answer = 1,
            });

            var request     = new HttpRequestMessage(HttpMethod.Put, $@"api2/test/responses/answer");
            request.Content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");
            
            if (customUserAgent is not null)
                request.Headers.Add("User-Agent", customUserAgent);

            var response     = (await HttpClient.SendAsync(request)).EnsureSuccessStatusCode();
            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());
            var feedback     = responseJson.ToObject<AnswerFeedback>()!;

            return feedback;
        }

        /// <summary>
        /// Сохранить ответ на вопрос
        /// </summary>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <param name="questionInfo">Информация о вопросе</param>
        /// <param name="answersId">Массив идентификаторов опций, которые являются выбранными ответами</param>
        /// <param name="isHomework">Привязанна ли сессия к домашнему заданию</param>
        /// <param name="customUserAgent">
        /// Пользовательское значение для заголовка "User-Agent"<para/>
        /// Если текущее значение заголовка не совпадает с переданным при подключении к прохождению теста, то любой ответ засчитается как неправильный
        /// </param>
        /// <returns>Обратный ответ на сохранение ответа на вопрос</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<AnswerFeedback> SaveAnswerAsync(long sessionId, QuestionInfo questionInfo, long[] answersId,
            bool isHomework, string? customUserAgent = null)
        {
            if (sessionId < 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId), "The identifier must be 0 or greater than 0");

            if (questionInfo is null)
                throw new ArgumentNullException(nameof(questionInfo));

            if (questionInfo.Id < 0)
                throw new ArgumentException("The question identifier must be 0 or greater than 0", nameof(questionInfo));

            if (questionInfo.Type == QuestionType.Unknown)
                throw new ArgumentException("Question type can`t equals " + nameof(QuestionType.Unknown), nameof(questionInfo));

            if (questionInfo.Point <= 0)
                throw new ArgumentException("The question point must be greater than 0", nameof(questionInfo));

            if (answersId is null)
                throw new ArgumentNullException(nameof(answersId));

            if (answersId.Any(id => id < 0))
                throw new ArgumentException("Each answer identifier must be 0 or greater than 0", nameof(answersId));

            return await SaveAnswerAsync(sessionId, questionInfo.Id, questionInfo.Type, questionInfo.Point, answersId, isHomework, customUserAgent);
        }

        /// <summary>
        /// Сохранить ответ на вопрос
        /// </summary>
        /// <param name="session">Сессия</param>
        /// <param name="questionId">Идентификатор вопроса</param>
        /// <param name="answersId">Массив идентификаторов опций, которые являются выбранными ответами</param>
        /// <param name="customUserAgent">
        /// Пользовательское значение для заголовка "User-Agent"<para/>
        /// Если текущее значение заголовка не совпадает с переданным при подключении к прохождению теста, то любой ответ засчитается как неправильный
        /// </param>
        /// <returns>Обратный ответ на сохранение ответа на вопрос</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<AnswerFeedback> SaveAnswerAsync(TestPassingSession session, long questionId,
            long[] answersId, string? customUserAgent = null)
        {
            if (session is null)
                throw new ArgumentNullException(nameof(session));

            if (session.Info is null)
                throw new ArgumentException("The session info is null", nameof(session));

            if (session.Info.Id < 0)
                throw new ArgumentException("The session identifier must be 0 or greater than 0", nameof(session));

            if (session.TestDocument?.Questions is null)
                throw new ArgumentException("The session test document or questions is null", nameof(session));

            var question = session.TestDocument.Questions.SingleOrDefault(question => question.Info?.Id == questionId);

            if (question is null)
                throw new ArgumentException("The session does not contains single question with id " + questionId, nameof(session));

            if (question.Info is null)
                throw new ArgumentNullException(nameof(question.Info));

            if (question.Info.Id < 0)
                throw new ArgumentException("The question identifier must be 0 or greater than 0", nameof(session));

            if (question.Info.Type == QuestionType.Unknown)
                throw new ArgumentException("Question type can`t equals " + nameof(QuestionType.Unknown), nameof(session));

            if (question.Info.Point <= 0)
                throw new ArgumentException("The question point must be greater than 0", nameof(session));

            if (answersId is null)
                throw new ArgumentNullException(nameof(answersId));

            if (answersId.Any(id => id < 0))
                throw new ArgumentException("Each answer identifier must be 0 or greater than 0", nameof(answersId));

            
            return await SaveAnswerAsync(session.Info!.Id, question.Info, answersId, session.HomeworkInfo is not null, customUserAgent);
        }

        /// <summary>
        /// Получить флеш-карты
        /// </summary>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <param name="sessionUuid">Универсальный уникальный Идентификатор сессии</param>
        /// <returns>Флеш-карты или <see langword="null"/>, если документ теста не существует либо в нём нет вопросов, либо просмотр флеш-карт отключён, либо сесиия выполнения теста не завершена</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<FlashCard[]?> GetFlashCardsAsync(long testDocumentId, string sessionUuid)
        {
            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            if (sessionUuid is null)
                throw new ArgumentNullException(nameof(sessionUuid));

            var request = new HttpRequestMessage(HttpMethod.Post, $@"api/test/documents/{testDocumentId}/flashcard");
            request.Headers.Add("Referer", @"https://naurok.com.ua/test/security_hole/flashcard");

            var requestJson = JToken.FromObject(new { uuid = sessionUuid });
            request.Content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");

            var response = (await HttpClient.SendAsync(request)).EnsureSuccessStatusCode();
            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());

            if ((bool)responseJson["result"]! == false)
                return null;

            // JToken.ToObject<FlashCard[]> не работает с NestedJsonConverter
            var flashCards = responseJson["cards"]!
                .Select(flashCardsJson =>
                {
                    var answerJson = (flashCardsJson["answer"] as JObject)!;

                    // если у ответа есть картинка и она одна меняем её на массив картинок
                    if (answerJson["image"] is not null)
                    {
                        answerJson.Add("images", new JArray(answerJson["image"]!));
                        answerJson.Remove("image");
                    }

                    var flashCard = flashCardsJson.ToObject<FlashCard>(new[] { new NestedJsonConverter() })!;

                    return flashCard;
                })
                .ToArray();

            return flashCards;
        }

        /// <summary>
        /// Получить карточки игры на сопостовление вопросов с ответами и лучшую (наименьшую) длительность игры
        /// </summary>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <param name="sessionUuid">Универсальный уникальный Идентификатор сессии</param>
        /// <returns>Карточки игры на сопостовление вопросов с ответами и лучшую (наименьшую) длительность игры или <see langword="null"/>, если документ теста не существует либо в нём менее двух вопросов, либо игра на сопоставление вопросов с ответами отключена, либо сесиия выполнения теста не завершена</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<(MatchGameCard[] Cards, TimeSpan? BestGameDuration)?> GetMatchGameCardsAndBestAsync(long testDocumentId, string sessionUuid)
        {
            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            if (sessionUuid is null)
                throw new ArgumentNullException(nameof(sessionUuid));

            var request = new HttpRequestMessage(HttpMethod.Post, $@"api/test/documents/{testDocumentId}/match");
            request.Headers.Add("Referer", @"https://naurok.com.ua/test/security_hole/match");

            var requestJson = JToken.FromObject(new { uuid = sessionUuid });
            request.Content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");

            var response        = (await HttpClient.SendAsync(request)).EnsureSuccessStatusCode();
            var responseJson    = JToken.Parse(await response.Content.ReadAsStringAsync());

            if (responseJson.Type == JTokenType.Null)
                return null;

            var bestResultExists = responseJson["score"]!.Type == JTokenType.Boolean && (bool)responseJson["score"]! == true;

            // Умножаем значение "score" на 10, чтобы они стало количеством миллисекунд из непонятно чего
            if (bestResultExists)
                responseJson["score"]!["score"] = responseJson["score"]!["score"]!.Value<long>() * 10;

            var matchGameCards   = responseJson["items"]!.ToObject<MatchGameCard[]>()!;
            var bestGameDuration = !bestResultExists ? null : responseJson["score"]?["score"]!.ToObject<TimeSpan>(new[] { new MillisecondsToTimeSpanJsonConverter() });

            return (matchGameCards, bestGameDuration);
        }

        /// <summary>
        /// Получить карточки игры на сопостовление вопросов с ответами
        /// </summary>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <param name="sessionUuid">Универсальный уникальный Идентификатор сессии</param>
        /// <returns>Карточки игры на сопостовление вопросов с ответами или <see langword="null"/>, если документ теста не существует либо в нём менее двух вопросов, либо игра на сопоставление вопросов с ответами отключена, либо сесиия выполнения теста не завершена</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<MatchGameCard[]?> GetMatchGameCardsAsync(long testDocumentId, string sessionUuid)
        {
            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            if (sessionUuid is null)
                throw new ArgumentNullException(nameof(sessionUuid));

            return (await GetMatchGameCardsAndBestAsync(testDocumentId, sessionUuid))?.Cards;
        }

        /// <summary>
        /// Сохранить результат прохождения игры на сопоставления вопросов с ответами
        /// </summary>
        /// <remarks>Возможной причиной возникновения исключения <see cref="HttpRequestException"/> 
        /// может быть отключение домашнего задания, несуществование объектов с указанными идентификаторами или несовпадение указанного идентификатора
        /// документа теста с идентификатором документа теста, связанным с указаннной сессией выполнения теста</remarks>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <param name="gameDuration">Продолжительность игры</param>
        /// <param name="sessionUuid">Универсальный уникальный Идентификатор сессии</param>
        /// <returns>Статистика игры на сопоставления вопросов с ответами</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<MatchGameStatistic> SaveMatchGameResultAsync(long testDocumentId, TimeSpan gameDuration, string sessionUuid)
        {
            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            if (sessionUuid is null)
                throw new ArgumentNullException(nameof(sessionUuid));

            var requestJson = JToken.FromObject(new
            {
                document_id = testDocumentId,
                score       = (long)(gameDuration.TotalMilliseconds / 10),
                uuid        = sessionUuid
            });

            var request     = new HttpRequestMessage(HttpMethod.Post, $@"api/test/documents/{testDocumentId}/match-update");
            request.Content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");

            var response     = (await HttpClient.SendAsync(request));//.EnsureSuccessStatusCode();
            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());

            // Умножаем все значения "score" на 10, чтобы они стали количеством миллисекунд из непонятно чего
            (responseJson["personal"]!["score"] as JValue)!.Value = responseJson["personal"]!["score"]!.Value<long>() * 10;

            foreach (var userStatisticJson in responseJson["rating"]!)
                (userStatisticJson["score"] as JValue)!.Value = userStatisticJson["score"]!.Value<long>() * 10;

            var matchGameStatistic = responseJson!.ToObject<MatchGameStatistic>()!;

            return matchGameStatistic;
        }
    }
}
