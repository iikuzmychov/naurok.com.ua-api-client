using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Класс для взаимодействия с API сайта "На Урок", доступным пользователям с правами учителя
    /// </summary>
    public class NaurokTeacher : NaurokUser
    {
        /// <summary>
        /// Имеется ли базовая сертефикация
        /// </summary>
        public bool HasBasicCertefication { get; } = true;

        /// <summary>
        /// Конструктор авторизации со специальным обработчиком
        /// </summary>
        /// <param name="phpSessionId">PHP-сессия авторизованного учителя (PHPSESSID)</param>
        /// <param name="handler">Специальный обработчик</param>
        /// <exception cref="ArgumentNullException"/>
        public NaurokTeacher(string phpSessionId, HttpClientHandler handler)
            : base(InitializeHttpClientHandler(handler, phpSessionId))
        {
            if (phpSessionId is null)
                throw new ArgumentNullException(nameof(phpSessionId));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Конструктор авторизации
        /// </summary>
        /// <param name="phpSessionId">PHP-сессия авторизованного учителя (PHPSESSID)</param>
        /// <exception cref="ArgumentNullException"/>
        public NaurokTeacher(string phpSessionId) : this(phpSessionId, new HttpClientHandler()) { }

        private static HttpClientHandler InitializeHttpClientHandler(HttpClientHandler handler, string phpSessionId)
        {
            if (handler.CookieContainer is null)
                handler.CookieContainer = new CookieContainer();

            handler.CookieContainer.Add(new Uri(Naurok.BaseWebsiteUrlAddress), new Cookie("PHPSESSID", phpSessionId));

            return handler;
        }

        /// <summary>
        /// Получить информацию о вопросе
        /// </summary>
        /// <param name="questionId">Идентификатор документа</param>
        /// <returns>Информация о вопросе или <see langword="null"/>, если вопроса не существует</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<QuestionInfo?> GetQuestionInfoAsync(long questionId)
        {
            if (questionId < 0)
                throw new ArgumentOutOfRangeException(nameof(questionId), "The identifier must be 0 or greater than 0");

            var response = (await HttpClient.GetAsync($@"api/test/questions/{questionId}"));

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());
            var questionInfo = responseJson.ToObject<QuestionInfo>()!;

            return questionInfo;
        }

        /// <summary>
        /// Получить флеш-карты
        /// </summary>
        /// <remarks>Требуется наличие базовой сертификации</remarks>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <returns>Флеш-карты или <see langword="null"/>, если документ теста не существует либо в нём нет вопросов</returns>
        /// <exception cref="NoBasicCertificationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<FlashCard[]?> GetFlashCardsAsync(long testDocumentId)
        {
            if (!HasBasicCertefication)
                throw new NoBasicCertificationException();

            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            return await GetFlashCardsAsync(testDocumentId, "");
        }

        /// <summary>
        /// Получить карточки игры на сопостовление вопросов с ответами и лучшую (наименьшую) длительность игры
        /// </summary>
        /// <remarks>Требуется наличие базовой сертификации</remarks>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <returns>Карточки игры на сопостовление вопросов с ответами и лучшую (наименьшую) длительность игры или <see langword="null"/>, если документ теста не существует либо в нём менее двух вопросов, либо игра на сопоставление вопросов с ответами отключена, либо сесиия выполнения теста не завершена</returns>
        /// <exception cref="NoBasicCertificationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<(MatchGameCard[] Cards, TimeSpan? BestGameDuration)?> GetMatchGameCardsAndBestAsync(long testDocumentId)
        {
            if (!HasBasicCertefication)
                throw new NoBasicCertificationException();

            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            return await GetMatchGameCardsAndBestAsync(testDocumentId, "");
        }

        /// <summary>
        /// Получить карточки игры на сопостовление вопросов с ответами
        /// </summary>
        /// <remarks>Требуется наличие базовой сертификации</remarks>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <returns>Карточки игры на сопостовление вопросов с ответами или <see langword="null"/>, если документ теста не существует либо в нём менее двух вопросов, либо игра на сопоставление вопросов с ответами отключена, либо сесиия выполнения теста не завершена</returns>
        /// <exception cref="NoBasicCertificationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<MatchGameCard[]?> GetMatchGameCardsAsync(long testDocumentId)
        {
            if (!HasBasicCertefication)
                throw new NoBasicCertificationException();

            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            return (await GetMatchGameCardsAndBestAsync(testDocumentId))?.Cards;
        }

        /// <summary>
        /// Сохранить результат прохождения игры на сопоставления вопросов с ответами
        /// </summary>
        /// <remarks>Требуется наличие базовой сертификации<para/>
        /// Возможной причиной возникновения исключения <see cref="HttpRequestException"/> 
        /// может быть отключение домашнего задания, неверно указанные идентификаторы или несовпадение указанного идентификатора
        /// документа теста с идентификатором документа теста, связанным с указаннной сессией выполнения теста</remarks>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <param name="gameDuration">Продолжительность игры</param>
        /// <returns>Статистика игры на сопоставления вопросов с ответами</returns>
        /// <exception cref="NoBasicCertificationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<MatchGameStatistic> SaveMatchGameResultAsync(long testDocumentId, TimeSpan gameDuration)
        {
            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            return await SaveMatchGameResultAsync(testDocumentId, gameDuration, "");
        }

        /// <summary>
        /// Получить документ теста
        /// </summary>
        /// <param name="testDocumentId">Идентификатор документа</param>
        /// <returns>Документ теста или <see langword="null"/>, если теста не существует либо вы не его автор</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<TestDocument?> GetTestDocumentAsync(long testDocumentId)
        {
            if (testDocumentId < 0)
                throw new ArgumentOutOfRangeException(nameof(testDocumentId), "The identifier must be 0 or greater than 0");

            var response     = (await HttpClient.GetAsync($@"api/test/documents/{testDocumentId}")).EnsureSuccessStatusCode();
            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());

            if (responseJson.Type == JTokenType.Boolean)
            {
                if (responseJson.Value<bool>() == false)
                    return null;
                else
                    throw new NotImplementedException();
            }

            var questions        = NaurokUser.DeserializeQuestions((responseJson["questions"]! as JArray)!);
            var testDocumentInfo = responseJson["document"]!.ToObject<TestDocumentInfo>();
            var testDocument     = new TestDocument(testDocumentInfo, questions);

            return testDocument;
        }

        /// <summary>
        /// Найти документы с похожими вопросами
        /// </summary>
        /// <param name="questionText">Текст вопроса</param>
        /// <param name="findMyTestDocumentsOnly">Следует ли искать только среди своих документов</param>
        /// <returns>Документы тестов</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<TestDocument[]> FindTestDocumentsByQuestionAsync(string questionText, bool findMyTestDocumentsOnly = false)
        {
            if (questionText is null)
                throw new ArgumentNullException(nameof(questionText));

            var response     = (await HttpClient.GetAsync($@"api/test/questions/search?q={questionText}&only_my={(findMyTestDocumentsOnly ? 1 : 0)}")).EnsureSuccessStatusCode();
            var responseJson = JToken.Parse(await response.Content.ReadAsStringAsync());

            var testsDocuments = responseJson
                .Select(testDocumentJson =>
                {
                    // удаляются "ненужные" токены, так как в контексте этого запроса они замененны своим строковы представлением (вместо id - украинское название)
                    foreach (var extraTokenName in new[] { "author", "grade", "subject" })
                        testDocumentJson[extraTokenName]!.Parent!.Remove();

                    var questions        = NaurokUser.DeserializeQuestions((testDocumentJson["questions"]! as JArray)!);
                    var testDocumentInfo = testDocumentJson!.ToObject<TestDocumentInfo>()!;
                    var testDocument     = new TestDocument(testDocumentInfo, questions);

                    return testDocument;
                })
                .ToArray();

            return testsDocuments!;
        }

        /// <summary>
        /// Получить общий отчёт о выполнении домашнего задания
        /// </summary>
        /// <param name="homeworkId">Идентификатор домашнего задания</param>
        /// <returns>Файл <b>.xls</b> в виде массива байт</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<byte[]> GetHomeworkReportAsync(long homeworkId)
        {
            if (homeworkId < 0)
                throw new ArgumentOutOfRangeException(nameof(homeworkId), "The identifier must be 0 or greater than 0");

            var response = (await HttpClient.GetAsync($@"test/homework/{homeworkId}/export/score")).EnsureSuccessStatusCode();
            var fileData = await response.Content.ReadAsByteArrayAsync();

            return fileData;
        }

        /// <summary>
        /// Получить подробный отчёт о выполнении домашнего задания.<br/>
        /// Включает в себя информацию о прохождении теста каждым учеником
        /// </summary>
        /// <param name="homeworkId">Идентификатор домашнего задания</param>
        /// <returns>Файл <b>.xls</b> в виде массива байт</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="HttpRequestException"/>
        public async Task<byte[]> GetHomeworkDetailedReportAsync(long homeworkId)
        {
            if (homeworkId < 0)
                throw new ArgumentOutOfRangeException(nameof(homeworkId), "The identifier must be 0 or greater than 0");

            var response = (await HttpClient.GetAsync($@"test/homework/{homeworkId}/detailed-export/score")).EnsureSuccessStatusCode();
            var fileData = await response.Content.ReadAsByteArrayAsync();

            return fileData;
        }
    }
}
