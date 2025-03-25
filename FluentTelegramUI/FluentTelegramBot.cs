using System.Threading;
using FluentTelegramUI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI
{
    /// <summary>
    /// The main class for interacting with the Telegram Bot API using Fluent UI
    /// </summary>
    public class FluentTelegramBot
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<FluentTelegramBot> _logger;
        private readonly FluentStyle _defaultStyle;
        
        /// <summary>
        /// Initializes a new instance of the FluentTelegramBot class
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public FluentTelegramBot(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _botClient = _serviceProvider.GetRequiredService<ITelegramBotClient>();
            _logger = _serviceProvider.GetService<ILogger<FluentTelegramBot>>() ?? 
                      CreateDefaultLogger();
            _defaultStyle = _serviceProvider.GetService<FluentStyle>();
        }
        
        private ILogger<FluentTelegramBot> CreateDefaultLogger()
        {
            return new Logger<FluentTelegramBot>(LogLevel.Information, string.Empty);
        }
        
        /// <summary>
        /// Sends a message to a Telegram chat
        /// </summary>
        /// <param name="chatId">The ID of the chat</param>
        /// <param name="message">The message to send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The sent message</returns>
        public async Task<Telegram.Bot.Types.Message> SendMessageAsync(ChatId chatId, Models.Message message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Sending message to chat {chatId}");
            
            var parseMode = message.ParseMarkdown ? ParseMode.MarkdownV2 : ParseMode.Html;
            
            return await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message.Text,
                parseMode: parseMode,
                replyMarkup: message.ToInlineKeyboardMarkup(),
                cancellationToken: cancellationToken);
        }
        
        /// <summary>
        /// Sends a message to a Telegram chat
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The sent message</returns>
        public async Task<Telegram.Bot.Types.Message> SendMessageAsync(Models.Message message, CancellationToken cancellationToken = default)
        {
            return await SendMessageAsync(new ChatId(0), message, cancellationToken);
        }
        
        /// <summary>
        /// Simple logger implementation
        /// </summary>
        private class Logger<T> : ILogger<T>
        {
            private readonly LogLevel _minLevel;
            private readonly string _name;
            
            public Logger(LogLevel minLevel, string name)
            {
                _minLevel = minLevel;
                _name = name;
            }
            
            public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
            
            public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel;
            
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }
                
                var message = formatter(state, exception);
                
                if (string.IsNullOrEmpty(message))
                {
                    return;
                }
                
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] {_name}: {message}");
                
                if (exception != null)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
            
            private class NullScope : IDisposable
            {
                public static NullScope Instance { get; } = new NullScope();
                
                private NullScope() { }
                
                public void Dispose() { }
            }
        }
    }
} 