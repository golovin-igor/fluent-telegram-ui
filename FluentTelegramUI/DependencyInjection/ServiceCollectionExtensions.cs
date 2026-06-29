using FluentTelegramUI.Handlers;
using FluentTelegramUI.Hosting;
using FluentTelegramUI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telegram.Bot;

namespace FluentTelegramUI.DependencyInjection;

/// <summary>
/// Dependency injection extensions for FluentTelegramUI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds FluentTelegramUI services to the service collection.
    /// </summary>
    public static IServiceCollection AddFluentTelegramUI(
        this IServiceCollection services,
        Action<FluentTelegramUIOptions> configure)
    {
        services.Configure(configure);
        services.TryAddSingleton<IStateStore, StateMachine>();
        services.TryAddSingleton<StateMachine>();
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<FluentTelegramUIOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.BotToken))
            {
                throw new InvalidOperationException("FluentTelegramUIOptions.BotToken must be configured.");
            }

            return new TelegramBotClient(options.BotToken);
        });
        services.AddSingleton<ScreenManager>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<FluentTelegramUIOptions>>().Value;
            return new ScreenManager(
                sp.GetRequiredService<ITelegramBotClient>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ScreenManager>>(),
                sp.GetRequiredService<StateMachine>(),
                options.DefaultStyle);
        });
        services.AddSingleton<IFluentUpdateHandler>(sp =>
        {
            var logger = sp.GetService<Microsoft.Extensions.Logging.ILogger<ScreenUpdateHandler>>();
            return new ScreenUpdateHandler(
                logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ScreenUpdateHandler>.Instance,
                sp.GetRequiredService<ScreenManager>());
        });
        services.AddSingleton<FluentTelegramBot>();
        services.AddHostedService<FluentTelegramBotHostedService>();
        return services;
    }
}
