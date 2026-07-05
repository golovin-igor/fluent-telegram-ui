using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentTelegramUI.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Collections.Generic;

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
            _logger.LogInformation("Received text message: {Text}", message.Text);

            if (message.Text == "/start")
            {
                // Full reset before navigating: clears state, current screen, and
                // navigation state (including the stale last message id).
                _screenManager.ResetChat(message.Chat.Id);
                _screenManager.SetCurrentState(message.Chat.Id, "initial");
                await _screenManager.NavigateToMainScreenAsync(message.Chat.Id, cancellationToken);
                return;
            }

            // Get current conversation state
            var currentState = _screenManager.GetCurrentState(message.Chat.Id);
            var currentScreen = _screenManager.StateMachine.GetCurrentScreen(message.Chat.Id);

            // Handle text input based on the current state
            if (!string.IsNullOrEmpty(currentState) && !string.IsNullOrEmpty(currentScreen) &&
                _screenManager.GetScreenById(currentScreen, out var screen) && screen != null)
            {
                // Check if the screen has a text input handler for the current state
                var handlerKey = CallbackPrefixes.TextInput + currentState;
                if (screen.EventHandlers.TryGetValue(handlerKey, out var handler))
                {
                    // Create context dictionary with useful information
                    var context = new Dictionary<string, object>
                    {
                        { "chatId", message.Chat.Id },
                        { "userId", message.From?.Id ?? 0 },
                        { "username", message.From?.Username ?? "" },
                        { "firstName", message.From?.FirstName ?? "" },
                        { "lastName", message.From?.LastName ?? "" },
                        { "messageId", message.MessageId },
                        { "message", message }
                    };

                    // Store the input text in state
                    _screenManager.SetState(message.Chat.Id, StateKeys.LastInput, message.Text);

                    try
                    {
                        bool result = await handler(message.Text, context);
                        if (result)
                        {
                            await _screenManager.RefreshCurrentScreenAsync(message.Chat.Id, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in text input handler: {Message}", ex.Message);
                    }
                }
            }
        }
        
        /// <inheritdoc/>
        public virtual Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received callback query: {Data}", callbackQuery.Data);
            return botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
        }
        
        /// <inheritdoc/>
        public virtual Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error while polling for updates");
            return Task.CompletedTask;
        }
    }
} 