using System.Text;
using Laraue.Core.DataAccess.Contracts;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Core.Telegram.Utils;

public class TelegramMessageBuilder
{
    private readonly List<string> _rows = new ();
    private readonly List<IEnumerable<InlineKeyboardButton>> _inlineKeyboardButtons = new ();
    private readonly List<IEnumerable<KeyboardButton>> _keyboardButtons = new ();

    private const string PageParameterName = "p";

    public TelegramMessageBuilder AppendDataRows<TData>(IPaginatedResult<TData> result, Func<TData, int, string> formatRow)
        where TData : class
    {
        _rows.AddRange(result.Data
            .Select(formatRow));

        return this;
    }

    public TelegramMessageBuilder AppendRow(string text)
    {
        _rows.Add(text);

        return this;
    }
    
    public TelegramMessageBuilder AddControlButtons<TData>(IPaginatedResult<TData> result, string route)
        where TData : class
    {
        if (result is {HasPreviousPage: false, HasNextPage: false})
        {
            return this;
        }

        var rowButtons = new List<InlineKeyboardButton>();
        if (result.HasPreviousPage)
        {
            rowButtons.Add(InlineKeyboardButton.WithCallbackData(
                "Previous ⬅",
                 $"{route}?{PageParameterName}={result.Page - 1}"));
        }
        if (result.HasNextPage)
        {
            rowButtons.Add(InlineKeyboardButton.WithCallbackData(
                "Next ➡",
                $"{route}?{PageParameterName}={result.Page + 1}"));
        }
        
        _inlineKeyboardButtons.Add(rowButtons);

        return this;
    }

    public TelegramMessageBuilder AddInlineKeyboardButtons<TData>(IPaginatedResult<TData> result, Func<TData, int, InlineKeyboardButton> getButton)
        where TData : class
    {
        var rowButtons = result.Data
            .Select(getButton);

        return AddInlineKeyboardButtons(rowButtons);
    }

    public TelegramMessageBuilder AddInlineKeyboardButtons(IEnumerable<InlineKeyboardButton> rowButtons)
    {
        var rowButtonsList = rowButtons.ToList();
        
        foreach (var rowButton in rowButtonsList)
        {
            var callbackDataLength = Encoding.UTF8.GetByteCount(rowButton.CallbackData ?? string.Empty);
            if (callbackDataLength > 64)
            {
                throw new InvalidOperationException($"The button with text '{rowButton.Text}' has callback '{rowButton.CallbackData}' with length {callbackDataLength}, <= 64 is required");
            }
        }       
        
        _inlineKeyboardButtons.Add(rowButtonsList);

        return this;
    }
    
    public TelegramMessageBuilder AddReplyKeyboardButtons(IEnumerable<KeyboardButton> rowButtons)
    {
        _keyboardButtons.Add(rowButtons);

        return this;
    }

    public string Text => string.Join("\n", _rows);
    public InlineKeyboardMarkup InlineKeyboard => new (_inlineKeyboardButtons);
    public ReplyKeyboardMarkup ReplyKeyboard => new (_keyboardButtons) { ResizeKeyboard = true };
}