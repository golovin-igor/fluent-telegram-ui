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

This guide will help you create a simple Telegram bot using FluentTelegramUI. By the end, you'll have a functional bot with interactive buttons and basic navigation.

## Creating a New Project

First, create a new console application:

```bash
dotnet new console -n MyTelegramBot
cd MyTelegramBot
```

Then, install the FluentTelegramUI package:

```bash
dotnet add package FluentTelegramUI
```

## Setting Up the Bot

Create a new file called `Program.cs` with the following content:

```csharp
using System;
using FluentTelegramUI;
using FluentTelegramUI.Models;
using FluentTelegramUI.Handlers;

namespace MyTelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Replace with your bot token from BotFather
            string botToken = "YOUR_BOT_TOKEN";
            
            // Create a bot with fluent configuration
            var bot = new TelegramBotBuilder()
                .WithToken(botToken)
                .WithFluentUI()
                .WithAutoStartReceiving()
                .Build();
                
            // Create a main screen
            var mainScreen = new Screen
            {
                Title = "Main Menu",
                Content = new Message
                {
                    Text = "Welcome to my bot! Please select an option:",
                    ParseMarkdown = true,
                    ButtonsPerRow = 2
                }
            };
            
            // Add buttons to the main screen
            mainScreen.Content.Buttons.Add(new Button 
            { 
                Text = "📝 About", 
                CallbackData = "about" 
            });
            
            mainScreen.Content.Buttons.Add(new Button 
            { 
                Text = "ℹ️ Help", 
                CallbackData = "help" 
            });
            
            // Register the main screen
            bot.RegisterScreen(mainScreen, true);
            
            // Add callbacks for the buttons
            mainScreen.OnCallback("about", async (data, context) => 
            {
                long chatId = (long)context["chatId"];
                string firstName = (string)context["firstName"];
                
                var message = new Message
                {
                    Text = $"*About This Bot*\n\nHello {firstName}! This is a sample bot created with FluentTelegramUI.",
                    ParseMarkdown = true
                };
                
                await bot.SendMessageAsync(chatId, message);
                return true;
            });
            
            mainScreen.OnCallback("help", async (data, context) => 
            {
                long chatId = (long)context["chatId"];
                
                var message = new Message
                {
                    Text = "*Help*\n\nThis bot demonstrates the basic features of FluentTelegramUI. " +
                           "You can navigate through screens and interact with buttons.",
                    ParseMarkdown = true
                };
                
                await bot.SendMessageAsync(chatId, message);
                return true;
            });
            
            // Keep the application running
            Console.WriteLine("Bot started! Press Enter to exit.");
            Console.ReadLine();
            
            // Stop the bot when done
            bot.StopReceiving();
        }
    }
}
```

## Running the Bot

Run the bot using the dotnet CLI:

```bash
dotnet run
```

## Testing the Bot

1. Open Telegram and search for your bot's username
2. Start a chat with your bot by sending the `/start` command
3. The bot should respond with a welcome message and buttons
4. Click the buttons to see the responses

## Adding Multiple Screens

Let's enhance our bot by adding navigation between multiple screens:

```csharp
// Create additional screens
var aboutScreen = new Screen
{
    Title = "About",
    Content = new Message
    {
        Text = "*About This Bot*\n\nThis is a sample bot created with FluentTelegramUI. " + 
               "It demonstrates screens, buttons, and navigation.",
        ParseMarkdown = true
    }
};

var helpScreen = new Screen
{
    Title = "Help",
    Content = new Message
    {
        Text = "*Help*\n\nAvailable commands:\n" +
               "/start - Show the main menu\n\n" +
               "Click the buttons to navigate between screens.",
        ParseMarkdown = true
    }
};

// Set parent relationships for back navigation
aboutScreen.WithParent(mainScreen);
helpScreen.WithParent(mainScreen);

// Register screens
bot.RegisterScreen(aboutScreen);
bot.RegisterScreen(helpScreen);

// Update callbacks to navigate to screens
mainScreen.OnCallback("about", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    await bot.NavigateToScreenAsync(chatId, aboutScreen.Id);
    return true;
});

mainScreen.OnCallback("help", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    await bot.NavigateToScreenAsync(chatId, helpScreen.Id);
    return true;
});
```

## Handling Text Input

You can also handle text input from users:

```csharp
var inputNameScreen = new Screen
{
    Title = "Input Your Name",
    Content = new Message
    {
        Text = "Please type your name:",
        ParseMarkdown = true
    }
};

inputNameScreen.WithParent(mainScreen);
bot.RegisterScreen(inputNameScreen);

// Add a button to the main screen for the name input
mainScreen.Content.Buttons.Add(new Button 
{ 
    Text = "✏️ Enter Name", 
    CallbackData = "input_name" 
});

// Add callback to navigate to the input screen
mainScreen.OnCallback("input_name", async (data, context) => 
{
    long chatId = (long)context["chatId"];
    
    // Set the current state to expect name input
    bot.SetCurrentState(chatId, "awaiting_name");
    
    await bot.NavigateToScreenAsync(chatId, inputNameScreen.Id);
    return true;
});

// Add a text input handler for the name
inputNameScreen.OnTextInput("awaiting_name", async (name, context) => 
{
    long chatId = (long)context["chatId"];
    
    // Store the name in state
    bot.SetState(chatId, "name", name);
    
    // Acknowledge the input
    var message = new Message
    {
        Text = $"Thank you! Your name has been set to: *{name}*",
        ParseMarkdown = true
    };
    
    await bot.SendMessageAsync(chatId, message);
    
    // Navigate back to main screen
    await bot.NavigateToScreenAsync(chatId, mainScreen.Id);
    return true;
});
```

## Next Steps

Congratulations! You now have a working Telegram bot with interactive buttons, multiple screens, and text input handling. To learn more about the library's features:

- Explore the [Core Components](../components/screens.md) documentation
- Check out the [Examples](../examples/basic-bot.md) for more complex scenarios
- Learn about [State Management](../advanced/state-management.md) for handling conversations

Remember to replace `YOUR_BOT_TOKEN` with your actual bot token from BotFather! 