using FluentTelegramUI.Models;

namespace FluentTelegramUI.Builders
{
    /// <summary>
    /// Builder for creating Message instances using a fluent interface
    /// </summary>
    public class MessageBuilder
    {
        private readonly Message _message = new();
        
        /// <summary>
        /// Sets the text content of the message
        /// </summary>
        /// <param name="text">The text content</param>
        /// <returns>The MessageBuilder instance for method chaining</returns>
        public MessageBuilder WithText(string text)
        {
            _message.Text = text;
            return this;
        }
        
        /// <summary>
        /// Sets whether to parse Markdown in the message text
        /// </summary>
        /// <param name="parseMarkdown">Whether to parse Markdown</param>
        /// <returns>The MessageBuilder instance for method chaining</returns>
        public MessageBuilder WithMarkdown(bool parseMarkdown = true)
        {
            _message.ParseMarkdown = parseMarkdown;
            return this;
        }
        
        /// <summary>
        /// Sets the style of the message
        /// </summary>
        /// <param name="style">The style to apply to the message</param>
        /// <returns>The MessageBuilder instance for method chaining</returns>
        public MessageBuilder WithStyle(FluentStyle style)
        {
            _message.Style = style;
            
            // Update style of all buttons
            foreach (var button in _message.Buttons)
            {
                button.Style = style;
            }
            
            return this;
        }
        
        /// <summary>
        /// Sets the number of buttons per row
        /// </summary>
        /// <param name="buttonsPerRow">The number of buttons per row</param>
        /// <returns>The MessageBuilder instance for method chaining</returns>
        public MessageBuilder WithButtonsPerRow(int buttonsPerRow)
        {
            if (buttonsPerRow < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(buttonsPerRow), "Buttons per row must be at least 1");
            }
            
            _message.ButtonsPerRow = buttonsPerRow;
            return this;
        }
        
        /// <summary>
        /// Adds a button to the message
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="callbackData">The callback data to send when the button is clicked</param>
        /// <returns>The MessageBuilder instance for method chaining</returns>
        public MessageBuilder WithButton(string text, string callbackData)
        {
            var button = new Button
            {
                Text = text,
                CallbackData = callbackData,
                Style = _message.Style
            };
            
            _message.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Adds a URL button to the message
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="url">The URL to open when the button is clicked</param>
        /// <returns>The MessageBuilder instance for method chaining</returns>
        public MessageBuilder WithUrlButton(string text, string url)
        {
            var button = new Button
            {
                Text = text,
                Url = url,
                Style = _message.Style
            };
            
            _message.Buttons.Add(button);
            return this;
        }

        /// <summary>
        /// Sets native Rich Message HTML content and clears Markdown.
        /// </summary>
        public MessageBuilder WithRichHtml(string html)
        {
            _message.RichHtml = html;
            _message.RichMarkdown = null;
            return this;
        }

        /// <summary>
        /// Sets native Rich Message Markdown content and clears HTML.
        /// </summary>
        public MessageBuilder WithRichMarkdown(string markdown)
        {
            _message.RichMarkdown = markdown;
            _message.RichHtml = null;
            return this;
        }
        
        /// <summary>
        /// Builds and returns the Message instance
        /// </summary>
        /// <returns>The configured Message instance</returns>
        public Message Build()
        {
            if (string.IsNullOrEmpty(_message.Text) && !_message.HasRichContent)
            {
                throw new InvalidOperationException("Message text cannot be empty");
            }
            
            return _message;
        }
    }
} 