using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentTelegramUI.Models;
using FluentTelegramUI.Handlers;
using FluentTelegramUI.Builders;

namespace FluentTelegramUI.Examples
{
    /// <summary>
    /// Example showing how to use screens in a Telegram bot
    /// </summary>
    public class ScreenExample
    {
        /// <summary>
        /// Runs the screen example
        /// </summary>
        /// <param name="token">The Telegram bot token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task RunAsync(string token)
        {
            Console.WriteLine("Starting screen example...");
            
            // Create a counter for demonstration
            var counter = 0;
            
            // Create the bot
            var bot = new TelegramBotBuilder()
                .WithToken(token)
                .WithFluentUI()
                .WithAutoStartReceiving()
                .Build();
            
            // Create screens
            var mainScreen = bot.CreateScreen("Main Menu", new Message
            {
                Text = "Welcome to the example bot! Please select an option:",
                ParseMarkdown = true
            }, true); // Set as main screen
            
            var counterScreen = bot.CreateScreen("Counter", new Message
            {
                Text = $"Current counter value: {counter}",
                ParseMarkdown = true
            });
            
            var settingsScreen = bot.CreateScreen("Settings", new Message
            {
                Text = "Configure bot settings here:",
                ParseMarkdown = true
            });
            
            var aboutScreen = bot.CreateScreen("About", new Message
            {
                Text = "This is a *FluentTelegramUI* example bot that demonstrates the screen system.",
                ParseMarkdown = true
            });
            
            // Add navigation between screens
            mainScreen.Content.Buttons.Add(new Button { Text = "📊 Counter", CallbackData = "screen:" + counterScreen.Id });
            mainScreen.Content.Buttons.Add(new Button { Text = "⚙️ Settings", CallbackData = "screen:" + settingsScreen.Id });
            mainScreen.Content.Buttons.Add(new Button { Text = "ℹ️ About", CallbackData = "screen:" + aboutScreen.Id });
            mainScreen.Content.ButtonsPerRow = 1;
            
            // Set up counter screen with controls
            counterScreen.ParentScreen = mainScreen;
            counterScreen.AddControl(new ButtonGroup(new List<Button>
            {
                new Button { Text = "➕ Increment", CallbackData = "counter:increment" },
                new Button { Text = "➖ Decrement", CallbackData = "counter:decrement" },
                new Button { Text = "🔄 Reset", CallbackData = "counter:reset" }
            }, 2));
            
            // Add counter event handlers
            counterScreen.OnCallback("counter:increment", async (data, context) => 
            {
                var chatId = (long)context["chatId"];
                var username = (string)context["username"];
                
                counter++;
                counterScreen.Content.Text = $"Current counter value: {counter} (updated by: {username})";
                return true; // Return true to refresh the screen
            });
            
            counterScreen.OnCallback("counter:decrement", async (data, context) => 
            {
                var chatId = (long)context["chatId"];
                var username = (string)context["username"];
                
                counter--;
                counterScreen.Content.Text = $"Current counter value: {counter} (updated by: {username})";
                return true;
            });
            
            counterScreen.OnCallback("counter:reset", async (data, context) => 
            {
                var chatId = (long)context["chatId"];
                var username = (string)context["username"];
                var firstName = (string)context["firstName"];
                
                counter = 0;
                counterScreen.Content.Text = $"Counter reset to 0 by {firstName}";
                return true;
            });
            
            // Set up settings screen with controls
            settingsScreen.ParentScreen = mainScreen;
            settingsScreen.AddControl(new TextButton("Toggle Dark Mode", "settings:toggle_dark"));
            settingsScreen.AddControl(new TextButton("Toggle Notifications", "settings:toggle_notifications"));
            
            // Add settings event handlers
            settingsScreen.OnCallback("settings:toggle_dark", async (data, context) => 
            {
                var userId = (long)context["userId"];
                return true;
            });
            
            settingsScreen.OnCallback("settings:toggle_notifications", async (data, context) => 
            {
                var userId = (long)context["userId"];
                return true;
            });
            
            // Set up about screen
            aboutScreen.ParentScreen = mainScreen;
            aboutScreen.Content.Text = "This is a *FluentTelegramUI* example bot that demonstrates the screen system.\n\n" +
                                       "Built with .NET and Telegram Bot API.\n\n" +
                                       "Version: 1.0.0";
            
            // Create a screen update handler
            var logger = new LoggerFactory().CreateLogger<ScreenUpdateHandler>();
            var handler = new ScreenUpdateHandler(logger, bot.ScreenManager);
            
            // Set the handler
            var botBuilder = new TelegramBotBuilder()
                .WithToken(token)
                .WithFluentUI()
                .WithUpdateHandler(handler)
                .WithAutoStartReceiving();
            
            // You can also configure screens with the builder:
            /*
            botBuilder.AddScreen("Settings", sb => {
                sb.WithContent("Configure bot settings here:", true)
                  .AddTextButton("Toggle Dark Mode", "settings:toggle_dark")
                  .AddTextButton("Toggle Notifications", "settings:toggle_notifications");
            });
            */
            
            // Build and start the bot
            var newBot = botBuilder.Build();
            
            Console.WriteLine("Bot started! Press Enter to exit.");
            Console.ReadLine();
            
            // Stop receiving updates when done
            bot.StopReceiving();
        }
    }
} 