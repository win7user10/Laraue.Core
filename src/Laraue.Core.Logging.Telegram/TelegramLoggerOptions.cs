using Microsoft.Extensions.Logging;

namespace Laraue.Core.Logging.Telegram
{
    public class TelegramLoggerOptions
    {
        /// <summary>
        /// Telegram token for bot.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Chat in which will be sent all log messages.
        /// </summary>
        public string ChatId { get; set; }

        /// <summary>
        /// Minimal level for logging.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Is logging enabled.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}