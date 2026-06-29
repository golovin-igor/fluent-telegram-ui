---
layout: default
title: State Management
parent: Advanced Topics
nav_order: 3
---

# State Management

FluentTelegramUI includes a per-chat **state machine** for conversation flows, control persistence, and localization culture.

## Accessing state

From a built bot:

```csharp
var bot = new TelegramBotBuilder().WithToken(token).Build();

// Convenience methods on FluentTelegramBot / ScreenManager
bot.SetState(chatId, "name", "Alice");
var name = bot.GetState<string>(chatId, "name");
bot.SetCurrentState(chatId, "awaiting_email");
bot.ClearState(chatId);
```

Under the hood, `ScreenManager.StateMachine` implements `IStateStore`.

## Conversation states

Use the `"state"` key for wizard-style flows (also exposed as `SetState(chatId, stateName)` on `StateMachine`):

```csharp
screen.OnTextInput("awaiting_name", async (text, context) =>
{
    var chatId = (long)context["chatId"]!;
    bot.SetState(chatId, "name", text);
    bot.SetCurrentState(chatId, "awaiting_email");
    await bot.NavigateToScreenAsync(chatId, "email-screen");
    return true;
});
```

`ScreenUpdateHandler` routes text input to handlers registered with `OnTextInput(stateName, ...)` when the chat's current state matches.

## Control state (per chat)

Interactive controls (toggle, carousel, accordion, rating) store values per chat under keys like:

```
ctrl:{screenId}:{controlId}:{property}
```

You normally do not set these manually — `ScreenBuilder` handlers call `ScreenManager.SetControlState` and `RefreshCurrentScreenAsync` for you.

## Current screen tracking

The state machine tracks the active screen ID per chat via `SetCurrentScreen` / `GetCurrentScreen`. Screen navigation updates this automatically.

## Reset on `/start`

When users send `/start`, `ScreenUpdateHandler`:

1. Clears all state variables for the chat (`ClearState`)
2. Sets conversation state to `"initial"`
3. Navigates to the main screen

Culture and custom keys are cleared on `/start`. Use language buttons with `OnSetCulture()` if users should switch locale after restarting.

## Dependency injection

Register a shared store with:

```csharp
services.AddFluentTelegramUI(options => { ... });
// IStateStore and StateMachine registered as singletons
```

Inject `IStateStore` into your own services when needed.

## Related

- [Context Parameters](../components/context-parameters.md) — data passed to handlers
- [Handling Callbacks](handling-callbacks.html) — routing and wildcards
- [Localization](localization.html) — `culture` state key
