using Telegram.Bot;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Hosting;

/// <summary>
/// Helpers for handling Telegram webhook updates in ASP.NET Core or other hosts.
/// </summary>
public static class FluentTelegramWebhook
{
    /// <summary>
    /// Processes a webhook update through the bot pipeline.
    /// </summary>
    public static Task ProcessUpdateAsync(
        FluentTelegramBot bot,
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken = default)
        => bot.ProcessUpdateAsync(client, update, cancellationToken);
}
