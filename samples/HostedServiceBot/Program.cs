using FluentTelegramUI;
using FluentTelegramUI.Builders;
using FluentTelegramUI.DependencyInjection;
using FluentTelegramUI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
    ?? throw new InvalidOperationException("Set TELEGRAM_BOT_TOKEN.");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddFluentTelegramUI(
            options =>
            {
                options.BotToken = token;
                options.AutoStartPolling = true;
                options.DefaultStyle = FluentStyle.Modern;
            },
            bot =>
            {
                var screen = new ScreenBuilder(bot, "Hosted Service Bot")
                    .WithContent("Running via AddFluentTelegramUI() and IHostedService polling.")
                    .AddToggle("Dark mode", "dark_mode", false)
                    .Build();

                bot.RegisterScreen(screen, isMainScreen: true);
            });

        services.AddLogging(logging => logging.AddConsole());
    })
    .Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("HostedServiceBot");
logger.LogInformation("HostedServiceBot is running. Press Ctrl+C to stop.");

await host.RunAsync();
