using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Manages UI screens and handles navigation and event routing
    /// </summary>
    public class ScreenManager
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<ScreenManager> _logger;
        private readonly Dictionary<long, NavigationState> _navigationStates = new();
        private readonly Dictionary<string, Screen> _registeredScreens = new();
        
        /// <summary>
        /// Gets the main screen from which navigation starts
        /// </summary>
        public Screen? MainScreen { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the ScreenManager class
        /// </summary>
        /// <param name="botClient">The bot client</param>
        /// <param name="logger">The logger</param>
        public ScreenManager(ITelegramBotClient botClient, ILogger<ScreenManager> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }
        
        /// <summary>
        /// Registers a screen with the manager
        /// </summary>
        /// <param name="screen">The screen to register</param>
        /// <param name="isMainScreen">Whether this screen is the main screen</param>
        public void RegisterScreen(Screen screen, bool isMainScreen = false)
        {
            _registeredScreens[screen.Id] = screen;
            
            if (isMainScreen)
            {
                MainScreen = screen;
            }
        }
        
        /// <summary>
        /// Sets a screen as the main screen
        /// </summary>
        /// <param name="screen">The screen to set as main</param>
        public void SetMainScreen(Screen screen)
        {
            if (!_registeredScreens.ContainsKey(screen.Id))
            {
                RegisterScreen(screen);
            }
            
            MainScreen = screen;
        }
        
        /// <summary>
        /// Navigates to the main screen for a specific chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task NavigateToMainScreenAsync(long chatId, CancellationToken cancellationToken = default)
        {
            if (MainScreen == null)
            {
                _logger.LogError("No main screen set");
                return;
            }
            
            await NavigateToScreenAsync(chatId, MainScreen.Id, cancellationToken);
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
            if (!_registeredScreens.TryGetValue(screenId, out var screen))
            {
                _logger.LogError($"Screen not found: {screenId}");
                return;
            }
            
            // Update navigation state
            var navState = GetOrCreateNavigationState(chatId);
            navState.CurrentScreenId = screenId;
            
            // Clear any previous message
            if (navState.LastMessageId != 0)
            {
                try
                {
                    await _botClient.DeleteMessageAsync(
                        chatId: chatId,
                        messageId: navState.LastMessageId,
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to delete previous message: {ex.Message}");
                }
            }
            
            // Render the screen content
            var message = await RenderScreenAsync(chatId, screen, cancellationToken);
            navState.LastMessageId = message.MessageId;
        }
        
        /// <summary>
        /// Handles a callback query
        /// </summary>
        /// <param name="callbackQuery">The callback query</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken = default)
        {
            var chatId = callbackQuery.Message?.Chat.Id;
            if (chatId == null)
            {
                _logger.LogWarning("Callback query has no chat ID");
                return;
            }
            
            var navState = GetOrCreateNavigationState(chatId.Value);
            if (string.IsNullOrEmpty(navState.CurrentScreenId) || 
                !_registeredScreens.TryGetValue(navState.CurrentScreenId, out var currentScreen))
            {
                _logger.LogWarning($"No current screen for chat {chatId}");
                return;
            }
            
            // Check if this is a navigation callback (screen:screenId)
            if (callbackQuery.Data?.StartsWith("screen:") == true)
            {
                var targetScreenId = callbackQuery.Data.Substring(7);
                await NavigateToScreenAsync(chatId.Value, targetScreenId, cancellationToken);
                await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                return;
            }
            
            // Handle "back" navigation
            if (callbackQuery.Data == "back" && currentScreen.ParentScreen != null)
            {
                await NavigateToScreenAsync(chatId.Value, currentScreen.ParentScreen.Id, cancellationToken);
                await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                return;
            }
            
            // Handle the callback with the screen's event handlers
            var handled = false;
            if (callbackQuery.Data != null && currentScreen.EventHandlers.TryGetValue(callbackQuery.Data, out var handler))
            {
                try
                {
                    handled = await handler(callbackQuery.Data);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in event handler: {ex.Message}");
                }
            }
            
            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
            
            // Refresh the screen if needed
            if (handled)
            {
                var message = await RenderScreenAsync(chatId.Value, currentScreen, cancellationToken);
                navState.LastMessageId = message.MessageId;
            }
        }
        
        private async Task<Telegram.Bot.Types.Message> RenderScreenAsync(long chatId, Screen screen, CancellationToken cancellationToken)
        {
            // Start with the content
            var mainMessage = screen.Content;
            
            // Add controls
            var allButtons = new List<Button>(mainMessage.Buttons);
            foreach (var control in screen.Controls)
            {
                var controlMsg = control.ToMessage();
                if (!string.IsNullOrEmpty(controlMsg.Text) && string.IsNullOrEmpty(mainMessage.Text))
                {
                    mainMessage.Text = controlMsg.Text;
                }
                allButtons.AddRange(controlMsg.Buttons);
            }
            
            // Add back button if there's a parent screen and back navigation is allowed
            if (screen.ParentScreen != null && screen.AllowBackNavigation)
            {
                allButtons.Add(new Button { Text = screen.BackButtonText, CallbackData = "back" });
            }
            
            // Prepare the final message
            var renderMessage = new Message
            {
                Text = !string.IsNullOrEmpty(screen.Title) ? $"*{screen.Title}*\n\n{mainMessage.Text}" : mainMessage.Text,
                ParseMarkdown = mainMessage.ParseMarkdown,
                Style = mainMessage.Style,
                Buttons = allButtons,
                ButtonsPerRow = mainMessage.ButtonsPerRow
            };
            
            // Send the message
            return await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: renderMessage.Text,
                parseMode: renderMessage.ParseMarkdown ? Telegram.Bot.Types.Enums.ParseMode.MarkdownV2 : Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: renderMessage.ToInlineKeyboardMarkup(),
                cancellationToken: cancellationToken);
        }
        
        private NavigationState GetOrCreateNavigationState(long chatId)
        {
            if (!_navigationStates.TryGetValue(chatId, out var state))
            {
                state = new NavigationState();
                _navigationStates[chatId] = state;
            }
            return state;
        }
        
        /// <summary>
        /// Tries to get a screen by its ID
        /// </summary>
        /// <param name="screenId">The screen ID</param>
        /// <param name="screen">The screen, if found</param>
        /// <returns>True if the screen was found, otherwise false</returns>
        public bool GetScreenById(string screenId, out Screen? screen)
        {
            if (_registeredScreens.TryGetValue(screenId, out var foundScreen))
            {
                screen = foundScreen;
                return true;
            }
            
            screen = null;
            return false;
        }
        
        /// <summary>
        /// Represents the navigation state for a chat
        /// </summary>
        private class NavigationState
        {
            /// <summary>
            /// The ID of the current screen
            /// </summary>
            public string CurrentScreenId { get; set; } = string.Empty;
            
            /// <summary>
            /// The ID of the last message sent
            /// </summary>
            public int LastMessageId { get; set; }
            
            /// <summary>
            /// Input data for the current screen
            /// </summary>
            public Dictionary<string, string> InputData { get; set; } = new();
        }
    }
} 