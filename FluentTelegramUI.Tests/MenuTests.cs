using System.Linq;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class MenuTests
    {
        [Fact]
        public void MenuBuilder_WithTitle_SetsTitleProperty()
        {
            // Arrange
            var builder = new MenuBuilder();
            var title = "Main Menu";
            
            // Act
            var menu = builder
                .WithTitle(title)
                .AddButton("Button", "button")
                .Build();
            
            // Assert
            Assert.Equal(title, menu.Title);
        }
        
        [Fact]
        public void MenuBuilder_AddButton_AddsButtonToButtons()
        {
            // Arrange
            var builder = new MenuBuilder();
            
            // Act
            var menu = builder
                .WithTitle("Main Menu")
                .AddButton("Profile", "profile")
                .AddButton("Settings", "settings")
                .Build();
            
            // Assert
            Assert.Equal(2, menu.Buttons.Count);
            Assert.Equal("Profile", menu.Buttons[0].Text);
            Assert.Equal("profile", menu.Buttons[0].CallbackData);
            Assert.Equal("Settings", menu.Buttons[1].Text);
            Assert.Equal("settings", menu.Buttons[1].CallbackData);
        }
        
        [Fact]
        public void MenuBuilder_AddUrlButton_AddsButtonToButtons()
        {
            // Arrange
            var builder = new MenuBuilder();
            
            // Act
            var menu = builder
                .WithTitle("Main Menu")
                .AddUrlButton("Visit Website", "https://example.com")
                .Build();
            
            // Assert
            Assert.Single(menu.Buttons);
            Assert.Equal("Visit Website", menu.Buttons[0].Text);
            Assert.Equal("https://example.com", menu.Buttons[0].Url);
            Assert.Equal(string.Empty, menu.Buttons[0].CallbackData);
        }
        
        [Fact]
        public void MenuBuilder_WithStyle_SetsStyleProperty()
        {
            // Arrange
            var builder = new MenuBuilder();
            var style = FluentStyle.Modern;
            
            // Act
            var menu = builder
                .WithTitle("Menu")
                .AddButton("Button 1", "callback_1")
                .WithStyle(style)
                .Build();
            
            // Assert
            Assert.Equal(style, menu.Style);
        }
        
        [Fact]
        public void MenuBuilder_WithStyle_UpdatesButtonStyles()
        {
            // Arrange
            var builder = new MenuBuilder();
            var style = FluentStyle.Modern;
            
            // Act
            var menu = builder
                .WithTitle("Menu")
                .AddButton("Button 1", "callback_1")
                .AddButton("Button 2", "callback_2")
                .WithStyle(style)
                .Build();
            
            // Assert
            Assert.All(menu.Buttons, button => Assert.Equal(style, button.Style));
        }
        
        [Fact]
        public void MenuBuilder_WithButtonsPerRow_SetsButtonsPerRowProperty()
        {
            // Arrange
            var builder = new MenuBuilder();
            var buttonsPerRow = 2;
            
            // Act
            var menu = builder
                .WithTitle("Main Menu")
                .AddButton("Button 1", "button1")
                .AddButton("Button 2", "button2")
                .WithButtonsPerRow(buttonsPerRow)
                .Build();
            
            // Assert
            Assert.Equal(buttonsPerRow, menu.ButtonsPerRow);
        }
        
        [Fact]
        public void MenuBuilder_WithButtonsPerRow_ThrowsException_WhenValueLessThanOne()
        {
            // Arrange
            var builder = new MenuBuilder()
                .WithTitle("Main Menu")
                .AddButton("Button", "button");
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithButtonsPerRow(0));
        }
        
        [Fact]
        public void MenuBuilder_Build_ThrowsException_WhenTitleIsEmpty()
        {
            // Arrange
            var builder = new MenuBuilder()
                .AddButton("Button", "button");
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }
        
        [Fact]
        public void MenuBuilder_Build_ThrowsException_WhenNoButtons()
        {
            // Arrange
            var builder = new MenuBuilder()
                .WithTitle("Main Menu");
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }
        
        [Fact]
        public void Menu_ToInlineKeyboardMarkup_ReturnsCorrectStructure_WithDefaultButtonsPerRow()
        {
            // Arrange
            var menu = new MenuBuilder()
                .WithTitle("Main Menu")
                .AddButton("Button 1", "button1")
                .AddButton("Button 2", "button2")
                .AddButton("Button 3", "button3")
                .Build();
            
            // Act
            var markup = menu.ToInlineKeyboardMarkup();
            var keyboard = markup.InlineKeyboard.ToArray();
            
            // Assert
            Assert.Equal(3, keyboard.Length);
            Assert.Single(keyboard[0]);
            Assert.Single(keyboard[1]);
            Assert.Single(keyboard[2]);
            Assert.Equal("Button 1", keyboard[0].ElementAt(0).Text);
            Assert.Equal("Button 2", keyboard[1].ElementAt(0).Text);
            Assert.Equal("Button 3", keyboard[2].ElementAt(0).Text);
        }
        
        [Fact]
        public void Menu_ToInlineKeyboardMarkup_ReturnsCorrectStructure_WithCustomButtonsPerRow()
        {
            // Arrange
            var menu = new MenuBuilder()
                .WithTitle("Main Menu")
                .AddButton("Button 1", "button1")
                .AddButton("Button 2", "button2")
                .AddButton("Button 3", "button3")
                .AddButton("Button 4", "button4")
                .WithButtonsPerRow(2)
                .Build();
            
            // Act
            var markup = menu.ToInlineKeyboardMarkup();
            var keyboard = markup.InlineKeyboard.ToArray();
            
            // Assert
            Assert.Equal(2, keyboard.Length);
            Assert.Equal(2, keyboard[0].Count());
            Assert.Equal(2, keyboard[1].Count());
            Assert.Equal("Button 1", keyboard[0].ElementAt(0).Text);
            Assert.Equal("Button 2", keyboard[0].ElementAt(1).Text);
            Assert.Equal("Button 3", keyboard[1].ElementAt(0).Text);
            Assert.Equal("Button 4", keyboard[1].ElementAt(1).Text);
        }
    }
} 