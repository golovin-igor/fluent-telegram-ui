using FluentTelegramUI;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using FluentTelegramUI.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Message = Telegram.Bot.Types.Message;

namespace FluentTelegramUI.Examples
{
    /// <summary>
    /// A simple example of a bot using FluentTelegramUI
    /// </summary>
    public class SimpleBot : IFluentUpdateHandler
    {
        private readonly FluentTelegramBot _bot;
        
        /// <summary>
        /// Initializes a new instance of the SimpleBot class
        /// </summary>
        /// <param name="token">The Telegram bot token</param>
        public SimpleBot(string token)
        {
            // Create the bot using the builder
            _bot = new TelegramBotBuilder()
                .WithToken(token)
                .WithFluentUI(FluentStyle.Modern)
                .WithUpdateHandler(this)
                .WithAutoStartReceiving()
                .Build();
            
            Console.WriteLine("SimpleBot is ready!");
        }
        
        /// <summary>
        /// Handles text messages received by the bot
        /// </summary>
        public async Task HandleTextMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received message: {message.Text}");
            
            if (message.Text == "/start")
            {
                await SendWelcomeMessageAsync(message.Chat.Id, cancellationToken);
            }
            else if (message.Text == "/help")
            {
                await SendHelpMessageAsync(message.Chat.Id, cancellationToken);
            }
            else
            {
                // Echo the message
                var responseMessage = new MessageBuilder()
                    .WithText($"You said: {message.Text}")
                    .WithMarkdown(true)
                    .WithButtonsPerRow(2)
                    .WithButton("Help", "/help")
                    .WithUrlButton("Visit Website", "https://example.com")
                    .Build();
                
                await _bot.SendMessageAsync(message.Chat.Id, responseMessage, cancellationToken);
            }
        }
        
        /// <summary>
        /// Handles callback queries (button clicks) received by the bot
        /// </summary>
        public async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received callback query: {callbackQuery.Data}");
            
            if (callbackQuery.Data == "/help")
            {
                await SendHelpMessageAsync(callbackQuery.Message.Chat.Id, cancellationToken);
            }
            
            // Answer the callback query
            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
        }
        
        /// <summary>
        /// Handles polling errors
        /// </summary>
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error while polling for updates: {exception.Message}");
            return Task.CompletedTask;
        }
        
        private async Task SendWelcomeMessageAsync(ChatId chatId, CancellationToken cancellationToken)
        {
            var welcomeMessage = new MessageBuilder()
                .WithText("*Welcome to the FluentTelegramUI Demo Bot!*\n\nThis bot demonstrates the capabilities of the FluentTelegramUI library.")
                .WithMarkdown(true)
                .WithButtonsPerRow(1)
                .WithButton("Get Help", "/help")
                .Build();
            
            await _bot.SendMessageAsync(chatId, welcomeMessage, cancellationToken);
        }
        
        private async Task SendHelpMessageAsync(ChatId chatId, CancellationToken cancellationToken)
        {
            var helpMessage = new MessageBuilder()
                .WithText("*Available Commands:*\n\n/start - Start the bot\n/help - Show this help message")
                .WithMarkdown(true)
                .Build();
            
            await _bot.SendMessageAsync(chatId, helpMessage, cancellationToken);
        }
        
        /// <summary>
        /// Example of running the simple bot
        /// </summary>
        public static void Main(string[] args)
        {
            // Replace with your actual bot token
            string token = "YOUR_BOT_TOKEN_HERE";
            
            // Create the bot
            var bot = new SimpleBot(token);
            
            // Keep the application running
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
} 