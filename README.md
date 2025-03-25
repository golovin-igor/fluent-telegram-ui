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

var bot = new TelegramBotBuilder()
    .WithToken("YOUR_BOT_TOKEN")
    .WithFluentUI()
    .Build();

// Create a fluent-style message
var message = new MessageBuilder()
    .WithText("Hello, World!")
    .WithStyle(FluentStyle.Modern)
    .WithButton("Click Me", "callback_data")
    .Build();

await bot.SendMessageAsync(message);
```

## Screen System

The Screen system allows you to create interactive UI screens with navigation and callbacks:

```csharp
// Create a bot with screens
var bot = new TelegramBotBuilder()
    .WithToken("YOUR_BOT_TOKEN")
    .WithFluentUI()
    .AddScreen("Main Menu", sb => {
        sb.WithContent("Welcome to the main menu!", true)
          .AddButton("Settings", "view_settings")
          .AddButton("Profile", "view_profile")
          .WithButtonsPerRow(1)
          .AsMainScreen(); // Mark as the main screen
    })
    .AddScreen("Settings", sb => {
        sb.WithContent("Configure settings:", true)
          .AddButton("Dark Mode", "toggle_dark_mode")
          .AddButton("Notifications", "toggle_notifications")
          .WithButtonsPerRow(2)
          .OnCallback("toggle_dark_mode", async (data) => {
              // Handle dark mode toggle
              return true; // Refresh screen
          });
    })
    .WithAutoStartReceiving()
    .Build();

// Configure navigation between screens
if (bot.TryGetScreen("Main Menu", out var mainScreen) && 
    bot.TryGetScreen("Settings", out var settingsScreen))
{
    // Set up navigation
    mainScreen.OnCallback("view_settings", async (data) => {
        await bot.NavigateToScreenAsync(chatId, settingsScreen.Id);
        return true;
    });
    
    // Set parent for back navigation
    settingsScreen.WithParent(mainScreen);
}
```

## State Management System

The FluentTelegramUI framework includes a powerful state management system to help manage complex conversation flows and maintain stateful interactions with users:

```csharp
// Accessing state from the bot
bot.SetState(chatId, "username", "JohnDoe");
string username = bot.GetState<string>(chatId, "username");

// Setting conversation state
bot.SetCurrentState(chatId, "awaitingEmail");
if (bot.GetCurrentState(chatId) == "awaitingEmail")
{
    // Handle email input
}

// Clearing state
bot.ClearState(chatId);
```

### Multi-Step Forms

You can combine StateMachine with Screen navigation to create multi-step input forms:

```csharp
// Create screens for each step
var nameScreen = bot.CreateScreen("Name Input", new Message { Text = "Enter your name:" });
var emailScreen = bot.CreateScreen("Email Input", new Message { Text = "Enter your email:" });

// Set up text input handling based on state
nameScreen.OnTextInput("awaitingName", async (name) => 
{
    bot.SetState(chatId, "name", name);
    bot.SetCurrentState(chatId, "awaitingEmail");
    await bot.NavigateToScreenAsync(chatId, emailScreen.Id);
    return true;
});

emailScreen.OnTextInput("awaitingEmail", async (email) => 
{
    bot.SetState(chatId, "email", email);
    bot.SetCurrentState(chatId, "complete");
    // Show completion screen with collected data
    return true;
});
```

## Examples

### Creating a Simple Menu

```csharp
var menu = new MenuBuilder()
    .WithTitle("Main Menu")
    .WithStyle(FluentStyle.Modern)
    .AddButton("Profile", "profile")
    .AddButton("Settings", "settings")
    .AddButton("Help", "help")
    .Build();
```

### Creating a Card Layout

```csharp
var card = new CardBuilder()
    .WithTitle("Product Card")
    .WithDescription("This is a beautiful product card")
    .WithImage("product.jpg")
    .WithPrice("$99.99")
    .WithActionButton("Buy Now", "buy")
    .Build();
```

### Creating Interactive Screens

```csharp
// Create a counter screen
var counterScreen = bot.CreateScreen("Counter", new Message
{
    Text = "Current counter value: 0",
    ParseMarkdown = true
});

// Add controls and event handlers
counterScreen.AddControl(new ButtonGroup(new List<Button>
{
    new Button { Text = "Increment", CallbackData = "counter:increment" },
    new Button { Text = "Decrement", CallbackData = "counter:decrement" },
    new Button { Text = "Reset", CallbackData = "counter:reset" }
}, 2));

// Variable to track state
var counter = 0;

// Add event handlers
counterScreen.OnCallback("counter:increment", async (data) => 
{
    counter++;
    counterScreen.Content.Text = $"Current counter value: {counter}";
    return true; // Return true to refresh the screen
});
```

## Project Structure

The project is organized into several key namespaces:

- `FluentTelegramUI` - Core functionality and entry points
- `FluentTelegramUI.Builders` - Builder classes for creating UI components
- `FluentTelegramUI.Models` - Data models and entities
- `FluentTelegramUI.Handlers` - Update handlers and bot event processing

## TODO

### High Priority
- [x] Implement core Fluent UI components (Buttons, Cards, Menus)
- [x] Create basic Telegram bot integration
- [ ] Set up CI/CD pipeline
- [x] Add unit tests for core components
- [ ] Create NuGet package

### UI Components
- [x] Add support for custom themes (via FluentStyle)
- [x] Implement screen-based UI navigation
- [ ] Implement responsive grid layouts
- [ ] Create reusable animation components
- [x] Add support for custom fonts and styles
- [ ] Implement accessibility features

### Bot Features
- [x] Add support for inline keyboards
- [x] Implement conversation flow management via screens
- [x] Create state management system
- [x] Add support for media messages
- [x] Implement error handling and retry mechanisms
- [x] Add screen navigation with back functionality

### Documentation
- [ ] Create detailed API documentation
- [x] Add code examples
- [x] Create a getting started guide
- [ ] Add troubleshooting section
- [ ] Create contribution guidelines

### Performance & Security
- [ ] Implement caching system
- [ ] Add rate limiting support
- [ ] Implement secure token storage
- [ ] Add request validation
- [ ] Optimize message handling

## Recent Updates

- Added screen system with navigation and callback handling
- Implemented main screen functionality as entry point for users
- Added back navigation with customizable text and behavior
- Enhanced UI controls with event handling capabilities
- Reorganized handlers into a dedicated `Handlers` namespace
- Updated testing framework to focus on behavior rather than implementation details
- Renamed `FluentStyle.Material` to `FluentStyle.Modern` for better design alignment
- Improved error handling in the bot initialization process
- Enhanced test suite with more robust testing methods

## Contributing

We welcome contributions! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by Microsoft's Fluent Design System
- Built on top of the Telegram Bot API
- Special thanks to all contributors

## Support

If you encounter any issues or have questions, please open an issue in the GitHub repository. 