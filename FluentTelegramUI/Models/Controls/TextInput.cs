namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a text input control
    /// </summary>
    public class TextInput : UIControl
    {
        /// <summary>
        /// The label for the input
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// The placeholder text
        /// </summary>
        public string Placeholder { get; set; } = string.Empty;

        /// <summary>
        /// Creates a new text input
        /// </summary>
        /// <param name="label">The input label</param>
        /// <param name="placeholder">The placeholder text</param>
        public TextInput(string label, string placeholder = "")
        {
            Label = label;
            Placeholder = placeholder;
        }

        /// <summary>
        /// Converts the text input to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            return new Message
            {
                Text = Label,
                Style = Style
            };
        }
    }
}
