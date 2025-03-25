---
layout: default
title: Context Parameters
parent: Core Components
nav_order: 5
---

# Context Parameters

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

## Overview

Context parameters provide a way to access important information about the user, chat, and interaction directly in your callback handlers. This feature eliminates the need for hardcoded IDs and makes your bot more dynamic and maintainable.

When a user interacts with your bot by clicking a button or sending a message, the context parameters give you access to information like:

- Who triggered the interaction (user ID, username, etc.)
- Where the interaction happened (chat ID)
- Additional details about the message or callback

## Available Parameters

The following context parameters are available in callback handlers:

| Parameter | Type | Description |
|:----------|:-----|:------------|
| `chatId` | `long` | ID of the chat where the interaction occurred |
| `userId` | `long` | ID of the user who triggered the callback |
| `username` | `string` | Username of the user (may be empty) |
| `firstName` | `string` | First name of the user |
| `lastName` | `string` | Last name of the user (may be empty) |
| `messageId` | `int` | ID of the message that contains the callback button |
| `callbackQuery` | `CallbackQuery` | The full Telegram CallbackQuery object |

For text input handlers, the context also includes:

| Parameter | Type | Description |
|:----------|:-----|:------------|
| `message` | `Message` | The full Telegram Message object |

## Using Context Parameters

To use context parameters, your callback handlers should accept two parameters:
1. The data parameter (callback data or text input)
2. The context dictionary

### In Callback Handlers

```csharp
screen.OnCallback("profile_button", async (data, context) => 
{
    // Extract user information from context
    long chatId = (long)context["chatId"];
    string username = (string)context["username"];
    string firstName = (string)context["firstName"];
    
    // Use the context information
    await bot.SendMessageAsync(chatId, new Message 
    {
        Text = $"Hello {firstName}! Your profile details will be shown here."
    });
    
    return true;
});
```

### In Text Input Handlers

```csharp
screen.OnTextInput("await_name", async (inputText, context) => 
{
    // Extract user information
    long chatId = (long)context["chatId"];
    long userId = (long)context["userId"];
    
    // Store the name in state machine
    bot.StateMachine.SetState(chatId, "name", inputText);
    
    // Navigate to the next screen
    await bot.NavigateToScreenAsync(chatId, nextScreen.Id);
    return true;
});
```

## Handling User-Specific Data

Context parameters make it easy to maintain user-specific data and personalized experiences:

```csharp
// Dictionary to store user preferences
var userPreferences = new Dictionary<long, Dictionary<string, bool>>();

screen.OnCallback("toggle_dark_mode", async (data, context) => 
{
    // Get user ID from context
    long userId = (long)context["userId"];
    
    // Ensure user has an entry in preferences
    if (!userPreferences.ContainsKey(userId))
    {
        userPreferences[userId] = new Dictionary<string, bool>
        {
            { "darkMode", false }
        };
    }
    
    // Toggle preference
    userPreferences[userId]["darkMode"] = !userPreferences[userId]["darkMode"];
    
    // Updating the UI or responding to the user
    // ...
    
    return true;
});
```

## Navigation Between Screens

Context parameters are particularly useful for navigation:

```csharp
screen.OnCallback("go_to_settings", async (data, context) => 
{
    // Get chat ID from context
    long chatId = (long)context["chatId"];
    
    // Navigate to the settings screen
    await bot.NavigateToScreenAsync(chatId, settingsScreen.Id);
    return true;
});
```

## Advanced Usage: Accessing the Raw Objects

You can access the raw CallbackQuery or Message objects for advanced scenarios:

```csharp
screen.OnCallback("advanced_action", async (data, context) => 
{
    // Get the raw CallbackQuery object
    var callbackQuery = (CallbackQuery)context["callbackQuery"];
    
    // Access advanced properties
    var inlineMessageId = callbackQuery.InlineMessageId;
    var chatInstance = callbackQuery.ChatInstance;
    
    // ... custom handling
    
    return true;
});
```

## Best Practices

- Always use context parameters instead of hardcoded IDs
- Check for null values when accessing optional properties like username or lastName
- Use type casting to ensure proper type conversion from the dictionary
- Consider storing the frequently used values in local variables for readability
- For large applications, you may want to create typed wrappers around the context dictionary 