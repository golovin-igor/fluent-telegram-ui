using System.Collections.Generic;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a button group control
    /// </summary>
    public class ButtonGroup : UIControl
    {
        /// <summary>
        /// The buttons in the group
        /// </summary>
        public List<Button> Buttons { get; set; } = new();

        /// <summary>
        /// The number of buttons per row
        /// </summary>
        public int ButtonsPerRow { get; set; } = 1;

        /// <summary>
        /// Creates a new button group
        /// </summary>
        /// <param name="buttons">The buttons in the group</param>
        /// <param name="buttonsPerRow">The number of buttons per row</param>
        public ButtonGroup(List<Button> buttons, int buttonsPerRow = 1)
        {
            Buttons = buttons;
            ButtonsPerRow = buttonsPerRow;
        }

        /// <summary>
        /// Adds a button to the group
        /// </summary>
        /// <param name="button">The button to add</param>
        /// <returns>The button group instance for method chaining</returns>
        public ButtonGroup AddButton(Button button)
        {
            Buttons.Add(button);
            return this;
        }

        /// <summary>
        /// Converts the button group to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            return new Message
            {
                Text = string.Empty,
                Buttons = Buttons,
                ButtonsPerRow = ButtonsPerRow,
                Style = Style
            };
        }
    }
}
