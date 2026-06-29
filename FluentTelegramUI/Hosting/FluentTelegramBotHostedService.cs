using FluentTelegramUI.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentTelegramUI.Hosting;

/// <summary>
/// Hosted service that starts and stops Telegram long-polling for a <see cref="FluentTelegramBot"/>.
/// </summary>
public sealed class FluentTelegramBotHostedService : IHostedService
{
    private readonly FluentTelegramBot _bot;
    private readonly ILogger<FluentTelegramBotHostedService> _logger;
    private readonly FluentTelegramUIOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentTelegramBotHostedService"/> class.
    /// </summary>
    public FluentTelegramBotHostedService(
        FluentTelegramBot bot,
        IOptions<FluentTelegramUIOptions> options,
        ILogger<FluentTelegramBotHostedService> logger)
    {
        _bot = bot;
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.AutoStartPolling)
        {
            return Task.CompletedTask;
        }

        _bot.StartReceiving(cancellationToken);
        _logger.LogInformation("FluentTelegramUI polling started.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _bot.StopReceiving();
        _logger.LogInformation("FluentTelegramUI polling stopped.");
        return Task.CompletedTask;
    }
}
