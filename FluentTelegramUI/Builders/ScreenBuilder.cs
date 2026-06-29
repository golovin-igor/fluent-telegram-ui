using FluentTelegramUI.Resources;
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
        /// Sets a stable screen identifier used for navigation callbacks.
        /// </summary>
        public ScreenBuilder WithId(string id)
        {
            _screen.Id = id;
            return this;
        }

        /// <summary>
        /// Sets the screen title from a localization resource key.
        /// </summary>
        public ScreenBuilder WithLocalizedTitle(string resourceKey)
        {
            _screen.TitleResourceKey = resourceKey;
            return this;
        }

        /// <summary>
        /// Sets the screen body from a localization resource key.
        /// </summary>
        public ScreenBuilder WithLocalizedContent(string resourceKey, bool parseMarkdown = true)
        {
            _screen.ContentResourceKey = resourceKey;
            _screen.Content.ParseMarkdown = parseMarkdown;
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
        /// Sets the chat culture when a callback is received and refreshes the screen.
        /// </summary>
        public ScreenBuilder OnSetCulture(string callbackData, string cultureName)
        {
            _screen.OnCallback(callbackData, async (_, state) =>
            {
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                _bot.StateMachine.SetState(chatId, LocalizationKeys.Culture, cultureName);
                await _bot.RefreshCurrentScreenAsync(chatId);
                return true;
            });
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
            var toggle = new Toggle(label, callbackData, isOn) { Id = callbackData };
            _screen.AddControl(toggle);
            WithToggleHandler(callbackData);
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
            WithCarouselHandler(carousel.Id);
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
            WithAccordionHandler(accordion.Id);
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
            var rating = new Rating(label, callbackDataPrefix, initialValue) { Id = callbackDataPrefix };
            _screen.AddControl(rating);
            _screen.OnCallback($"{callbackDataPrefix}:*", async (data, state) =>
            {
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                var parts = data.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var value))
                {
                    _bot.ScreenManager.SetControlState(chatId, _screen.Id, callbackDataPrefix, "value", value);
                    await _bot.ScreenManager.RefreshCurrentScreenAsync(chatId);
                }

                return true;
            });
            return this;
        }
        
        /// <summary>
        /// Adds a carousel navigation handler to handle prev/next callbacks
        /// </summary>
        /// <param name="carouselId">The ID of the carousel to handle</param>
        /// <returns>The screen builder instance for method chaining</returns>
        public ScreenBuilder WithCarouselHandler(string carouselId)
        {
            _screen.OnCallback($"carousel:{carouselId}:prev", async (data, state) =>
            {
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                var carouselControl = _screen.Controls.FirstOrDefault(c => c.Id == carouselId) as ImageCarousel;
                if (carouselControl != null)
                {
                    var index = _bot.ScreenManager.GetControlState(chatId, _screen.Id, carouselId, "index", carouselControl.CurrentIndex);
                    if (index > 0)
                    {
                        _bot.ScreenManager.SetControlState(chatId, _screen.Id, carouselId, "index", index - 1);
                        await _bot.ScreenManager.RefreshCurrentScreenAsync(chatId);
                    }
                }

                return true;
            });

            _screen.OnCallback($"carousel:{carouselId}:next", async (data, state) =>
            {
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                var carouselControl = _screen.Controls.FirstOrDefault(c => c.Id == carouselId) as ImageCarousel;
                if (carouselControl != null)
                {
                    var index = _bot.ScreenManager.GetControlState(chatId, _screen.Id, carouselId, "index", carouselControl.CurrentIndex);
                    if (index < carouselControl.ImageUrls.Count - 1)
                    {
                        _bot.ScreenManager.SetControlState(chatId, _screen.Id, carouselId, "index", index + 1);
                        await _bot.ScreenManager.RefreshCurrentScreenAsync(chatId);
                    }
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
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                _bot.ScreenManager.SetControlState(chatId, _screen.Id, toggleId, "isOn", true);
                await _bot.ScreenManager.RefreshCurrentScreenAsync(chatId);
                return true;
            });

            _screen.OnCallback($"{toggleId}:off", async (data, state) =>
            {
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                _bot.ScreenManager.SetControlState(chatId, _screen.Id, toggleId, "isOn", false);
                await _bot.ScreenManager.RefreshCurrentScreenAsync(chatId);
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
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                _bot.ScreenManager.SetControlState(chatId, _screen.Id, accordionId, "expanded", true);
                await _bot.ScreenManager.RefreshCurrentScreenAsync(chatId);
                return true;
            });

            _screen.OnCallback($"accordion:{accordionId}:collapse", async (data, state) =>
            {
                if (!TryGetChatId(state, out var chatId))
                {
                    return true;
                }

                _bot.ScreenManager.SetControlState(chatId, _screen.Id, accordionId, "expanded", false);
                await _bot.ScreenManager.RefreshCurrentScreenAsync(chatId);
                return true;
            });

            return this;
        }

        private static bool TryGetChatId(Dictionary<string, object> state, out long chatId)
        {
            if (state.TryGetValue("chatId", out var chatIdObj) && chatIdObj is long value)
            {
                chatId = value;
                return true;
            }

            chatId = 0;
            return false;
        }
        
        /// <summary>
        /// Builds the screen
        /// </summary>
        /// <returns>The screen</returns>
        public Screen Build()
        {
            _bot.RegisterScreen(_screen);
            if (_screen.IsMainScreen)
            {
                _bot.SetMainScreen(_screen);
            }

            return _screen;
        }
    }
} 