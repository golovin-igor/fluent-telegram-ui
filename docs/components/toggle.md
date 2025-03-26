---
layout: default
title: Toggle Component
parent: Components
nav_order: 3
---

# Toggle Component
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

The Toggle component provides a way to let users turn features on and off with a simple switch-like control. It's ideal for settings screens, preferences, and any feature that has two states (enabled/disabled).

![Toggle Component Preview](/assets/images/components/toggle.png)

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Label` | `string` | The label text displayed next to the toggle |
| `IsOn` | `bool` | Whether the toggle is currently in the ON position |
| `CallbackData` | `string` | The base callback data that will have `:on` or `:off` appended |
| `OnText` | `string` | Text to display when toggle is ON (default "ON") |
| `OffText` | `string` | Text to display when toggle is OFF (default "OFF") |
| `Style` | `FluentStyle` | The visual style to apply to the toggle |

## Usage

### Basic Usage

The simplest way to add a Toggle is through the ScreenBuilder:

```csharp
screenBuilder
    .AddToggle("Dark Mode", "dark_mode", false)
    .WithToggleHandler("dark_mode");
```

This creates a Toggle with the label "Dark Mode", a callback prefix of "dark_mode", and an initial state of OFF.

### Handling Toggle Events

When a toggle is switched, it generates callbacks with either `:on` or `:off` appended to the base callback data:

```csharp
screen.OnCallback("dark_mode:on", async (data, context) => {
    // The toggle was switched to ON
    Console.WriteLine("Dark mode enabled");
    return true;
});

screen.OnCallback("dark_mode:off", async (data, context) => {
    // The toggle was switched to OFF
    Console.WriteLine("Dark mode disabled");
    return true;
});
```

You can simplify this by using the `WithToggleHandler` extension method, which automatically sets up these callbacks:

```csharp
// This adds the toggle AND sets up the callback handlers
screenBuilder
    .AddToggle("Dark Mode", "dark_mode", false)
    .WithToggleHandler("dark_mode");
```

### Customizing Toggle Text

You can customize the text displayed when the toggle is ON or OFF:

```csharp
var toggle = new Toggle("Notifications", "notifications", true)
{
    OnText = "ENABLED",
    OffText = "DISABLED"
};

screen.AddControl(toggle);
```

With this configuration, the toggle will display "Notifications: ENABLED" when on and "Notifications: DISABLED" when off.

## Advanced Example

This example creates a settings screen with multiple toggles:

```csharp
var settingsScreen = new ScreenBuilder(bot, "Settings")
    .WithContent("Configure your preferences:")
    
    // Add multiple toggles
    .AddToggle("Dark Mode", "dark_mode", true)
    .WithToggleHandler("dark_mode")
    
    .AddToggle("Notifications", "notifications", true)
    .WithToggleHandler("notifications")
    
    .AddToggle("Sound Effects", "sound", false)
    .WithToggleHandler("sound")
    
    .AddToggle("Auto-save", "autosave", true)
    .WithToggleHandler("autosave")
    
    // Add a back button
    .AddNavigationButton("Back", "main_menu")
    .Build();

// Handle the toggle changes
settingsScreen.OnCallback("dark_mode:on", async (data, context) => {
    // Save the dark mode preference
    long userId = (long)context["userId"];
    userPreferences[userId]["darkMode"] = true;
    return true;
});

settingsScreen.OnCallback("dark_mode:off", async (data, context) => {
    // Save the dark mode preference
    long userId = (long)context["userId"];
    userPreferences[userId]["darkMode"] = false;
    return true;
});

// Register the screen
bot.RegisterScreen(settingsScreen);
```

## Implementation Details

The Toggle component is implemented as a class that derives from `UIControl`. When rendered, it creates a message with the toggle state and a button to switch the state.

When the button is pressed, the toggle state is reversed, and the screen is refreshed with the new state. 