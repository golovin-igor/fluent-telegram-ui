using System;
using System.Collections.Generic;

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
            string stateText = IsOn ? OnText : OffText;
            string fullCallbackData = $"{CallbackData}:{(IsOn ? "off" : "on")}";
            
            return new Message
            {
                Text = $"{Label}: {stateText}",
                Buttons = new() { new Button { Text = IsOn ? $"Turn {OffText}" : $"Turn {OnText}", CallbackData = fullCallbackData, Style = Style } }
            };
        }
    }

    /// <summary>
    /// Represents an image carousel control
    /// </summary>
    public class ImageCarousel : UIControl
    {
        /// <summary>
        /// The list of image URLs
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();
        
        /// <summary>
        /// The current image index
        /// </summary>
        public int CurrentIndex { get; set; } = 0;
        
        /// <summary>
        /// The captions for the images
        /// </summary>
        public List<string> Captions { get; set; } = new();
        
        /// <summary>
        /// Creates a new image carousel
        /// </summary>
        /// <param name="imageUrls">The list of image URLs</param>
        /// <param name="captions">The captions for the images</param>
        public ImageCarousel(List<string> imageUrls, List<string>? captions = null)
        {
            ImageUrls = imageUrls;
            
            if (captions != null)
            {
                Captions = captions;
                // Make sure we have captions for all images
                while (Captions.Count < ImageUrls.Count)
                {
                    Captions.Add(string.Empty);
                }
            }
            else
            {
                // No captions provided, initialize with empty strings
                Captions = new List<string>();
                for (int i = 0; i < ImageUrls.Count; i++)
                {
                    Captions.Add(string.Empty);
                }
            }
        }
        
        /// <summary>
        /// Converts the image carousel to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            var message = new Message
            {
                Text = Captions[CurrentIndex],
                ImageUrl = ImageUrls[CurrentIndex],
                Style = Style,
                Buttons = new()
            };
            
            // Add navigation buttons if we have multiple images
            if (ImageUrls.Count > 1)
            {
                if (CurrentIndex > 0)
                {
                    message.Buttons.Add(new Button { Text = "◀️ Prev", CallbackData = $"carousel:{Id}:prev" });
                }
                
                message.Buttons.Add(new Button { Text = $"{CurrentIndex + 1}/{ImageUrls.Count}", CallbackData = $"carousel:{Id}:info" });
                
                if (CurrentIndex < ImageUrls.Count - 1)
                {
                    message.Buttons.Add(new Button { Text = "Next ▶️", CallbackData = $"carousel:{Id}:next" });
                }
                
                message.ButtonsPerRow = 3;
            }
            
            return message;
        }
    }

    /// <summary>
    /// Represents a progress indicator control
    /// </summary>
    public class ProgressIndicator : UIControl
    {
        /// <summary>
        /// The label for the progress indicator
        /// </summary>
        public string Label { get; set; } = string.Empty;
        
        /// <summary>
        /// The current progress value (0-100)
        /// </summary>
        public int Progress { get; set; } = 0;
        
        /// <summary>
        /// Whether to show the percentage text
        /// </summary>
        public bool ShowPercentage { get; set; } = true;
        
        /// <summary>
        /// The character to use for filled progress
        /// </summary>
        public string FilledChar { get; set; } = "█";
        
        /// <summary>
        /// The character to use for empty progress
        /// </summary>
        public string EmptyChar { get; set; } = "░";
        
        /// <summary>
        /// The total width of the progress bar
        /// </summary>
        public int Width { get; set; } = 10;
        
        /// <summary>
        /// Creates a new progress indicator
        /// </summary>
        /// <param name="label">The progress label</param>
        /// <param name="progress">The initial progress value (0-100)</param>
        public ProgressIndicator(string label, int progress = 0)
        {
            Label = label;
            Progress = Math.Clamp(progress, 0, 100);
        }
        
        /// <summary>
        /// Converts the progress indicator to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            int filledCount = (int)Math.Round(Progress / 100.0 * Width);
            string progressBar = string.Concat(
                string.Concat(new string(FilledChar[0], filledCount)), 
                string.Concat(new string(EmptyChar[0], Width - filledCount))
            );
            
            string percentageText = ShowPercentage ? $" {Progress}%" : string.Empty;
            
            return new Message
            {
                Text = $"{Label}: {progressBar}{percentageText}",
                Style = Style
            };
        }
    }

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
            string buttonText = IsExpanded ? "▼ Collapse" : "▶ Expand";
            string callbackData = $"accordion:{Id}:{(IsExpanded ? "collapse" : "expand")}";
            
            return new Message
            {
                Text = IsExpanded ? $"*{Title}*\n\n{Content}" : $"*{Title}*",
                ParseMarkdown = true,
                Style = Style,
                Buttons = new() { new Button { Text = buttonText, CallbackData = callbackData } }
            };
        }
    }

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

    /// <summary>
    /// Represents text alignment options
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// Left alignment
        /// </summary>
        Left,
        
        /// <summary>
        /// Center alignment
        /// </summary>
        Center,
        
        /// <summary>
        /// Right alignment
        /// </summary>
        Right
    }

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
            var message = new Message
            {
                Text = $"{Label}: {(Value > 0 ? string.Concat(Enumerable.Repeat("⭐", Value)) : "Not rated")}",
                Style = Style,
                Buttons = new List<Button>()
            };
            
            // Add rating buttons
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