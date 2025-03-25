using System.Threading;
using FluentTelegramUI.Models;
using FluentTelegramUI.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
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
        private readonly IFluentUpdateHandler _updateHandler;
        private readonly ScreenManager _screenManager;
        private CancellationTokenSource? _receivingCts;
        
        /// <summary>
        /// Gets the screen manager
        /// </summary>
        public ScreenManager ScreenManager => _screenManager;
        
        /// <summary>
        /// Gets the main screen from which navigation starts
        /// </summary>
        public Screen? MainScreen => _screenManager.MainScreen;
        
        /// <summary>
        /// Sets the update handler
        /// </summary>
        /// <param name="updateHandler">The update handler to use</param>
        public void SetUpdateHandler(IFluentUpdateHandler updateHandler)
        {
            _updateHandler = updateHandler;
        }
        
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
            if (_defaultStyle == null)
            {
                _defaultStyle = FluentStyle.Default;
            }
            
            // Create the screen manager
            var screenManagerLogger = _serviceProvider.GetService<ILogger<ScreenManager>>() ?? 
                                     new Logger<ScreenManager>(LogLevel.Information, typeof(ScreenManager).Name);
            _screenManager = new ScreenManager(_botClient, screenManagerLogger);
            
            // Create an update handler
            _updateHandler = _serviceProvider.GetService<IFluentUpdateHandler>();
            if (_updateHandler == null)
            {
                var handlerLogger = CreateDefaultLoggerForHandler();
                _updateHandler = new DefaultFluentUpdateHandler(handlerLogger);
            }
        }
        
        private ILogger<FluentTelegramBot> CreateDefaultLogger()
        {
            return new Logger<FluentTelegramBot>(LogLevel.Information, string.Empty);
        }
        
        private ILogger<DefaultFluentUpdateHandler> CreateDefaultLoggerForHandler()
        {
            return new Logger<DefaultFluentUpdateHandler>(LogLevel.Information, typeof(DefaultFluentUpdateHandler).Name);
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
        /// Starts receiving updates from Telegram
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        public void StartReceiving(CancellationToken cancellationToken = default)
        {
            _receivingCts = new CancellationTokenSource();
            
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, _receivingCts.Token);
            
            // Create an update handler
            var updateHandler = new UpdateHandler(HandleUpdateAsync, HandlePollingErrorAsync);
            
            // Start receiving
            _botClient.StartReceiving(
                updateHandler: updateHandler,
                receiverOptions: new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                },
                cancellationToken: linkedCts.Token);
            
            _logger.LogInformation("Started receiving updates from Telegram");
        }
        
        /// <summary>
        /// Stops receiving updates from Telegram
        /// </summary>
        public void StopReceiving()
        {
            if (_receivingCts != null)
            {
                _receivingCts.Cancel();
                _receivingCts.Dispose();
                _receivingCts = null;
                
                _logger.LogInformation("Stopped receiving updates from Telegram");
            }
        }
        
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message && message.Text is { } messageText)
            {
                // Handle text message
                await _updateHandler.HandleTextMessageAsync(botClient, message, cancellationToken);
            }
            else if (update.CallbackQuery is { } callbackQuery)
            {
                // Handle callback query using the screen manager first
                await _screenManager.HandleCallbackQueryAsync(callbackQuery, cancellationToken);
                
                // Then the regular update handler
                await _updateHandler.HandleCallbackQueryAsync(botClient, callbackQuery, cancellationToken);
            }
        }
        
        private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await _updateHandler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }
        
        /// <summary>
        /// Registers a screen with the bot
        /// </summary>
        /// <param name="screen">The screen to register</param>
        /// <param name="isMainScreen">Whether this screen is the main screen</param>
        public void RegisterScreen(Screen screen, bool isMainScreen = false)
        {
            _screenManager.RegisterScreen(screen, isMainScreen);
        }
        
        /// <summary>
        /// Sets a screen as the main screen
        /// </summary>
        /// <param name="screen">The screen to set as main</param>
        public void SetMainScreen(Screen screen)
        {
            _screenManager.SetMainScreen(screen);
        }
        
        /// <summary>
        /// Navigates to a screen for a specific chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="screenId">The screen ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task NavigateToScreenAsync(long chatId, string screenId, CancellationToken cancellationToken = default)
        {
            await _screenManager.NavigateToScreenAsync(chatId, screenId, cancellationToken);
        }
        
        /// <summary>
        /// Navigates to the main screen for a specific chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task NavigateToMainScreenAsync(long chatId, CancellationToken cancellationToken = default)
        {
            await _screenManager.NavigateToMainScreenAsync(chatId, cancellationToken);
        }
        
        /// <summary>
        /// Tries to get a screen by its ID
        /// </summary>
        /// <param name="screenId">The screen ID</param>
        /// <param name="screen">The screen, if found</param>
        /// <returns>True if the screen was found, otherwise false</returns>
        public bool TryGetScreen(string screenId, out Screen? screen)
        {
            return _screenManager.GetScreenById(screenId, out screen);
        }
        
        /// <summary>
        /// Creates a new screen instance
        /// </summary>
        /// <param name="title">The screen title</param>
        /// <param name="content">The screen content</param>
        /// <param name="isMainScreen">Whether this screen is the main screen</param>
        /// <returns>A new screen instance</returns>
        public Screen CreateScreen(string title, Models.Message? content = null, bool isMainScreen = false)
        {
            var screen = new Screen
            {
                Title = title,
                Content = content ?? new Models.Message()
            };
            
            RegisterScreen(screen, isMainScreen);
            return screen;
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