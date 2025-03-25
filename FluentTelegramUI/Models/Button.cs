using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a button in the Fluent Telegram UI
    /// </summary>
    public class Button
    {
        /// <summary>
        /// The text displayed on the button
        /// </summary>
        public string Text { get; set; } = string.Empty;
        
        /// <summary>
        /// The callback data sent when the button is clicked
        /// </summary>
        public string CallbackData { get; set; } = string.Empty;
        
        /// <summary>
        /// The URL to open when the button is clicked (for URL buttons)
        /// </summary>
        public string? Url { get; set; }
        
        /// <summary>
        /// The style of the button
        /// </summary>
        public FluentStyle Style { get; set; } = FluentStyle.Default;
        
        /// <summary>
        /// Converts the button to an InlineKeyboardButton for Telegram API
        /// </summary>
        /// <returns>An InlineKeyboardButton</returns>
        public InlineKeyboardButton ToInlineKeyboardButton()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                return InlineKeyboardButton.WithUrl(Text, Url);
            }
            
            return InlineKeyboardButton.WithCallbackData(Text, CallbackData);
        }
    }
} 