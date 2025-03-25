using System.Collections.Generic;
using FluentTelegramUI.Models;
using FluentTelegramUI.Handlers;
using FluentTelegramUI.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace FluentTelegramUI
{
    /// <summary>
    /// Builder for creating a Telegram bot with Fluent UI
    /// </summary>
    public class TelegramBotBuilder
    {
        private string _token = string.Empty;
        private FluentStyle _defaultStyle = FluentStyle.Default;
        private readonly Dictionary<Type, Type> _services = new();
        private IFluentUpdateHandler? _updateHandler;
        private bool _autoStartReceiving = false;
        private List<(string Title, Action<ScreenBuilder> Configure, bool IsMainScreen)> _screenBuilders = new();
        private string? _mainScreenId = null;
        
        /// <summary>
        /// Sets the Telegram bot token
        /// </summary>
        /// <param name="token">The Telegram bot token</param>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder WithToken(string token)
        {
            _token = token;
            return this;
        }
        
        /// <summary>
        /// Enables Fluent UI with the specified default style
        /// </summary>
        /// <param name="style">The default style to use</param>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder WithFluentUI(FluentStyle style = FluentStyle.Default)
        {
            _defaultStyle = style;
            return this;
        }
        
        /// <summary>
        /// Sets the update handler for the bot
        /// </summary>
        /// <param name="updateHandler">The update handler to use</param>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder WithUpdateHandler(IFluentUpdateHandler updateHandler)
        {
            _updateHandler = updateHandler;
            return this;
        }
        
        /// <summary>
        /// Enables automatic start of receiving updates when the bot is built
        /// </summary>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder WithAutoStartReceiving()
        {
            _autoStartReceiving = true;
            return this;
        }
        
        /// <summary>
        /// Adds a service to the bot's service collection
        /// </summary>
        /// <param name="serviceType">The type of the service</param>
        /// <param name="implementationType">The implementation type of the service</param>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder AddService(Type serviceType, Type implementationType)
        {
            _services[serviceType] = implementationType;
            return this;
        }
        
        /// <summary>
        /// Adds a service to the bot's service collection
        /// </summary>
        /// <typeparam name="TService">The type of the service</typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service</typeparam>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder AddService<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _services[typeof(TService)] = typeof(TImplementation);
            return this;
        }
        
        /// <summary>
        /// Adds a screen to the bot
        /// </summary>
        /// <param name="title">The screen title</param>
        /// <param name="configure">The configuration action</param>
        /// <param name="isMainScreen">Whether this screen is the main screen</param>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder AddScreen(string title, Action<ScreenBuilder> configure, bool isMainScreen = false)
        {
            _screenBuilders.Add((title, configure, isMainScreen));
            return this;
        }
        
        /// <summary>
        /// Sets the main screen by ID
        /// </summary>
        /// <param name="screenId">The ID of the screen to set as main</param>
        /// <returns>The TelegramBotBuilder instance for method chaining</returns>
        public TelegramBotBuilder WithMainScreen(string screenId)
        {
            _mainScreenId = screenId;
            return this;
        }
        
        /// <summary>
        /// Builds and returns a FluentTelegramBot instance
        /// </summary>
        /// <returns>A FluentTelegramBot instance</returns>
        public FluentTelegramBot Build()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new InvalidOperationException("Bot token cannot be empty");
            }
            
            // Create service collection
            var serviceCollection = new ServiceCollection();
            
            // Add ITelegramBotClient
            serviceCollection.AddSingleton<ITelegramBotClient>(new TelegramBotClient(_token));
            
            // We can't add FluentStyle as a singleton since it's an enum, not a reference type
            
            // Add update handler if provided
            if (_updateHandler != null)
            {
                serviceCollection.AddSingleton<IFluentUpdateHandler>(_updateHandler);
            }
            
            // Register services
            foreach (var service in _services)
            {
                serviceCollection.AddScoped(service.Key, service.Value);
            }
            
            // Build service provider manually
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            // Create bot instance
            var bot = new FluentTelegramBot(serviceProvider);
            
            // Register screens
            var screenIdMap = new Dictionary<int, string>();
            int screenIndex = 0;
            
            foreach (var (title, configure, isMainScreen) in _screenBuilders)
            {
                var screenBuilder = new ScreenBuilder(bot, title);
                
                // Set main screen flag before configuring
                if (isMainScreen)
                {
                    screenBuilder.AsMainScreen();
                }
                
                configure(screenBuilder);
                var screen = screenBuilder.Build();
                screenIdMap[screenIndex] = screen.Id;
                
                // Set as main screen if this is the main screen or if it was configured as a main screen
                if (isMainScreen || screen.IsMainScreen)
                {
                    bot.SetMainScreen(screen);
                }
                
                screenIndex++;
            }
            
            // Set main screen by ID if provided and we don't have a main screen yet
            if (_mainScreenId != null && bot.MainScreen == null)
            {
                if (bot.TryGetScreen(_mainScreenId, out var screen) && screen != null)
                {
                    bot.SetMainScreen(screen);
                }
            }
            
            // Create a screen update handler if one is not already set
            if (_updateHandler == null)
            {
                var logger = serviceProvider.GetService<ILogger<ScreenUpdateHandler>>();
                if (logger == null)
                {
                    logger = new LoggerFactory().CreateLogger<ScreenUpdateHandler>();
                }
                
                _updateHandler = new ScreenUpdateHandler(logger, bot.ScreenManager);
                WithUpdateHandler(_updateHandler);
            }
            
            // Start receiving updates if enabled
            if (_autoStartReceiving)
            {
                bot.StartReceiving();
            }
            
            return bot;
        }
    }
} 