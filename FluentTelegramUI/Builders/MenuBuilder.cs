using FluentTelegramUI.Models;

namespace FluentTelegramUI.Builders
{
    /// <summary>
    /// Builder for creating Menu instances using a fluent interface
    /// </summary>
    public class MenuBuilder
    {
        private readonly Menu _menu = new();
        
        /// <summary>
        /// Sets the title of the menu
        /// </summary>
        /// <param name="title">The title to display</param>
        /// <returns>The MenuBuilder instance for method chaining</returns>
        public MenuBuilder WithTitle(string title)
        {
            _menu.Title = title;
            return this;
        }
        
        /// <summary>
        /// Sets the number of buttons per row
        /// </summary>
        /// <param name="buttonsPerRow">The number of buttons per row</param>
        /// <returns>The MenuBuilder instance for method chaining</returns>
        public MenuBuilder WithButtonsPerRow(int buttonsPerRow)
        {
            if (buttonsPerRow < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(buttonsPerRow), "Buttons per row must be at least 1");
            }
            
            _menu.ButtonsPerRow = buttonsPerRow;
            return this;
        }
        
        /// <summary>
        /// Adds a button to the menu
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="callbackData">The callback data to send when the button is clicked</param>
        /// <returns>The MenuBuilder instance for method chaining</returns>
        public MenuBuilder AddButton(string text, string callbackData)
        {
            var button = new Button
            {
                Text = text,
                CallbackData = callbackData,
                Style = _menu.Style
            };
            
            _menu.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Adds a URL button to the menu
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="url">The URL to open when the button is clicked</param>
        /// <returns>The MenuBuilder instance for method chaining</returns>
        public MenuBuilder AddUrlButton(string text, string url)
        {
            var button = new Button
            {
                Text = text,
                Url = url,
                Style = _menu.Style
            };
            
            _menu.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Sets the style of the menu
        /// </summary>
        /// <param name="style">The style to apply to the menu</param>
        /// <returns>The MenuBuilder instance for method chaining</returns>
        public MenuBuilder WithStyle(FluentStyle style)
        {
            _menu.Style = style;
            
            // Update style of all buttons
            foreach (var button in _menu.Buttons)
            {
                button.Style = style;
            }
            
            return this;
        }
        
        /// <summary>
        /// Builds and returns the Menu instance
        /// </summary>
        /// <returns>The configured Menu instance</returns>
        public Menu Build()
        {
            // Validate menu
            if (string.IsNullOrEmpty(_menu.Title))
            {
                throw new InvalidOperationException("Menu title cannot be empty");
            }
            
            if (_menu.Buttons.Count == 0)
            {
                throw new InvalidOperationException("Menu must have at least one button");
            }
            
            return _menu;
        }
    }
} 