namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents an accordion/collapsible section control
    /// </summary>
    public class Accordion : UIControl
    {
        /// <summary>
        /// The header title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The content text
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Whether the accordion is expanded
        /// </summary>
        public bool IsExpanded { get; set; } = false;

        /// <summary>
        /// Creates a new accordion
        /// </summary>
        /// <param name="title">The header title</param>
        /// <param name="content">The content text</param>
        /// <param name="isExpanded">Whether the accordion is initially expanded</param>
        public Accordion(string title, string content, bool isExpanded = false)
        {
            Title = title;
            Content = content;
            IsExpanded = isExpanded;
        }

        /// <summary>
        /// Converts the accordion to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            return ToMessage(new ScreenRenderContext(0, new Screen(), (_, defaultValue) => defaultValue!));
        }

        public override Message ToMessage(ScreenRenderContext context)
        {
            bool isExpanded = context.GetControlState(this, "expanded", IsExpanded);
            string buttonText = isExpanded ? "▼ Collapse" : "▶ Expand";
            string callbackData = $"{CallbackPrefixes.Accordion}{Id}:{(isExpanded ? AccordionActions.Collapse : AccordionActions.Expand)}";

            return new Message
            {
                Text = isExpanded ? $"<b>{Title}</b>\n\n{Content}" : $"<b>{Title}</b>",
                ParseMarkdown = true,
                Style = Style,
                Buttons = new() { new Button { Text = buttonText, CallbackData = callbackData } }
            };
        }
    }
}
