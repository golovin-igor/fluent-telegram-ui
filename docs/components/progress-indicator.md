---
layout: default
title: Progress Indicator
parent: Components
nav_order: 5
---

# Progress Indicator
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

The Progress Indicator component allows you to display completion status visually with a text-based progress bar. This is useful for showing download progress, task completion, or any process that has a measurable completion percentage.

![Progress Indicator Preview](/assets/images/components/progress-indicator.png)

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Label` | `string` | The descriptive label for the progress bar |
| `Progress` | `int` | Current progress value (0-100) |
| `ShowPercentage` | `bool` | Whether to display the percentage number |
| `FilledChar` | `string` | Character used for the filled portion of the bar (default "█") |
| `EmptyChar` | `string` | Character used for the empty portion of the bar (default "░") |
| `Width` | `int` | Total width of the progress bar in characters (default 10) |
| `Style` | `FluentStyle` | The visual style to apply to the progress indicator |

## Usage

### Basic Usage

The simplest way to add a Progress Indicator is through the ScreenBuilder:

```csharp
screenBuilder
    .AddProgressIndicator("Download Progress", 25);
```

This creates a progress indicator with the label "Download Progress" and sets it to 25% completion.

### Customizing Appearance

You can customize the appearance of the progress bar by setting its properties:

```csharp
var progressIndicator = new ProgressIndicator("File Upload", 60)
{
    FilledChar = "■",
    EmptyChar = "□",
    Width = 20,
    ShowPercentage = true
};

screen.AddControl(progressIndicator);
```

This creates a wider progress bar with different characters and explicitly shows the percentage.

### Progress Bar Without Percentage

If you want a clean look without the percentage number, you can disable it:

```csharp
screenBuilder
    .AddProgressIndicator("Installation Progress", 80)
    .Build()
    .Controls.OfType<ProgressIndicator>()
    .First()
    .ShowPercentage = false;
```

## Advanced Example

This example creates a download progress screen with multiple progress indicators:

```csharp
var downloadScreen = new ScreenBuilder(bot, "Downloads")
    .WithContent("Current download status:")
    
    // Add a title
    .AddRichText("File Downloads in Progress", isBold: true, alignment: TextAlignment.Center)
    
    // Add multiple progress indicators
    .AddProgressIndicator("System Update", 15)
    .AddProgressIndicator("Media Files", 42)
    .AddProgressIndicator("Document Package", 78)
    .AddProgressIndicator("Backup Process", 100)
    
    // Add a refresh button
    .AddButton("Refresh Status", "refresh_status")
    
    // Add a navigation button
    .AddNavigationButton("Back to Dashboard", "dashboard")
    .Build();

// Handler for the refresh button
downloadScreen.OnCallback("refresh_status", async (data, context) => {
    if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
    {
        // Update the progress values
        var rnd = new Random();
        
        foreach (var control in downloadScreen.Controls)
        {
            if (control is ProgressIndicator indicator && indicator.Progress < 100)
            {
                // Simulate progress update
                indicator.Progress = Math.Min(indicator.Progress + rnd.Next(5, 15), 100);
            }
        }
        
        // Refresh the screen with new progress values
        await bot.NavigateToScreenAsync(chatId, downloadScreen.Id);
    }
    return true;
});

// Register the screen
bot.RegisterScreen(downloadScreen);
```

## Implementation Details

The Progress Indicator component is implemented as a class that derives from `UIControl`. When rendered, it creates a message with a visual representation of progress using text characters.

The progress bar is constructed by using a filled character (█ by default) for the completed portion and an empty character (░ by default) for the remaining portion. The width property determines how many characters wide the progress bar should be.

When the `ShowPercentage` property is true, the component also displays the numerical percentage value (e.g., "25%") next to the progress bar. 