using System.Threading;
using FluentTelegramUI.Models;
using FluentTelegramUI.DependencyInjection;
using FluentTelegramUI.Handlers;
using Microsoft.Extensions.Options;
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
        private IFluentUpdateHandler _updateHandler;
        private readonly ScreenManager _screenManager;
        private CancellationTokenSource? _receivingCts;
        
        /// <summary>
        /// Gets the screen manager
        /// </summary>
        public ScreenManager ScreenManager => _screenManager;
        
        /// <summary>
        /// Gets the state machine for managing application state
        /// </summary>
        public StateMachine StateMachine => _screenManager.StateMachine;
        
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
            _defaultStyle = _serviceProvider.GetService<IOptions<FluentTelegramUIOptions>>()?.Value.DefaultStyle
                ?? FluentStyle.Default;
            
            var screenManagerLogger = _serviceProvider.GetService<ILogger<ScreenManager>>() ?? 
                                     new Logger<ScreenManager>(LogLevel.Information, typeof(ScreenManager).Name);
            _screenManager = _serviceProvider.GetService<ScreenManager>()
                ?? new ScreenManager(_botClient, screenManagerLogger, new StateMachine(), _defaultStyle);
            
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
        public async Task<Telegram.Bot.Types.Message> SendMessageAsync(ChatId chatId, Models.Message message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending message to chat {ChatId}", chatId);
            
            return await _botClient.SendMessage(
                chatId: chatId,
                text: message.Text,
                parseMode: message.ParseMarkdown ? ParseMode.Html : ParseMode.None,
                replyMarkup: message.ToInlineKeyboardMarkup(),
                cancellationToken: cancellationToken);
        }
        
        /// <summary>
        /// Starts receiving updates from Telegram
        /// </summary>
        public void StartReceiving(CancellationToken cancellationToken = default)
        {
            _receivingCts = new CancellationTokenSource();
            
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, _receivingCts.Token);
            
            var updateHandler = new UpdateHandler(HandleUpdateAsync, HandlePollingErrorAsync);
            
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

        /// <summary>
        /// Processes a single Telegram update through the bot pipeline.
        /// </summary>
        public Task ProcessUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
            => HandleUpdateAsync(botClient, update, cancellationToken);
        
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { Text: not null })
            {
                await _updateHandler.HandleTextMessageAsync(botClient, update.Message, cancellationToken);
            }
            else if (update.CallbackQuery is { } callbackQuery)
            {
                var handledByScreens = await _screenManager.HandleCallbackQueryAsync(callbackQuery, cancellationToken);
                if (!handledByScreens)
                {
                    await _updateHandler.HandleCallbackQueryAsync(botClient, callbackQuery, cancellationToken);
                }
            }
        }
        
        private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await _updateHandler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }
        
        /// <summary>
        /// Registers a screen with the bot
        /// </summary>
        public void RegisterScreen(Screen screen, bool isMainScreen = false)
        {
            _screenManager.RegisterScreen(screen, isMainScreen);
        }
        
        /// <summary>
        /// Sets a screen as the main screen
        /// </summary>
        public void SetMainScreen(Screen screen)
        {
            _screenManager.SetMainScreen(screen);
        }
        
        /// <summary>
        /// Navigates to a screen for a specific chat
        /// </summary>
        public Task NavigateToScreenAsync(long chatId, string screenId, CancellationToken cancellationToken = default)
            => _screenManager.NavigateToScreenAsync(chatId, screenId, cancellationToken);
        
        /// <summary>
        /// Navigates to the main screen for a specific chat
        /// </summary>
        public Task NavigateToMainScreenAsync(long chatId, CancellationToken cancellationToken = default)
            => _screenManager.NavigateToMainScreenAsync(chatId, cancellationToken);

        /// <summary>
        /// Refreshes the current screen for a chat without changing navigation state.
        /// </summary>
        public Task RefreshCurrentScreenAsync(long chatId, CancellationToken cancellationToken = default)
            => _screenManager.RefreshCurrentScreenAsync(chatId, cancellationToken);
        
        /// <summary>
        /// Tries to get a screen by its ID
        /// </summary>
        public bool TryGetScreen(string screenId, out Screen? screen)
        {
            return _screenManager.GetScreenById(screenId, out screen);
        }
        
        /// <summary>
        /// Creates a new screen instance
        /// </summary>
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
        /// Sets a state variable for the specified chat
        /// </summary>
        public void SetState<T>(long chatId, string key, T value)
        {
            _screenManager.SetState(chatId, key, value);
        }
        
        /// <summary>
        /// Gets a state variable for the specified chat
        /// </summary>
        public T GetState<T>(long chatId, string key, T defaultValue = default!)
        {
            return _screenManager.GetState(chatId, key, defaultValue);
        }
        
        /// <summary>
        /// Clears all state for the specified chat
        /// </summary>
        public void ClearState(long chatId)
        {
            _screenManager.ClearState(chatId);
        }
        
        /// <summary>
        /// Gets the current workflow state name for the specified chat
        /// </summary>
        public string? GetCurrentState(long chatId)
        {
            return _screenManager.GetCurrentState(chatId);
        }
        
        /// <summary>
        /// Sets the current workflow state for the specified chat
        /// </summary>
        public void SetCurrentState(long chatId, string stateName)
        {
            _screenManager.SetCurrentState(chatId, stateName);
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
            
            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
            
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
