using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentTelegramUI.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Manages UI screens and handles navigation and event routing. Thread-safe
    /// for concurrent updates across different chats, and serializes updates
    /// within a single chat via per-chat locks.
    /// </summary>
    public class ScreenManager
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<ScreenManager> _logger;
        private readonly ConcurrentDictionary<long, NavigationState> _navigationStates = new();
        private readonly ConcurrentDictionary<long, SemaphoreSlim> _chatLocks = new();
        private readonly Dictionary<string, Screen> _registeredScreens = new();
        private readonly StateMachine _stateMachine;
        private readonly FluentStyle _defaultStyle;
        private readonly ILocalizationService? _localization;
        private readonly ScreenRenderer _renderer;

        /// <summary>
        /// Gets the main screen from which navigation starts
        /// </summary>
        public Screen? MainScreen { get; private set; }

        /// <summary>
        /// Gets the state machine instance
        /// </summary>
        public StateMachine StateMachine => _stateMachine;

        /// <summary>
        /// Initializes a new instance of the ScreenManager class
        /// </summary>
        public ScreenManager(ITelegramBotClient botClient, ILogger<ScreenManager> logger)
            : this(botClient, logger, new StateMachine(), FluentStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ScreenManager class
        /// </summary>
        public ScreenManager(
            ITelegramBotClient botClient,
            ILogger<ScreenManager> logger,
            StateMachine stateMachine,
            FluentStyle defaultStyle = FluentStyle.Default,
            ILocalizationService? localization = null)
        {
            _botClient = botClient;
            _logger = logger;
            _stateMachine = stateMachine;
            _defaultStyle = defaultStyle;
            _localization = localization;
            _renderer = new ScreenRenderer(botClient, logger, stateMachine, defaultStyle, localization);
        }

        /// <summary>
        /// Registers a screen with the manager
        /// </summary>
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
        public void SetMainScreen(Screen screen)
        {
            if (!_registeredScreens.ContainsKey(screen.Id))
            {
                RegisterScreen(screen);
            }

            MainScreen = screen;
        }

        /// <summary>
        /// Tries to get a screen by its ID
        /// </summary>
        public bool TryGetScreen(string screenId, out Screen? screen)
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
        /// Navigates to the main screen for a specific chat
        /// </summary>
        public Task NavigateToMainScreenAsync(long chatId, CancellationToken cancellationToken = default)
        {
            if (MainScreen == null)
            {
                _logger.LogError("No main screen set");
                return Task.CompletedTask;
            }

            return NavigateToScreenAsync(chatId, MainScreen.Id, cancellationToken);
        }

        /// <summary>
        /// Navigates to a screen for a specific chat
        /// </summary>
        public async Task NavigateToScreenAsync(long chatId, string screenId, CancellationToken cancellationToken = default)
        {
            using (await LockChatAsync(chatId))
            {
                await NavigateToScreenCoreAsync(chatId, screenId, cancellationToken);
            }
        }

        /// <summary>
        /// Refreshes the current screen in place for a chat.
        /// </summary>
        public async Task RefreshCurrentScreenAsync(long chatId, CancellationToken cancellationToken = default)
        {
            using (await LockChatAsync(chatId))
            {
                await RefreshCurrentScreenCoreAsync(chatId, cancellationToken);
            }
        }

        /// <summary>
        /// Resets all per-chat state: state variables, current screen, and
        /// navigation state (including the tracked last message id). Call this
        /// on <c>/start</c> to fully reset a chat before navigating. Thread-safe
        /// via concurrent collections; does not block on per-chat async locks.
        /// </summary>
        public void ResetChat(long chatId)
        {
            _stateMachine.ResetChat(chatId);
            _navigationStates.TryRemove(chatId, out _);
        }

        /// <summary>
        /// Handles a callback query. Returns true when the callback was handled
        /// by the screen system (navigation, back, or a matching screen handler);
        /// false when no screen-level handler matched so the caller may forward
        /// it to a custom <see cref="FluentTelegramUI.Handlers.IFluentUpdateHandler"/>.
        /// </summary>
        public async Task<bool> HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken = default)
        {
            var chatId = callbackQuery.Message?.Chat.Id;
            if (chatId == null)
            {
                _logger.LogWarning("Callback query has no chat ID");
                return false;
            }

            Func<string, Dictionary<string, object>, Task<bool>>? handler = null;
            string? screenIdBeforeHandler = null;
            Dictionary<string, object>? context = null;

            using (await LockChatAsync(chatId.Value))
            {
                var navState = GetOrCreateNavigationState(chatId.Value);
                if (string.IsNullOrEmpty(navState.CurrentScreenId)
                    || !_registeredScreens.TryGetValue(navState.CurrentScreenId, out var currentScreen))
                {
                    _logger.LogWarning("No current screen for chat {ChatId}", chatId);
                    return false;
                }

                if (callbackQuery.Data?.StartsWith(CallbackPrefixes.Screen, StringComparison.Ordinal) == true)
                {
                    var targetScreenId = callbackQuery.Data[CallbackPrefixes.Screen.Length..];
                    await NavigateToScreenCoreAsync(chatId.Value, targetScreenId, cancellationToken);
                    await AnswerCallbackAsync(callbackQuery.Id, cancellationToken);
                    return true;
                }

                if (callbackQuery.Data == CallbackPrefixes.Back && currentScreen.ParentScreen != null)
                {
                    await NavigateToScreenCoreAsync(chatId.Value, currentScreen.ParentScreen.Id, cancellationToken);
                    await AnswerCallbackAsync(callbackQuery.Id, cancellationToken);
                    return true;
                }

                if (callbackQuery.Data != null
                    && CallbackMatcher.TryResolveHandler(currentScreen, callbackQuery.Data, out handler))
                {
                    screenIdBeforeHandler = currentScreen.Id;
                    context = BuildContext(chatId.Value, callbackQuery);
                }
            }

            // Invoke handlers outside the chat lock so Refresh/Navigate inside
            // handlers can acquire it without deadlocking.
            var handled = false;
            if (handler != null && callbackQuery.Data != null && context != null)
            {
                try
                {
                    handled = await handler(callbackQuery.Data, context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in event handler: {Message}", ex.Message);
                }
            }

            using (await LockChatAsync(chatId.Value))
            {
                await AnswerCallbackAsync(callbackQuery.Id, cancellationToken);

                // Refresh only when still on the same screen. Handlers that navigate
                // already displayed the target via NavigateToScreenAsync.
                if (handled
                    && screenIdBeforeHandler != null
                    && GetOrCreateNavigationState(chatId.Value).CurrentScreenId == screenIdBeforeHandler)
                {
                    await RefreshCurrentScreenCoreAsync(chatId.Value, cancellationToken);
                }

                return handled;
            }
        }

        /// <summary>
        /// Sets per-chat control state for a screen control.
        /// </summary>
        public void SetControlState<T>(long chatId, string screenId, string controlId, string property, T value)
        {
            var key = ScreenRenderContext.ControlStateKey(screenId, controlId, property);
            _stateMachine.SetState(chatId, key, value!);
        }

        /// <summary>
        /// Gets per-chat control state for a screen control.
        /// </summary>
        public T GetControlState<T>(long chatId, string screenId, string controlId, string property, T defaultValue = default!)
        {
            var key = ScreenRenderContext.ControlStateKey(screenId, controlId, property);
            return _stateMachine.GetState(chatId, key, defaultValue);
        }

        public void SetState<T>(long chatId, string key, T value) => _stateMachine.SetState(chatId, key, value);

        public T GetState<T>(long chatId, string key, T defaultValue = default!) =>
            _stateMachine.GetState(chatId, key, defaultValue);

        public void ClearState(long chatId) => _stateMachine.ClearState(chatId);

        public string? GetCurrentState(long chatId) => _stateMachine.GetState<string?>(chatId, StateKeys.Workflow);

        public void SetCurrentState(long chatId, string stateName) => _stateMachine.SetState(chatId, StateKeys.Workflow, stateName);

        public bool GetScreenById(string screenId, out Screen? screen) => TryGetScreen(screenId, out screen);

        private async Task NavigateToScreenCoreAsync(long chatId, string screenId, CancellationToken cancellationToken)
        {
            if (!_registeredScreens.TryGetValue(screenId, out var screen))
            {
                _logger.LogError("Screen not found: {ScreenId}", screenId);
                return;
            }

            var navState = GetOrCreateNavigationState(chatId);
            if (!string.IsNullOrEmpty(navState.CurrentScreenId)
                && navState.CurrentScreenId != screenId)
            {
                PerChatScreenViewKeys.Clear(_stateMachine, chatId, navState.CurrentScreenId);
            }

            navState.CurrentScreenId = screenId;
            _stateMachine.SetCurrentScreen(chatId, screenId);

            var message = await _renderer.DisplayAsync(chatId, screen, navState.LastMessageId, forceNewMessage: false, cancellationToken);
            navState.LastMessageId = message.MessageId;
        }

        private async Task RefreshCurrentScreenCoreAsync(long chatId, CancellationToken cancellationToken)
        {
            var navState = GetOrCreateNavigationState(chatId);
            if (string.IsNullOrEmpty(navState.CurrentScreenId)
                || !_registeredScreens.TryGetValue(navState.CurrentScreenId, out var screen))
            {
                return;
            }

            var message = await _renderer.DisplayAsync(chatId, screen, navState.LastMessageId, forceNewMessage: false, cancellationToken);
            navState.LastMessageId = message.MessageId;
        }

        private static Dictionary<string, object> BuildContext(long chatId, CallbackQuery callbackQuery)
        {
            return new FluentCallbackContext
            {
                ChatId = chatId,
                UserId = callbackQuery.From.Id,
                Username = callbackQuery.From.Username ?? "",
                FirstName = callbackQuery.From.FirstName ?? "",
                LastName = callbackQuery.From.LastName ?? "",
                MessageId = callbackQuery.Message?.MessageId ?? 0,
                CallbackQuery = callbackQuery
            }.ToDictionary();
        }

        private Task AnswerCallbackAsync(string callbackQueryId, CancellationToken cancellationToken) =>
            _botClient.AnswerCallbackQuery(callbackQueryId, cancellationToken: cancellationToken);

        private NavigationState GetOrCreateNavigationState(long chatId) =>
            _navigationStates.GetOrAdd(chatId, _ => new NavigationState());

        private Task<ChatLock> LockChatAsync(long chatId)
        {
            var gate = _chatLocks.GetOrAdd(chatId, _ => new SemaphoreSlim(1, 1));
            return ChatLock.AcquireAsync(gate);
        }

        private sealed class NavigationState
        {
            public string CurrentScreenId { get; set; } = string.Empty;
            public int LastMessageId { get; set; }
        }

        private sealed class ChatLock : IDisposable
        {
            private readonly SemaphoreSlim _gate;
            private bool _disposed;

            private ChatLock(SemaphoreSlim gate) => _gate = gate;

            public static async Task<ChatLock> AcquireAsync(SemaphoreSlim gate)
            {
                await gate.WaitAsync().ConfigureAwait(false);
                return new ChatLock(gate);
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _gate.Release();
            }
        }
    }
}
