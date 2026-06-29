---
layout: default
title: Quick Start Guide
parent: Getting Started
nav_order: 2
---

# Quick Start Guide

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

This guide walks through a minimal bot using `TelegramBotBuilder` and `ScreenBuilder` on **.NET 10** with **Telegram.Bot 22**.

## Create a project

```bash
dotnet new console -n MyTelegramBot
cd MyTelegramBot
dotnet add package FluentTelegramUI
```

## Minimal bot

Replace `Program.cs` with:

```csharp
using FluentTelegramUI;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
    ?? "YOUR_BOT_TOKEN";

var bot = new TelegramBotBuilder()
    .WithToken(token)
    .WithFluentUI(FluentStyle.Modern)
    .AddScreen("Main Menu", screen => screen
        .WithId("main")
        .WithContent("Welcome! Choose an option:")
        .AddNavigationButton("About", "about")
        .AddNavigationButton("Help", "help"), isMainScreen: true)
    .AddScreen("About", screen => screen
        .WithId("about")
        .WithContent("Built with FluentTelegramUI.")
        .AddNavigationButton("Back", "main"))
    .AddScreen("Help", screen => screen
        .WithId("help")
        .WithContent("Send /start to return to the menu.")
        .AddNavigationButton("Back", "main"))
    .WithAutoStartReceiving()
    .Build();

Console.WriteLine("Bot running. Press Enter to stop.");
Console.ReadLine();
bot.StopReceiving();
```

Run it:

```bash
export TELEGRAM_BOT_TOKEN="your-token"
dotnet run
```

In Telegram, send `/start` to open the main menu, then use the inline buttons to navigate.

## Key concepts

- **`WithId("main")`** — stable screen ID used in `screen:main` navigation callbacks. Set before `Build()` registers the screen.
- **`AddNavigationButton`** — creates a button with callback data `screen:{targetId}` handled by the screen system.
- **`WithAutoStartReceiving()`** — starts long-polling when the bot is built.

## Dependency injection variant

For ASP.NET Core or generic host apps, use `AddFluentTelegramUI()`:

```csharp
using FluentTelegramUI.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => services.AddFluentTelegramUI(
        options => options.BotToken = token,
        bot =>
        {
            var main = new ScreenBuilder(bot, "Main Menu")
                .WithId("main")
                .WithContent("Hello!")
                .Build();
            bot.RegisterScreen(main, isMainScreen: true);
        }))
    .Build();

await host.RunAsync();
```

See [`samples/HostedServiceBot`](https://github.com/golovin-igor/fluent-telegram-ui/tree/main/samples/HostedServiceBot) and [`samples/WebhookBot`](https://github.com/golovin-igor/fluent-telegram-ui/tree/main/samples/WebhookBot).

## Interactive controls

Add toggles, carousels, ratings, and more with `ScreenBuilder` extension methods:

```csharp
.AddToggle("Dark mode", "dark_mode", false)
.AddRating("Rate us", "rating", 0)
```

See [Advanced UI Components](../components/advanced-components.md) and the [`AdvancedComponentsBot`](https://github.com/golovin-igor/fluent-telegram-ui/tree/main/samples/AdvancedComponentsBot) sample.

## Localization

Localize titles and content with resource keys:

```csharp
.WithLocalizedTitle("WelcomeMessage")
.WithLocalizedContent("SettingsMessage")
.OnSetCulture("lang:de", "de")
```

See [Localization](../advanced/localization.md).

## Next steps

- [Installation](installation.md) — prerequisites and NuGet setup
- [Samples](https://github.com/golovin-igor/fluent-telegram-ui/tree/main/samples) — runnable reference bots
- [Migrating from Telegram.Bot](../advanced/migrating.md) — upgrade an existing bot
