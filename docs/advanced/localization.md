---
layout: default
title: Localization
parent: Advanced Topics
nav_order: 2
---

# Localization
{: .no_toc }

FluentTelegramUI ships with embedded resources (`Strings.resx`, `Strings.de.resx`) and a per-chat localization service. Screen titles and body text can be resolved from resource keys at render time.

## Per-chat culture

Culture is stored in the state machine under the key `culture` for each chat. Use `ILocalizationService` or `ScreenBuilder.OnSetCulture()` to change it:

```csharp
using FluentTelegramUI;
using FluentTelegramUI.Builders;

var bot = new TelegramBotBuilder()
    .WithToken(token)
    .AddScreen("Home", screen => screen
        .WithId("home")
        .WithLocalizedTitle("WelcomeMessage")
        .WithLocalizedContent("SettingsMessage")
        .AddButton("English", "lang:en")
        .AddButton("Deutsch", "lang:de")
        .OnSetCulture("lang:en", "en")
        .OnSetCulture("lang:de", "de"), isMainScreen: true)
    .WithAutoStartReceiving()
    .Build();
```

When the user taps **Deutsch**, the screen refreshes in German using `Strings.de.resx`.

## Resource keys on screens

| Property | Purpose |
|----------|---------|
| `TitleResourceKey` | Localized screen title |
| `ContentResourceKey` | Localized body text |
| `BackButtonResourceKey` | Localized back button label (reserved) |

Set them via `ScreenBuilder`:

```csharp
.WithLocalizedTitle("WelcomeMessage")
.WithLocalizedContent("SettingsMessage")
```

Or directly on `Screen`:

```csharp
screen.TitleResourceKey = "WelcomeMessage";
screen.ContentResourceKey = "SettingsMessage";
```

## Dependency injection

When using `AddFluentTelegramUI()`, register the default culture in options:

```csharp
services.AddFluentTelegramUI(options =>
{
    options.BotToken = token;
    options.DefaultCulture = "en";
});
```

`ILocalizationService` is registered automatically and bound to the shared `IStateStore`.

## Adding languages

1. Add `Strings.<culture>.resx` under `FluentTelegramUI/Resources/` (for example `Strings.fr.resx`).
2. Copy keys from `Strings.resx` and translate values.
3. Rebuild the library — satellite assemblies are embedded automatically.

## Command handlers

`LocalizedCommandHandler` resolves strings per chat using the same culture storage. Inject `ILocalizationService` when registering a custom handler:

```csharp
new LocalizedCommandHandler(botClient, localizationService);
```

## Sample

See [`samples/LocalizedScreenBot`](https://github.com/golovin-igor/fluent-telegram-ui/tree/main/samples/LocalizedScreenBot) for a runnable bot with English/German switching.
