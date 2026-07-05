namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a text button control
    /// </summary>
    public class TextButton : UIControl
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
        /// Creates a new text button
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="callbackData">The callback data</param>
        public TextButton(string text, string callbackData)
        {
            Text = text;
            CallbackData = callbackData;
        }

        /// <summary>
        /// Converts the button to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            return new Message
            {
                Text = string.Empty,
                Buttons = new() { new Button { Text = Text, CallbackData = CallbackData, Style = Style } }
            };
        }
    }
}
