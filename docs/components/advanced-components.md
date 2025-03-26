---
layout: default
title: Advanced UI Components
parent: Components
nav_order: 2
has_children: true
---

# Advanced UI Components
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

FluentTelegramUI includes a rich set of advanced UI components that allow you to create sophisticated and interactive Telegram bot interfaces. These components go beyond basic buttons and text to provide a more engaging user experience.

![Advanced Components Overview](/assets/images/components/advanced-components.png)

## Available Components

| Component | Description | Usage |
|-----------|-------------|-------|
| [Toggle](toggle.html) | On/off switch for settings and features | `AddToggle()` |
| [Image Carousel](image-carousel.html) | Display multiple images with navigation | `AddImageCarousel()` |
| [Progress Indicator](progress-indicator.html) | Show completion status with a visual bar | `AddProgressIndicator()` |
| [Accordion](accordion.html) | Collapsible section to show/hide content | `AddAccordion()` |
| [Rich Text](rich-text.html) | Text with formatting and alignment options | `AddRichText()` |
| [Rating](rating.html) | Star-based rating system for feedback | `AddRating()` |

## Adding Components to Screens

All components can be added to screens using the ScreenBuilder's fluent interface:

```csharp
var screen = new ScreenBuilder(bot, "Advanced UI Demo")
    .WithContent("This screen demonstrates advanced UI components:")
    
    // Add components
    .AddToggle("Dark Mode", "dark_mode", false)
    .AddProgressIndicator("Download Progress", 75)
    .AddRichText("Important Information", isBold: true)
    .AddRating("Rate this example", "rate_example", 0)
    
    // Add navigation
    .AddNavigationButton("Back", "main_menu")
    .Build();
```

## Component Handlers

Several components require handlers to manage their interactive behavior. The ScreenBuilder provides convenient methods to set up these handlers:

```csharp
screenBuilder
    // Add toggle with handler
    .AddToggle("Dark Mode", "dark_mode", false)
    .WithToggleHandler("dark_mode")
    
    // Add carousel with handler
    .AddImageCarousel(imageUrls, captions)
    .WithCarouselHandler("gallery")
    
    // Add accordion with handler
    .AddAccordion("FAQ", "Frequently asked questions...", false)
    .WithAccordionHandler("faq");
```

These handler methods automatically create the necessary callback handlers for the component's interactive elements.

## Complete Example

Here's a comprehensive example showing all the advanced UI components working together:

```csharp
var advancedDemoScreen = new ScreenBuilder(bot, "Advanced UI Components")
    .WithContent("This screen demonstrates the advanced UI components:")
    
    // Add RichText header
    .AddRichText("Advanced UI Components Demo", 
        isBold: true, 
        alignment: TextAlignment.Center)
    
    // Add Toggle component
    .AddToggle("Dark Mode", "dark_mode", false)
    .WithToggleHandler("dark_mode")
    
    // Add Progress Indicator
    .AddProgressIndicator("Download Progress", 65)
    
    // Add Accordion
    .AddAccordion("What are these components?", 
        "These are advanced UI components that allow you to create more interactive and " +
        "engaging Telegram bot interfaces. They provide functionality beyond basic buttons " +
        "and text messages.", 
        false)
    .WithAccordionHandler("about_components")
    
    // Add Image Carousel
    .AddImageCarousel(
        new List<string> {
            "https://example.com/image1.jpg",
            "https://example.com/image2.jpg",
            "https://example.com/image3.jpg"
        },
        new List<string> {
            "Toggle Component",
            "Progress Indicator",
            "Rating Component"
        })
    .WithCarouselHandler("ui_examples")
    
    // Add Rating
    .AddRating("Rate these components", "rate_components", 0)
    
    // Handle rating submissions
    .OnCallback("rate_components:*", async (data, context) => {
        var rating = int.Parse(data.Split(':')[1]);
        Console.WriteLine($"User rated components: {rating} stars");
        
        // Update the UI to show the selected rating
        var ratingControl = advancedDemoScreen.Controls
            .OfType<Rating>()
            .FirstOrDefault(r => r.CallbackDataPrefix == "rate_components");
            
        if (ratingControl != null)
        {
            ratingControl.Value = rating;
            
            if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
            {
                await bot.NavigateToScreenAsync(chatId, advancedDemoScreen.Id);
            }
        }
        
        return true;
    })
    
    // Add navigation
    .AddNavigationButton("Back to Main Menu", "main_menu")
    .Build();

// Register the screen
bot.RegisterScreen(advancedDemoScreen);
```

## Best Practices

When using advanced UI components, keep these best practices in mind:

1. **Don't Overcrowd**: While these components add functionality, too many on a single screen can be overwhelming. Use them judiciously.

2. **Use Handlers**: Always set up the proper handlers for interactive components using the `With[Component]Handler()` methods.

3. **Consider Performance**: Complex screens with many components may take longer to render and update.

4. **Provide Clear Instructions**: If components require user interaction, make sure to provide clear instructions on how to use them.

5. **Test on Mobile**: Many Telegram users access bots on mobile devices, so test your UI on smaller screens.

6. **Accessibility**: Ensure your UI is accessible by providing clear labels and adequate contrast.

## Next Steps

Explore each component in detail by clicking on their names in the [Available Components](#available-components) table above. 