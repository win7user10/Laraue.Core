﻿using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Extensions;

public static class UpdateExtensions
{
    public static User? GetUser(this Update update)
    {
        return update.Message?.From
            ?? update.CallbackQuery?.From;
    }
    
    public static long GetUserId(this Update update)
    {
        return update.GetUser()!.Id;
    }
}