using FluentTelegramUI.Models;

namespace FluentTelegramUI.Builders
{
    /// <summary>
    /// Builder for creating Card instances using a fluent interface
    /// </summary>
    public class CardBuilder
    {
        private readonly Card _card = new();
        
        /// <summary>
        /// Sets the title of the card
        /// </summary>
        /// <param name="title">The title to display</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithTitle(string title)
        {
            _card.Title = title;
            return this;
        }
        
        /// <summary>
        /// Sets the description of the card
        /// </summary>
        /// <param name="description">The description to display</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithDescription(string description)
        {
            _card.Description = description;
            return this;
        }
        
        /// <summary>
        /// Sets the image URL of the card
        /// </summary>
        /// <param name="imageUrl">The URL of the image to display</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithImage(string imageUrl)
        {
            _card.ImageUrl = imageUrl;
            return this;
        }
        
        /// <summary>
        /// Adds additional information to the card
        /// </summary>
        /// <param name="key">The key or label for the information</param>
        /// <param name="value">The value of the information</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithInfo(string key, string value)
        {
            _card.AdditionalInfo[key] = value;
            return this;
        }
        
        /// <summary>
        /// Sets the price information in the card
        /// </summary>
        /// <param name="price">The price to display</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithPrice(string price)
        {
            return WithInfo("Price", price);
        }
        
        /// <summary>
        /// Adds an action button to the card
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="callbackData">The callback data to send when the button is clicked</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithActionButton(string text, string callbackData)
        {
            var button = new Button
            {
                Text = text,
                CallbackData = callbackData,
                Style = _card.Style
            };
            
            _card.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Adds a URL button to the card
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="url">The URL to open when the button is clicked</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithUrlButton(string text, string url)
        {
            var button = new Button
            {
                Text = text,
                Url = url,
                Style = _card.Style
            };
            
            _card.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Sets the style of the card
        /// </summary>
        /// <param name="style">The style to apply to the card</param>
        /// <returns>The CardBuilder instance for method chaining</returns>
        public CardBuilder WithStyle(FluentStyle style)
        {
            _card.Style = style;
            
            // Update style of all buttons
            foreach (var button in _card.Buttons)
            {
                button.Style = style;
            }
            
            return this;
        }
        
        /// <summary>
        /// Builds and returns the Card instance
        /// </summary>
        /// <returns>The configured Card instance</returns>
        public Card Build()
        {
            // Validate card
            if (string.IsNullOrEmpty(_card.Title))
            {
                throw new InvalidOperationException("Card title cannot be empty");
            }
            
            return _card;
        }
    }
} 