namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a toggle/switch control
    /// </summary>
    public class Toggle : UIControl
    {
        /// <summary>
        /// The label for the toggle
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Whether the toggle is on or off
        /// </summary>
        public bool IsOn { get; set; } = false;

        /// <summary>
        /// The callback data sent when the toggle is switched
        /// </summary>
        public string CallbackData { get; set; } = string.Empty;

        /// <summary>
        /// The text to display when the toggle is on
        /// </summary>
        public string OnText { get; set; } = "ON";

        /// <summary>
        /// The text to display when the toggle is off
        /// </summary>
        public string OffText { get; set; } = "OFF";

        /// <summary>
        /// Creates a new toggle
        /// </summary>
        /// <param name="label">The toggle label</param>
        /// <param name="callbackData">The callback data</param>
        /// <param name="isOn">The initial state</param>
        public Toggle(string label, string callbackData, bool isOn = false)
        {
            Label = label;
            CallbackData = callbackData;
            IsOn = isOn;
        }

        /// <summary>
        /// Converts the toggle to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            return ToMessage(new ScreenRenderContext(0, new Screen(), (_, defaultValue) => defaultValue!));
        }

        public override Message ToMessage(ScreenRenderContext context)
        {
            bool isOn = context.GetControlState(this, "isOn", IsOn);
            string stateText = isOn ? OnText : OffText;
            string fullCallbackData = $"{CallbackData}:{(isOn ? ToggleActions.Off : ToggleActions.On)}";

            return new Message
            {
                Text = $"{Label}: {stateText}",
                Buttons = new() { new Button { Text = isOn ? $"Turn {OffText}" : $"Turn {OnText}", CallbackData = fullCallbackData, Style = Style } }
            };
        }
    }
}
