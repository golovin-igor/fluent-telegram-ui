using System.Collections.Generic;
using FluentTelegramUI.Models;
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
            
            // Register services
            foreach (var service in _services)
            {
                serviceCollection.AddScoped(service.Key, service.Value);
            }
            
            // Build service provider manually
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            return new FluentTelegramBot(serviceProvider);
        }
    }
} 