using KuzCode.NaurokApiClient;
using KuzCode.NaurokApiClient.RealTime;
using Microsoft.Extensions.Configuration;
using System;

namespace ConsoleTests
{
    class Program
    {
        private static IConfigurationRoot Config;

        private static void TestUserMethods()
        {
            var user = new NaurokUser();

            var session     = user.GetSessionAsync(Config.GetValue<long>("sessionId")).Result;
            var sessionUuid = session.Info.Uuid;
            var homeworkId  = session.HomeworkInfo?.Id;
            //var sessionInfo = user.EndSession(Config.GetValue<long>("sessionId")).Result;

            var flashCards = user.GetFlashCardsAsync(Config.GetValue<long>("documentId"), sessionUuid).Result;

            var matchGameCardsAndBest = user.GetMatchGameCardsAndBestAsync(Config.GetValue<long>("documentId"), sessionUuid).Result;
            //var matchGameStatistics   = user.SaveMatchGameResult(Config.GetValue<long>("documentId"), TimeSpan.FromSeconds(4), sessionUuid).Result;

            //var answerFeedback = user.SaveAnswer(session, Config.GetValue<long>("sessionId"), new[] { 40473815 }, UserAgent).Result;
        }

        private static void TestTeacherMethods()
        {
            var teacher = new NaurokTeacher(Config["teacherPhpSessionId"]);

            var session     = teacher.GetSessionAsync(Config.GetValue<long>("sessionId")).Result;
            var sessionUuid = session.Info.Uuid;
            var homeworkId  = session.HomeworkInfo?.Id;
            //var sessionInfo = teacher.EndSession(Config.GetValue<long>("sessionId")).Result;

            var flashCards              = teacher.GetFlashCardsAsync(Config.GetValue<long>("documentId")).Result;
            var flashCardsBySessionUuid = teacher.GetFlashCardsAsync(Config.GetValue<long>("documentId"), sessionUuid).Result;

            var matchGameCardsAndBest              = teacher.GetMatchGameCardsAndBestAsync(Config.GetValue<long>("documentId")).Result;
            var matchGameCardsAndBestBySessionUuid = teacher.GetMatchGameCardsAndBestAsync(Config.GetValue<long>("documentId"), sessionUuid).Result;

            //var matchGameStatistics              = teacher.SaveMatchGameResult(DocumentId, TimeSpan.FromSeconds(4)).Result;
            //var matchGameStatisticsBySessionUuid = teacher.SaveMatchGameResult(DocumentId, TimeSpan.FromSeconds(4), sessionUuid).Result;

            //var answerFeedback = teacher.SaveAnswer(session, Config.GetValue<long>("sessionId"), new[] { 40473815 }, UserAgent).Result;

            var questionInfo = teacher.GetQuestionInfoAsync(5174281).Result;
            var testDocument = teacher.GetTestDocumentAsync(Config.GetValue<long>("documentId")).Result;

            //var testDocumentsFindedByQuestion = teacher.FindTestDocumentsByQuestion(testDocument.Questions[0].Info.HtmlText).Result;

            //var homeworkReportData = teacher.GetHomeworkReport(homeworkId).Result;
            //File.WriteAllBytes(@"C:\Users\User\Desktop\HomeworkReport.xls", homeworkReportData);

            //var homeworkDetailedReportData = teacher.GetHomeworkDetailedReport(homeworkId).Result;
            //File.WriteAllBytes(@"C:\Users\User\Desktop\HomeworkDetailedReport.xls", homeworkDetailedReportData);
        }

        private static void TestRealTimeClientMethods()
        {
            Func<RealTimeUserInfo, string> userInfoToString = (userInfo) => $"{{id: {userInfo.ClientId}, room: {userInfo.HomeworkId}, session: {userInfo.SessionId?.ToString() ?? "null"}, name: {userInfo.Name ?? "null"}}}";

            var user = new RealTimeUser();
            user.OnDisconnected += (sender, reason) =>
            {
                lock (Console.Out)
                    Console.WriteLine($"Отключенно");
            };
            user.OnError += (sender, errorText) =>
            {
                lock (Console.Out)
                    Console.WriteLine($"Ошибка: {errorText}");
            };
            user.OnTestingStarted += (sender, args) =>
            {
                lock (Console.Out)
                    Console.WriteLine($"Тестирование начато");
            };
            user.OnTestingStoped += (sender, args) =>
            {
                lock (Console.Out)
                    Console.WriteLine($"Тестирование остановленно");
            };
            user.OnUsersListChanged += (sender, usersInfo) =>
            {
                Console.WriteLine("Список пользователей:");

                foreach (var userInfo in usersInfo)
                    Console.WriteLine(userInfoToString(userInfo));
            };
            user.OnSessionsInfoChanged += (sender, sessionsInfo) =>
            {
                lock(Console.Out)
                    Console.WriteLine("sessionsInfoUpdated");
            };

            var homeworkId = new NaurokUser().GetSessionAsync(Config.GetValue<long>("sessionId")).Result.HomeworkInfo.Id;
            user.ConnectAsStudentAsync(6594811, Config.GetValue<long>("sessionId"), $"ученик {DateTime.Now.Ticks}").Wait();
            Console.WriteLine($"Подключенно. Я: {userInfoToString(user.Info)}");
            //user.RemoveUser("HIWyDNuw7ecMwNdgACWx").Wait();
            //user.StartChallengeAsync().Wait();
            //user.StopChallengeAsync().Wait();
            //user.CancelChallengeAsync().Wait();
            user.RequestSessionsInfoAsync().Wait();

            Console.ReadKey();

            if (user.IsConnected)
                user.DisconnectAsync().Wait();

            user.DisconnectAsync().Wait();
        }

        static void Main(string[] args)
        {
            Config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            TestUserMethods();
            TestTeacherMethods();
            TestRealTimeClientMethods();
        }
    }
}
