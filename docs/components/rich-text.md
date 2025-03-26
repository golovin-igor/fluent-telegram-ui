---
layout: default
title: Rich Text Component
parent: Components
nav_order: 7
---

# Rich Text Component
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

The Rich Text component allows you to display formatted text with various styling options in your Telegram bot interface. It supports bold, italic, and underlined text, as well as different text alignments, enabling you to create more visually appealing and organized content.

![Rich Text Component Preview](/assets/images/components/rich-text.png)

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | The text content to display |
| `IsBold` | `bool` | Whether to apply bold formatting |
| `IsItalic` | `bool` | Whether to apply italic formatting |
| `IsUnderlined` | `bool` | Whether to apply underline formatting |
| `Alignment` | `TextAlignment` | The text alignment (Left, Center, or Right) |
| `Style` | `FluentStyle` | The visual style to apply to the text |

## Usage

### Basic Usage

The simplest way to add Rich Text is through the ScreenBuilder:

```csharp
screenBuilder
    .AddRichText("Important information", isBold: true);
```

This creates a Rich Text component with the text "Important information" in bold.

### Combining Formatting Options

You can combine different formatting options:

```csharp
screenBuilder
    .AddRichText("Critical Alert", 
        isBold: true, 
        isItalic: true, 
        isUnderlined: true);
```

This creates text that has bold, italic, and underline formatting all applied.

### Text Alignment

You can control the alignment of the text:

```csharp
// Center-aligned text
screenBuilder
    .AddRichText("Centered Title", 
        isBold: true, 
        alignment: TextAlignment.Center);

// Right-aligned text
screenBuilder
    .AddRichText("Right-aligned Info", 
        alignment: TextAlignment.Right);
```

These create text that is centered or right-aligned within the message.

## Advanced Example

This example creates a screen with various rich text formatting options:

```csharp
var formattingExampleScreen = new ScreenBuilder(bot, "Text Formatting")
    .WithContent("Below are examples of rich text formatting:")
    
    // Headings
    .AddRichText("Title: Large and Bold", isBold: true, alignment: TextAlignment.Center)
    
    // Paragraph with emphasis
    .AddRichText("This is a paragraph with some words in italic for emphasis. " +
                "This helps to draw attention to important concepts or terms.", isItalic: false)
    
    // Important information
    .AddRichText("⚠️ IMPORTANT NOTICE ⚠️", isBold: true, isUnderlined: true, alignment: TextAlignment.Center)
    
    // Quoted text (right-aligned)
    .AddRichText("\"The measure of intelligence is the ability to change.\"", alignment: TextAlignment.Right)
    .AddRichText("- Albert Einstein", isItalic: true, alignment: TextAlignment.Right)
    
    // Mixed formatting in the same text
    .AddRichText("You can combine bold and italic formatting in the same text when needed.", 
        isBold: true, isItalic: true)
    
    // List with left alignment
    .AddRichText("Key Features:", isBold: true)
    .AddRichText("• Simple to use\n• Powerful formatting\n• Multiple alignment options")
    
    // Navigation
    .AddNavigationButton("Back to Examples", "formatting_examples")
    .Build();

// Register the screen
bot.RegisterScreen(formattingExampleScreen);
```

## Text with Multiple Paragraphs

For longer texts with multiple paragraphs, you can use the newline character (\n):

```csharp
screenBuilder
    .AddRichText("First Paragraph: This is the introduction to the topic.\n\n" +
                "Second Paragraph: This continues the discussion with more details.\n\n" +
                "Third Paragraph: This concludes the topic with a summary.",
                alignment: TextAlignment.Left);
```

## Creating Documentation

Rich Text is perfect for creating documentation or help sections:

```csharp
var helpScreen = new ScreenBuilder(bot, "Help")
    .AddRichText("User Guide", isBold: true, alignment: TextAlignment.Center)
    
    .AddRichText("Getting Started:", isBold: true)
    .AddRichText("1. Create an account\n2. Set up your profile\n3. Start using features")
    
    .AddRichText("Common Commands:", isBold: true)
    .AddRichText("/start - Begin the bot interaction\n" +
                "/help - Show this help screen\n" +
                "/settings - Configure your preferences")
    
    .AddNavigationButton("Back", "main_menu")
    .Build();

// Register the screen
bot.RegisterScreen(helpScreen);
```

## Implementation Details

The Rich Text component is implemented as a class that derives from `UIControl`. When rendered, it applies Markdown formatting to the text based on the active formatting options:

- Bold text is wrapped with asterisks: `*bold text*`
- Italic text is wrapped with underscores: `_italic text_`
- Underlined text is wrapped with double underscores: `__underlined text__`

For text alignment:
- Left alignment is the default and requires no special formatting
- Center alignment adds spaces before and after each line
- Right alignment adds more spaces before each line to push the text to the right

The formatting is applied in a consistent order: bold first, then italic, then underline. This ensures that the formatting is properly nested and works reliably in Telegram's Markdown implementation. 