using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Manages state for conversations across different users/chats. Thread-safe.
    /// </summary>
    public class StateMachine : IStateStore
    {
        private readonly ConcurrentDictionary<long, ConcurrentDictionary<string, object>> _states = new();
        private readonly ConcurrentDictionary<long, string> _currentScreens = new();

        /// <summary>
        /// Gets the value of a state variable for the specified chat
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve</typeparam>
        /// <param name="chatId">The chat ID</param>
        /// <param name="key">The state variable key</param>
        /// <param name="defaultValue">The default value to return if the state variable doesn't exist</param>
        /// <returns>The state variable value or the default value</returns>
        public T GetState<T>(long chatId, string key, T defaultValue = default!)
        {
            if (_states.TryGetValue(chatId, out var chatState) &&
                chatState.TryGetValue(key, out var value) &&
                value is T typedValue)
            {
                return typedValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets the value of a state variable for the specified chat
        /// </summary>
        /// <typeparam name="T">The type of value to set</typeparam>
        /// <param name="chatId">The chat ID</param>
        /// <param name="key">The state variable key</param>
        /// <param name="value">The value to set</param>
        public void SetState<T>(long chatId, string key, T value)
        {
            var chatState = _states.GetOrAdd(chatId, _ => new ConcurrentDictionary<string, object>());
            chatState[key] = value!;
        }

        /// <summary>
        /// Removes a state variable for the specified chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="key">The state variable key</param>
        /// <returns>True if the state variable was removed, false otherwise</returns>
        public bool RemoveState(long chatId, string key)
        {
            return _states.TryGetValue(chatId, out var chatState) &&
                   chatState.TryRemove(key, out _);
        }

        /// <summary>
        /// Clears all state variables for the specified chat. Does not reset the
        /// current screen — use <see cref="ResetChat"/> for a full reset.
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        public void ClearState(long chatId)
        {
            _states.TryRemove(chatId, out _);
        }

        /// <summary>
        /// Resets all state for the specified chat: state variables and the
        /// current screen tracker. Use this on <c>/start</c> instead of
        /// <see cref="ClearState"/> to avoid leaving stale screen pointers.
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        public void ResetChat(long chatId)
        {
            _states.TryRemove(chatId, out _);
            _currentScreens.TryRemove(chatId, out _);
        }

        /// <summary>
        /// Sets the ID of the current screen for the specified chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="screenId">The screen ID</param>
        public void SetCurrentScreen(long chatId, string screenId)
        {
            _currentScreens[chatId] = screenId;
        }

        /// <summary>
        /// Clears the current screen tracker for the specified chat.
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        public void ClearCurrentScreen(long chatId)
        {
            _currentScreens.TryRemove(chatId, out _);
        }

        /// <summary>
        /// Gets the ID of the current screen for the specified chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <returns>The screen ID or null if not set</returns>
        public string? GetCurrentScreen(long chatId)
        {
            return _currentScreens.TryGetValue(chatId, out var screenId) ? screenId : null;
        }

        /// <summary>
        /// Checks if the specified chat is in the given state
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="stateName">The state name to check</param>
        /// <returns>True if the chat is in the specified state, false otherwise</returns>
        public bool IsInState(long chatId, string stateName)
        {
            var currentState = GetState<string>(chatId, StateKeys.Workflow);
            return currentState == stateName;
        }

        /// <summary>
        /// Sets the current workflow state for the specified chat.
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="stateName">The state name</param>
        public void SetState(long chatId, string stateName)
        {
            SetState(chatId, StateKeys.Workflow, stateName);
        }

        /// <summary>
        /// Gets all state variables for the specified chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <returns>Dictionary of state variables or an empty dictionary if not found</returns>
        public Dictionary<string, object> GetAllState(long chatId)
        {
            return _states.TryGetValue(chatId, out var chatState)
                ? new Dictionary<string, object>(chatState)
                : new Dictionary<string, object>();
        }
    }
}
