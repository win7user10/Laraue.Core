using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Laraue.Core.Logging.Telegram
{
    public class TelegramLoggerProvider : ILoggerProvider
    {
        private readonly TelegramLoggerOptions _options;
        private readonly HttpClient _client;

        public TelegramLoggerProvider(TelegramLoggerOptions options, HttpClient client)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TelegramLogger(categoryName, _options, _client);
        }

        public void Dispose()
        {
        }
    }
}