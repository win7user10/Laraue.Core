using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Laraue.Core.Logging.Telegram
{
    public class TelegramLogger : ILogger
    {
        private readonly string _namespace;
        private readonly HttpClient _client;
        private readonly TelegramLoggerOptions _options;

        private const string Api = "https://api.telegram.org/bot{0}/sendMessage?text={1}&chat_id={2}&parse_mode=html";

        public TelegramLogger(string @namespace, TelegramLoggerOptions options, HttpClient client)
        {
            _namespace = @namespace;
            _options = options;
            _client = client;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _options.IsEnabled && logLevel >= _options.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var message = new StringBuilder();
                message.Append($"<b>{logLevel}: {_namespace}</b>\n");
                message.Append(formatter.Invoke(state, exception));
                message.Append(exception.StackTrace);
                Task.Run(() => _client.GetAsync(ApiUrl(message.ToString())));
            }
        }

        private string ApiUrl(string text) => string.Format(Api, _options.Token, text, _options.ChatId);
    }
}