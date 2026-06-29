using FluentTelegramUI.Models;
using FluentTelegramUI.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace FluentTelegramUI.DependencyInjection;

internal static class LocalizationServiceRegistration
{
    internal static LocalizationService AddFluentTelegramUILocalization(
        IServiceCollection services,
        IStateStore stateStore,
        string defaultCulture)
    {
        var localization = new LocalizationService { DefaultCulture = defaultCulture };
        localization.BindStateStore(stateStore);
        services.AddSingleton<ILocalizationService>(localization);
        services.AddSingleton(localization);
        return localization;
    }
}
