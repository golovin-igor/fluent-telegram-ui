---
layout: default
title: Rating Component
parent: Components
nav_order: 8
---

# Rating Component
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

The Rating component provides a way for users to give feedback or ratings on a scale of 1 to 5 stars. This is useful for gathering user opinions, reviews, satisfaction levels, or any scenario where you need to collect user ratings.

![Rating Component Preview](/assets/images/components/rating.png)

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Label` | `string` | The descriptive label for the rating |
| `Value` | `int` | Current rating value (0-5, where 0 means not rated) |
| `CallbackDataPrefix` | `string` | The prefix for callback data that will have the rating value appended |
| `Style` | `FluentStyle` | The visual style to apply to the rating component |

## Usage

### Basic Usage

The simplest way to add a Rating component is through the ScreenBuilder:

```csharp
screenBuilder
    .AddRating("Rate our service", "rate_service", 0);
```

This creates a rating component with the label "Rate our service", a callback prefix of "rate_service", and an initial value of 0 (not rated).

### Handling Rating Submissions

When a user selects a rating value, a callback is generated with the rating value (1-5) appended to the callback prefix:

```csharp
screen.OnCallback("rate_service:*", async (data, context) => {
    // Extract the rating value from the callback data
    var rating = int.Parse(data.Split(':')[1]);
    
    // Do something with the rating
    Console.WriteLine($"User rated the service: {rating} stars");
    
    // Update the rating UI to show the selected value
    var ratingControl = screen.Controls.FirstOrDefault(c => c is Rating) as Rating;
    if (ratingControl != null)
    {
        ratingControl.Value = rating;
        
        // Refresh the screen to show the updated rating
        if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
        {
            await bot.NavigateToScreenAsync(chatId, screen.Id);
        }
    }
    
    return true;
});
```

### Pre-Populated Rating

You can create a rating component with an initial value already set:

```csharp
screenBuilder
    .AddRating("Rate our app", "rate_app", 4);
```

This creates a rating component with an initial value of 4 stars.

## Advanced Example

This example creates a feedback screen with multiple rating components:

```csharp
var feedbackScreen = new ScreenBuilder(bot, "Feedback")
    .WithContent("We value your feedback! Please rate the following aspects of our service:")
    
    // Add a title
    .AddRichText("Customer Satisfaction Survey", isBold: true, alignment: TextAlignment.Center)
    
    // Add multiple rating components
    .AddRating("Overall Experience", "rate_overall", 0)
    .AddRating("Customer Support", "rate_support", 0)
    .AddRating("Product Quality", "rate_quality", 0)
    .AddRating("Value for Money", "rate_value", 0)
    
    // Add a comment section
    .AddRichText("Additional Comments:", isBold: true)
    .AddTextInput("comments", "Enter your comments here...")
    
    // Add a submit button
    .AddButton("Submit Feedback", "submit_feedback")
    
    // Add a navigation button
    .AddNavigationButton("Cancel", "main_menu")
    .Build();

// Handle rating callbacks
string[] ratingCategories = { "overall", "support", "quality", "value" };
foreach (var category in ratingCategories)
{
    // Use a closure to capture the category
    var currentCategory = category;
    
    feedbackScreen.OnCallback($"rate_{currentCategory}:*", async (data, context) => {
        // Extract the rating value
        var rating = int.Parse(data.Split(':')[1]);
        
        // Store the rating in the user's state
        if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
        {
            // Store the rating in the state machine
            bot.SetState(chatId, $"rating_{currentCategory}", rating);
            
            // Update the UI
            var ratingControl = feedbackScreen.Controls
                .OfType<Rating>()
                .FirstOrDefault(r => r.CallbackDataPrefix == $"rate_{currentCategory}");
                
            if (ratingControl != null)
            {
                ratingControl.Value = rating;
                await bot.NavigateToScreenAsync(chatId, feedbackScreen.Id);
            }
        }
        
        return true;
    });
}

// Handle submit button
feedbackScreen.OnCallback("submit_feedback", async (data, context) => {
    if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
    {
        // Collect all the ratings from state
        var overallRating = bot.GetState<int>(chatId, "rating_overall", 0);
        var supportRating = bot.GetState<int>(chatId, "rating_support", 0);
        var qualityRating = bot.GetState<int>(chatId, "rating_quality", 0);
        var valueRating = bot.GetState<int>(chatId, "rating_value", 0);
        
        // Calculate average rating
        var averageRating = (overallRating + supportRating + qualityRating + valueRating) / 4.0;
        
        // Send a thank you message
        await bot.SendMessageAsync(chatId, new Message
        {
            Text = $"Thank you for your feedback! Your average rating: {averageRating:F1} stars.",
            ParseMarkdown = true
        });
        
        // Clear the state and go back to main menu
        bot.ClearState(chatId);
        await bot.NavigateToScreenAsync(chatId, "main_menu");
    }
    
    return true;
});

// Register the screen
bot.RegisterScreen(feedbackScreen);
```

## Rating Analytics

You can use rating components to gather analytics about user satisfaction:

```csharp
// Dictionary to track all ratings
var ratings = new Dictionary<string, List<int>>
{
    {"overall", new List<int>()},
    {"support", new List<int>()},
    {"quality", new List<int>()},
    {"value", new List<int>()}
};

// In the rating callbacks:
feedbackScreen.OnCallback("rate_overall:*", async (data, context) => {
    var rating = int.Parse(data.Split(':')[1]);
    
    // Add to our analytics tracker
    ratings["overall"].Add(rating);
    
    // Update the UI
    // ...
    
    return true;
});

// To generate statistics:
public Dictionary<string, double> CalculateAverageRatings()
{
    var averages = new Dictionary<string, double>();
    
    foreach (var category in ratings.Keys)
    {
        if (ratings[category].Count > 0)
        {
            averages[category] = ratings[category].Average();
        }
        else
        {
            averages[category] = 0;
        }
    }
    
    return averages;
}
```

## Implementation Details

The Rating component is implemented as a class that derives from `UIControl`. When rendered, it creates a message with the current rating displayed as stars (⭐) and a row of buttons numbered 1 through 5 for the user to select a rating.

If the rating is 0 (not rated), it displays "Not rated" instead of stars. Once a rating is selected, it displays the corresponding number of star emojis.

The component automatically arranges the 5 rating buttons in a single row for an intuitive rating experience. 