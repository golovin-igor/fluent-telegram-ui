using System.Collections.Generic;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Models;

/// <summary>
/// Typed access helpers for the dictionary-based handler context.
/// Lets handlers read strongly-typed values without casting magic strings,
/// while keeping the <see cref="Dictionary{String, Object}"/> contract stable.
/// </summary>
public static class CallbackContextExtensions
{
    /// <summary>Gets the chat id from the context, or 0 if absent.</summary>
    public static long GetChatId(this Dictionary<string, object> context)
        => context.TryGetValue("chatId", out var v) ? System.Convert.ToInt64(v) : 0;

    /// <summary>Gets the user id from the context, or 0 if absent.</summary>
    public static long GetUserId(this Dictionary<string, object> context)
        => context.TryGetValue("userId", out var v) ? System.Convert.ToInt64(v) : 0;

    /// <summary>Gets the username from the context, or empty string if absent.</summary>
    public static string GetUsername(this Dictionary<string, object> context)
        => context.TryGetValue("username", out var v) ? v?.ToString() ?? string.Empty : string.Empty;

    /// <summary>Gets the first name from the context, or empty string if absent.</summary>
    public static string GetFirstName(this Dictionary<string, object> context)
        => context.TryGetValue("firstName", out var v) ? v?.ToString() ?? string.Empty : string.Empty;

    /// <summary>Gets the last name from the context, or empty string if absent.</summary>
    public static string GetLastName(this Dictionary<string, object> context)
        => context.TryGetValue("lastName", out var v) ? v?.ToString() ?? string.Empty : string.Empty;

    /// <summary>Gets the message id from the context, or 0 if absent.</summary>
    public static int GetMessageId(this Dictionary<string, object> context)
        => context.TryGetValue("messageId", out var v) ? System.Convert.ToInt32(v) : 0;

    /// <summary>Gets the callback query from the context, or null if absent.</summary>
    public static CallbackQuery? GetCallbackQuery(this Dictionary<string, object> context)
        => context.TryGetValue("callbackQuery", out var v) ? v as CallbackQuery : null;

    /// <summary>Gets the message from the context, or null if absent.</summary>
    public static Telegram.Bot.Types.Message? GetMessage(this Dictionary<string, object> context)
        => context.TryGetValue("message", out var v) ? v as Telegram.Bot.Types.Message : null;

    /// <summary>Builds a typed <see cref="FluentCallbackContext"/> view of the context.</summary>
    public static FluentCallbackContext AsTyped(this Dictionary<string, object> context)
        => FluentCallbackContext.FromDictionary(context);
}
