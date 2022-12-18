using Laraue.Core.Telegram.Utils;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Views;

/// <summary>
/// The command to update previously sent message.
/// </summary>
/// <param name="Data"></param>
/// <param name="ChatId"></param>
/// <param name="MessageId"></param>
/// <param name="CallbackQueryId"></param>
/// <typeparam name="TData"></typeparam>
public record UpdateMessageCommand<TData>(TData Data, long ChatId, int MessageId, string CallbackQueryId)
    : IRequest;

/// <summary>
/// Base message handler implementation.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TData"></typeparam>
public abstract class UpdateMessageCommandHandler<TCommand, TData> : IRequestHandler<TCommand>
    where TCommand : UpdateMessageCommand<TData>
{
    private readonly ITelegramBotClient _client;

    protected UpdateMessageCommandHandler(ITelegramBotClient client)
    {
        _client = client;
    }

    public async Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var messageBuilder = new TelegramMessageBuilder();
        
        HandleInternal(request, messageBuilder);
        
        await _client.EditMessageTextAsync(
            request.ChatId,
            request.MessageId,
            messageBuilder.Text,
            ParseMode.Html,
            replyMarkup: messageBuilder.InlineKeyboard,
            cancellationToken: cancellationToken);

        await _client.AnswerCallbackQueryAsync(
            request.CallbackQueryId,
            cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
    
    protected abstract void HandleInternal(TCommand request, TelegramMessageBuilder telegramMessageBuilder);
}