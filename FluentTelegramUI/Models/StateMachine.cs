using System;
using System.Collections.Generic;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Manages state for conversations across different users/chats
    /// </summary>
    public class StateMachine
    {
        private readonly Dictionary<long, Dictionary<string, object>> _states = new();
        private readonly Dictionary<long, string> _currentScreens = new();
        
        /// <summary>
        /// Gets the value of a state variable for the specified chat
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve</typeparam>
        /// <param name="chatId">The chat ID</param>
        /// <param name="key">The state variable key</param>
        /// <param name="defaultValue">The default value to return if the state variable doesn't exist</param>
        /// <returns>The state variable value or the default value</returns>
        public T GetState<T>(long chatId, string key, T defaultValue = default)
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
            if (!_states.TryGetValue(chatId, out var chatState))
            {
                chatState = new Dictionary<string, object>();
                _states[chatId] = chatState;
            }
            
            chatState[key] = value;
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
                   chatState.Remove(key);
        }
        
        /// <summary>
        /// Clears all state variables for the specified chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        public void ClearState(long chatId)
        {
            _states.Remove(chatId);
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
        /// Gets the ID of the current screen for the specified chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <returns>The screen ID or null if not set</returns>
        public string GetCurrentScreen(long chatId)
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
            var currentState = GetState<string>(chatId, "state");
            return currentState == stateName;
        }
        
        /// <summary>
        /// Sets the current state for the specified chat
        /// </summary>
        /// <param name="chatId">The chat ID</param>
        /// <param name="stateName">The state name</param>
        public void SetState(long chatId, string stateName)
        {
            SetState(chatId, "state", stateName);
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