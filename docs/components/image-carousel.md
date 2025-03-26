---
layout: default
title: Image Carousel
parent: Components
nav_order: 4
---

# Image Carousel
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

The Image Carousel component allows you to display a series of images with navigation controls, making it easy for users to browse through multiple images. It's perfect for product galleries, photo albums, tutorials, and any situation where you need to showcase multiple images.

![Image Carousel Preview](/assets/images/components/image-carousel.png)

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `ImageUrls` | `List<string>` | List of URLs for the images to display |
| `Captions` | `List<string>` | List of captions to show with each image |
| `CurrentIndex` | `int` | The index of the currently displayed image |
| `Style` | `FluentStyle` | The visual style to apply to the carousel |

## Usage

### Basic Usage

The simplest way to add an Image Carousel is through the ScreenBuilder:

```csharp
var imageUrls = new List<string>
{
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg",
    "https://example.com/image3.jpg"
};

var captions = new List<string>
{
    "Beautiful sunset over the mountains",
    "Ancient temple hidden in the forest",
    "Coastal view with crashing waves"
};

screenBuilder
    .AddImageCarousel(imageUrls, captions)
    .WithCarouselHandler("gallery");
```

This creates a carousel with three images and their corresponding captions. The `WithCarouselHandler` method sets up the navigation callbacks automatically.

### Handling Navigation Events

When a user navigates through the carousel, it generates callbacks with either `:prev` or `:next` appended to the base callback data:

```csharp
// These handlers are set up automatically by WithCarouselHandler
screen.OnCallback("carousel:gallery:prev", async (data, state) => {
    // Handle navigation to the previous image
    var carouselControl = screen.Controls.FirstOrDefault(c => c.Id == "gallery") as ImageCarousel;
    if (carouselControl != null && carouselControl.CurrentIndex > 0)
    {
        carouselControl.CurrentIndex--;
        // Refresh the screen...
    }
    return true;
});

screen.OnCallback("carousel:gallery:next", async (data, state) => {
    // Handle navigation to the next image
    var carouselControl = screen.Controls.FirstOrDefault(c => c.Id == "gallery") as ImageCarousel;
    if (carouselControl != null && carouselControl.CurrentIndex < carouselControl.ImageUrls.Count - 1)
    {
        carouselControl.CurrentIndex++;
        // Refresh the screen...
    }
    return true;
});
```

### Carousel Without Captions

You can create a carousel without captions by omitting the captions parameter:

```csharp
var imageUrls = new List<string>
{
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg",
    "https://example.com/image3.jpg"
};

screenBuilder
    .AddImageCarousel(imageUrls)
    .WithCarouselHandler("gallery");
```

In this case, empty strings will be used for captions.

## Advanced Example

This example creates a photo gallery screen with a carousel:

```csharp
var galleryScreen = new ScreenBuilder(bot, "Photo Gallery")
    .WithContent("Browse through our photo collection:")
    
    // Add rich text as a header
    .AddRichText("Nature Photography Collection", isBold: true, alignment: TextAlignment.Center)
    
    // Add the image carousel
    .AddImageCarousel(
        new List<string> {
            "https://example.com/nature1.jpg",
            "https://example.com/nature2.jpg",
            "https://example.com/nature3.jpg",
            "https://example.com/nature4.jpg",
            "https://example.com/nature5.jpg"
        },
        new List<string> {
            "Mountain sunrise in the Alps",
            "Lush rainforest in the Amazon",
            "Desert landscape at sunset",
            "Arctic ice formations",
            "Waterfall in a tropical forest"
        })
    .WithCarouselHandler("nature_photos")
    
    // Add an action button
    .AddButton("Download Current Photo", "download_photo")
    
    // Add a navigation button
    .AddNavigationButton("Back to Gallery Menu", "gallery_menu")
    .Build();

// Handler for the download button
galleryScreen.OnCallback("download_photo", async (data, context) => {
    if (context.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
    {
        // Find the carousel to get the current image URL
        var carousel = galleryScreen.Controls.FirstOrDefault(c => c is ImageCarousel) as ImageCarousel;
        if (carousel != null)
        {
            var currentImageUrl = carousel.ImageUrls[carousel.CurrentIndex];
            var caption = carousel.Captions[carousel.CurrentIndex];
            
            // Send the image as a document
            await bot.SendDocumentAsync(chatId, new InputFileUrl(currentImageUrl), 
                caption: $"Here's your download: {caption}");
        }
    }
    return true;
});

// Register the screen
bot.RegisterScreen(galleryScreen);
```

## Implementation Details

The Image Carousel component is implemented as a class that derives from `UIControl`. When rendered, it displays the current image with navigation buttons.

The carousel's navigation state is maintained across interactions. When a user presses the "Next" or "Previous" button, the `CurrentIndex` property is updated, and the screen is refreshed to show the newly selected image.

The navigation buttons are only shown when needed - the "Previous" button is hidden when on the first image, and the "Next" button is hidden when on the last image. A counter showing the current position (e.g., "2/5") is always displayed. 