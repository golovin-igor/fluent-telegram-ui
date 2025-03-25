---
layout: default
title: Context Usage Examples
parent: Examples
nav_order: 4
---

# Context Usage Examples

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

This page provides practical examples of how to use context parameters in your FluentTelegramUI bot.

## Basic Context Usage

Here's a simple example showing how to access and use context parameters in callbacks:

```csharp
using FluentTelegramUI;
using FluentTelegramUI.Models;
using System;

// Create a main screen with a button
var mainScreen = new Screen
{
    Title = "Context Example",
    Content = new Message
    {
        Text = "Click the button to see context information",
        ParseMarkdown = true
    }
};

// Add a button
mainScreen.Content.Buttons.Add(new Button
{
    Text = "👤 Show My Info",
    CallbackData = "show_info"
});

// Add a callback that displays context information
mainScreen.OnCallback("show_info", async (data, context) => 
{
    // Extract context parameters
    long chatId = (long)context["chatId"];
    long userId = (long)context["userId"];
    string username = (string)context["username"] ?? "No username";
    string firstName = (string)context["firstName"];
    string lastName = (string)context["lastName"] ?? "";
    
    // Create a response message with user information
    var infoMessage = new Message
    {
        Text = $"*Your Information*\n\n" +
               $"👤 Name: {firstName} {lastName}\n" +
               $"🔖 Username: @{username}\n" +
               $"🆔 User ID: `{userId}`\n" +
               $"💬 Chat ID: `{chatId}`",
        ParseMarkdown = true
    };
    
    // Send the message
    await bot.SendMessageAsync(chatId, infoMessage);
    return true;
});

// Register the screen
bot.RegisterScreen(mainScreen, true);
```

## Personalized Navigation

This example shows how to implement personalized navigation using context parameters:

```csharp
// Create two screens
var mainScreen = new Screen
{
    Title = "Main Menu",
    Content = new Message
    {
        Text = "Welcome! Where would you like to go?",
        ParseMarkdown = true,
        ButtonsPerRow = 2
    }
};

var adminPanel = new Screen
{
    Title = "Admin Panel",
    Content = new Message
    {
        Text = "*Admin Panel*\n\nThis area is for administrators only.",
        ParseMarkdown = true
    }
};

var userArea = new Screen
{
    Title = "User Area",
    Content = new Message
    {
        Text = "*User Area*\n\nThis area is for regular users.",
        ParseMarkdown = true
    }
};

// Add buttons to the main screen
mainScreen.Content.Buttons.Add(new Button
{
    Text = "⚙️ Admin Panel",
    CallbackData = "go_admin"
});

mainScreen.Content.Buttons.Add(new Button
{
    Text = "👥 User Area",
    CallbackData = "go_user"
});

// Set up parent relationships
adminPanel.WithParent(mainScreen);
userArea.WithParent(mainScreen);

// Register all screens
bot.RegisterScreen(mainScreen, true);
bot.RegisterScreen(adminPanel);
bot.RegisterScreen(userArea);

// List of admin user IDs
var adminUserIds = new List<long> { 123456789, 987654321 }; // Replace with actual admin IDs

// Callback for admin panel navigation with permission check
mainScreen.OnCallback("go_admin", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    long userId = (long)context["userId"];
    string firstName = (string)context["firstName"];
    
    // Check if user is admin
    if (adminUserIds.Contains(userId))
    {
        // User is admin, allow navigation
        await bot.NavigateToScreenAsync(chatId, adminPanel.Id);
    }
    else
    {
        // User is not admin, send error message
        var errorMessage = new Message
        {
            Text = $"Sorry {firstName}, you don't have permission to access the Admin Panel.",
            ParseMarkdown = true
        };
        
        await bot.SendMessageAsync(chatId, errorMessage);
    }
    
    return true;
});

// Callback for user area navigation
mainScreen.OnCallback("go_user", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    await bot.NavigateToScreenAsync(chatId, userArea.Id);
    return true;
});
```

## User Preferences Management

This example shows how to use context parameters for storing user preferences:

```csharp
// Dictionary to store user preferences (in a real application, use a database)
var userPreferences = new Dictionary<long, Dictionary<string, object>>();

// Create a preferences screen
var preferencesScreen = new Screen
{
    Title = "Preferences",
    Content = new Message
    {
        Text = "Configure your preferences:",
        ParseMarkdown = true,
        ButtonsPerRow = 2
    }
};

// Add preference toggle buttons
preferencesScreen.Content.Buttons.Add(new Button
{
    Text = "🌙 Dark Mode",
    CallbackData = "toggle_dark_mode"
});

preferencesScreen.Content.Buttons.Add(new Button
{
    Text = "🔔 Notifications",
    CallbackData = "toggle_notifications"
});

// Register the screen
bot.RegisterScreen(preferencesScreen);

// Helper function to ensure user has preferences record
void EnsureUserPreferences(long userId)
{
    if (!userPreferences.ContainsKey(userId))
    {
        userPreferences[userId] = new Dictionary<string, object>
        {
            { "darkMode", false },
            { "notifications", true }
        };
    }
}

// Helper function to get preference status emoji
string GetStatusEmoji(bool isEnabled) => isEnabled ? "✅" : "❌";

// Callback to toggle dark mode
preferencesScreen.OnCallback("toggle_dark_mode", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    long userId = (long)context["userId"];
    
    // Ensure user has preferences
    EnsureUserPreferences(userId);
    
    // Toggle the preference
    bool currentSetting = (bool)userPreferences[userId]["darkMode"];
    userPreferences[userId]["darkMode"] = !currentSetting;
    
    // Get updated settings
    bool darkMode = (bool)userPreferences[userId]["darkMode"];
    bool notifications = (bool)userPreferences[userId]["notifications"];
    
    // Update screen content
    preferencesScreen.Content.Text = $"Configure your preferences:\n\n" +
                                    $"🌙 Dark Mode: {GetStatusEmoji(darkMode)}\n" +
                                    $"🔔 Notifications: {GetStatusEmoji(notifications)}";
    
    // Update screen
    await bot.RefreshScreenAsync(chatId, preferencesScreen.Id);
    return true;
});

// Callback to toggle notifications
preferencesScreen.OnCallback("toggle_notifications", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    long userId = (long)context["userId"];
    
    // Ensure user has preferences
    EnsureUserPreferences(userId);
    
    // Toggle the preference
    bool currentSetting = (bool)userPreferences[userId]["notifications"];
    userPreferences[userId]["notifications"] = !currentSetting;
    
    // Get updated settings
    bool darkMode = (bool)userPreferences[userId]["darkMode"];
    bool notifications = (bool)userPreferences[userId]["notifications"];
    
    // Update screen content
    preferencesScreen.Content.Text = $"Configure your preferences:\n\n" +
                                    $"🌙 Dark Mode: {GetStatusEmoji(darkMode)}\n" +
                                    $"🔔 Notifications: {GetStatusEmoji(notifications)}";
    
    // Update screen
    await bot.RefreshScreenAsync(chatId, preferencesScreen.Id);
    return true;
});
```

## Group Chat vs Private Chat

This example shows how to handle different contexts for group chats versus private chats:

```csharp
// Create a main screen
var mainScreen = new Screen
{
    Title = "Main Menu",
    Content = new Message
    {
        Text = "Welcome to the bot!",
        ParseMarkdown = true
    }
};

// Add a check context button
mainScreen.Content.Buttons.Add(new Button
{
    Text = "🔍 Check Context",
    CallbackData = "check_context"
});

// Register the screen
bot.RegisterScreen(mainScreen, true);

// Callback to analyze the context
mainScreen.OnCallback("check_context", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    
    // Access the full CallbackQuery object for more context
    var callbackQuery = (Telegram.Bot.Types.CallbackQuery)context["callbackQuery"];
    var chat = callbackQuery.Message.Chat;
    
    string chatType = chat.Type.ToString();
    string responseText = "";
    
    // Handle different chat types
    if (chatType == "Private")
    {
        // Private chat context
        string firstName = (string)context["firstName"];
        
        responseText = $"Hello {firstName}!\n\n" +
                      $"You're in a private chat with this bot.\n" +
                      $"Chat ID: `{chatId}`";
    }
    else if (chatType == "Group" || chatType == "Supergroup")
    {
        // Group chat context
        string firstName = (string)context["firstName"];
        string groupName = chat.Title;
        
        responseText = $"Hello {firstName}!\n\n" +
                      $"You're in a group chat called *{groupName}*\n" +
                      $"Group Chat ID: `{chatId}`\n" +
                      $"Number of Members: [Unknown - API limitation]";
    }
    else if (chatType == "Channel")
    {
        // Channel context
        string channelName = chat.Title;
        
        responseText = $"This is a channel called *{channelName}*\n" +
                      $"Channel ID: `{chatId}`";
    }
    
    // Create response message
    var responseMessage = new Message
    {
        Text = $"*Context Analysis*\n\n{responseText}",
        ParseMarkdown = true
    };
    
    // Send response
    await bot.SendMessageAsync(chatId, responseMessage);
    return true;
});
```

## Accessing Raw Telegram Objects

For advanced scenarios, you can access the raw Telegram API objects:

```csharp
// Create a screen
var advancedScreen = new Screen
{
    Title = "Advanced Context",
    Content = new Message
    {
        Text = "Click the button to see advanced context information",
        ParseMarkdown = true
    }
};

// Add a button
advancedScreen.Content.Buttons.Add(new Button
{
    Text = "🔬 Technical Details",
    CallbackData = "technical_details"
});

// Register the screen
bot.RegisterScreen(advancedScreen);

// Callback with raw object access
advancedScreen.OnCallback("technical_details", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    
    // Access raw CallbackQuery object
    var callbackQuery = (Telegram.Bot.Types.CallbackQuery)context["callbackQuery"];
    
    // Access raw Message object
    var message = callbackQuery.Message;
    
    // Create response with technical details
    var responseText = $"*Technical Details*\n\n" +
                      $"CallbackQuery ID: `{callbackQuery.Id}`\n" +
                      $"Chat Instance: `{callbackQuery.ChatInstance}`\n" +
                      $"Message ID: `{message.MessageId}`\n" +
                      $"Message Date: `{message.Date.ToUniversalTime():yyyy-MM-dd HH:mm:ss} UTC`\n";
                      
    if (message.ReplyMarkup != null)
    {
        responseText += $"Has Inline Keyboard: Yes\n";
        responseText += $"Number of Keyboard Rows: `{message.ReplyMarkup.InlineKeyboard.Length}`\n";
    }
    
    if (message.Entities != null)
    {
        responseText += $"Message Entities: `{message.Entities.Length}`\n";
    }
    
    // Send response
    var responseMessage = new Message
    {
        Text = responseText,
        ParseMarkdown = true
    };
    
    await bot.SendMessageAsync(chatId, responseMessage);
    return true;
});
```

## Next Steps

Now that you've seen how to use context parameters in various scenarios, you can:

- Learn more about [handling callbacks](../advanced/handling-callbacks.md)
- Explore [state management](../advanced/state-management.md) for more complex user interactions
- Check out the [API reference](../api/screen.md) for detailed documentation on the Screen class 