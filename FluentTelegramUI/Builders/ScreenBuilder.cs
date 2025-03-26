using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FluentTelegramUI.Models;

namespace FluentTelegramUI.Builders
{
    /// <summary>
    /// Builder for creating screens in a fluent way
    /// </summary>
    public class ScreenBuilder
    {
        private readonly Screen _screen;
        private readonly FluentTelegramBot _bot;
        
        /// <summary>
        /// Initializes a new instance of the ScreenBuilder class
        /// </summary>
        /// <param name="bot">The bot instance</param>
        /// <param name="title">The screen title</param>
        public ScreenBuilder(FluentTelegramBot bot, string title)
        {
            _bot = bot;
            _screen = new Screen { Title = title };
            _bot.RegisterScreen(_screen);
        }
        
        /// <summary>
        /// Sets the content message of the screen
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="parseMarkdown">Whether to parse markdown in the text</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithContent(string text, bool parseMarkdown = true)
        {
            _screen.Content = new Message
            {
                Text = text,
                ParseMarkdown = parseMarkdown
            };
            return this;
        }
        
        /// <summary>
        /// Sets the content message of the screen
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithContent(Message message)
        {
            _screen.Content = message;
            return this;
        }
        
        /// <summary>
        /// Adds a button to the screen
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="callbackData">The callback data</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddButton(string text, string callbackData)
        {
            var button = new Button
            {
                Text = text,
                CallbackData = callbackData
            };
            
            _screen.Content.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Adds a text button control to the screen
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="callbackData">The callback data</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddTextButton(string text, string callbackData)
        {
            var button = new TextButton(text, callbackData);
            _screen.AddControl(button);
            return this;
        }
        
        /// <summary>
        /// Adds a button group control to the screen
        /// </summary>
        /// <param name="buttons">The buttons in the group</param>
        /// <param name="buttonsPerRow">The number of buttons per row</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddButtonGroup(List<Button> buttons, int buttonsPerRow = 2)
        {
            var buttonGroup = new ButtonGroup(buttons, buttonsPerRow);
            _screen.AddControl(buttonGroup);
            return this;
        }
        
        /// <summary>
        /// Adds a text input control to the screen
        /// </summary>
        /// <param name="label">The input label</param>
        /// <param name="placeholder">The placeholder text</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddTextInput(string label, string placeholder = "")
        {
            var input = new TextInput(label, placeholder);
            _screen.AddControl(input);
            return this;
        }
        
        /// <summary>
        /// Sets the number of buttons per row
        /// </summary>
        /// <param name="buttonsPerRow">The number of buttons per row</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithButtonsPerRow(int buttonsPerRow)
        {
            _screen.Content.ButtonsPerRow = buttonsPerRow;
            return this;
        }
        
        /// <summary>
        /// Adds a callback handler for a specific data
        /// </summary>
        /// <param name="callbackData">The callback data to handle</param>
        /// <param name="handler">The handler function</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder OnCallback(string callbackData, Func<string, Dictionary<string, object>, Task<bool>> handler)
        {
            _screen.OnCallback(callbackData, handler);
            return this;
        }
        
        /// <summary>
        /// Adds a text input handler for a specific state
        /// </summary>
        /// <param name="stateName">The state name when this handler should be active</param>
        /// <param name="handler">The handler function that receives the text input</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder OnTextInput(string stateName, Func<string, Dictionary<string, object>, Task<bool>> handler)
        {
            _screen.OnTextInput(stateName, handler);
            return this;
        }
        
        /// <summary>
        /// Creates a navigation button to another screen
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="targetScreenId">The target screen ID</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddNavigationButton(string text, string targetScreenId)
        {
            var button = new Button
            {
                Text = text,
                CallbackData = $"screen:{targetScreenId}"
            };
            
            _screen.Content.Buttons.Add(button);
            return this;
        }
        
        /// <summary>
        /// Sets the parent screen
        /// </summary>
        /// <param name="parentScreen">The parent screen</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithParent(Screen parentScreen)
        {
            _screen.ParentScreen = parentScreen;
            return this;
        }
        
        /// <summary>
        /// Sets whether back navigation is allowed
        /// </summary>
        /// <param name="allow">Whether to allow back navigation</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AllowBack(bool allow = true)
        {
            _screen.AllowBackNavigation = allow;
            return this;
        }
        
        /// <summary>
        /// Sets the text for the back button
        /// </summary>
        /// <param name="text">The text to display on the back button</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithBackButtonText(string text)
        {
            _screen.BackButtonText = text;
            return this;
        }
        
        /// <summary>
        /// Sets this screen as a main screen
        /// </summary>
        /// <param name="isMain">Whether this is a main screen</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AsMainScreen(bool isMain = true)
        {
            _screen.IsMainScreen = isMain;
            if (isMain)
            {
                _bot.SetMainScreen(_screen);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a toggle/switch control to the screen
        /// </summary>
        /// <param name="label">The label for the toggle</param>
        /// <param name="callbackData">The callback data prefix</param>
        /// <param name="isOn">The initial state</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddToggle(string label, string callbackData, bool isOn = false)
        {
            var toggle = new Toggle(label, callbackData, isOn);
            _screen.AddControl(toggle);
            return this;
        }
        
        /// <summary>
        /// Adds an image carousel control to the screen
        /// </summary>
        /// <param name="imageUrls">The list of image URLs</param>
        /// <param name="captions">The captions for the images</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddImageCarousel(List<string> imageUrls, List<string>? captions = null)
        {
            var carousel = new ImageCarousel(imageUrls, captions);
            _screen.AddControl(carousel);
            return this;
        }
        
        /// <summary>
        /// Adds a progress indicator control to the screen
        /// </summary>
        /// <param name="label">The label for the progress</param>
        /// <param name="progress">The initial progress value (0-100)</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddProgressIndicator(string label, int progress = 0)
        {
            var indicator = new ProgressIndicator(label, progress);
            _screen.AddControl(indicator);
            return this;
        }
        
        /// <summary>
        /// Adds an accordion/collapsible section control to the screen
        /// </summary>
        /// <param name="title">The title for the accordion</param>
        /// <param name="content">The content text</param>
        /// <param name="isExpanded">Whether the accordion is initially expanded</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddAccordion(string title, string content, bool isExpanded = false)
        {
            var accordion = new Accordion(title, content, isExpanded);
            _screen.AddControl(accordion);
            return this;
        }
        
        /// <summary>
        /// Adds a rich text component to the screen
        /// </summary>
        /// <param name="text">The text content</param>
        /// <param name="isBold">Whether to use bold formatting</param>
        /// <param name="isItalic">Whether to use italic formatting</param>
        /// <param name="isUnderlined">Whether to use underline formatting</param>
        /// <param name="alignment">The text alignment</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddRichText(string text, bool isBold = false, bool isItalic = false, bool isUnderlined = false, TextAlignment alignment = TextAlignment.Left)
        {
            var richText = new RichText(text)
            {
                IsBold = isBold,
                IsItalic = isItalic,
                IsUnderlined = isUnderlined,
                Alignment = alignment
            };
            
            _screen.AddControl(richText);
            return this;
        }
        
        /// <summary>
        /// Adds a rating control to the screen
        /// </summary>
        /// <param name="label">The label for the rating</param>
        /// <param name="callbackDataPrefix">The callback data prefix</param>
        /// <param name="initialValue">The initial rating value (0-5)</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder AddRating(string label, string callbackDataPrefix, int initialValue = 0)
        {
            var rating = new Rating(label, callbackDataPrefix, initialValue);
            _screen.AddControl(rating);
            return this;
        }
        
        /// <summary>
        /// Adds a carousel navigation handler to handle prev/next callbacks
        /// </summary>
        /// <param name="carouselId">The ID of the carousel to handle</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithCarouselHandler(string carouselId)
        {
            // Add handlers for carousel navigation
            _screen.OnCallback($"carousel:{carouselId}:prev", async (data, state) => 
            {
                var carouselControl = _screen.Controls.FirstOrDefault(c => c.Id == carouselId) as ImageCarousel;
                if (carouselControl != null && carouselControl.CurrentIndex > 0)
                {
                    carouselControl.CurrentIndex--;
                    await _bot.UpdateScreen(_screen.Id);
                }
                return true;
            });
            
            _screen.OnCallback($"carousel:{carouselId}:next", async (data, state) => 
            {
                var carouselControl = _screen.Controls.FirstOrDefault(c => c.Id == carouselId) as ImageCarousel;
                if (carouselControl != null && carouselControl.CurrentIndex < carouselControl.ImageUrls.Count - 1)
                {
                    carouselControl.CurrentIndex++;
                    await _bot.UpdateScreen(_screen.Id);
                }
                return true;
            });
            
            return this;
        }
        
        /// <summary>
        /// Adds a toggle handler to handle on/off callbacks
        /// </summary>
        /// <param name="toggleId">The ID of the toggle to handle</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithToggleHandler(string toggleId)
        {
            _screen.OnCallback($"{toggleId}:on", async (data, state) => 
            {
                var toggleControl = _screen.Controls.FirstOrDefault(c => c.Id == toggleId) as Toggle;
                if (toggleControl != null)
                {
                    toggleControl.IsOn = true;
                    await _bot.UpdateScreen(_screen.Id);
                }
                return true;
            });
            
            _screen.OnCallback($"{toggleId}:off", async (data, state) => 
            {
                var toggleControl = _screen.Controls.FirstOrDefault(c => c.Id == toggleId) as Toggle;
                if (toggleControl != null)
                {
                    toggleControl.IsOn = false;
                    await _bot.UpdateScreen(_screen.Id);
                }
                return true;
            });
            
            return this;
        }
        
        /// <summary>
        /// Adds an accordion handler to handle expand/collapse callbacks
        /// </summary>
        /// <param name="accordionId">The ID of the accordion to handle</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithAccordionHandler(string accordionId)
        {
            _screen.OnCallback($"accordion:{accordionId}:expand", async (data, state) => 
            {
                var accordionControl = _screen.Controls.FirstOrDefault(c => c.Id == accordionId) as Accordion;
                if (accordionControl != null)
                {
                    accordionControl.IsExpanded = true;
                    await _bot.UpdateScreen(_screen.Id);
                }
                return true;
            });
            
            _screen.OnCallback($"accordion:{accordionId}:collapse", async (data, state) => 
            {
                var accordionControl = _screen.Controls.FirstOrDefault(c => c.Id == accordionId) as Accordion;
                if (accordionControl != null)
                {
                    accordionControl.IsExpanded = false;
                    await _bot.UpdateScreen(_screen.Id);
                }
                return true;
            });
            
            return this;
        }
        
        /// <summary>
        /// Builds the screen
        /// </summary>
        /// <returns>The screen</returns>
        public Screen Build()
        {
            return _screen;
        }
    }
} 