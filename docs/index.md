---
layout: default
title: Home
nav_order: 1
description: "FluentTelegramUI - A modern, fluent-style UI framework for creating Telegram bots with .NET"
permalink: /
---

# FluentTelegramUI
{: .fs-9 }

A modern, fluent-style UI framework for creating beautiful and interactive Telegram bots with C# .NET.
{: .fs-6 .fw-300 }

[Get Started](getting-started){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 }
[View it on GitHub](https://github.com/yourusername/fluent-telegram-ui){: .btn .fs-5 .mb-4 .mb-md-0 }

---

## Overview

FluentTelegramUI is a comprehensive framework for building Telegram bots with a focus on beautiful user interfaces and clean, maintainable code. Inspired by Microsoft's Fluent Design System, it brings a modern, consistent, and sophisticated look to your Telegram bots.

### Key Features

- 🎨 **Fluent Design System-inspired UI** - Beautiful, modern interfaces
- 🔧 **Easy-to-use builder pattern** - Create complex UIs with minimal code
- 📱 **Responsive and adaptive layouts** - Works great on all devices
- 🎛️ **Advanced UI components** - Toggles, carousels, rating systems, and more
- 🧠 **Integrated state machine** - Manage complex conversation flows
- 🔄 **Screen-based navigation** - Create multi-screen experiences with back functionality
- 📦 **NuGet package** - Easy integration into your projects

### What's New

We've recently added several advanced UI components:

- **Toggle/Switch Component** - For enabling/disabling features
- **Image Carousel** - For browsing through multiple images
- **Progress Indicator** - For showing completion status
- **Accordion/Collapsible Sections** - For hiding/showing content
- **Rich Text** - For text with advanced formatting
- **Rating System** - For collecting user feedback

## Getting Started

FluentTelegramUI is designed to be easy to use, with a minimal learning curve. Here's a quick example to get you started:

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

// Create a screen with UI components
var mainScreen = new ScreenBuilder(bot, "Main Menu")
    .WithContent("Welcome to my bot!")
    .AddButton("Settings", "open_settings")
    .AddToggle("Dark Mode", "dark_mode", false)
    .WithToggleHandler("dark_mode")
    .AddRichText("Important Information", isBold: true, alignment: TextAlignment.Center)
    .Build();

// Register the screen
bot.RegisterScreen(mainScreen, isMainScreen: true);
```

See the [Getting Started](getting-started) guide for more details.

## Why FluentTelegramUI?

Traditional Telegram bot frameworks focus primarily on handling commands and callbacks, with minimal support for creating rich user interfaces. FluentTelegramUI takes a different approach by treating the UI as a first-class citizen.

Benefits include:

- 🚀 **Faster Development** - Build sophisticated bots in less time
- 🎯 **Improved User Experience** - Create interfaces users love to interact with
- 🧩 **Component-Based Architecture** - Reuse UI elements across your bot
- 🔍 **Clear, Readable Code** - Understand what your UI will look like from the code
- 🛠️ **Built-In Best Practices** - Follow Telegram bot design patterns without extra effort

## Documentation

Comprehensive documentation is available:

- [Getting Started](getting-started) - Set up your environment and build your first bot
- [Components](components) - Explore available UI components
- [Advanced Topics](advanced) - Dive into state management, navigation, and more
- [API Reference](api) - Detailed API documentation
- [Examples](examples) - Real-world examples to learn from

## Community and Contributions

This is an open-source project, and we welcome contributions! Check out our [Contributing Guide](contributing) to get started.

## License

- [Messages](components/messages.md)
- [Buttons](components/buttons.md)
- [Screens](components/screens.md)
- [Navigation](components/navigation.md)
- [Context Parameters](components/context-parameters.md)

## Advanced Topics

- [State Management](advanced/state-management.md)
- [Custom Controls](advanced/custom-controls.md)
- [Handling Callbacks](advanced/handling-callbacks.md)
- [Error Handling](advanced/error-handling.md)
- [Migrating from Telegram.Bot](advanced/migrating.md)

## Examples

- [Basic Bot](examples/basic-bot.md)
- [Multi-screen Navigation](examples/multi-screen.md)
- [Form Input](examples/form-input.md)
- [Context Usage Examples](examples/context-usage.md)

## API Reference

- [FluentTelegramBot](api/fluent-telegram-bot.md)
- [Screen](api/screen.md)
- [Message](api/message.md)
- [Button](api/button.md)
- [ScreenManager](api/screen-manager.md)
- [StateMachine](api/state-machine.md)

## Contributing

- [How to Contribute](contributing/how-to-contribute.md)
- [Development Setup](contributing/development-setup.md)
- [Coding Standards](contributing/coding-standards.md)
- [Pull Request Process](contributing/pull-request-process.md)

## Support and Community

- [GitHub Discussions](https://github.com/ORIGINAL-OWNER/fluent-telegram-ui/discussions)
- [Issue Tracker](https://github.com/ORIGINAL-OWNER/fluent-telegram-ui/issues) 