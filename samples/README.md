# Samples

Runnable examples for FluentTelegramUI v0.2+ (.NET 10, Telegram.Bot 22).

| Project | Description |
|---------|-------------|
| [SimpleScreenBot](SimpleScreenBot/) | Minimal bot using `TelegramBotBuilder` and long-polling |
| [HostedServiceBot](HostedServiceBot/) | Generic host with `AddFluentTelegramUI()` and automatic polling |
| [WebhookBot](WebhookBot/) | ASP.NET Core webhook endpoint with optional secret-token validation |
| [AdvancedComponentsBot](AdvancedComponentsBot/) | Demo of toggles, carousel, progress, accordion, rich text, and rating |
| [LocalizedScreenBot](LocalizedScreenBot/) | Per-chat localization with resource keys and culture switching |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A bot token from [@BotFather](https://t.me/botfather)

```bash
export TELEGRAM_BOT_TOKEN="your-token-here"
```

## Run

```bash
# SimpleScreenBot (TelegramBotBuilder)
dotnet run --project samples/SimpleScreenBot

# HostedServiceBot (DI + IHostedService)
dotnet run --project samples/HostedServiceBot

# Advanced UI components demo
dotnet run --project samples/AdvancedComponentsBot

# Localized screens (English / Deutsch)
dotnet run --project samples/LocalizedScreenBot

# WebhookBot (ASP.NET Core)
dotnet run --project samples/WebhookBot
```

### Webhook setup

1. Expose your app on HTTPS (e.g. ngrok, cloud deploy).
2. Optionally set `TELEGRAM_WEBHOOK_SECRET` — Telegram sends it in `X-Telegram-Bot-Api-Secret-Token`.
3. Register the webhook with Telegram:

```bash
curl "https://api.telegram.org/bot$TELEGRAM_BOT_TOKEN/setWebhook" \
  -d "url=https://your-host/bot/webhook" \
  -d "secret_token=$TELEGRAM_WEBHOOK_SECRET"
```

Legacy reference material lives under `FluentTelegramUI/Examples/README.md` (source files were migrated into `samples/`).
