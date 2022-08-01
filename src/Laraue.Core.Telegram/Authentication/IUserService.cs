namespace Laraue.Core.Telegram.Authentication;

public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginData loginData);
    Task<LoginResponse> RegisterAsync(LoginData loginData);
    Task<LoginResponse> LoginOrRegisterAsync(TelegramData loginData);
}

public record LoginResponse(string Token, string UserId);
public record LoginData(string Username, string Password);
public record TelegramData(long Id, string Username);