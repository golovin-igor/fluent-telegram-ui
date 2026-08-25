---
layout: default
title: Rich Messages
parent: Components
nav_order: 9
---

# Rich Messages

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

Native Telegram [Rich Messages](https://core.telegram.org/bots/api#inputrichmessage) (`sendRichMessage`) are opt-in. Existing screens still send ordinary HTML via `SendMessage` unless you set rich content.

This is different from the [Rich Text](rich-text.md) control, which only wraps bold/italic/underline into a plain message.

## Send a rich message

```csharp
var message = new MessageBuilder()
    .WithRichHtml("<h1>Hello</h1><p>Native rich formatting.</p>")
    .WithUrlButton("GitHub", "https://github.com/golovin-igor/fluent-telegram-ui")
    .Build();

await bot.SendMessageAsync(chatId, message);
```

Use `WithRichMarkdown` instead of `WithRichHtml` when you prefer Markdown. Exactly one of HTML or Markdown must be set.

## Screens

```csharp
.AddScreen("Welcome", screen => screen
    .WithId("welcome")
    .WithRichHtml("<h1>Welcome</h1><p>This screen is a native rich message.</p>")
    .AddUrlButton("Docs", "https://golovin-igor.github.io/fluent-telegram-ui/"),
    isMainScreen: true)
```

On `/start`, the screen system navigates to the main screen. If that screen has rich content, FluentTelegramUI calls `SendRichMessage`. Later refreshes use `EditMessageText` with `rich_message`.

The rich payload is the full body: screen title and `FluentStyle` HTML templates are not injected. Do not mix advanced controls (toggles, carousels, accordions) onto a rich screen yet.

## Sample

[`samples/RichMessageBot`](https://github.com/golovin-igor/fluent-telegram-ui/tree/main/samples/RichMessageBot) sends a pitch for fluent-telegram-ui on `/start`.

```bash
export TELEGRAM_BOT_TOKEN="your-token"
dotnet run --project samples/RichMessageBot
```
