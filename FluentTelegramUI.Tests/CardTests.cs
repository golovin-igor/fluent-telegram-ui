using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class CardTests
    {
        [Fact]
        public void CardBuilder_WithTitle_SetsTitleProperty()
        {
            // Arrange
            var builder = new CardBuilder();
            var title = "Product Card";
            
            // Act
            var card = builder
                .WithTitle(title)
                .Build();
            
            // Assert
            Assert.Equal(title, card.Title);
        }
        
        [Fact]
        public void CardBuilder_WithDescription_SetsDescriptionProperty()
        {
            // Arrange
            var builder = new CardBuilder();
            var description = "This is a beautiful product card";
            
            // Act
            var card = builder
                .WithTitle("Product Card")
                .WithDescription(description)
                .Build();
            
            // Assert
            Assert.Equal(description, card.Description);
        }
        
        [Fact]
        public void CardBuilder_WithImage_SetsImageUrlProperty()
        {
            // Arrange
            var builder = new CardBuilder();
            var imageUrl = "https://example.com/image.jpg";
            
            // Act
            var card = builder
                .WithTitle("Product Card")
                .WithImage(imageUrl)
                .Build();
            
            // Assert
            Assert.Equal(imageUrl, card.ImageUrl);
        }
        
        [Fact]
        public void CardBuilder_WithInfo_AddsToAdditionalInfoDictionary()
        {
            // Arrange
            var builder = new CardBuilder();
            
            // Act
            var card = builder
                .WithTitle("Product Card")
                .WithInfo("Color", "Red")
                .WithInfo("Size", "Medium")
                .Build();
            
            // Assert
            Assert.Equal(2, card.AdditionalInfo.Count);
            Assert.Equal("Red", card.AdditionalInfo["Color"]);
            Assert.Equal("Medium", card.AdditionalInfo["Size"]);
        }
        
        [Fact]
        public void CardBuilder_WithPrice_AddsToAdditionalInfoDictionary()
        {
            // Arrange
            var builder = new CardBuilder();
            var price = "$99.99";
            
            // Act
            var card = builder
                .WithTitle("Product Card")
                .WithPrice(price)
                .Build();
            
            // Assert
            Assert.Equal(1, card.AdditionalInfo.Count);
            Assert.Equal(price, card.AdditionalInfo["Price"]);
        }
        
        [Fact]
        public void CardBuilder_WithActionButton_AddsButtonToButtons()
        {
            // Arrange
            var builder = new CardBuilder();
            
            // Act
            var card = builder
                .WithTitle("Product Card")
                .WithActionButton("Buy Now", "buy")
                .Build();
            
            // Assert
            Assert.Single(card.Buttons);
            Assert.Equal("Buy Now", card.Buttons[0].Text);
            Assert.Equal("buy", card.Buttons[0].CallbackData);
            Assert.Null(card.Buttons[0].Url);
        }
        
        [Fact]
        public void CardBuilder_WithUrlButton_AddsButtonToButtons()
        {
            // Arrange
            var builder = new CardBuilder();
            
            // Act
            var card = builder
                .WithTitle("Product Card")
                .WithUrlButton("Visit Website", "https://example.com")
                .Build();
            
            // Assert
            Assert.Single(card.Buttons);
            Assert.Equal("Visit Website", card.Buttons[0].Text);
            Assert.Equal("https://example.com", card.Buttons[0].Url);
            Assert.Equal(string.Empty, card.Buttons[0].CallbackData);
        }
        
        [Fact]
        public void CardBuilder_WithStyle_SetsStyleProperty()
        {
            // Arrange
            var builder = new CardBuilder();
            var style = FluentStyle.Modern;
            
            // Act
            var card = builder
                .WithTitle("Card Title")
                .WithDescription("Card Description")
                .WithStyle(style)
                .Build();
            
            // Assert
            Assert.Equal(style, card.Style);
        }
        
        [Fact]
        public void CardBuilder_WithStyle_UpdatesButtonStyles()
        {
            // Arrange
            var builder = new CardBuilder();
            var style = FluentStyle.Modern;
            
            // Act
            var card = builder
                .WithTitle("Card Title")
                .WithDescription("Card Description")
                .WithActionButton("Button 1", "callback_1")
                .WithActionButton("Button 2", "callback_2")
                .WithStyle(style)
                .Build();
            
            // Assert
            Assert.All(card.Buttons, button => Assert.Equal(style, button.Style));
        }
        
        [Fact]
        public void CardBuilder_Build_ThrowsException_WhenTitleIsEmpty()
        {
            // Arrange
            var builder = new CardBuilder();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }
        
        [Fact]
        public void Card_ToMessageText_ReturnsFormattedText()
        {
            // Arrange
            var card = new CardBuilder()
                .WithTitle("Product Card")
                .WithDescription("This is a beautiful product card")
                .WithPrice("$99.99")
                .Build();
            
            // Act
            var messageText = card.ToMessageText();
            
            // Assert
            Assert.Contains("*Product Card*", messageText);
            Assert.Contains("This is a beautiful product card", messageText);
            Assert.Contains("Price: $99.99", messageText);
        }
    }
} 