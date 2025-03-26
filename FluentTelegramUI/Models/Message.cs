using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a message in the Fluent Telegram UI
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The text content of the message
        /// </summary>
        public string Text { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether to parse Markdown in the message text
        /// </summary>
        public bool ParseMarkdown { get; set; } = true;
        
        /// <summary>
        /// The style of the message
        /// </summary>
        public FluentStyle Style { get; set; } = FluentStyle.Default;
        
        /// <summary>
        /// The buttons to display with the message
        /// </summary>
        public List<Button> Buttons { get; set; } = new();
        
        /// <summary>
        /// The number of buttons per row
        /// </summary>
        public int ButtonsPerRow { get; set; } = 1;
        
        /// <summary>
        /// The URL of an image to include with the message
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// The caption for the image, if different from the message text
        /// </summary>
        public string ImageCaption { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether this message contains an image
        /// </summary>
        public bool HasImage => !string.IsNullOrEmpty(ImageUrl);
        
        /// <summary>
        /// Converts the message buttons to an InlineKeyboardMarkup for Telegram API
        /// </summary>
        /// <returns>An InlineKeyboardMarkup</returns>
        public InlineKeyboardMarkup? ToInlineKeyboardMarkup()
        {
            if (Buttons.Count == 0)
            {
                return null;
            }
            
            var rows = new List<List<InlineKeyboardButton>>();
            var currentRow = new List<InlineKeyboardButton>();
            
            for (int i = 0; i < Buttons.Count; i++)
            {
                currentRow.Add(Buttons[i].ToInlineKeyboardButton());
                
                // Start a new row if we've reached the maximum buttons per row
                // or if this is the last button
                if ((i + 1) % ButtonsPerRow == 0 || i == Buttons.Count - 1)
                {
                    rows.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
            }
            
            return new InlineKeyboardMarkup(rows);
        }
        
        /// <summary>
        /// Gets the effective caption for an image message
        /// </summary>
        /// <returns>The caption text</returns>
        public string GetEffectiveImageCaption()
        {
            return !string.IsNullOrEmpty(ImageCaption) ? ImageCaption : Text;
        }
    }
} 