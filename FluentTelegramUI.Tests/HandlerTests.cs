using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class HandlerTests
    {
        [Fact]
        public async Task DefaultFluentUpdateHandler_HandleTextMessageAsync_DoesNotThrowException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DefaultFluentUpdateHandler>>();
            var handler = new DefaultFluentUpdateHandler(loggerMock.Object);
            var botClientMock = new Mock<ITelegramBotClient>();
            var message = new Message { Text = "Hello, world!" };
            var cancellationToken = CancellationToken.None;
            
            // Act
            var exception = await Record.ExceptionAsync(() => 
                handler.HandleTextMessageAsync(botClientMock.Object, message, cancellationToken));
            
            // Assert
            exception.Should().BeNull();
        }
        
        [Fact]
        public void DefaultFluentUpdateHandler_HandleCallbackQueryAsync_ImplementsInterface()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DefaultFluentUpdateHandler>>();
            var handler = new DefaultFluentUpdateHandler(loggerMock.Object);
            
            // Assert
            // Verify the handler implements the interface method
            handler.Should().BeAssignableTo<IFluentUpdateHandler>();
            typeof(DefaultFluentUpdateHandler).Should().HaveMethod("HandleCallbackQueryAsync", 
                new[] { typeof(ITelegramBotClient), typeof(CallbackQuery), typeof(CancellationToken) });
        }
        
        [Fact]
        public async Task DefaultFluentUpdateHandler_HandlePollingErrorAsync_DoesNotThrowException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DefaultFluentUpdateHandler>>();
            var handler = new DefaultFluentUpdateHandler(loggerMock.Object);
            var botClientMock = new Mock<ITelegramBotClient>();
            var exception = new Exception("Test error");
            var cancellationToken = CancellationToken.None;
            
            // Act
            var thrownException = await Record.ExceptionAsync(() => 
                handler.HandlePollingErrorAsync(botClientMock.Object, exception, cancellationToken));
            
            // Assert
            thrownException.Should().BeNull();
        }
        
        [Fact]
        public async Task UpdateHandler_HandleUpdateAsync_CallsUpdateHandler()
        {
            // Arrange
            var updateHandlerCalled = false;
            var errorHandlerCalled = false;
            
            Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler = 
                (client, update, ct) => {
                    updateHandlerCalled = true;
                    return Task.CompletedTask;
                };
                
            Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler = 
                (client, ex, ct) => {
                    errorHandlerCalled = true;
                    return Task.CompletedTask;
                };
                
            var handler = new UpdateHandler(updateHandler, errorHandler);
            var botClientMock = new Mock<ITelegramBotClient>();
            var update = new Update();
            var cancellationToken = CancellationToken.None;
            
            // Act
            await handler.HandleUpdateAsync(botClientMock.Object, update, cancellationToken);
            
            // Assert
            updateHandlerCalled.Should().BeTrue();
            errorHandlerCalled.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateHandler_HandlePollingErrorAsync_CallsErrorHandler()
        {
            // Arrange
            var updateHandlerCalled = false;
            var errorHandlerCalled = false;
            
            Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler = 
                (client, update, ct) => {
                    updateHandlerCalled = true;
                    return Task.CompletedTask;
                };
                
            Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler = 
                (client, ex, ct) => {
                    errorHandlerCalled = true;
                    return Task.CompletedTask;
                };
                
            var handler = new UpdateHandler(updateHandler, errorHandler);
            var botClientMock = new Mock<ITelegramBotClient>();
            var exception = new Exception("Test error");
            var cancellationToken = CancellationToken.None;
            
            // Act
            await handler.HandlePollingErrorAsync(botClientMock.Object, exception, cancellationToken);
            
            // Assert
            updateHandlerCalled.Should().BeFalse();
            errorHandlerCalled.Should().BeTrue();
        }
        
        [Fact]
        public void UpdateHandler_Constructor_ThrowsException_WhenUpdateHandlerIsNull()
        {
            // Arrange
            Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler = 
                (client, ex, ct) => Task.CompletedTask;
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UpdateHandler(null, errorHandler));
        }
        
        [Fact]
        public void UpdateHandler_Constructor_ThrowsException_WhenErrorHandlerIsNull()
        {
            // Arrange
            Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler = 
                (client, update, ct) => Task.CompletedTask;
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UpdateHandler(updateHandler, null));
        }
    }
} 