using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentTelegramUI.Handlers;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Examples
{
    /// <summary>
    /// Example demonstrating how to use context parameters in callbacks
    /// </summary>
    public class ContextExample
    {
        /// <summary>
        /// Runs the context example
        /// </summary>
        /// <param name="token">Telegram bot token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task RunAsync(string token)
        {
            // Create the bot using the builder
            var botBuilder = new TelegramBotBuilder()
                .WithToken(token)
                .WithFluentUI();
                
            var bot = botBuilder.Build();
            
            // Create screens
            var mainScreen = new Screen
            {
                Title = "User Preferences",
                Content = new FluentTelegramUI.Models.Message
                {
                    Text = "Welcome! This bot demonstrates context parameters in callbacks.\nClick a button to proceed:",
                    ParseMarkdown = true,
                    ButtonsPerRow = 1
                }
            };
            
            var userInfoScreen = new Screen
            {
                Title = "Your Information",
                Content = new FluentTelegramUI.Models.Message
                {
                    Text = "Here's what we know about you:",
                    ParseMarkdown = true
                }
            };
            
            var preferencesScreen = new Screen
            {
                Title = "Your Preferences",
                Content = new FluentTelegramUI.Models.Message
                {
                    Text = "Customize your preferences:",
                    ParseMarkdown = true,
                    ButtonsPerRow = 2
                }
            };
            
            // Set up navigation structure
            userInfoScreen.WithParent(mainScreen);
            preferencesScreen.WithParent(mainScreen);
            
            // Add buttons to main screen
            mainScreen.Content.Buttons.Add(new Button { Text = "View My Information", CallbackData = "view_info" });
            mainScreen.Content.Buttons.Add(new Button { Text = "My Preferences", CallbackData = "view_preferences" });
            
            // Create a dictionary to store user preferences
            var userPreferences = new Dictionary<long, Dictionary<string, bool>>();
            
            // Add callback handlers to main screen
            mainScreen.OnCallback("view_info", async (data, context) => 
            {
                // Get user info from context
                long chatId = (long)context["chatId"];
                long userId = (long)context["userId"];
                string username = (string)context["username"];
                string firstName = (string)context["firstName"];
                string lastName = (string)context["lastName"];
                
                // Update the user info screen with context data
                userInfoScreen.Content.Text = $"*Your Information*\n\n" +
                    $"User ID: `{userId}`\n" +
                    $"Username: {(string.IsNullOrEmpty(username) ? "Not set" : "@" + username)}\n" +
                    $"First Name: {firstName}\n" +
                    $"Last Name: {(string.IsNullOrEmpty(lastName) ? "Not set" : lastName)}\n" +
                    $"Chat ID: `{chatId}`\n\n" +
                    "This information was retrieved from the context parameter!";
                
                // Navigate to the user info screen
                await bot.NavigateToScreenAsync(chatId, userInfoScreen.Id);
                return true;
            });
            
            mainScreen.OnCallback("view_preferences", async (data, context) => 
            {
                // Get chat ID from context
                long chatId = (long)context["chatId"];
                
                // Ensure the user has preference data
                if (!userPreferences.ContainsKey(chatId))
                {
                    userPreferences[chatId] = new Dictionary<string, bool>
                    {
                        { "notifications", true },
                        { "darkMode", false },
                        { "soundEffects", true }
                    };
                }
                
                // Update the preferences screen text
                UpdatePreferencesScreen(chatId);
                
                // Navigate to the preferences screen
                await bot.NavigateToScreenAsync(chatId, preferencesScreen.Id);
                return true;
            });
            
            // Add preference toggle buttons
            preferencesScreen.Content.Buttons.Add(new Button { Text = "Toggle Notifications", CallbackData = "toggle_notifications" });
            preferencesScreen.Content.Buttons.Add(new Button { Text = "Toggle Dark Mode", CallbackData = "toggle_dark_mode" });
            preferencesScreen.Content.Buttons.Add(new Button { Text = "Toggle Sound Effects", CallbackData = "toggle_sound_effects" });
            
            // Add callback handlers for preferences
            preferencesScreen.OnCallback("toggle_notifications", async (data, context) => 
            {
                long chatId = (long)context["chatId"];
                string username = (string)context["username"];
                
                // Toggle the notification preference for this user
                userPreferences[chatId]["notifications"] = !userPreferences[chatId]["notifications"];
                
                // Log who made the change
                Console.WriteLine($"User {username} (ID: {chatId}) toggled notifications to {userPreferences[chatId]["notifications"]}");
                
                // Update the screen text
                UpdatePreferencesScreen(chatId);
                return true;
            });
            
            preferencesScreen.OnCallback("toggle_dark_mode", async (data, context) => 
            {
                long chatId = (long)context["chatId"];
                string firstName = (string)context["firstName"];
                
                // Toggle the dark mode preference for this user
                userPreferences[chatId]["darkMode"] = !userPreferences[chatId]["darkMode"];
                
                // Log who made the change
                Console.WriteLine($"User {firstName} (ID: {chatId}) toggled dark mode to {userPreferences[chatId]["darkMode"]}");
                
                // Update the screen text
                UpdatePreferencesScreen(chatId);
                return true;
            });
            
            preferencesScreen.OnCallback("toggle_sound_effects", async (data, context) => 
            {
                long chatId = (long)context["chatId"];
                
                // Toggle the sound effects preference for this user
                userPreferences[chatId]["soundEffects"] = !userPreferences[chatId]["soundEffects"];
                
                // Update the screen text
                UpdatePreferencesScreen(chatId);
                return true;
            });
            
            // Helper method to update the preferences screen text
            void UpdatePreferencesScreen(long chatId)
            {
                var prefs = userPreferences[chatId];
                preferencesScreen.Content.Text = $"*Your Preferences*\n\n" +
                    $"🔔 Notifications: {(prefs["notifications"] ? "✅ On" : "❌ Off")}\n" +
                    $"🌙 Dark Mode: {(prefs["darkMode"] ? "✅ On" : "❌ Off")}\n" +
                    $"🔊 Sound Effects: {(prefs["soundEffects"] ? "✅ On" : "❌ Off")}\n\n" +
                    "Click a button to toggle a setting:";
            }
            
            // Register screens with the bot
            bot.RegisterScreen(mainScreen, true);
            bot.RegisterScreen(userInfoScreen);
            bot.RegisterScreen(preferencesScreen);
            
            // Create and set update handler
            var logger = new LoggerFactory().CreateLogger<ScreenUpdateHandler>();
            var handler = new ScreenUpdateHandler(logger, bot.ScreenManager);
            bot.SetUpdateHandler(handler);
            
            // Start receiving updates
            bot.StartReceiving();
            
            Console.WriteLine("Bot started! Press Enter to exit.");
            Console.ReadLine();
            
            // Stop receiving updates when done
            bot.StopReceiving();
        }
    }
} 