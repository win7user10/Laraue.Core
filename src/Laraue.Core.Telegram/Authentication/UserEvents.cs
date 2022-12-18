namespace Laraue.Core.Telegram.Authentication;

public class UserEvents
{
    public event Action<RegisteredUserData>? OnUserRegistered;

    internal void FireOnUserRegistered(RegisteredUserData registeredUserData)
    {
        OnUserRegistered?.Invoke(registeredUserData);
    }
}

public record RegisteredUserData(long TelegramId, string Id);