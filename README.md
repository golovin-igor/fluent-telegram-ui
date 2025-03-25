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
    .WithStyle(FluentStyle.Material)
    .WithButton("Click Me", "callback_data")
    .Build();

await bot.SendMessageAsync(message);
```

## Examples

### Creating a Simple Menu

```csharp
var menu = new MenuBuilder()
    .WithTitle("Main Menu")
    .WithStyle(FluentStyle.Material)
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