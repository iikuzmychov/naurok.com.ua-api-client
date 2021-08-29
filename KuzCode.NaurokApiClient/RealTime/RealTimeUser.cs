using H.Socket.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KuzCode.NaurokApiClient.RealTime
{
    /// <summary>
    /// Класс для взаимодействия с API веб-сокет сервера "На Урок", доступным любому пользователю, включая неавторизованных
    /// </summary>
    public class RealTimeUser : IDisposable, IAsyncDisposable
    {
        private SocketIoClient _socketIoClient = new SocketIoClient();
        private RealTimeTestPassingSessionInfo[]? _lastSessionsInfo;

        /// <summary>
        /// Подключён ли к веб-сокет серверу
        /// </summary>
        public bool IsConnected { get; private set; } = false;

        /// <summary>
        /// Информация (<see langword="null"/> - пользователь не подключён к веб-сокет серверу)
        /// </summary>
        public RealTimeUserInfo? Info { get; private set; }

        /// <summary>
        /// Событие, которое возникает при отключении пользователя от сервера
        /// </summary>
        public EventHandler<string>? OnDisconnected;

        /// <summary>
        /// Событие, которое возникает при ошибке сервера или ошибке обработки ответа
        /// </summary>
        public EventHandler<string>? OnError;

        /// <summary>
        /// Событие, которое возникает, когда список подключённых пользователей изменился
        /// </summary>
        public EventHandler<RealTimeUserInfo[]>? OnUsersListChanged;

        /// <summary>
        /// Событие, которое возникает при начале тестирования
        /// </summary>
        public EventHandler? OnTestingStarted;

        /// <summary>
        /// Событие, которое возникает при прекращении тестирования
        /// </summary>
        public EventHandler? OnTestingStoped;

        /// <summary>
        /// Событие, которое возникает, когда информация о сессиях изменилась
        /// </summary>
        public EventHandler<RealTimeTestPassingSessionInfo[]>? OnSessionsInfoChanged;

        public RealTimeUser()
        {
            SetUpSocketIoClient();
        }

        /// <summary>
        /// Конструктор с прокси
        /// </summary>
        /// <param name="proxy">Прокси</param>
        public RealTimeUser(IWebProxy proxy) : this()
        {
            _socketIoClient.Proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
        }

        private void SetUpSocketIoClient()
        {
            _socketIoClient.Disconnected += (sender, args) =>
            {
                IsConnected = false;

                Info          = null;
                _lastSessionsInfo = null;

                OnDisconnected?.Invoke(this, args.Reason);
            };
            _socketIoClient.ErrorReceived += (sender, args) => OnError?.Invoke(this, args.Value);
            _socketIoClient.ExceptionOccurred += (sender, args) => OnError?.Invoke(this, args.Value.Message);

            _socketIoClient.On("participants", async response =>
            {
                var jsonResponse = JToken.Parse(response.ToString());
                var usersInfo    = jsonResponse["userList"]!.ToObject<RealTimeUserInfo[]>()!;

                if (usersInfo.SingleOrDefault(userInfo => userInfo.ClientId == _socketIoClient.Id) is null)
                {
                    await _socketIoClient.DisconnectAsync();
                    return;
                }

                OnUsersListChanged?.Invoke(this, usersInfo);
            });
            _socketIoClient.On("removed", async response =>
            {
                var jsonResponse = JToken.Parse(response.ToString());

                if (jsonResponse["userList"] is null)
                {
                    await _socketIoClient.DisconnectAsync();
                    return;
                }

                var usersInfo = jsonResponse["userList"]!.ToObject<RealTimeUserInfo[]>()!;

                OnUsersListChanged?.Invoke(this, usersInfo);
            });
            _socketIoClient.On("startChallenge", response => OnTestingStarted?.Invoke(this, new EventArgs()));
            _socketIoClient.On("stopChallenge", response => OnTestingStoped?.Invoke(this, new EventArgs()));
            _socketIoClient.On("scoreboard", response =>
            {
                var jsonResponse = JToken.Parse(response.ToString());
                var sessionsInfo = jsonResponse["scoreboard"]!.ToObject<RealTimeTestPassingSessionInfo[]>()!;

                if (_lastSessionsInfo is null || Enumerable.SequenceEqual(_lastSessionsInfo, sessionsInfo) == false)
                {
                    _lastSessionsInfo = sessionsInfo;
                    OnSessionsInfoChanged?.Invoke(this, sessionsInfo);
                }
            });
        }

        private async Task ConnectAsync(long homeworkId, object sessionId, object name)
        {
            await _socketIoClient.ConnectAsync(new Uri(Naurok.BaseWebsocketUrlAddress));

            var data = new
            {
                room    = homeworkId,
                session = sessionId,
                name    = name
            };

            await _socketIoClient.Emit("join", data);
            IsConnected = true;
        }

        /// <summary>
        /// Подключиться к веб-сокет серверу как ученик
        /// </summary>
        /// <param name="homeworkId">Идентификатор домашнего задания</param>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <param name="name">Имя</param>
        /// <exception cref="AggregateException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public async Task ConnectAsStudentAsync(long homeworkId, long sessionId, string name)
        {
            if (homeworkId < 0)
                throw new ArgumentOutOfRangeException(nameof(homeworkId), "The identifier must be greater than 0");

            if (sessionId < 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId), "The identifier must be greater than 0");

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (IsConnected)
                throw new AggregateException("User already connected");

            await ConnectAsync(homeworkId, sessionId, name);

            Info = new RealTimeUserInfo()
            {
                ClientId   = _socketIoClient.Id,
                HomeworkId = homeworkId,
                SessionId  = sessionId,
                Name       = name,
            };
        }

        /// <summary>
        /// Подключиться к веб-сокет серверу как учитель
        /// </summary>
        /// <param name="homeworkId">Идентификатор домашнего задания</param>
        /// <exception cref="AggregateException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public async Task ConnectAsTeacherAsync(int homeworkId)
        {
            if (homeworkId < 0)
                throw new ArgumentOutOfRangeException(nameof(homeworkId), "The identifier must be greater than 0");

            if (IsConnected)
                throw new AggregateException("User already connected");

            await ConnectAsync(homeworkId, "admin", false);

            Info = new RealTimeUserInfo()
            {
                ClientId   = _socketIoClient.Id,
                HomeworkId = homeworkId,
            };
        }

        /// <summary>
        /// Исключить пользователя
        /// </summary>
        /// <param name="userClientId">Идентификатор клиента пользователя, которого необходимо удалить</param>
        /// <exception cref="AggregateException"/>
        /// <exception cref="ArgumentNullException"/>
        public async Task RemoveUserAsync(string userClientId)
        {
            if (userClientId is null)
                throw new ArgumentNullException(nameof(userClientId));

            if (!IsConnected)
                throw new AggregateException("User not connected");
            
            var data = new
            {
                room      = Info!.HomeworkId,
                client_id = userClientId
            };

            await _socketIoClient.Emit("remove", data);
        }

        /// <summary>
        /// Начать тестирование
        /// </summary>
        /// <exception cref="AggregateException"/>
        public async Task StartTestingAsync()
        {
            if (!IsConnected)
                throw new AggregateException("User not connected");

            var data = new { room = Info!.HomeworkId };
            await _socketIoClient.Emit("start", data);
        }

        /// <summary>
        /// Прекратить тестирование
        /// </summary>
        /// <remarks>Отличие от метода <see cref="CancelTestingAsync"/> в том, что он отменяет уже начатое тестирование, а данный метод прекращает не начатое</remarks>
        /// <exception cref="AggregateException"/>
        public async Task StopTestingAsync()
        {
            if (!IsConnected)
                throw new AggregateException("User not connected");

            var data = new { room = Info!.HomeworkId };
            await _socketIoClient.Emit("stop", data);
        }

        /// <summary>
        /// Отменить тестирование
        /// </summary>
        /// <remarks>Отличие от метода <see cref="StopTestingAsync"/> в том, что он прекращает уже начатое тестирование, а данный метод отменяет не начатое</remarks>
        /// <exception cref="AggregateException"/>
        public async Task CancelTestingAsync()
        {
            if (!IsConnected)
                throw new AggregateException("User not connected");

            var data = new { room = Info!.HomeworkId };
            await _socketIoClient.Emit("cancel", data);
        }

        /// <summary>
        /// Запросить информацию о сессиях
        /// </summary>
        /// <exception cref="AggregateException"/>
        public async Task RequestSessionsInfoAsync()
        {
            if (!IsConnected)
                throw new AggregateException("User not connected");

            var data = new { room = Info!.HomeworkId };
            await _socketIoClient.Emit("scoreboard", data);
        }

        /// <summary>
        /// Отключиться от веб-сокет сервера
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (IsConnected)
                return;
            
            await _socketIoClient.DisconnectAsync();
            IsConnected = false;
        }

        public void Dispose()
        {
            _socketIoClient.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync() => await _socketIoClient.DisposeAsync();
    }
}
