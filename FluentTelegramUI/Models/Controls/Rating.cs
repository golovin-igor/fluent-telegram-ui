using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a rating control (1-5 stars)
    /// </summary>
    public class Rating : UIControl
    {
        /// <summary>
        /// The label for the rating
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// The current rating value (1-5)
        /// </summary>
        public int Value { get; set; } = 0;

        /// <summary>
        /// The callback data prefix
        /// </summary>
        public string CallbackDataPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Creates a new rating control
        /// </summary>
        /// <param name="label">The rating label</param>
        /// <param name="callbackDataPrefix">The callback data prefix</param>
        /// <param name="initialValue">The initial rating value (0-5)</param>
        public Rating(string label, string callbackDataPrefix, int initialValue = 0)
        {
            Label = label;
            CallbackDataPrefix = callbackDataPrefix;
            Value = Math.Clamp(initialValue, 0, 5);
        }

        /// <summary>
        /// Converts the rating to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            return ToMessage(new ScreenRenderContext(0, new Screen(), (_, defaultValue) => defaultValue!));
        }

        public override Message ToMessage(ScreenRenderContext context)
        {
            int value = context.GetControlState(this, "value", Value);
            var message = new Message
            {
                Text = $"{Label}: {(value > 0 ? string.Concat(Enumerable.Repeat("⭐", value)) : "Not rated")}",
                Style = Style,
                Buttons = new List<Button>()
            };

            for (int i = 1; i <= 5; i++)
            {
                message.Buttons.Add(new Button
                {
                    Text = i.ToString(),
                    CallbackData = $"{CallbackDataPrefix}:{i}"
                });
            }

            message.ButtonsPerRow = 5;
            return message;
        }
    }
}
