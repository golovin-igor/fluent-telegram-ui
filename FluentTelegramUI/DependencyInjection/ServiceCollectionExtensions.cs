using FluentTelegramUI.Handlers;
using FluentTelegramUI.Hosting;
using FluentTelegramUI.Models;
using FluentTelegramUI.Resources;
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
        => AddFluentTelegramUI(services, configure, configureBot: null);

    /// <summary>
    /// Adds FluentTelegramUI services and runs <paramref name="configureBot"/> once at startup
    /// (for example, to register screens on the bot instance).
    /// </summary>
    public static IServiceCollection AddFluentTelegramUI(
        this IServiceCollection services,
        Action<FluentTelegramUIOptions> configureOptions,
        Action<FluentTelegramBot>? configureBot)
    {
        FluentTelegramUIServiceRegistrations.AddCore(services, configureOptions);

        services.TryAddSingleton<FluentTelegramBot>();

        if (configureBot != null)
        {
            services.AddSingleton(configureBot);
            services.AddHostedService<FluentTelegramBotSetupHostedService>();
        }

        services.AddHostedService<FluentTelegramBotHostedService>();

        return services;
    }
}
