using FluentAssertions;
using FluentTelegramUI.Models;
using FluentTelegramUI.Resources;
using Xunit;

namespace FluentTelegramUI.Tests;

public class LocalizationServiceTests
{
    [Fact]
    public void GetString_UsesGermanCultureStoredForChat()
    {
        var stateMachine = new StateMachine();
        var localization = new LocalizationService { DefaultCulture = "en" };
        localization.BindStateStore(stateMachine);
        localization.SetCulture(42, "de");

        localization.GetString(42, "WelcomeMessage")
            .Should().Be("Willkommen bei Fluent Telegram UI!");
    }

    [Fact]
    public void GetString_FallsBackToDefaultCultureWhenChatHasNoCulture()
    {
        var localization = new LocalizationService { DefaultCulture = "en" };

        localization.GetString(1, "WelcomeMessage")
            .Should().Be("Welcome to Fluent Telegram UI!");
    }
}
