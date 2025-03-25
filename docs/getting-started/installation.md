---
layout: default
title: Installation
parent: Getting Started
nav_order: 1
---

# Installation

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

## Prerequisites

Before installing FluentTelegramUI, make sure you have the following:

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later installed
- A Telegram Bot Token (obtained from [@BotFather](https://t.me/botfather))
- Basic familiarity with C# and .NET development

## Installing via NuGet Package Manager

The easiest way to install FluentTelegramUI is via the NuGet Package Manager:

1. Open your project in Visual Studio or your preferred IDE
2. Open the NuGet Package Manager
3. Search for "FluentTelegramUI"
4. Click "Install"

Alternatively, you can install it via the Package Manager Console:

```
PM> Install-Package FluentTelegramUI
```

## Installing via .NET CLI

If you prefer using the .NET CLI, you can install FluentTelegramUI with this command:

```bash
dotnet add package FluentTelegramUI
```

## Manual Installation

You can also download the source code and build it yourself:

1. Clone the repository:
   ```bash
   git clone https://github.com/ORIGINAL-OWNER/fluent-telegram-ui.git
   ```

2. Build the project:
   ```bash
   cd fluent-telegram-ui
   dotnet build
   ```

3. Reference the built DLL in your project

## Adding the Reference to Your Project

After installation, add the following using statements to your C# files:

```csharp
using FluentTelegramUI;
using FluentTelegramUI.Models;
using FluentTelegramUI.Handlers;
```

## Dependencies

FluentTelegramUI relies on the following packages, which will be automatically installed:

- Telegram.Bot (latest stable version)
- Microsoft.Extensions.DependencyInjection (7.0.0 or later)
- Microsoft.Extensions.Logging (7.0.0 or later)

## Verifying Installation

To verify that the installation was successful, create a simple test project:

```csharp
using FluentTelegramUI;
using FluentTelegramUI.Models;
using System;

class Program
{
    static void Main(string[] args)
    {
        // Create a new bot instance
        var bot = new TelegramBotBuilder()
            .WithToken("YOUR_BOT_TOKEN")
            .WithFluentUI()
            .Build();
            
        Console.WriteLine("Bot created successfully!");
    }
}
```

## Next Steps

Now that you've installed FluentTelegramUI, check out the [Quick Start Guide](quick-start.md) to create your first bot! 