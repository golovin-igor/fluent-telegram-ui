using FluentTelegramUI;
using FluentTelegramUI.Builders;
using FluentTelegramUI.DependencyInjection;
using FluentTelegramUI.Hosting;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using FluentMessage = FluentTelegramUI.Models.Message;

var builder = WebApplication.CreateBuilder(args);

var token = builder.Configuration["TELEGRAM_BOT_TOKEN"]
    ?? Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
    ?? throw new InvalidOperationException("Set TELEGRAM_BOT_TOKEN.");

builder.Services.AddFluentTelegramUI(
    options =>
    {
        options.BotToken = token;
        options.AutoStartPolling = false;
        options.DefaultStyle = FluentStyle.Modern;
        options.WebhookSecretToken = builder.Configuration["TELEGRAM_WEBHOOK_SECRET"]
            ?? Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_SECRET");
    },
    bot =>
    {
        var mainScreen = new ScreenBuilder(bot, "Webhook Bot")
            .WithContent("This bot receives updates via webhook.")
            .AddButton("Ping", "ping")
            .Build();

        mainScreen.OnCallback("ping", async (_, context) =>
        {
            var chatId = (long)context["chatId"]!;
            await bot.SendMessageAsync(chatId, new FluentMessage { Text = "Pong!" });
            return true;
        });

        bot.RegisterScreen(mainScreen, isMainScreen: true);
    });

builder.Services.AddLogging();

var app = builder.Build();

app.MapGet("/", () => "FluentTelegramUI WebhookBot is running.");

app.MapPost("/bot/webhook", async (
    Update update,
    FluentTelegramBot bot,
    ITelegramBotClient client,
    IOptions<FluentTelegramUIOptions> options,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    var secret = options.Value.WebhookSecretToken;
    if (!string.IsNullOrEmpty(secret))
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var header)
            || header != secret)
        {
            return Results.Unauthorized();
        }
    }

    await FluentTelegramWebhook.ProcessUpdateAsync(bot, client, update, cancellationToken);
    return Results.Ok();
});

app.Run();
