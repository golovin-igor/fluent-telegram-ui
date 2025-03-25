using Xunit;
using FluentAssertions;
using FluentTelegramUI.Models;
using Telegram.Bot;
using Moq;

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
        public void TelegramBotBuilder_WithToken_DoesNotThrowException()
        {
            // Arrange
            var builder = new TelegramBotBuilder();
            var token = "test_token";
            
            // Act & Assert
            var exception = Record.Exception(() => builder.WithToken(token));
            
            // No exception should be thrown
            exception.Should().BeNull();
        }
        
        [Fact]
        public void TelegramBotBuilder_WithFluentUI_DoesNotThrowException()
        {
            // Arrange
            var builder = new TelegramBotBuilder();
            var style = FluentStyle.Modern;
            
            // Act & Assert
            var exception = Record.Exception(() => builder.WithFluentUI(style));
            
            // No exception should be thrown
            exception.Should().BeNull();
        }
    }
} 