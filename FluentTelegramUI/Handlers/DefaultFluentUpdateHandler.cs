using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Handlers
{
    /// <summary>
    /// Default implementation of the IFluentUpdateHandler interface
    /// </summary>
    public class DefaultFluentUpdateHandler : IFluentUpdateHandler
    {
        private readonly ILogger<DefaultFluentUpdateHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the DefaultFluentUpdateHandler class
        /// </summary>
        /// <param name="logger">The logger</param>
        public DefaultFluentUpdateHandler(ILogger<DefaultFluentUpdateHandler> logger)
        {
            _logger = logger;
        }
        
        /// <inheritdoc/>
        public virtual Task HandleTextMessageAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received text message: {message.Text}");
            return Task.CompletedTask;
        }
        
        /// <inheritdoc/>
        public virtual Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received callback query: {callbackQuery.Data}");
            return botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
        }
        
        /// <inheritdoc/>
        public virtual Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error while polling for updates");
            return Task.CompletedTask;
        }
    }
} 