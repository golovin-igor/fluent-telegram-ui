using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentTelegramUI.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Handlers
{
    /// <summary>
    /// Implementation of IFluentUpdateHandler that works with screens
    /// </summary>
    public class ScreenUpdateHandler : IFluentUpdateHandler
    {
        private readonly ILogger<ScreenUpdateHandler> _logger;
        private readonly ScreenManager _screenManager;
        
        /// <summary>
        /// Initializes a new instance of the ScreenUpdateHandler class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="screenManager">The screen manager</param>
        public ScreenUpdateHandler(ILogger<ScreenUpdateHandler> logger, ScreenManager screenManager)
        {
            _logger = logger;
            _screenManager = screenManager;
        }
        
        /// <inheritdoc/>
        public virtual async Task HandleTextMessageAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received text message: {message.Text}");
            
            if (message.Text == "/start" && message.Chat.Id != null)
            {
                // Navigate to main screen for new users
                await _screenManager.NavigateToMainScreenAsync(message.Chat.Id, cancellationToken);
            }
        }
        
        /// <inheritdoc/>
        public virtual async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received callback query: {callbackQuery.Data}");
            
            // The ScreenManager already handles the callback, just answer it
            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
        }
        
        /// <inheritdoc/>
        public virtual Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error while polling for updates");
            return Task.CompletedTask;
        }
    }
} 