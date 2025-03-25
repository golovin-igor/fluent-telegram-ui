using System;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Base class for UI controls in the Fluent Telegram UI
    /// </summary>
    public abstract class UIControl
    {
        /// <summary>
        /// Unique identifier for the control
        /// </summary>
        public string Id { get; set; } = IdGenerator.GenerateShortId();
        
        /// <summary>
        /// The style of the control
        /// </summary>
        public FluentStyle Style { get; set; } = FluentStyle.Default;
        
        /// <summary>
        /// Converts the control to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public abstract Message ToMessage();
    }
    
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