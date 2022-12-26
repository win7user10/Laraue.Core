using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Controllers;
using Laraue.Core.Telegram.Extensions;
using Laraue.Core.Telegram.Router;
using Laraue.Core.Telegram.Router.Middleware;
using Laraue.Core.Telegram.Router.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace Laraue.Core.Tests.Telegram.Controllers;

public class ControllerTests
{
    private readonly TestServer _testServer;
    
    public ControllerTests()
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices((c, s) =>
            {
                s.AddTelegramCore(new TelegramBotClientOptions("a"))
                    .AddRouteResponseFunctionality<InMemoryResponseAwaiterStorage>(ServiceLifetime.Singleton)
                    .AddTelegramMiddleware<AuthTelegramMiddleware>()
                    .AddScoped<IUserService, MockedUserService>();
            })
            .Configure(a => a.MapTelegramRequests("/test"));
        
        _testServer = new TestServer(hostBuilder);
    }

    private async Task<string> SendRequestAsync(Update update)
    {
        var resp = await _testServer.CreateClient().PostAsync(
            "test",
            new StringContent(JsonConvert.SerializeObject(update),
                Encoding.UTF8,
                "text/json"));

        resp.EnsureSuccessStatusCode();

        return await resp.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task MessageRoute_ShouldResponseCorrectlyAsync()
    {
        var result = await SendRequestAsync(new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = "user",
                    Id = 123,
                    IsBot = false,
                },
                Text = "message",
                Date = DateTime.UtcNow,
                Chat = new Chat
                {
                    Id = 1,
                }
            },
        });
        
        Assert.Equal("message", result);
    }
    
    [Fact]
    public async Task CallbackRoute_ShouldResponseCorrectlyAsync()
    {
        var result = await SendRequestAsync(new Update
        {
            CallbackQuery = new CallbackQuery
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = "user",
                    Id = 123,
                    IsBot = false,
                },
                Data = "callback",
                Id = "123",
                ChatInstance = "123",
            },
        });
        
        Assert.Equal("callback", result);
    }
    
    [Fact]
    public async Task MessageRoute_ShouldAwaitResponseOnMessageAsync()
    {
        var sendMessage = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = "user",
                    Id = 123,
                    IsBot = false,
                },
                Text = "awaitResponse",
                Date = DateTime.UtcNow,
                Chat = new Chat
                {
                    Id = 1,
                }
            },
        };
        
        var result = await SendRequestAsync(sendMessage);
        Assert.Equal("awaitResponse", result);
        
        result = await SendRequestAsync(sendMessage);
        Assert.Equal("awaited", result);
        
        result = await SendRequestAsync(sendMessage);
        Assert.Equal("awaitResponse", result);
    }

    public class TestTelegramController : TelegramController
    {
        private readonly IResponseAwaiterStorage _responseAwaiterStorage;

        public TestTelegramController(IResponseAwaiterStorage responseAwaiterStorage)
        {
            _responseAwaiterStorage = responseAwaiterStorage;
        }

        [TelegramMessageRoute("message")]
        public Task<string?> ExecuteMessageAsync(TelegramRequestContext requestContext)
        {
            return Task.FromResult(requestContext.Update.Message!.Text);
        }
        
        [TelegramCallbackRoute("callback")]
        public Task<string?> ExecuteCallbackAsync(TelegramRequestContext requestContext)
        {
            return Task.FromResult(requestContext.Update.CallbackQuery!.Data);
        }
        
        [TelegramMessageRoute("awaitResponse")]
        public async Task<string?> ExecuteMessageWithResponseAwaiterAsync(TelegramRequestContext requestContext)
        {
            await _responseAwaiterStorage.SetAsync<MessageResponseAwaiter>(requestContext.UserId!);
            
            return requestContext.Update.Message!.Text;
        }
    }

    public sealed class MessageResponseAwaiter : BaseResponseAwaiter
    {
        protected override bool TryValidate(Update update, [NotNullWhen(false)] out string? errorMessage)
        {
            errorMessage = null;
            return true;
        }

        protected override Task<object?> ExecuteRouteAsync(TelegramRequestContext telegramRequestContext)
        {
            return Task.FromResult((object?) "awaited");
        }
    }

    public class InMemoryResponseAwaiterStorage : IResponseAwaiterStorage
    {
        private readonly Dictionary<string, Type> _awaitersMap = new ();

        public Task<Type?> TryGetAsync(string userId)
        {
            _awaitersMap.TryGetValue(userId, out var awaiterType);

            return Task.FromResult(awaiterType);
        }

        public Task SetAsync<TResponseAwaiter>(string userId)
            where TResponseAwaiter : IResponseAwaiter
        {
            _awaitersMap[userId] = typeof(TResponseAwaiter);
            
            return Task.CompletedTask;
        }

        public Task ResetAsync(string userId)
        {
            _awaitersMap.Remove(userId);
            
            return Task.CompletedTask;
        }
    }

    public sealed class MockedUserService : IUserService
    {
        public Task<LoginResponse> LoginAsync(LoginData loginData)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> RegisterAsync(LoginData loginData)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> LoginOrRegisterAsync(TelegramData loginData)
        {
            return Task.FromResult(new LoginResponse("abc", "123"));
        }
    }
}