using FluentTelegramUI;
using FluentTelegramUI.Models;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
    ?? throw new InvalidOperationException("Set TELEGRAM_BOT_TOKEN.");

var bot = new TelegramBotBuilder()
    .WithToken(token)
    .WithFluentUI(FluentStyle.Modern)
    .AddScreen("Main Menu", builder => builder
        .WithContent("Welcome to FluentTelegramUI!")
        .AddToggle("Notifications", "notifications", true), isMainScreen: true)
    .WithAutoStartReceiving()
    .Build();

Console.WriteLine("SimpleScreenBot is running. Press Enter to stop.");
Console.ReadLine();
bot.StopReceiving();
