---
layout: default
title: Accordion Component
parent: Components
nav_order: 6
---

# Accordion Component
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

The Accordion component (also known as a collapsible section) provides a way to show or hide content, making your interface more compact and organized. This is particularly useful for FAQs, terms of service, help sections, or any content that would be too lengthy to display all at once.

![Accordion Component Preview](/assets/images/components/accordion.png)

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | The title/header text that always remains visible |
| `Content` | `string` | The content text that is shown/hidden when expanded/collapsed |
| `IsExpanded` | `bool` | Whether the accordion section is currently expanded |
| `Style` | `FluentStyle` | The visual style to apply to the accordion |

## Usage

### Basic Usage

The simplest way to add an Accordion is through the ScreenBuilder:

```csharp
screenBuilder
    .AddAccordion("Frequently Asked Questions", 
        "1. How do I use this bot?\n" +
        "2. How do I change settings?\n" +
        "3. How do I contact support?", 
        false)
    .WithAccordionHandler("faq");
```

This creates a collapsed accordion with the title "Frequently Asked Questions" and content listing three questions. The `WithAccordionHandler` method automatically sets up the callback handlers for expanding and collapsing.

### Handling Expand/Collapse Events

When a user clicks the expand or collapse button, callbacks are generated with either `:expand` or `:collapse` appended to the accordion ID:

```csharp
screen.OnCallback("accordion:faq:expand", async (data, context) => {
    // Handle expanding the accordion
    var accordionControl = screen.Controls.FirstOrDefault(c => c.Id == "faq") as Accordion;
    if (accordionControl != null)
    {
        accordionControl.IsExpanded = true;
        // Refresh the screen...
    }
    return true;
});

screen.OnCallback("accordion:faq:collapse", async (data, context) => {
    // Handle collapsing the accordion
    var accordionControl = screen.Controls.FirstOrDefault(c => c.Id == "faq") as Accordion;
    if (accordionControl != null)
    {
        accordionControl.IsExpanded = false;
        // Refresh the screen...
    }
    return true;
});
```

The `WithAccordionHandler` method sets up these callbacks automatically.

### Initially Expanded Accordion

You can create an accordion that starts in the expanded state:

```csharp
screenBuilder
    .AddAccordion("Important Announcement", "Please read this important information...", true)
    .WithAccordionHandler("announcement");
```

The third parameter (`true`) indicates that the accordion should initially be expanded.

## Advanced Example

This example creates a help screen with multiple accordions:

```csharp
var helpScreen = new ScreenBuilder(bot, "Help Center")
    .WithContent("Welcome to the Help Center. Tap a section to expand it:")
    
    // Add a subtitle
    .AddRichText("Common Questions & Answers", isBold: true, alignment: TextAlignment.Center)
    
    // Add multiple accordions
    .AddAccordion("Getting Started", 
        "1. Create an account by clicking 'Register'\n" +
        "2. Complete your profile\n" +
        "3. Start exploring our features", 
        false)
    .WithAccordionHandler("getting_started")
    
    .AddAccordion("Account Settings", 
        "• To change your password, go to Settings > Security\n" +
        "• To update your email, go to Settings > Profile\n" +
        "• To manage notifications, go to Settings > Notifications", 
        false)
    .WithAccordionHandler("account_settings")
    
    .AddAccordion("Troubleshooting", 
        "If you're experiencing issues:\n\n" +
        "1. Make sure you have the latest app version\n" +
        "2. Try restarting the app\n" +
        "3. Check your internet connection\n" +
        "4. Contact support if the issue persists", 
        false)
    .WithAccordionHandler("troubleshooting")
    
    .AddAccordion("Contact Support", 
        "Email: support@example.com\n" +
        "Phone: 555-123-4567\n" +
        "Hours: Monday-Friday, 9am-5pm EST", 
        false)
    .WithAccordionHandler("contact_support")
    
    // Add a navigation button
    .AddNavigationButton("Back to Main Menu", "main_menu")
    .Build();

// Register the screen
bot.RegisterScreen(helpScreen);
```

## Data Tracking Example

You can use accordions to track which help topics users are most interested in:

```csharp
// Dictionary to track section opens by users
var topicInterest = new Dictionary<string, int>
{
    {"getting_started", 0},
    {"account_settings", 0},
    {"troubleshooting", 0},
    {"contact_support", 0}
};

helpScreen.OnCallback("accordion:getting_started:expand", async (data, context) => {
    // Increment the counter for this topic
    topicInterest["getting_started"]++;
    
    // Find and expand the accordion
    var accordion = helpScreen.Controls.FirstOrDefault(c => c.Id == "getting_started") as Accordion;
    if (accordion != null)
    {
        accordion.IsExpanded = true;
        
        if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
        {
            await bot.NavigateToScreenAsync(chatId, helpScreen.Id);
        }
    }
    return true;
});

// Add similar handlers for other topics
```

## Implementation Details

The Accordion component is implemented as a class that derives from `UIControl`. When rendered, it creates a message with the title always visible and the content shown or hidden based on the `IsExpanded` property.

When expanded, the message shows both the title and content with a "▼ Collapse" button. When collapsed, only the title is shown with a "▶ Expand" button.

The rendering is done using Markdown to properly format the title (in bold) and content. 