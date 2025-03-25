# Fluent Telegram UI

A modern, fluent-style UI framework for creating Telegram bots using C# .NET. This project aims to provide an intuitive and elegant way to build Telegram bots with a focus on clean, maintainable code and beautiful user interfaces.

## Features

- 🎨 Fluent Design System-inspired UI components
- 🔧 Easy-to-use builder pattern for creating bot interactions
- 📱 Responsive and adaptive layouts
- 🎯 Built-in support for common Telegram bot patterns
- 🚀 High-performance and scalable architecture
- 📦 NuGet package available for easy integration
- 🔄 Screen-based navigation with back functionality
- 🎛️ Interactive UI controls with callback handling
- 🧠 Integrated state machine for managing conversation flows

## Getting Started

### Prerequisites

- .NET 6.0 or later
- A Telegram Bot Token (get it from [@BotFather](https://t.me/botfather))

### Installation

```bash
dotnet add package FluentTelegramUI
```

### Basic Usage

```csharp
using FluentTelegramUI;
using Microsoft.Extensions.DependencyInjection;

// Configure services
var services = new ServiceCollection()
    .AddSingleton<ITelegramBotClient>(new TelegramBotClient("YOUR_BOT_TOKEN"))
    .AddLogging()
    .BuildServiceProvider();

// Create a bot
var bot = new TelegramBotBuilder()
    .WithServiceProvider(services)
    .WithFluentUI()
    .Build();

// Create a fluent-style message
var message = new Message
{
    Text = "Hello, World!",
    ParseMarkdown = true
};

message.Buttons.Add(new Button
{
    Text = "Click Me",
    CallbackData = "click_action"
});

// Send the message
await bot.SendMessageAsync(123456789, message);

// Start receiving updates
bot.StartReceiving();
```

## Screen System

The Screen system allows you to create interactive UI screens with navigation and callbacks:

```csharp
// Create screens
var mainScreen = new Screen
{
    Title = "Main Menu",
    Content = new Message
    {
        Text = "Welcome to the main menu!",
        ParseMarkdown = true
    }
};

var settingsScreen = new Screen
{
    Title = "Settings",
    Content = new Message
    {
        Text = "Configure your settings:",
        ParseMarkdown = true
    }
};

// Add buttons to screens
mainScreen.Content.Buttons.Add(new Button
{
    Text = "Settings",
    CallbackData = "view_settings"
});

settingsScreen.Content.Buttons.Add(new Button
{
    Text = "Dark Mode",
    CallbackData = "toggle_dark_mode"
});

// Register screens with the bot
bot.RegisterScreen(mainScreen, true); // Set as main screen
bot.RegisterScreen(settingsScreen);

// Set up navigation
mainScreen.OnCallback("view_settings", async (data, context) => 
{
    // The context contains useful information:
    // - chatId: The ID of the current chat
    // - userId: The ID of the user who pressed the button
    // - username: The username of the user
    // - firstName: The first name of the user
    // - lastName: The last name of the user
    // - messageId: The ID of the message containing the button
    // - callbackQuery: The full CallbackQuery object
    
    long chatId = (long)context["chatId"];
    await bot.NavigateToScreenAsync(chatId, settingsScreen.Id);
    return true;
});

// Set parent for back navigation
settingsScreen.WithParent(mainScreen);

// Set up callback for toggle_dark_mode action
settingsScreen.OnCallback("toggle_dark_mode", async (data, context) => {
    // Access user information from context
    long userId = (long)context["userId"];
    string username = (string)context["username"];
    
    Console.WriteLine($"Dark mode toggled by {username} (ID: {userId})");
    
    // Handle dark mode toggle
    return true; // Refresh screen
});
```

## State Machine

FluentTelegramUI includes a powerful state machine for managing conversation flows:

```csharp
// Set state for a chat
bot.StateMachine.SetState(chatId, "username", "JohnDoe");
string username = bot.StateMachine.GetState<string>(chatId, "username");

// Set current conversation state
bot.StateMachine.SetState(chatId, "awaiting_email");

// Check current state
if (bot.StateMachine.IsInState(chatId, "awaiting_email"))
{
    // Handle email input
}

// Clear state
bot.StateMachine.ClearState(chatId);
```

### Multi-Step Forms Example

You can combine StateMachine with Screen navigation to create multi-step input forms:

```csharp
// Create screens for registration flow
var welcomeScreen = new Screen { 
    Title = "Welcome", 
    Content = new Message { 
        Text = "Welcome to the registration process. Click below to start:", 
        ParseMarkdown = true 
    } 
};

var nameScreen = new Screen { 
    Title = "Name Input", 
    Content = new Message { 
        Text = "Please enter your name:", 
        ParseMarkdown = true 
    } 
};

var emailScreen = new Screen { 
    Title = "Email Input", 
    Content = new Message { 
        Text = "Please enter your email:", 
        ParseMarkdown = true 
    } 
};

// Add button to welcome screen
welcomeScreen.Content.Buttons.Add(new Button { 
    Text = "Start Registration", 
    CallbackData = "start_registration" 
});

// Register all screens
bot.RegisterScreen(welcomeScreen, true);
bot.RegisterScreen(nameScreen);
bot.RegisterScreen(emailScreen);

// Set up handlers
welcomeScreen.OnCallback("start_registration", async (data, context) => 
{
    // Get chatId from context
    long chatId = (long)context["chatId"];
    
    // Set initial state for the registration process
    bot.StateMachine.SetState(chatId, "awaiting_name");
    
    // Navigate to the name input screen
    await bot.NavigateToScreenAsync(chatId, nameScreen.Id);
    return true;
});

// Set up name input handler
nameScreen.OnTextInput("awaiting_name", async (name, context) => 
{
    // Get chatId from context
    long chatId = (long)context["chatId"];
    
    // Store the name
    bot.StateMachine.SetState(chatId, "name", name);
    
    // Transition to next state
    bot.StateMachine.SetState(chatId, "awaiting_email");
    
    // Navigate to the email screen
    await bot.NavigateToScreenAsync(chatId, emailScreen.Id);
    return true;
});

// Set up email input handler
emailScreen.OnTextInput("awaiting_email", async (email, context) => 
{
    // Get chatId and user info from context
    long chatId = (long)context["chatId"];
    string firstName = (string)context["firstName"];
    
    // Validate email
    if (!email.Contains("@"))
    {
        // Invalid email, don't proceed
        return true; // Refresh screen with error
    }
    
    // Store the email
    bot.StateMachine.SetState(chatId, "email", email);
    
    // Complete the process
    bot.StateMachine.SetState(chatId, "complete");
    
    // Display completion message
    return true;
});

// Set up update handler for the bot to handle text inputs
var handler = new ScreenUpdateHandler(loggerFactory.CreateLogger<ScreenUpdateHandler>(), bot.ScreenManager);
bot.SetUpdateHandler(handler);
```

## Project Structure

The project is organized into several key namespaces:

- `FluentTelegramUI` - Core functionality and entry points
- `FluentTelegramUI.Models` - Data models including Screen and StateMachine
- `FluentTelegramUI.Handlers` - Update handlers and bot event processing

## Context Parameters

FluentTelegramUI passes context parameters to callback handlers, giving you easy access to important information like the user's details and chat ID. This eliminates the need for hard-coded IDs and makes your code more dynamic.

### Available Context Parameters

The following parameters are available in the context dictionary:

| Parameter | Type | Description |
|-----------|------|-------------|
| `chatId` | `long` | ID of the chat where the interaction occurred |
| `userId` | `long` | ID of the user who triggered the callback |
| `username` | `string` | Username of the user (may be empty) |
| `firstName` | `string` | First name of the user |
| `lastName` | `string` | Last name of the user (may be empty) |
| `messageId` | `int` | ID of the message that contains the callback button |
| `callbackQuery` | `CallbackQuery` | The full Telegram CallbackQuery object |

For text input handlers, the context also includes:
| Parameter | Type | Description |
|-----------|------|-------------|
| `message` | `Message` | The full Telegram Message object |

### Example Usage

```csharp
// Handler with context parameters
screen.OnCallback("show_profile", async (data, context) => 
{
    // Extract user information from context
    long chatId = (long)context["chatId"];
    long userId = (long)context["userId"];
    string username = (string)context["username"];
    string firstName = (string)context["firstName"];
    
    // Use the extracted information
    Console.WriteLine($"User {firstName} (@{username}) requested their profile");
    
    // Store user preference in a dictionary using their user ID
    userPreferences[userId] = new Dictionary<string, bool>
    {
        { "notifications", true },
        { "darkMode", false }
    };
    
    // Navigate to profile screen using the chat ID from context
    await bot.NavigateToScreenAsync(chatId, profileScreen.Id);
    return true;
});
```

This feature makes it much easier to build bots that maintain per-user state, provide personalized experiences, and work correctly in group chats.

## Dependencies

- Microsoft.Extensions.DependencyInjection (7.0.0)
- Microsoft.Extensions.Logging (7.0.0)
- Telegram.Bot (19.0.0)
- shortid (4.0.0)

## Status

This project is currently in development. While core functionality is working, there are still features to be implemented and improvements to be made.

### Completed
- [x] Core Telegram Bot integration
- [x] Screen-based UI components
- [x] Navigation system with back functionality
- [x] State machine for conversation management
- [x] Update handlers for processing bot events

### In Progress
- [ ] Complete documentation
- [ ] Publish NuGet package
- [ ] Add more UI controls and components
- [ ] Implement more examples

## Contributing

Contributions are welcome! If you find bugs or have feature requests, please open an issue on GitHub.

## License

This project is licensed under the MIT License.

## Acknowledgments

- Inspired by Microsoft's Fluent Design System
- Built on top of the Telegram Bot API
- Special thanks to all contributors

## Support

If you encounter any issues or have questions, please open an issue in the GitHub repository. 