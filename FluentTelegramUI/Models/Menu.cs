using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a menu UI component
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// The title of the menu
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// The buttons in the menu
        /// </summary>
        public List<Button> Buttons { get; set; } = new();
        
        /// <summary>
        /// The style of the menu
        /// </summary>
        public FluentStyle Style { get; set; } = FluentStyle.Default;
        
        /// <summary>
        /// The number of buttons per row
        /// </summary>
        public int ButtonsPerRow { get; set; } = 1;
        
        /// <summary>
        /// Converts the menu to an InlineKeyboardMarkup for Telegram API
        /// </summary>
        /// <returns>An InlineKeyboardMarkup</returns>
        public InlineKeyboardMarkup ToInlineKeyboardMarkup()
        {
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
    }
} 