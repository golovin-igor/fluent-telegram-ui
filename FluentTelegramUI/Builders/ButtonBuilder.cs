using FluentTelegramUI.Models;

namespace FluentTelegramUI.Builders
{
    /// <summary>
    /// Builder for creating Button instances using a fluent interface
    /// </summary>
    public class ButtonBuilder
    {
        private readonly Button _button = new();
        
        /// <summary>
        /// Sets the text of the button
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <returns>The ButtonBuilder instance for method chaining</returns>
        public ButtonBuilder WithText(string text)
        {
            _button.Text = text;
            return this;
        }
        
        /// <summary>
        /// Sets the callback data of the button
        /// </summary>
        /// <param name="callbackData">The callback data to send when the button is clicked</param>
        /// <returns>The ButtonBuilder instance for method chaining</returns>
        public ButtonBuilder WithCallbackData(string callbackData)
        {
            _button.CallbackData = callbackData;
            return this;
        }
        
        /// <summary>
        /// Sets the URL of the button
        /// </summary>
        /// <param name="url">The URL to open when the button is clicked</param>
        /// <returns>The ButtonBuilder instance for method chaining</returns>
        public ButtonBuilder WithUrl(string url)
        {
            _button.Url = url;
            return this;
        }
        
        /// <summary>
        /// Sets the style of the button
        /// </summary>
        /// <param name="style">The style to apply to the button</param>
        /// <returns>The ButtonBuilder instance for method chaining</returns>
        public ButtonBuilder WithStyle(FluentStyle style)
        {
            _button.Style = style;
            return this;
        }
        
        /// <summary>
        /// Builds and returns the Button instance
        /// </summary>
        /// <returns>The configured Button instance</returns>
        public Button Build()
        {
            // Validate button
            if (string.IsNullOrEmpty(_button.Text))
            {
                throw new InvalidOperationException("Button text cannot be empty");
            }
            
            if (string.IsNullOrEmpty(_button.CallbackData) && string.IsNullOrEmpty(_button.Url))
            {
                throw new InvalidOperationException("Button must have either a callback data or URL");
            }
            
            return _button;
        }
    }
} 