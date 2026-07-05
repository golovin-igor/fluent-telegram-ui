using System.Collections.Generic;
using FluentTelegramUI.DependencyInjection;
using FluentTelegramUI.Models;
using FluentTelegramUI.Handlers;
using FluentTelegramUI.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

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
        private ITelegramBotClient? _botClientOverride;
        
        /// <summary>
        /// Uses a pre-configured bot client instead of creating one from the token.
        /// Useful for testing or custom client configuration.
        /// </summary>
        public TelegramBotBuilder WithBotClient(ITelegramBotClient botClient)
        {
            _botClientOverride = botClient;
            return this;
        }
        
        /// <summary>
        /// Sets the Telegram bot token
        /// </summary>
        public TelegramBotBuilder WithToken(string token)
        {
            _token = token;
            return this;
        }
        
        /// <summary>
        /// Enables Fluent UI with the specified default style
        /// </summary>
        public TelegramBotBuilder WithFluentUI(FluentStyle style = FluentStyle.Default)
        {
            _defaultStyle = style;
            return this;
        }
        
        /// <summary>
        /// Sets the update handler for the bot
        /// </summary>
        public TelegramBotBuilder WithUpdateHandler(IFluentUpdateHandler updateHandler)
        {
            _updateHandler = updateHandler;
            return this;
        }
        
        /// <summary>
        /// Enables automatic start of receiving updates when the bot is built
        /// </summary>
        public TelegramBotBuilder WithAutoStartReceiving()
        {
            _autoStartReceiving = true;
            return this;
        }
        
        /// <summary>
        /// Adds a service to the bot's service collection
        /// </summary>
        public TelegramBotBuilder AddService(Type serviceType, Type implementationType)
        {
            _services[serviceType] = implementationType;
            return this;
        }
        
        /// <summary>
        /// Adds a service to the bot's service collection
        /// </summary>
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
        public TelegramBotBuilder AddScreen(string title, Action<ScreenBuilder> configure, bool isMainScreen = false)
        {
            _screenBuilders.Add((title, configure, isMainScreen));
            return this;
        }
        
        /// <summary>
        /// Sets the main screen by ID
        /// </summary>
        public TelegramBotBuilder WithMainScreen(string screenId)
        {
            _mainScreenId = screenId;
            return this;
        }
        
        /// <summary>
        /// Builds and returns a FluentTelegramBot instance
        /// </summary>
        public FluentTelegramBot Build()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new InvalidOperationException("Bot token cannot be empty");
            }
            
            var serviceCollection = new ServiceCollection();

            // Pre-register the bot client override (or one from the token) so the
            // shared core's TryAdd registration does not replace it.
            serviceCollection.AddSingleton<ITelegramBotClient>(_botClientOverride ?? new TelegramBotClient(_token));

            FluentTelegramUIServiceRegistrations.AddCore(serviceCollection, options =>
            {
                options.BotToken = _token;
                options.DefaultStyle = _defaultStyle;
                options.DefaultCulture = "en";
            });
            serviceCollection.AddLogging();

            if (_updateHandler != null)
            {
                serviceCollection.AddSingleton<IFluentUpdateHandler>(_updateHandler);
            }

            foreach (var service in _services)
            {
                serviceCollection.AddScoped(service.Key, service.Value);
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var bot = new FluentTelegramBot(serviceProvider);

            foreach (var (title, configure, isMainScreen) in _screenBuilders)
            {
                var screenBuilder = new ScreenBuilder(bot, title);

                if (isMainScreen)
                {
                    screenBuilder.AsMainScreen();
                }

                configure(screenBuilder);
                var screen = screenBuilder.Build();

                if (isMainScreen || screen.IsMainScreen)
                {
                    bot.SetMainScreen(screen);
                }
            }

            if (_mainScreenId != null && bot.MainScreen == null)
            {
                if (bot.TryGetScreen(_mainScreenId, out var screen) && screen != null)
                {
                    bot.SetMainScreen(screen);
                }
            }

            if (_updateHandler == null)
            {
                var logger = serviceProvider.GetService<ILogger<ScreenUpdateHandler>>()
                    ?? new LoggerFactory().CreateLogger<ScreenUpdateHandler>();
                bot.SetUpdateHandler(new ScreenUpdateHandler(logger, bot.ScreenManager));
            }

            if (_autoStartReceiving)
            {
                bot.StartReceiving();
            }

            return bot;
        }
    }
}
