using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class ButtonTests
    {
        [Fact]
        public void ButtonBuilder_WithText_SetsTextProperty()
        {
            // Arrange
            var builder = new ButtonBuilder();
            var text = "Click Me";
            
            // Act
            var button = builder
                .WithText(text)
                .WithCallbackData("callback_data")
                .Build();
            
            // Assert
            Assert.Equal(text, button.Text);
        }
        
        [Fact]
        public void ButtonBuilder_WithCallbackData_SetsCallbackDataProperty()
        {
            // Arrange
            var builder = new ButtonBuilder();
            var callbackData = "callback_data";
            
            // Act
            var button = builder
                .WithText("Click Me")
                .WithCallbackData(callbackData)
                .Build();
            
            // Assert
            Assert.Equal(callbackData, button.CallbackData);
        }
        
        [Fact]
        public void ButtonBuilder_WithUrl_SetsUrlProperty()
        {
            // Arrange
            var builder = new ButtonBuilder();
            var url = "https://example.com";
            
            // Act
            var button = builder
                .WithText("Click Me")
                .WithUrl(url)
                .Build();
            
            // Assert
            Assert.Equal(url, button.Url);
        }
        
        [Fact]
        public void ButtonBuilder_WithStyle_SetsStyleProperty()
        {
            // Arrange
            var builder = new ButtonBuilder();
            var style = FluentStyle.Material;
            
            // Act
            var button = builder
                .WithText("Click Me")
                .WithCallbackData("callback_data")
                .WithStyle(style)
                .Build();
            
            // Assert
            Assert.Equal(style, button.Style);
        }
        
        [Fact]
        public void ButtonBuilder_Build_ThrowsException_WhenTextIsEmpty()
        {
            // Arrange
            var builder = new ButtonBuilder()
                .WithCallbackData("callback_data");
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }
        
        [Fact]
        public void ButtonBuilder_Build_ThrowsException_WhenCallbackDataAndUrlAreEmpty()
        {
            // Arrange
            var builder = new ButtonBuilder()
                .WithText("Click Me");
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }
        
        [Fact]
        public void Button_ToInlineKeyboardButton_ReturnsCallbackDataButton_WhenUrlIsNull()
        {
            // Arrange
            var button = new ButtonBuilder()
                .WithText("Click Me")
                .WithCallbackData("callback_data")
                .Build();
            
            // Act
            var inlineKeyboardButton = button.ToInlineKeyboardButton();
            
            // Assert
            Assert.Equal("Click Me", inlineKeyboardButton.Text);
            Assert.Equal("callback_data", inlineKeyboardButton.CallbackData);
            Assert.Null(inlineKeyboardButton.Url);
        }
        
        [Fact]
        public void Button_ToInlineKeyboardButton_ReturnsUrlButton_WhenUrlIsNotNull()
        {
            // Arrange
            var button = new ButtonBuilder()
                .WithText("Click Me")
                .WithUrl("https://example.com")
                .Build();
            
            // Act
            var inlineKeyboardButton = button.ToInlineKeyboardButton();
            
            // Assert
            Assert.Equal("Click Me", inlineKeyboardButton.Text);
            Assert.Equal("https://example.com", inlineKeyboardButton.Url);
            Assert.Null(inlineKeyboardButton.CallbackData);
        }
    }
} 