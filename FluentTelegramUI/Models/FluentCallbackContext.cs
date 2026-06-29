using System.Collections.Generic;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Models;

/// <summary>
/// Typed context passed to screen callback and text-input handlers.
/// </summary>
public sealed class FluentCallbackContext
{
    /// <summary>
    /// Creates a context from the legacy dictionary representation.
    /// </summary>
    public static FluentCallbackContext FromDictionary(Dictionary<string, object> context)
    {
        return new FluentCallbackContext
        {
            ChatId = context.TryGetValue("chatId", out var chatId) ? Convert.ToInt64(chatId) : 0,
            UserId = context.TryGetValue("userId", out var userId) ? Convert.ToInt64(userId) : 0,
            Username = context.TryGetValue("username", out var username) ? username?.ToString() ?? "" : "",
            FirstName = context.TryGetValue("firstName", out var firstName) ? firstName?.ToString() ?? "" : "",
            LastName = context.TryGetValue("lastName", out var lastName) ? lastName?.ToString() ?? "" : "",
            MessageId = context.TryGetValue("messageId", out var messageId) ? Convert.ToInt32(messageId) : 0,
            CallbackQuery = context.TryGetValue("callbackQuery", out var callbackQuery) ? callbackQuery as CallbackQuery : null,
            Message = context.TryGetValue("message", out var message) ? message as Telegram.Bot.Types.Message : null,
            Raw = context
        };
    }

    /// <summary>ID of the chat where the interaction occurred.</summary>
    public long ChatId { get; init; }

    /// <summary>ID of the user who triggered the interaction.</summary>
    public long UserId { get; init; }

    /// <summary>Username of the user, if available.</summary>
    public string Username { get; init; } = "";

    /// <summary>First name of the user.</summary>
    public string FirstName { get; init; } = "";

    /// <summary>Last name of the user, if available.</summary>
    public string LastName { get; init; } = "";

    /// <summary>ID of the message containing the interaction.</summary>
    public int MessageId { get; init; }

    /// <summary>The callback query, when applicable.</summary>
    public CallbackQuery? CallbackQuery { get; init; }

    /// <summary>The text message, when applicable.</summary>
    public Telegram.Bot.Types.Message? Message { get; init; }

    /// <summary>Original dictionary for backward compatibility.</summary>
    public IReadOnlyDictionary<string, object> Raw { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// Builds the legacy dictionary representation.
    /// </summary>
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>
        {
            ["chatId"] = ChatId,
            ["userId"] = UserId,
            ["username"] = Username,
            ["firstName"] = FirstName,
            ["lastName"] = LastName,
            ["messageId"] = MessageId
        };

        if (CallbackQuery != null)
        {
            dict["callbackQuery"] = CallbackQuery;
        }

        if (Message != null)
        {
            dict["message"] = Message;
        }

        return dict;
    }
}
