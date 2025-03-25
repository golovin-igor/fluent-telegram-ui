using System;
using System.Linq;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class MessageTests
    {
        [Fact]
        public void MessageBuilder_WithText_SetsTextProperty()
        {
            // Arrange
            var builder = new MessageBuilder();
            var text = "Hello, World!";
            
            // Act
            var message = builder
                .WithText(text)
                .Build();
            
            // Assert
            Assert.Equal(text, message.Text);
        }
        
        [Fact]
        public void MessageBuilder_WithMarkdown_SetsParseMarkdownProperty()
        {
            // Arrange
            var builder = new MessageBuilder();
            
            // Act
            var message = builder
                .WithText("Hello, World!")
                .WithMarkdown(false)
                .Build();
            
            // Assert
            Assert.False(message.ParseMarkdown);
        }
        
        [Fact]
        public void MessageBuilder_WithStyle_SetsStyleProperty()
        {
            // Arrange
            var builder = new MessageBuilder();
            var style = FluentStyle.Modern;
            
            // Act
            var message = builder
                .WithText("Hello, world!")
                .WithStyle(style)
                .Build();
            
            // Assert
            Assert.Equal(style, message.Style);
        }
        
        [Fact]
        public void MessageBuilder_WithButton_AddsButtonToButtons()
        {
            // Arrange
            var builder = new MessageBuilder();
            
            // Act
            var message = builder
                .WithText("Hello, World!")
                .WithButton("Click Me", "click_me")
                .Build();
            
            // Assert
            Assert.Single(message.Buttons);
            Assert.Equal("Click Me", message.Buttons[0].Text);
            Assert.Equal("click_me", message.Buttons[0].CallbackData);
            Assert.Null(message.Buttons[0].Url);
        }
        
        [Fact]
        public void MessageBuilder_WithUrlButton_AddsButtonToButtons()
        {
            // Arrange
            var builder = new MessageBuilder();
            
            // Act
            var message = builder
                .WithText("Hello, World!")
                .WithUrlButton("Visit Website", "https://example.com")
                .Build();
            
            // Assert
            Assert.Single(message.Buttons);
            Assert.Equal("Visit Website", message.Buttons[0].Text);
            Assert.Equal("https://example.com", message.Buttons[0].Url);
            Assert.Equal(string.Empty, message.Buttons[0].CallbackData);
        }
        
        [Fact]
        public void MessageBuilder_WithButtonsPerRow_SetsButtonsPerRowProperty()
        {
            // Arrange
            var builder = new MessageBuilder();
            var buttonsPerRow = 2;
            
            // Act
            var message = builder
                .WithText("Hello, World!")
                .WithButton("Button 1", "button1")
                .WithButton("Button 2", "button2")
                .WithButtonsPerRow(buttonsPerRow)
                .Build();
            
            // Assert
            Assert.Equal(buttonsPerRow, message.ButtonsPerRow);
        }
        
        [Fact]
        public void MessageBuilder_WithButtonsPerRow_ThrowsException_WhenValueLessThanOne()
        {
            // Arrange
            var builder = new MessageBuilder()
                .WithText("Hello, World!");
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithButtonsPerRow(0));
        }
        
        [Fact]
        public void MessageBuilder_WithStyle_UpdatesButtonStyles()
        {
            // Arrange
            var builder = new MessageBuilder();
            var style = FluentStyle.Modern;
            
            // Act
            var message = builder
                .WithText("Hello, world!")
                .WithButton("Button 1", "callback_1")
                .WithButton("Button 2", "callback_2")
                .WithStyle(style)
                .Build();
            
            // Assert
            Assert.All(message.Buttons, button => Assert.Equal(style, button.Style));
        }
        
        [Fact]
        public void MessageBuilder_Build_ThrowsException_WhenTextIsEmpty()
        {
            // Arrange
            var builder = new MessageBuilder();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }
        
        [Fact]
        public void Message_ToInlineKeyboardMarkup_ReturnsNull_WhenNoButtons()
        {
            // Arrange
            var message = new MessageBuilder()
                .WithText("Hello, World!")
                .Build();
            
            // Act
            var markup = message.ToInlineKeyboardMarkup();
            
            // Assert
            Assert.Null(markup);
        }
        
        [Fact]
        public void Message_ToInlineKeyboardMarkup_ReturnsCorrectStructure_WithDefaultButtonsPerRow()
        {
            // Arrange
            var message = new MessageBuilder()
                .WithText("Hello, World!")
                .WithButton("Button 1", "button1")
                .WithButton("Button 2", "button2")
                .WithButton("Button 3", "button3")
                .Build();
            
            // Act
            var markup = message.ToInlineKeyboardMarkup();
            var keyboard = markup!.InlineKeyboard.ToArray();
            
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
        public void Message_ToInlineKeyboardMarkup_ReturnsCorrectStructure_WithCustomButtonsPerRow()
        {
            // Arrange
            var message = new MessageBuilder()
                .WithText("Hello, World!")
                .WithButton("Button 1", "button1")
                .WithButton("Button 2", "button2")
                .WithButton("Button 3", "button3")
                .WithButton("Button 4", "button4")
                .WithButtonsPerRow(2)
                .Build();
            
            // Act
            var markup = message.ToInlineKeyboardMarkup();
            var keyboard = markup!.InlineKeyboard.ToArray();
            
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