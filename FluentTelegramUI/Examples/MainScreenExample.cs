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
    /// Example showing how to use the main screen functionality
    /// </summary>
    public class MainScreenExample
    {
        /// <summary>
        /// Runs the main screen example
        /// </summary>
        /// <param name="token">The Telegram bot token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task RunAsync(string token)
        {
            Console.WriteLine("Starting main screen example...");
            
            // Create the bot using the builder
            var bot = new TelegramBotBuilder()
                .WithToken(token)
                .WithFluentUI()
                .AddScreen("Main Menu", sb => {
                    sb.WithContent("Welcome to the main menu! This is the starting point for navigation.", true)
                      .AddButton("🔍 View Categories", "view_categories")
                      .AddButton("📝 My Profile", "view_profile")
                      .AddButton("⚙️ Settings", "view_settings")
                      .WithButtonsPerRow(1)
                      .AsMainScreen(); // Mark this as the main screen
                })
                .AddScreen("Categories", sb => {
                    sb.WithContent("Browse categories:", true)
                      .AddButton("📚 Books", "category_books")
                      .AddButton("🎮 Games", "category_games")
                      .AddButton("🎵 Music", "category_music")
                      .WithButtonsPerRow(2)
                      .OnCallback("category_books", async (data, context) => {
                          // Handle books category
                          return true;
                      })
                      .OnCallback("category_games", async (data, context) => {
                          // Handle games category
                          return true;
                      })
                      .OnCallback("category_music", async (data, context) => {
                          // Handle music category
                          return true;
                      });
                })
                .AddScreen("Profile", sb => {
                    sb.WithContent("Your profile information:", true)
                      .AddButton("✏️ Edit Profile", "edit_profile")
                      .WithBackButtonText("↩️ Return to Main Menu") // Customize back button text
                      .OnCallback("edit_profile", async (data, context) => {
                          // Handle edit profile
                          return true;
                      });
                })
                .AddScreen("Settings", sb => {
                    sb.WithContent("App settings:", true)
                      .AddButton("🌙 Dark Mode", "toggle_dark_mode")
                      .AddButton("🔔 Notifications", "toggle_notifications")
                      .AddButton("🔒 Privacy", "privacy_settings")
                      .WithButtonsPerRow(2)
                      .OnCallback("toggle_dark_mode", async (data, context) => {
                          // Toggle dark mode
                          return true;
                      })
                      .OnCallback("toggle_notifications", async (data, context) => {
                          // Toggle notifications
                          return true;
                      })
                      .OnCallback("privacy_settings", async (data, context) => {
                          // Privacy settings
                          return true;
                      });
                })
                .WithAutoStartReceiving()
                .Build();
            
            // Set up screen connections and navigation
            if (bot.TryGetScreen("Main Menu", out var mainScreen) && 
                bot.TryGetScreen("Categories", out var categoriesScreen) && 
                bot.TryGetScreen("Profile", out var profileScreen) && 
                bot.TryGetScreen("Settings", out var settingsScreen))
            {
                // Set up navigation from main menu to other screens
                mainScreen?.OnCallback("view_categories", async (data, context) => {
                    long chatId = (long)context["chatId"];
                    await bot.NavigateToScreenAsync(chatId, categoriesScreen.Id);
                    return true;
                });
                
                mainScreen?.OnCallback("view_profile", async (data, context) => {
                    long chatId = (long)context["chatId"];
                    await bot.NavigateToScreenAsync(chatId, profileScreen.Id);
                    return true;
                });
                
                mainScreen?.OnCallback("view_settings", async (data, context) => {
                    long chatId = (long)context["chatId"];
                    await bot.NavigateToScreenAsync(chatId, settingsScreen.Id);
                    return true;
                });
                
                // Set parent screens for back navigation
                categoriesScreen?.WithParent(mainScreen);
                profileScreen?.WithParent(mainScreen);
                settingsScreen?.WithParent(mainScreen);
                
                // Disable back navigation for certain screens if needed
                // For example: profileScreen?.AllowBack(false);
            }
            
            // Create a screen update handler
            var logger = new LoggerFactory().CreateLogger<ScreenUpdateHandler>();
            var handler = new ScreenUpdateHandler(logger, bot.ScreenManager);
            bot.SetUpdateHandler(handler);
            
            Console.WriteLine("Bot started! Press Enter to exit.");
            Console.ReadLine();
            
            // Stop receiving updates when done
            bot.StopReceiving();
        }
    }
} 