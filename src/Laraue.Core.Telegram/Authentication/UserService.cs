using Laraue.Core.Exceptions.Web;
using Laraue.Core.Telegram.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Laraue.Core.Telegram.Authentication;

public class UserService : IUserService
{
    private readonly UserManager<TelegramIdentityUser> _userManager;
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly IdentityOptions _identityOptions;

    public UserService(
        UserManager<TelegramIdentityUser> userManager,
        IOptions<IdentityOptions> identityOptions,
        IOptions<AuthenticationOptions> authenticationOptions)
    {
        _userManager = userManager;
        _authenticationOptions = authenticationOptions.Value;
        _identityOptions = identityOptions.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginData loginData)
    {
        var(login, password) = loginData;
        var user = await _userManager.FindByNameAsync(login);
        
        if (user is null)
        {
            throw new UnauthorizedRequestException(new Dictionary<string, string[]>
            {
                [nameof(loginData.Username)] = new []{"User is missing"},
            });
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedRequestException(new Dictionary<string, string[]>
            {
                [nameof(loginData.Password)] = new []{"The password is not correct"},
            });
        }

        var token = BearerHelper.GenerateAccessToken(user.Id, login, _authenticationOptions);
        return new LoginResponse(token, user.Id);
    }

    public async Task<LoginResponse> RegisterAsync(LoginData loginData)
    {
        var (login, password) = loginData;
        var result = await _userManager.CreateAsync(new TelegramIdentityUser { UserName = login }, password);
        if (result.Succeeded)
        {
            return await LoginAsync(loginData);
        }
        
        var errorStringsMap = result.Errors
            .Select(x => new
            {
                x.Description,
                Field = MappingIdentityErrorDescriber[x.Code]
            })
            .GroupBy(arg => arg.Field, arg => arg.Description)
            .ToDictionary(x => x.Key, x => x.ToArray());
            
        throw new BadRequestException(errorStringsMap);

    }

    public async Task<LoginResponse> LoginOrRegisterAsync(TelegramData telegramData)
    {
        var userName = $"tg_{telegramData.Username}";
        var user = await _userManager.FindByNameAsync(userName);

        if (user is not null)
        {
            return new LoginResponse(
                BearerHelper.GenerateAccessToken(user.Id, userName, _authenticationOptions),
                user.Id);
        }

        var password = GenerateRandomPassword(_identityOptions.Password);
        var result = await _userManager.CreateAsync(
            new TelegramIdentityUser()
            {
                UserName = userName,
                TelegramId = telegramData.Id,
            }, password);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.Errors.ToString());
        }
        
        user = await _userManager.FindByNameAsync(userName);
        
        return new LoginResponse(
            BearerHelper.GenerateAccessToken(user.Id, userName, _authenticationOptions),
            user.Id);
    }
    
    private static string GenerateRandomPassword(PasswordOptions opts)
    {
        var randomChars = new[] 
        {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",
            "abcdefghijkmnopqrstuvwxyz",
            "0123456789",
            "!@$?_-"
        };

        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();

        if (opts.RequireUppercase)
            chars.Insert(rand.Next(0, chars.Count), 
                randomChars[0][rand.Next(0, randomChars[0].Length)]);

        if (opts.RequireLowercase)
            chars.Insert(rand.Next(0, chars.Count), 
                randomChars[1][rand.Next(0, randomChars[1].Length)]);

        if (opts.RequireDigit)
            chars.Insert(rand.Next(0, chars.Count), 
                randomChars[2][rand.Next(0, randomChars[2].Length)]);

        if (opts.RequireNonAlphanumeric)
            chars.Insert(rand.Next(0, chars.Count), 
                randomChars[3][rand.Next(0, randomChars[3].Length)]);

        for (var i = chars.Count; i < opts.RequiredLength
                                  || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
        {
            var rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count), 
                rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }
    
    private static Dictionary<string, string> MappingIdentityErrorDescriber = new()
    {
        [nameof(IdentityErrorDescriber.PasswordTooShort)] = nameof(LoginData.Password),
        [nameof(IdentityErrorDescriber.PasswordMismatch)] = nameof(LoginData.Password),
        [nameof(IdentityErrorDescriber.PasswordRequiresDigit)] = nameof(LoginData.Password),
        [nameof(IdentityErrorDescriber.PasswordRequiresLower)] = nameof(LoginData.Password),
        [nameof(IdentityErrorDescriber.PasswordRequiresUpper)] = nameof(LoginData.Password),
        [nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)] = nameof(LoginData.Password),
        [nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars)] = nameof(LoginData.Password),
        [nameof(IdentityErrorDescriber.DuplicateUserName)] = nameof(LoginData.Username),
        [nameof(IdentityErrorDescriber.InvalidUserName)] = nameof(LoginData.Username),
    };
}