using Microsoft.Extensions.Hosting;

namespace FluentTelegramUI.Hosting;

internal sealed class FluentTelegramBotSetupHostedService : IHostedService
{
    private readonly FluentTelegramBot _bot;
    private readonly Action<FluentTelegramBot> _configure;

    public FluentTelegramBotSetupHostedService(FluentTelegramBot bot, Action<FluentTelegramBot> configure)
    {
        _bot = bot;
        _configure = configure;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _configure(_bot);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
