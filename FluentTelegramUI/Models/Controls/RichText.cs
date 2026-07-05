using System.Linq;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a rich text component with formatting options
    /// </summary>
    public class RichText : UIControl
    {
        /// <summary>
        /// The text content
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use bold formatting
        /// </summary>
        public bool IsBold { get; set; } = false;

        /// <summary>
        /// Whether to use italic formatting
        /// </summary>
        public bool IsItalic { get; set; } = false;

        /// <summary>
        /// Whether to use underline formatting
        /// </summary>
        public bool IsUnderlined { get; set; } = false;

        /// <summary>
        /// The text alignment
        /// </summary>
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;

        /// <summary>
        /// Creates a new rich text component
        /// </summary>
        /// <param name="text">The text content</param>
        public RichText(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Converts the rich text to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            string formattedText = Text;

            // Apply formatting in a consistent order (bold first, then italic, then underline)
            if (IsBold)
            {
                formattedText = $"*{formattedText}*";
            }

            if (IsItalic)
            {
                formattedText = $"_{formattedText}_";
            }

            if (IsUnderlined)
            {
                formattedText = $"__{formattedText}__";
            }

            if (Alignment == TextAlignment.Center)
            {
                formattedText = formattedText.Split('\n').Select(line => $"      {line}      ").Aggregate((a, b) => $"{a}\n{b}");
            }
            else if (Alignment == TextAlignment.Right)
            {
                formattedText = formattedText.Split('\n').Select(line => $"                {line}").Aggregate((a, b) => $"{a}\n{b}");
            }

            return new Message
            {
                Text = formattedText,
                ParseMarkdown = true,
                Style = Style
            };
        }
    }
}
