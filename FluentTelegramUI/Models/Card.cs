using System.Collections.Generic;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a card-style UI component
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The title of the card
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// The description or content of the card
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// The URL of the image to display in the card
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Additional information to display in the card (e.g., price, date, etc.)
        /// </summary>
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();
        
        /// <summary>
        /// Buttons to display in the card
        /// </summary>
        public List<Button> Buttons { get; set; } = new();
        
        /// <summary>
        /// The style of the card
        /// </summary>
        public FluentStyle Style { get; set; } = FluentStyle.Default;
        
        /// <summary>
        /// Converts the card to a formatted message text
        /// </summary>
        /// <returns>The formatted message text</returns>
        public string ToMessageText()
        {
            var formattedText = new List<string>();
            
            // Add title
            if (!string.IsNullOrEmpty(Title))
            {
                formattedText.Add($"*{Title}*");
            }
            
            // Add description
            if (!string.IsNullOrEmpty(Description))
            {
                formattedText.Add(Description);
            }
            
            // Add additional info
            foreach (var info in AdditionalInfo)
            {
                formattedText.Add($"{info.Key}: {info.Value}");
            }
            
            return string.Join("\n\n", formattedText);
        }
    }
} 