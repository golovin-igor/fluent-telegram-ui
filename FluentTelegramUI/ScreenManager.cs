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
        // ... existing code ...
        
        /// <summary>
        /// Tries to get a screen by its ID
        /// </summary>
        /// <param name="screenId">The screen ID</param>
        /// <param name="screen">The screen, if found</param>
        /// <returns>True if the screen was found, otherwise false</returns>
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
        
        // ... existing code ...
    }
} 