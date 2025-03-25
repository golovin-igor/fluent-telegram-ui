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
            _logger.LogInformation($"Received text message: {message.Text}");
            
            if (message.Text == "/start" && message.Chat.Id != null)
            {
                // Navigate to main screen for new users
                await _screenManager.NavigateToMainScreenAsync(message.Chat.Id, cancellationToken);
                
                // Reset the state machine for this chat
                _screenManager.ClearState(message.Chat.Id);
                _screenManager.SetCurrentState(message.Chat.Id, "initial");
                return;
            }
            
            // Get current conversation state
            var currentState = _screenManager.GetCurrentState(message.Chat.Id);
            var currentScreen = _screenManager.StateMachine.GetCurrentScreen(message.Chat.Id);
            
            // Handle text input based on the current state
            if (!string.IsNullOrEmpty(currentState) && !string.IsNullOrEmpty(currentScreen) && 
                _screenManager.GetScreenById(currentScreen, out var screen))
            {
                // Check if the screen has a text input handler for the current state
                var handlerKey = $"text_input:{currentState}";
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
                    _screenManager.SetState(message.Chat.Id, "last_input", message.Text);
                    
                    // Call the handler with the text and context
                    bool result = await handler(message.Text, context);
                    if (result)
                    {
                        // Refresh the screen if the handler returns true
                        await _screenManager.NavigateToScreenAsync(message.Chat.Id, currentScreen, cancellationToken);
                    }
                }
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