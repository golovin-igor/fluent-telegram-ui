---
layout: default
title: Handling Callbacks
parent: Advanced Topics
nav_order: 4
---

# Handling Callbacks

Callbacks drive inline buttons and interactive controls. FluentTelegramUI routes them through the screen system first, then optional custom handlers.

## Routing order

When a callback arrives, `ScreenManager`:

1. **`screen:{id}`** — navigates to another registered screen
2. **`back`** — navigates to the parent screen (when configured)
3. **Exact handler** — `screen.OnCallback("my_action", ...)`
4. **Wildcard handler** — patterns such as `rate:*` or `service-rating:*`

If the screen system handles the callback, the default `IFluentUpdateHandler` is **not** invoked (avoids double `AnswerCallbackQuery`).

## Screen navigation callbacks

Use stable screen IDs and navigation buttons:

```csharp
.AddScreen("Menu", s => s
    .WithId("menu")
    .AddNavigationButton("Settings", "settings"), isMainScreen: true)
.AddScreen("Settings", s => s
    .WithId("settings")
    .AddNavigationButton("Back", "menu"))
```

`AddNavigationButton` emits callback data `screen:settings`.

## Custom handlers

```csharp
screen.OnCallback("confirm_order", async (data, context) =>
{
    var chatId = (long)context["chatId"]!;
    // respond, update state, refresh screen...
    await bot.RefreshCurrentScreenAsync(chatId);
    return true; // handled
});
```

Return `true` when the handler completed successfully.

## Wildcard patterns

Register patterns with `*` as a segment wildcard:

```csharp
screen.OnCallback("rate:*", async (data, context) =>
{
    var value = data.Split(':')[1];
    // ...
    return true;
});
```

`CallbackMatcher` matches exact keys first, then wildcard entries.

## Built-in control callbacks

| Control | Callback pattern |
|---------|------------------|
| Toggle | `{id}:on` / `{id}:off` |
| Carousel | `carousel:{id}:prev` / `:next` |
| Accordion | `accordion:{id}:expand` / `:collapse` |
| Rating | `{prefix}:{value}` with wildcard handler |

Use `WithToggleHandler`, `AddImageCarousel`, `AddAccordion`, and `AddRating` on `ScreenBuilder` — handlers are wired automatically.

## Typed context

Prefer `FluentCallbackContext` when you need strong typing:

```csharp
using FluentTelegramUI.Models;

screen.OnCallback("act", async (data, context) =>
{
    var ctx = FluentCallbackContext.FromDictionary(context);
    var chatId = ctx.ChatId;
    return true;
});
```

## Fallback handler

Provide a custom `IFluentUpdateHandler` with `WithUpdateHandler()` for callbacks the screen system does not handle:

```csharp
new TelegramBotBuilder()
    .WithToken(token)
    .WithUpdateHandler(myHandler)
    .Build();
```

## Related

- [Context Parameters](../components/context-parameters.md)
- [State Management](state-management.html)
