using FluentTelegramUI.Handlers;
using FluentTelegramUI.Models;
using FluentTelegramUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace FluentTelegramUI.DependencyInjection;

/// <summary>
/// Shared core service registrations used by both <see cref="TelegramBotBuilder"/>
/// and <see cref="ServiceCollectionExtensions.AddFluentTelegramUI"/>. Keeps the
/// two composition roots consistent.
/// </summary>
internal static class FluentTelegramUIServiceRegistrations
{
    /// <summary>
    /// Registers shared core services. Uses <c>TryAdd</c> for the bot client and
    /// update handler so callers can pre-register overrides (e.g. a mock client
    /// or a custom <see cref="IFluentUpdateHandler"/>).
    /// </summary>
    public static IServiceCollection AddCore(
        IServiceCollection services,
        Action<FluentTelegramUIOptions> configureOptions)
    {
        services.Configure(configureOptions);

        // Register the concrete StateMachine first, then IStateStore as a
        // forward to the same singleton so all consumers share one instance.
        services.TryAddSingleton<StateMachine>();
        services.TryAddSingleton<IStateStore>(sp => sp.GetRequiredService<StateMachine>());

        services.TryAddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FluentTelegramUIOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.BotToken))
            {
                throw new InvalidOperationException("FluentTelegramUIOptions.BotToken must be configured.");
            }

            return new TelegramBotClient(options.BotToken);
        });

        services.TryAddSingleton<LocalizationService>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FluentTelegramUIOptions>>().Value;
            var stateStore = sp.GetRequiredService<IStateStore>();
            var logger = sp.GetService<ILogger<LocalizationService>>();
            var localization = new LocalizationService(logger) { DefaultCulture = options.DefaultCulture };
            localization.BindStateStore(stateStore);
            return localization;
        });
        services.TryAddSingleton<ILocalizationService>(sp => sp.GetRequiredService<LocalizationService>());

        services.TryAddSingleton<ScreenRenderer>();
        services.TryAddSingleton<ScreenManager>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FluentTelegramUIOptions>>().Value;
            return new ScreenManager(
                sp.GetRequiredService<ITelegramBotClient>(),
                sp.GetRequiredService<ILogger<ScreenManager>>(),
                sp.GetRequiredService<StateMachine>(),
                options.DefaultStyle,
                sp.GetRequiredService<ILocalizationService>());
        });

        services.TryAddSingleton<IFluentUpdateHandler>(sp =>
        {
            var logger = sp.GetService<ILogger<ScreenUpdateHandler>>()
                ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ScreenUpdateHandler>.Instance;
            return new ScreenUpdateHandler(logger, sp.GetRequiredService<ScreenManager>());
        });

        return services;
    }
}
