using FluentTelegramUI.Models;

namespace FluentTelegramUI.DependencyInjection;

/// <summary>
/// Configuration options for FluentTelegramUI.
/// </summary>
public sealed class FluentTelegramUIOptions
{
    /// <summary>Telegram bot token.</summary>
    public string BotToken { get; set; } = "";

    /// <summary>Default UI style for rendered screens.</summary>
    public FluentStyle DefaultStyle { get; set; } = FluentStyle.Default;

    /// <summary>When true, starts long-polling automatically via hosted service.</summary>
    public bool AutoStartPolling { get; set; } = true;

    /// <summary>Optional secret token for webhook validation (Telegram X-Telegram-Bot-Api-Secret-Token header).</summary>
    public string? WebhookSecretToken { get; set; }
}
