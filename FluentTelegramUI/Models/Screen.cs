using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using shortid;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a UI screen in the Fluent Telegram UI
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// Unique identifier for the screen
        /// </summary>
        public string Id { get; set; } = IdGenerator.GenerateShortId();
        
        /// <summary>
        /// The title of the screen
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// The main content message of the screen
        /// </summary>
        public Message Content { get; set; } = new();
        
        /// <summary>
        /// Additional UI controls for the screen
        /// </summary>
        public List<UIControl> Controls { get; set; } = new();
        
        /// <summary>
        /// Event handlers for the screen
        /// </summary>
        public Dictionary<string, Func<string, Task<bool>>> EventHandlers { get; set; } = new();
        
        /// <summary>
        /// The parent screen to navigate back to (if any)
        /// </summary>
        public Screen? ParentScreen { get; set; }
        
        /// <summary>
        /// Whether this screen allows navigation back to the parent screen
        /// </summary>
        public bool AllowBackNavigation { get; set; } = true;
        
        /// <summary>
        /// Whether this screen is a main screen
        /// </summary>
        public bool IsMainScreen { get; set; }
        
        /// <summary>
        /// Text to display on the back button
        /// </summary>
        public string BackButtonText { get; set; } = "⬅️ Back";
        
        /// <summary>
        /// Adds an event handler for a specific callback data
        /// </summary>
        /// <param name="callbackData">The callback data to handle</param>
        /// <param name="handler">The handler function</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen OnCallback(string callbackData, Func<string, Task<bool>> handler)
        {
            EventHandlers[callbackData] = handler;
            return this;
        }
        
        /// <summary>
        /// Adds a text input handler for a specific state
        /// </summary>
        /// <param name="stateName">The state name when this handler should be active</param>
        /// <param name="handler">The handler function that receives the text input</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen OnTextInput(string stateName, Func<string, Task<bool>> handler)
        {
            EventHandlers[$"text_input:{stateName}"] = handler;
            return this;
        }
        
        /// <summary>
        /// Adds a control to the screen
        /// </summary>
        /// <param name="control">The control to add</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen AddControl(UIControl control)
        {
            Controls.Add(control);
            return this;
        }
        
        /// <summary>
        /// Adds multiple controls to the screen
        /// </summary>
        /// <param name="controls">The controls to add</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen AddControls(IEnumerable<UIControl> controls)
        {
            Controls.AddRange(controls);
            return this;
        }
        
        /// <summary>
        /// Sets the content message for the screen
        /// </summary>
        /// <param name="message">The message to set as content</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen WithContent(Message message)
        {
            Content = message;
            return this;
        }
        
        /// <summary>
        /// Sets the parent screen
        /// </summary>
        /// <param name="parent">The parent screen</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen WithParent(Screen parent)
        {
            ParentScreen = parent;
            return this;
        }
        
        /// <summary>
        /// Sets whether back navigation is allowed
        /// </summary>
        /// <param name="allow">Whether to allow back navigation</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen AllowBack(bool allow = true)
        {
            AllowBackNavigation = allow;
            return this;
        }
        
        /// <summary>
        /// Sets the text for the back button
        /// </summary>
        /// <param name="text">The text to display on the back button</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen WithBackButtonText(string text)
        {
            BackButtonText = text;
            return this;
        }
        
        /// <summary>
        /// Sets whether this screen is a main screen
        /// </summary>
        /// <param name="isMain">Whether this is a main screen</param>
        /// <returns>The screen instance for method chaining</returns>
        public Screen AsMainScreen(bool isMain = true)
        {
            IsMainScreen = isMain;
            return this;
        }
    }
} 