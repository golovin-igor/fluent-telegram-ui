using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentTelegramUI.Models;
using FluentTelegramUI.Builders;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FluentTelegramUI.Examples
{
    /// <summary>
    /// Example of advanced UI components usage
    /// </summary>
    public class AdvancedUIComponentsExample
    {
        /// <summary>
        /// Run the example
        /// </summary>
        /// <param name="token">The Telegram bot token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task Run(string token)
        {
            // Create bot with fluent interface
            var bot = new TelegramBotBuilder()
                .WithToken(token)
                .WithFluentUI(FluentStyle.Modern)
                .AddScreen("Advanced UI Components", ConfigureAdvancedComponentsScreen, isMainScreen: true)
                .AddScreen("Toggle Example", ConfigureToggleScreen)
                .AddScreen("Carousel Example", ConfigureCarouselScreen)
                .AddScreen("Progress Example", ConfigureProgressScreen)
                .AddScreen("Accordion Example", ConfigureAccordionScreen)
                .AddScreen("Rich Text Example", ConfigureRichTextScreen)
                .AddScreen("Rating Example", ConfigureRatingScreen)
                .WithAutoStartReceiving()
                .Build();
            
            // Wait for user to press Enter
            Console.WriteLine("Press Enter to stop the bot");
            Console.ReadLine();
            
            // Stop receiving updates
            bot.StopReceiving();
            
            // Return completed task
            await Task.CompletedTask;
        }

        /// <summary>
        /// Configure the main advanced components screen
        /// </summary>
        /// <param name="screen">The screen builder</param>
        private static void ConfigureAdvancedComponentsScreen(ScreenBuilder screen)
        {
            screen.WithContent("Welcome to the Advanced UI Components Demo!\n\nSelect a component to see its demo:")
                .AddNavigationButton("Toggle Switch", "toggle-example")
                .AddNavigationButton("Image Carousel", "carousel-example")
                .AddNavigationButton("Progress Indicator", "progress-example")
                .AddNavigationButton("Accordion", "accordion-example")
                .AddNavigationButton("Rich Text", "rich-text-example")
                .AddNavigationButton("Rating", "rating-example")
                .WithButtonsPerRow(2);
        }

        /// <summary>
        /// Configure the toggle example screen
        /// </summary>
        /// <param name="screen">The screen builder</param>
        private static void ConfigureToggleScreen(ScreenBuilder screen)
        {
            var toggleId = "settings-toggle";
            
            screen.WithContent("Toggle Switch Example")
                .AddRichText("Toggles let users switch features on and off", isBold: true)
                .AddToggle("Dark Mode", toggleId, false)
                .WithToggleHandler(toggleId)
                .AddToggle("Notifications", "notification-toggle", true)
                .WithToggleHandler("notification-toggle")
                .AddToggle("Auto-Reply", "auto-reply-toggle", false)
                .WithToggleHandler("auto-reply-toggle")
                .AddNavigationButton("Back to Menu", "advanced-ui-components");
        }

        /// <summary>
        /// Configure the carousel example screen
        /// </summary>
        /// <param name="screen">The screen builder</param>
        private static void ConfigureCarouselScreen(ScreenBuilder screen)
        {
            var carouselId = "nature-carousel";
            var imageUrls = new List<string>
            {
                "https://images.unsplash.com/photo-1470071459604-3b5ec3a7fe05",
                "https://images.unsplash.com/photo-1447752875215-b2761acb3c5d",
                "https://images.unsplash.com/photo-1472214103451-9374bd1c798e",
                "https://images.unsplash.com/photo-1501854140801-50d01698950b"
            };
            
            var captions = new List<string>
            {
                "Beautiful sunset over the mountains",
                "Majestic tree in the forest",
                "Calm lake reflecting the sky",
                "Peaceful mountain range"
            };
            
            screen.WithContent("Image Carousel Example")
                .AddRichText("Carousels let users browse through multiple images", isBold: true)
                .AddImageCarousel(imageUrls, captions)
                .WithCarouselHandler(carouselId)
                .AddNavigationButton("Back to Menu", "advanced-ui-components");
        }

        /// <summary>
        /// Configure the progress example screen
        /// </summary>
        /// <param name="screen">The screen builder</param>
        private static void ConfigureProgressScreen(ScreenBuilder screen)
        {
            screen.WithContent("Progress Indicator Example")
                .AddRichText("Progress indicators show completion status", isBold: true)
                .AddProgressIndicator("Download", 25)
                .AddProgressIndicator("Upload", 50)
                .AddProgressIndicator("Processing", 75)
                .AddProgressIndicator("Completed", 100)
                .OnCallback("update-progress", async (data, state) =>
                {
                    // In a real app, you would update the progress here
                    return true;
                })
                .AddNavigationButton("Back to Menu", "advanced-ui-components");
        }

        /// <summary>
        /// Configure the accordion example screen
        /// </summary>
        /// <param name="screen">The screen builder</param>
        private static void ConfigureAccordionScreen(ScreenBuilder screen)
        {
            var faqId = "faq-accordion";
            var featuresId = "features-accordion";
            var termsId = "terms-accordion";
            
            screen.WithContent("Accordion Example")
                .AddRichText("Accordions hide and show content sections", isBold: true)
                .AddAccordion("FAQ", "1. How do I create a bot?\n2. How do I add commands?\n3. How do I customize my bot?", false)
                .WithAccordionHandler(faqId)
                .AddAccordion("Features", "• Easy to use fluent interface\n• Modern UI components\n• Customizable styles\n• Comprehensive documentation", false)
                .WithAccordionHandler(featuresId)
                .AddAccordion("Terms of Service", "These are the terms of service for using this bot. Please read carefully before proceeding...", false)
                .WithAccordionHandler(termsId)
                .AddNavigationButton("Back to Menu", "advanced-ui-components");
        }

        /// <summary>
        /// Configure the rich text example screen
        /// </summary>
        /// <param name="screen">The screen builder</param>
        private static void ConfigureRichTextScreen(ScreenBuilder screen)
        {
            screen.WithContent("Rich Text Example")
                .AddRichText("This text is in bold", isBold: true)
                .AddRichText("This text is in italics", isItalic: true)
                .AddRichText("This text is underlined", isUnderlined: true)
                .AddRichText("This text has multiple styles", isBold: true, isItalic: true, isUnderlined: true)
                .AddRichText("This text is center-aligned", alignment: TextAlignment.Center)
                .AddRichText("This text is right-aligned", alignment: TextAlignment.Right)
                .AddNavigationButton("Back to Menu", "advanced-ui-components");
        }

        /// <summary>
        /// Configure the rating example screen
        /// </summary>
        /// <param name="screen">The screen builder</param>
        private static void ConfigureRatingScreen(ScreenBuilder screen)
        {
            screen.WithContent("Rating Example")
                .AddRichText("Ratings let users provide feedback", isBold: true)
                .AddRating("Rate our service", "service-rating", 0)
                .OnCallback("service-rating:*", async (data, state) =>
                {
                    var rating = int.Parse(data.Split(':')[1]);
                    
                    // In a real app, you would store the rating here
                    if (state.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long chatId)
                    {
                        // We would need the screen manager to update the screen properly
                        // This is a simplified example for demonstration purposes
                        return true;
                    }
                    
                    return true;
                })
                .AddRating("Rate our bot", "bot-rating", 4)
                .AddRating("Rate our UI", "ui-rating", 5)
                .AddNavigationButton("Back to Menu", "advanced-ui-components");
        }
    }
} 