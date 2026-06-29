using FluentTelegramUI;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
    ?? throw new InvalidOperationException("Set TELEGRAM_BOT_TOKEN.");

var bot = new TelegramBotBuilder()
    .WithToken(token)
    .WithFluentUI(FluentStyle.Modern)
    .AddScreen("Home", ConfigureHomeScreen, isMainScreen: true)
    .WithAutoStartReceiving()
    .Build();

Console.WriteLine("LocalizedScreenBot is running. Press Enter to stop.");
Console.ReadLine();
bot.StopReceiving();

static void ConfigureHomeScreen(ScreenBuilder screen)
{
    screen.WithId("home")
        .WithLocalizedTitle("WelcomeMessage")
        .WithLocalizedContent("SettingsMessage")
        .AddButton("English", "lang:en")
        .AddButton("Deutsch", "lang:de")
        .OnSetCulture("lang:en", "en")
        .OnSetCulture("lang:de", "de");
}
