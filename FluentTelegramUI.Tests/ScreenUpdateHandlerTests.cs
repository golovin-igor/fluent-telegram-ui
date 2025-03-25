using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Handlers;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class ScreenUpdateHandlerTests
    {
        private readonly Mock<ILogger<ScreenUpdateHandler>> _loggerMock;
        private readonly Mock<ScreenManager> _screenManagerMock;
        private readonly ScreenUpdateHandler _handler;
        
        public ScreenUpdateHandlerTests()
        {
            _loggerMock = new Mock<ILogger<ScreenUpdateHandler>>();
            _screenManagerMock = new Mock<ScreenManager>();
            _handler = new ScreenUpdateHandler(_loggerMock.Object, _screenManagerMock.Object);
        }
        
        [Fact]
        public void ScreenUpdateHandler_Constructor_InitializesProperties()
        {
            // Assert
            _handler.Should().BeAssignableTo<IFluentUpdateHandler>();
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandleTextMessageAsync_NavigatesToMainScreen_ForStartCommand()
        {
            // Arrange
            var botClientMock = new Mock<ITelegramBotClient>();
            var message = new Telegram.Bot.Types.Message
            {
                Text = "/start",
                Chat = new Chat { Id = 123 }
            };
            var cancellationToken = CancellationToken.None;
            
            _screenManagerMock.Setup(m => m.NavigateToMainScreenAsync(
                It.IsAny<long>(), 
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);
            
            // Act
            await _handler.HandleTextMessageAsync(botClientMock.Object, message, cancellationToken);
            
            // Assert
            _screenManagerMock.Verify(m => m.NavigateToMainScreenAsync(123, cancellationToken), Times.Once);
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandleTextMessageAsync_DoesNotNavigate_ForOtherMessages()
        {
            // Arrange
            var botClientMock = new Mock<ITelegramBotClient>();
            var message = new Telegram.Bot.Types.Message
            {
                Text = "Hello, world!",
                Chat = new Chat { Id = 123 }
            };
            var cancellationToken = CancellationToken.None;
            
            // Act
            await _handler.HandleTextMessageAsync(botClientMock.Object, message, cancellationToken);
            
            // Assert
            _screenManagerMock.Verify(m => m.NavigateToMainScreenAsync(
                It.IsAny<long>(), 
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandleCallbackQueryAsync_AnswersCallbackQuery()
        {
            // Arrange
            var botClientMock = new Mock<ITelegramBotClient>();
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = "test_callback",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 123 } }
            };
            var cancellationToken = CancellationToken.None;
            
            botClientMock.Setup(m => m.AnswerCallbackQueryAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
            
            // Act
            await _handler.HandleCallbackQueryAsync(botClientMock.Object, callbackQuery, cancellationToken);
            
            // Assert
            botClientMock.Verify(m => m.AnswerCallbackQueryAsync(
                "callback-id", 
                null, 
                false, 
                null, 
                0, 
                cancellationToken
            ), Times.Once);
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandlePollingErrorAsync_LogsError()
        {
            // Arrange
            var botClientMock = new Mock<ITelegramBotClient>();
            var exception = new Exception("Test error");
            var cancellationToken = CancellationToken.None;
            
            // Act
            await _handler.HandlePollingErrorAsync(botClientMock.Object, exception, cancellationToken);
            
            // Assert
            _loggerMock.Invocations.Should().NotBeEmpty();
        }
    }
} 