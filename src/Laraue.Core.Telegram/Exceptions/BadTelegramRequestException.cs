namespace Laraue.Core.Telegram.Exceptions;

public class BadTelegramRequestException : Exception
{
    public BadTelegramRequestException(string message) : base(message)
    {}
}