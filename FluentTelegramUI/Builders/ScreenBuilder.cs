using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentTelegramUI.Models;

namespace FluentTelegramUI.Builders
{
    /// <summary>
    /// Builder for creating screens in a fluent way
    /// </summary>
    public class ScreenBuilder
    {
        private readonly Screen _screen;
        private readonly FluentTelegramBot _bot;
        
        /// <summary>
        /// Initializes a new instance of the ScreenBuilder class
        /// </summary>
        /// <param name="bot">The bot instance</param>
        /// <param name="title">The screen title</param>
        public ScreenBuilder(FluentTelegramBot bot, string title)
        {
            _bot = bot;
            _screen = new Screen { Title = title };
            _bot.RegisterScreen(_screen);
        }
        
        /// <summary>
        /// Sets the content message of the screen
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="parseMarkdown">Whether to parse markdown in the text</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithContent(string text, bool parseMarkdown = true)
        {
            _screen.Content = new Message
            {
                Text = text,
                ParseMarkdown = parseMarkdown
            };
            return this;
        }
        
        /// <summary>
        /// Sets the content message of the screen
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithContent(Message message)
        {
            _screen.Content = message;
            return this;
        }
        
        /// <summary>
        /// Adds a button to the screen
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="callbackData">The callback data</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddButton(string text, string callbackData)
        {
            var button = new Button
            {
                Text = text,
                CallbackData = callbackData
            };
            
            _screen.Content.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Adds a text button control to the screen
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="callbackData">The callback data</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddTextButton(string text, string callbackData)
        {
            var button = new TextButton(text, callbackData);
            _screen.AddControl(button);
            return this;
        }
        
        /// <summary>
        /// Adds a button group control to the screen
        /// </summary>
        /// <param name="buttons">The buttons in the group</param>
        /// <param name="buttonsPerRow">The number of buttons per row</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddButtonGroup(List<Button> buttons, int buttonsPerRow = 2)
        {
            var buttonGroup = new ButtonGroup(buttons, buttonsPerRow);
            _screen.AddControl(buttonGroup);
            return this;
        }
        
        /// <summary>
        /// Adds a text input control to the screen
        /// </summary>
        /// <param name="label">The input label</param>
        /// <param name="placeholder">The placeholder text</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddTextInput(string label, string placeholder = "")
        {
            var input = new TextInput(label, placeholder);
            _screen.AddControl(input);
            return this;
        }
        
        /// <summary>
        /// Sets the number of buttons per row
        /// </summary>
        /// <param name="buttonsPerRow">The number of buttons per row</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithButtonsPerRow(int buttonsPerRow)
        {
            _screen.Content.ButtonsPerRow = buttonsPerRow;
            return this;
        }
        
        /// <summary>
        /// Adds a callback handler for a specific data
        /// </summary>
        /// <param name="callbackData">The callback data to handle</param>
        /// <param name="handler">The handler function</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder OnCallback(string callbackData, Func<string, Dictionary<string, object>, Task<bool>> handler)
        {
            _screen.OnCallback(callbackData, handler);
            return this;
        }
        
        /// <summary>
        /// Adds a text input handler for a specific state
        /// </summary>
        /// <param name="stateName">The state name when this handler should be active</param>
        /// <param name="handler">The handler function that receives the text input</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder OnTextInput(string stateName, Func<string, Dictionary<string, object>, Task<bool>> handler)
        {
            _screen.OnTextInput(stateName, handler);
            return this;
        }
        
        /// <summary>
        /// Creates a navigation button to another screen
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="targetScreenId">The target screen ID</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddNavigationButton(string text, string targetScreenId)
        {
            var button = new Button
            {
                Text = text,
                CallbackData = $"screen:{targetScreenId}"
            };
            
            _screen.Content.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Sets the parent screen
        /// </summary>
        /// <param name="parentScreen">The parent screen</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithParent(Screen parentScreen)
        {
            _screen.ParentScreen = parentScreen;
            return this;
        }
        
        /// <summary>
        /// Sets whether back navigation is allowed
        /// </summary>
        /// <param name="allow">Whether to allow back navigation</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AllowBack(bool allow = true)
        {
            _screen.AllowBackNavigation = allow;
            return this;
        }
        
        /// <summary>
        /// Sets the text for the back button
        /// </summary>
        /// <param name="text">The text to display on the back button</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithBackButtonText(string text)
        {
            _screen.BackButtonText = text;
            return this;
        }
        
        /// <summary>
        /// Sets this screen as a main screen
        /// </summary>
        /// <param name="isMain">Whether this is a main screen</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AsMainScreen(bool isMain = true)
        {
            _screen.IsMainScreen = isMain;
            if (isMain)
            {
                _bot.SetMainScreen(_screen);
            }
            return this;
        }
        
        /// <summary>
        /// Gets the built screen
        /// </summary>
        /// <returns>The screen</returns>
        public Screen Build()
        {
            return _screen;
        }
    }
} 