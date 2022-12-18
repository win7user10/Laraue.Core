using MediatR;

namespace Laraue.Core.Telegram.Models;

/// <summary>
/// This class should be inherited by any telegram request handler.
/// </summary>
/// <typeparam name="TData"></typeparam>
public record BaseCommand<TData> : IRequest
{
    public string UserId { get; init; }

    public TData Data { get; init; }
}