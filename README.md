# Fluent Telegram UI

A modern, fluent-style UI framework for creating Telegram bots using C# .NET. This project aims to provide an intuitive and elegant way to build Telegram bots with a focus on clean, maintainable code and beautiful user interfaces.

## Features

- 🎨 Fluent Design System-inspired UI components
- 🔧 Easy-to-use builder pattern for creating bot interactions
- 📱 Responsive and adaptive layouts
- 🎯 Built-in support for common Telegram bot patterns
- 🚀 High-performance and scalable architecture
- 📦 NuGet package available for easy integration

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
- [ ] Implement responsive grid layouts
- [ ] Create reusable animation components
- [x] Add support for custom fonts and styles
- [ ] Implement accessibility features

### Bot Features
- [x] Add support for inline keyboards
- [ ] Implement conversation flow management
- [ ] Create state management system
- [x] Add support for media messages
- [x] Implement error handling and retry mechanisms

### Documentation
- [ ] Create detailed API documentation
- [x] Add code examples
- [ ] Create a getting started guide
- [ ] Add troubleshooting section
- [ ] Create contribution guidelines

### Performance & Security
- [ ] Implement caching system
- [ ] Add rate limiting support
- [ ] Implement secure token storage
- [ ] Add request validation
- [ ] Optimize message handling

## Recent Updates

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