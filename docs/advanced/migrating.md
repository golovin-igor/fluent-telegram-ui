---
layout: default
title: Migrating from Telegram.Bot
parent: Advanced Topics
nav_order: 1
---

# Migrating from Telegram.Bot
{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

## Overview

This guide will help you migrate an existing Telegram bot built with the Telegram.Bot library to FluentTelegramUI. While FluentTelegramUI is built on top of Telegram.Bot, it introduces a different paradigm for bot development, focusing on UI components and screen-based navigation.

The migration process can be approached incrementally, allowing you to adopt FluentTelegramUI's features gradually while maintaining existing functionality.

## Basic Migration Steps

### 1. Install the FluentTelegramUI Package

First, add the FluentTelegramUI NuGet package to your project:

```bash
dotnet add package FluentTelegramUI
```

### 2. Update Bot Initialization

#### Before (Telegram.Bot):

```csharp
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

// Create the bot client
var botClient = new TelegramBotClient("YOUR_BOT_TOKEN");

// Set up update handlers
var updateHandler = new UpdateHandler();
using var cts = new CancellationTokenSource();

// Start receiving updates
botClient.StartReceiving(
    updateHandler.HandleUpdateAsync,
    updateHandler.HandleErrorAsync,
    new ReceiverOptions(),
    cts.Token
);
```

#### After (FluentTelegramUI):

```csharp
using FluentTelegramUI;
using Microsoft.Extensions.DependencyInjection;

// Set up dependency injection
var services = new ServiceCollection()
    .AddSingleton<ITelegramBotClient>(new TelegramBotClient("YOUR_BOT_TOKEN"))
    .AddLogging()
    .BuildServiceProvider();

// Create the bot with the fluent builder
var bot = new TelegramBotBuilder()
    .WithServiceProvider(services)
    .WithFluentUI()
    .Build();

// Start receiving updates
bot.StartReceiving();
```

### 3. Convert Message Handlers to Screens

#### Before (Telegram.Bot):

```csharp
public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is { } message)
    {
        if (message.Text == "/start")
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Welcome to my bot! Choose an option:",
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("Option 1", "option1") },
                    new[] { InlineKeyboardButton.WithCallbackData("Option 2", "option2") }
                }),
                cancellationToken: cancellationToken
            );
        }
    }
    else if (update.CallbackQuery is { } callbackQuery)
    {
        if (callbackQuery.Data == "option1")
        {
            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "You selected Option 1",
                cancellationToken: cancellationToken
            );
        }
        else if (callbackQuery.Data == "option2")
        {
            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "You selected Option 2",
                cancellationToken: cancellationToken
            );
        }
    }
}
```

#### After (FluentTelegramUI):

```csharp
// Create and register the main screen
var mainScreen = new ScreenBuilder(bot, "Main Menu")
    .WithContent("Welcome to my bot! Choose an option:")
    .AddButton("Option 1", "option1")
    .AddButton("Option 2", "option2")
    .Build();

// Register the screen
bot.RegisterScreen(mainScreen, isMainScreen: true);

// Create option screens
var option1Screen = new ScreenBuilder(bot, "Option 1")
    .WithContent("You selected Option 1")
    .AddNavigationButton("Back to Main Menu", mainScreen.Id)
    .Build();

var option2Screen = new ScreenBuilder(bot, "Option 2")
    .WithContent("You selected Option 2")
    .AddNavigationButton("Back to Main Menu", mainScreen.Id)
    .Build();

// Register option screens
bot.RegisterScreen(option1Screen);
bot.RegisterScreen(option2Screen);

// Set up navigation
mainScreen.OnCallback("option1", async (data, context) => {
    long chatId = (long)context["chatId"];
    await bot.NavigateToScreenAsync(chatId, option1Screen.Id);
    return true;
});

mainScreen.OnCallback("option2", async (data, context) => {
    long chatId = (long)context["chatId"];
    await bot.NavigateToScreenAsync(chatId, option2Screen.Id);
    return true;
});
```

### 4. Migrate Command Handlers

Most Telegram bots use commands like `/start` and `/help`. Here's how to migrate them:

#### Before (Telegram.Bot):

```csharp
if (message.Text == "/start")
{
    // Handle start command
}
else if (message.Text == "/help")
{
    // Handle help command
}
```

#### After (FluentTelegramUI):

You can add custom command handlers to the bot:

```csharp
// Setup command handlers
bot.SetCommandHandler("/start", async (message, cancellationToken) => {
    await bot.NavigateToMainScreenAsync(message.Chat.Id, cancellationToken);
    return true;
});

bot.SetCommandHandler("/help", async (message, cancellationToken) => {
    await bot.NavigateToScreenAsync(message.Chat.Id, "help-screen", cancellationToken);
    return true;
});
```

## Common Migration Scenarios

### Handling Inline Keyboards

#### Before (Telegram.Bot):

```csharp
var inlineKeyboard = new InlineKeyboardMarkup(new[]
{
    new[] { InlineKeyboardButton.WithCallbackData("Button 1", "btn1") },
    new[] { InlineKeyboardButton.WithCallbackData("Button 2", "btn2") }
});

await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Choose an option:",
    replyMarkup: inlineKeyboard
);
```

#### After (FluentTelegramUI):

```csharp
var screen = new ScreenBuilder(bot, "Options Screen")
    .WithContent("Choose an option:")
    .AddButton("Button 1", "btn1")
    .AddButton("Button 2", "btn2")
    .WithButtonsPerRow(1) // One button per row
    .Build();

bot.RegisterScreen(screen);
await bot.NavigateToScreenAsync(chatId, screen.Id);
```

### Handling Callback Queries

#### Before (Telegram.Bot):

```csharp
if (callbackQuery.Data == "settings")
{
    // Show settings menu
}
else if (callbackQuery.Data.StartsWith("set_lang:"))
{
    string language = callbackQuery.Data.Substring(9);
    // Set language preference
}
```

#### After (FluentTelegramUI):

```csharp
// Regular callback
settingsScreen.OnCallback("notifications", async (data, context) => {
    // Handle notifications button press
    return true;
});

// Pattern matching with wildcard
settingsScreen.OnCallback("set_lang:*", async (data, context) => {
    string language = data.Split(':')[1];
    // Set language preference
    return true;
});
```

### Managing User State

#### Before (Telegram.Bot):

```csharp
// Using a dictionary to track user state
private Dictionary<long, UserState> _userStates = new Dictionary<long, UserState>();

// In the handler
if (_userStates.TryGetValue(message.From.Id, out var state))
{
    if (state.AwaitingName)
    {
        state.Name = message.Text;
        state.AwaitingName = false;
        state.AwaitingEmail = true;
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"Thanks, {state.Name}! Now, please enter your email."
        );
    }
    else if (state.AwaitingEmail)
    {
        // Process email
    }
}
```

#### After (FluentTelegramUI):

```csharp
// Using the built-in state machine
nameScreen.OnTextInput("awaiting_name", async (name, context) => {
    long chatId = (long)context["chatId"];
    long userId = (long)context["userId"];
    
    // Store the name
    bot.SetState(chatId, "name", name);
    
    // Update state
    bot.SetState(chatId, "awaiting_email");
    
    // Navigate to email screen
    await bot.NavigateToScreenAsync(chatId, "email-screen");
    return true;
});

emailScreen.OnTextInput("awaiting_email", async (email, context) => {
    long chatId = (long)context["chatId"];
    
    // Get the stored name
    string name = bot.GetState<string>(chatId, "name");
    
    // Process both name and email
    await bot.SendMessageAsync(chatId, new Message { 
        Text = $"Thank you, {name}! Your email {email} has been registered."
    });
    
    // Clear state
    bot.ClearState(chatId);
    return true;
});
```

## Advanced Migration Techniques

### Incremental Migration Strategy

You may not need to migrate your entire bot at once. Consider an incremental approach:

1. **Dual-Mode Operation**: Initially, use both FluentTelegramUI and direct Telegram.Bot calls.

```csharp
// Create FluentTelegramUI bot
var fluentBot = new TelegramBotBuilder()
    .WithServiceProvider(services)
    .WithFluentUI()
    .Build();

// Keep a reference to the Telegram.Bot client for legacy code
var botClient = services.GetRequiredService<ITelegramBotClient>();

// Set a custom update handler for non-migrated parts
fluentBot.SetCustomUpdateHandler(async (update, cancellationToken) => {
    // Let FluentTelegramUI handle screen navigation
    if (update.CallbackQuery?.Data?.StartsWith("screen:") == true)
        return false;
        
    // Handle legacy callbacks
    if (update.CallbackQuery != null)
    {
        await HandleLegacyCallbackAsync(botClient, update.CallbackQuery);
        return true;
    }
    
    return false;
});
```

2. **Migrate Screen by Screen**: Start by moving your main screens to FluentTelegramUI, while keeping specialized features in your original code.

### Preserving Custom Logic

If your bot contains complex custom logic, you can integrate it with FluentTelegramUI:

```csharp
// Inject your existing services
var customService = new YourCustomService();

// Use in screen callbacks
mainScreen.OnCallback("complex_action", async (data, context) => {
    long chatId = (long)context["chatId"];
    
    // Execute your custom logic
    var result = await customService.ProcessActionAsync(data);
    
    // Update the UI based on the result
    if (result.Success)
    {
        await bot.NavigateToScreenAsync(chatId, "success-screen");
    }
    else
    {
        // Set an error message to display on the error screen
        bot.SetState(chatId, "error_message", result.ErrorMessage);
        await bot.NavigateToScreenAsync(chatId, "error-screen");
    }
    
    return true;
});
```

### Handling Forms with Multiple Fields

For complex forms, leverage the state machine and context parameters:

```csharp
// Registration process screens
var registrationStartScreen = new ScreenBuilder(bot, "Registration")
    .WithContent("Let's start the registration process. What's your name?")
    .Build();

bot.RegisterScreen(registrationStartScreen);

// Setup text input handlers for various stages
registrationStartScreen.OnTextInput("*", async (name, context) => {
    long chatId = (long)context["chatId"];
    
    // Store the name
    bot.SetState(chatId, "name", name);
    
    // Set the next state
    bot.SetState(chatId, "awaiting_email");
    
    // Send the next question
    await bot.SendMessageAsync(chatId, new Message {
        Text = "Great! Now, what's your email address?"
    });
    
    return true;
});

// Then handle email input
bot.SetTextInputHandler("awaiting_email", async (message, cancellationToken) => {
    var email = message.Text;
    long chatId = message.Chat.Id;
    
    // Validate email
    if (!IsValidEmail(email))
    {
        await bot.SendMessageAsync(chatId, new Message {
            Text = "That doesn't look like a valid email. Please try again."
        });
        return true;
    }
    
    // Store email
    bot.SetState(chatId, "email", email);
    
    // Continue the form
    bot.SetState(chatId, "awaiting_age");
    
    await bot.SendMessageAsync(chatId, new Message {
        Text = "How old are you?"
    });
    
    return true;
});

// Complete the registration process
bot.SetTextInputHandler("awaiting_age", async (message, cancellationToken) => {
    var ageText = message.Text;
    long chatId = message.Chat.Id;
    
    if (!int.TryParse(ageText, out int age))
    {
        await bot.SendMessageAsync(chatId, new Message {
            Text = "Please enter a valid number for your age."
        });
        return true;
    }
    
    // Get all form data from state
    string name = bot.GetState<string>(chatId, "name");
    string email = bot.GetState<string>(chatId, "email");
    
    // Process registration
    await RegisterUserAsync(name, email, age);
    
    // Navigate to thank you screen
    await bot.NavigateToScreenAsync(chatId, "thank-you-screen");
    
    // Clear state
    bot.ClearState(chatId);
    
    return true;
});
```

### Migrating Media Handling

If your bot handles photos, documents, or other media:

```csharp
// Setup a photo handler
bot.SetPhotoHandler(async (message, cancellationToken) => {
    long chatId = message.Chat.Id;
    var photoId = message.Photo.Last().FileId;
    
    // Store the photo ID in the state
    bot.SetState(chatId, "last_photo_id", photoId);
    
    // Navigate to the photo processing screen
    await bot.NavigateToScreenAsync(chatId, "photo-processing-screen");
    
    return true;
});

// In your photo processing screen
photoProcessingScreen.OnCallback("apply_filter", async (data, context) => {
    long chatId = (long)context["chatId"];
    
    // Get the photo ID from state
    string photoId = bot.GetState<string>(chatId, "last_photo_id");
    
    // Process the photo
    var processedPhotoFile = await ProcessPhotoWithFilterAsync(photoId);
    
    // Send the processed photo
    await bot.SendPhotoAsync(chatId, 
        new InputFileStream(processedPhotoFile),
        caption: "Here's your photo with the filter applied!");
    
    return true;
});
```

## Best Practices for Migration

### 1. Plan Your Screen Hierarchy

Before migrating, map out your bot's navigation flow. Identify:

- Main screens
- Sub-screens
- Modal-like screens
- Form flows

Create a visual diagram of how screens connect to help plan your migration.

### 2. Use Consistent Naming Conventions

Adopting clear naming conventions helps maintain a clean codebase:

- Screen IDs: Use kebab-case for screen identifiers (e.g., `"main-menu"`, `"user-settings"`)
- Callback Data: Use descriptive prefixes for callback data (e.g., `"settings:theme"`, `"profile:edit"`)
- State Keys: Use snake_case for state machine keys (e.g., `"user_name"`, `"awaiting_payment"`)

### 3. Separate UI from Business Logic

While FluentTelegramUI focuses on UI, your business logic should be separate:

```csharp
// Define your business logic in separate classes
public class UserService
{
    public async Task<bool> RegisterUserAsync(string name, string email, int age)
    {
        // Implementation
    }
}

// Inject and use in your screens
var userService = new UserService();

registrationScreen.OnCallback("submit", async (data, context) => {
    // Get data from state
    long chatId = (long)context["chatId"];
    var name = bot.GetState<string>(chatId, "name");
    var email = bot.GetState<string>(chatId, "email");
    var age = bot.GetState<int>(chatId, "age");
    
    // Use service to handle business logic
    bool success = await userService.RegisterUserAsync(name, email, age);
    
    // Update UI based on result
    if (success)
    {
        await bot.NavigateToScreenAsync(chatId, "registration-success");
    }
    else
    {
        await bot.NavigateToScreenAsync(chatId, "registration-error");
    }
    
    return true;
});
```

### 4. Implement Error Handling

Robust error handling is crucial for a good user experience:

```csharp
// Global error handler
bot.SetErrorHandler(async (exception, context) => {
    if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
    {
        // Log the error
        Console.WriteLine($"Error: {exception.Message}");
        
        // Send a user-friendly error message
        await bot.SendMessageAsync(chatId, new Message {
            Text = "Sorry, something went wrong. Please try again later."
        });
        
        // For critical errors, navigate to an error screen
        if (exception is CriticalException)
        {
            await bot.NavigateToScreenAsync(chatId, "error-screen");
        }
    }
});
```

### 5. Leverage Advanced UI Components

Upgrade your bot's UI with FluentTelegramUI's advanced components:

- Replace toggle settings with the `Toggle` component
- Convert image galleries to an `ImageCarousel`
- Use `Accordion` for expandable FAQ sections
- Add `RichText` for better text formatting
- Implement `ProgressIndicator` for ongoing processes
- Add `Rating` for collecting user feedback

### 6. Testing Migrated Code

Test your migrated bot thoroughly:

```csharp
// Create a test helper to simulate user interactions
public class BotTester
{
    private readonly FluentTelegramBot _bot;
    
    public BotTester(FluentTelegramBot bot)
    {
        _bot = bot;
    }
    
    public async Task SimulateCommandAsync(long chatId, string command)
    {
        var message = new Telegram.Bot.Types.Message
        {
            Chat = new Telegram.Bot.Types.Chat { Id = chatId },
            Text = command
        };
        
        // Trigger command handler
        await _bot.HandleCommandAsync(message);
    }
    
    public async Task SimulateCallbackAsync(long chatId, string callbackData)
    {
        // Create an update with a callback query
        var callbackQuery = new Telegram.Bot.Types.CallbackQuery
        {
            Message = new Telegram.Bot.Types.Message { Chat = new Telegram.Bot.Types.Chat { Id = chatId } },
            Data = callbackData
        };
        
        // Process the callback
        await _bot.HandleCallbackQueryAsync(callbackQuery);
    }
}
```

## Conclusion

Migrating from Telegram.Bot to FluentTelegramUI involves a paradigm shift from event-based handling to a screen-based UI approach. While the migration requires some upfront effort, the benefits include:

- **Cleaner code** with separation between UI and business logic
- **More structured navigation** with the screen system
- **Enhanced UI capabilities** with advanced components
- **Simplified state management** with the built-in state machine
- **More maintainable codebase** as your bot grows in complexity

Remember that migration can be incremental, allowing you to adopt FluentTelegramUI features at your own pace. Start with the main screens and most-used features, then gradually refactor the rest of your bot as time allows. 