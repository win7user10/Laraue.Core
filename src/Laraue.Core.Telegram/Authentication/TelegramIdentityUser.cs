using Microsoft.AspNetCore.Identity;

namespace Laraue.Core.Telegram.Authentication;

public class TelegramIdentityUser : IdentityUser
{
    public long? TelegramId { get; init; }
}