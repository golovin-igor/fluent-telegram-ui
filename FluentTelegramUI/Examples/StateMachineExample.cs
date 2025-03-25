using System;
using System.Threading.Tasks;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging;
using FluentTelegramUI.Handlers;

namespace FluentTelegramUI.Examples
{
    /// <summary>
    /// Example showing how to use the StateMachine for a multi-step form
    /// </summary>
    public class StateMachineExample
    {
        /// <summary>
        /// Runs the state machine example
        /// </summary>
        /// <param name="token">The Telegram bot token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task RunAsync(string token)
        {
            Console.WriteLine("Starting state machine example...");
            
            // Create the bot
            var bot = new TelegramBotBuilder()
                .WithToken(token)
                .WithFluentUI()
                .WithAutoStartReceiving()
                .Build();
            
            // Create the screens for the multi-step form
            var welcomeScreen = bot.CreateScreen("Welcome", new Message
            {
                Text = "Welcome to the registration form! Click Start to begin.",
                ParseMarkdown = true
            }, true); // This is our main screen
            
            var nameScreen = bot.CreateScreen("Name Input", new Message
            {
                Text = "Please enter your full name:",
                ParseMarkdown = true
            });
            
            var emailScreen = bot.CreateScreen("Email Input", new Message
            {
                Text = "Please enter your email address:",
                ParseMarkdown = true
            });
            
            var ageScreen = bot.CreateScreen("Age Input", new Message
            {
                Text = "Please enter your age:",
                ParseMarkdown = true
            });
            
            var summaryScreen = bot.CreateScreen("Registration Summary", new Message
            {
                Text = "Thank you for registering! Here's a summary of your information:",
                ParseMarkdown = true
            });
            
            // Set parent screens for navigation hierarchy
            nameScreen.WithParent(welcomeScreen);
            emailScreen.WithParent(nameScreen);
            ageScreen.WithParent(emailScreen);
            summaryScreen.WithParent(ageScreen);
            
            // Set up welcome screen
            welcomeScreen.Content.Buttons.Add(new Button 
            { 
                Text = "Start Registration", 
                CallbackData = "start_registration" 
            });
            
            welcomeScreen.OnCallback("start_registration", async (data, context) => 
            {
                // Get the chat ID from context
                long chatId = (long)context["chatId"];
                var username = (string)context["username"];
                
                // Set initial state for the registration process
                bot.SetCurrentState(chatId, "awaiting_name");
                
                // Navigate to the name input screen
                await bot.NavigateToScreenAsync(chatId, nameScreen.Id);
                return true;
            });
            
            // Set up name screen
            nameScreen.OnTextInput("awaiting_name", async (name, context) => 
            {
                // Get the chat ID from context
                long chatId = (long)context["chatId"];
                
                // Store the name
                bot.SetState(chatId, "name", name);
                
                // Transition to next state
                bot.SetCurrentState(chatId, "awaiting_email");
                
                // Navigate to the email screen
                await bot.NavigateToScreenAsync(chatId, emailScreen.Id);
                return true;
            });
            
            // Set up email screen
            emailScreen.OnTextInput("awaiting_email", async (email, context) => 
            {
                // Get the chat ID from context
                long chatId = (long)context["chatId"];
                
                // Simple email validation
                if (!email.Contains("@"))
                {
                    // Update the screen text to show error
                    emailScreen.Content.Text = "Invalid email format. Please enter a valid email address:";
                    return true; // Refresh the screen but stay in the same state
                }
                
                // Store the email
                bot.SetState(chatId, "email", email);
                
                // Transition to next state
                bot.SetCurrentState(chatId, "awaiting_age");
                
                // Navigate to the age screen
                await bot.NavigateToScreenAsync(chatId, ageScreen.Id);
                return true;
            });
            
            // Set up age screen
            ageScreen.OnTextInput("awaiting_age", async (ageText, context) => 
            {
                // Get the chat ID from context
                long chatId = (long)context["chatId"];
                var firstName = (string)context["firstName"];
                
                // Try to parse the age
                if (!int.TryParse(ageText, out int age) || age < 1 || age > 120)
                {
                    // Update the screen text to show error
                    ageScreen.Content.Text = "Invalid age. Please enter a valid age (1-120):";
                    return true; // Refresh the screen but stay in the same state
                }
                
                // Store the age
                bot.SetState(chatId, "age", age);
                
                // Transition to summary state
                bot.SetCurrentState(chatId, "complete");
                
                // Update the summary screen text with the collected information
                var name = bot.GetState<string>(chatId, "name");
                var email = bot.GetState<string>(chatId, "email");
                
                summaryScreen.Content.Text = $"*Registration Complete!*\n\n" +
                    $"Name: {name} ({firstName})\n" +
                    $"Email: {email}\n" +
                    $"Age: {age}\n\n" +
                    "Thank you for registering!";
                
                summaryScreen.Content.Buttons.Add(new Button
                {
                    Text = "Start Over",
                    CallbackData = "start_over"
                });
                
                // Navigate to the summary screen
                await bot.NavigateToScreenAsync(chatId, summaryScreen.Id);
                return true;
            });
            
            // Add a handler for starting over
            summaryScreen.OnCallback("start_over", async (data, context) =>
            {
                // Get the chat ID from context
                long chatId = (long)context["chatId"];
                
                // Clear all state
                bot.ClearState(chatId);
                
                // Set initial state
                bot.SetCurrentState(chatId, "initial");
                
                // Go back to welcome screen
                await bot.NavigateToScreenAsync(chatId, welcomeScreen.Id);
                return true;
            });
            
            // Create a screen update handler
            var logger = new LoggerFactory().CreateLogger<ScreenUpdateHandler>();
            var handler = new ScreenUpdateHandler(logger, bot.ScreenManager);
            
            // Set up our handler to store the chat ID in context before processing
            var originalHandleText = handler.HandleTextMessageAsync;
            
            // Use our handler
            bot.SetUpdateHandler(handler);
            
            Console.WriteLine("Bot started! Send /start to begin the registration flow.");
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            
            // Stop receiving updates when done
            bot.StopReceiving();
        }
    }
} 