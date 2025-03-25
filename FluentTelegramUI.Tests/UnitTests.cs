using Xunit;
using FluentAssertions;
using FluentTelegramUI.Models;
using Telegram.Bot;

namespace FluentTelegramUI.Tests;

public class UnitTests
{
    [Fact]
    public void Test_Setup_ShouldWork()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        actual.Should().Be(expected);
    }

    public class TelegramBotBuilderTests
    {
        [Fact]
        public void TelegramBotBuilder_Build_ThrowsException_WhenTokenIsEmpty()
        {
            // Arrange
            var builder = new TelegramBotBuilder();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }
        
        [Fact]
        public void TelegramBotBuilder_WithToken_SetsToken()
        {
            // Arrange
            var builder = new TelegramBotBuilder();
            var token = "test_token";
            
            // Act
            builder.WithToken(token);
            
            // Assert
            // This is an indirect test since token is a private field
            // We can't assert directly on it, but we can verify the bot builds successfully
            var bot = builder.Build();
            Assert.NotNull(bot);
        }
        
        [Fact]
        public void TelegramBotBuilder_WithFluentUI_SetsDefaultStyle()
        {
            // Arrange
            var builder = new TelegramBotBuilder();
            var style = FluentStyle.Material;
            
            // Act
            builder.WithToken("test_token").WithFluentUI(style);
            
            // Assert
            // This is an indirect test since style is a private field
            var bot = builder.Build();
            Assert.NotNull(bot);
        }
    }
} 