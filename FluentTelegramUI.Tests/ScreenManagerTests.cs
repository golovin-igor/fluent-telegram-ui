using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class ScreenManagerTests
    {
        private readonly Mock<ITelegramBotClient> _botClientMock;
        private readonly Mock<ILogger<ScreenManager>> _loggerMock;
        private readonly ScreenManager _screenManager;
        
        public ScreenManagerTests()
        {
            _botClientMock = new Mock<ITelegramBotClient>();
            _loggerMock = new Mock<ILogger<ScreenManager>>();
            _screenManager = new ScreenManager(_botClientMock.Object, _loggerMock.Object);
        }
        
        [Fact]
        public void ScreenManager_Constructor_InitializesProperties()
        {
            // Assert
            _screenManager.MainScreen.Should().BeNull();
        }
        
        [Fact]
        public void ScreenManager_RegisterScreen_AddsScreenToCollection()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            
            // Act
            _screenManager.RegisterScreen(screen);
            
            // Assert
            bool success = _screenManager.GetScreenById(screen.Id, out var retrievedScreen);
            success.Should().BeTrue();
            retrievedScreen.Should().BeSameAs(screen);
        }
        
        [Fact]
        public void ScreenManager_RegisterScreen_WithMainScreen_SetsMainScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Main Screen" };
            
            // Act
            _screenManager.RegisterScreen(screen, true);
            
            // Assert
            _screenManager.MainScreen.Should().BeSameAs(screen);
        }
        
        [Fact]
        public void ScreenManager_SetMainScreen_UpdatesMainScreen()
        {
            // Arrange
            var screen1 = new Screen { Title = "Screen 1" };
            var screen2 = new Screen { Title = "Screen 2" };
            
            _screenManager.RegisterScreen(screen1, true);
            
            // Act
            _screenManager.SetMainScreen(screen2);
            
            // Assert
            _screenManager.MainScreen.Should().BeSameAs(screen2);
        }
        
        [Fact]
        public void ScreenManager_SetMainScreen_RegistersScreenIfNotAlreadyRegistered()
        {
            // Arrange
            var screen = new Screen { Title = "Main Screen" };
            
            // Act
            _screenManager.SetMainScreen(screen);
            
            // Assert
            _screenManager.MainScreen.Should().BeSameAs(screen);
            
            bool success = _screenManager.GetScreenById(screen.Id, out var retrievedScreen);
            success.Should().BeTrue();
            retrievedScreen.Should().BeSameAs(screen);
        }
        
        [Fact]
        public async Task ScreenManager_NavigateToMainScreen_NavigatesToMainScreen()
        {
            // Arrange
            var mainScreen = new Screen { Title = "Main Screen" };
            _screenManager.RegisterScreen(mainScreen, true);
            
            var messageCount = 0;
            _botClientMock.Setup(m => m.SendTextMessageAsync(
                It.IsAny<ChatId>(), 
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => messageCount++)
            .ReturnsAsync(new Telegram.Bot.Types.Message());
            
            // Act
            await _screenManager.NavigateToMainScreenAsync(123);
            
            // Assert
            messageCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_NavigateToScreen_NavigatesToSpecifiedScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            _screenManager.RegisterScreen(screen);
            
            var messageCount = 0;
            _botClientMock.Setup(m => m.SendTextMessageAsync(
                It.IsAny<ChatId>(), 
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => messageCount++)
            .ReturnsAsync(new Telegram.Bot.Types.Message());
            
            // Act
            await _screenManager.NavigateToScreenAsync(123, screen.Id);
            
            // Assert
            messageCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_NavigateToScreen_LogsError_WhenScreenNotFound()
        {
            // Arrange
            var nonExistentScreenId = "non-existent-id";
            
            var messageCount = 0;
            _botClientMock.Setup(m => m.SendTextMessageAsync(
                It.IsAny<ChatId>(), 
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => messageCount++)
            .ReturnsAsync(new Telegram.Bot.Types.Message());
            
            // Act
            await _screenManager.NavigateToScreenAsync(123, nonExistentScreenId);
            
            // Assert
            messageCount.Should().Be(0);
            
            // Verify that an error is logged
            _loggerMock.Invocations.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task ScreenManager_HandleCallbackQuery_HandlesScreenNavigation()
        {
            // Arrange
            var mainScreen = new Screen { Title = "Main Screen" };
            var settingsScreen = new Screen { Title = "Settings Screen" };
            
            _screenManager.RegisterScreen(mainScreen, true);
            _screenManager.RegisterScreen(settingsScreen);
            
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = $"screen:{settingsScreen.Id}",
                Message = new Message { Chat = new Chat { Id = 123 } }
            };
            
            var messageCount = 0;
            _botClientMock.Setup(m => m.SendTextMessageAsync(
                It.IsAny<ChatId>(), 
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => messageCount++)
            .ReturnsAsync(new Telegram.Bot.Types.Message());
            
            var callbackAnswerCount = 0;
            _botClientMock.Setup(m => m.AnswerCallbackQueryAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<string>(), 
                It.IsAny<int>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => callbackAnswerCount++)
            .ReturnsAsync(true);
            
            // Navigate to main screen first to set current screen
            await _screenManager.NavigateToScreenAsync(123, mainScreen.Id);
            
            // Reset counter after setup
            messageCount = 0;
            
            // Act
            await _screenManager.HandleCallbackQueryAsync(callbackQuery);
            
            // Assert
            messageCount.Should().Be(1); // One for navigation
            callbackAnswerCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_HandleCallbackQuery_HandlesBackNavigation()
        {
            // Arrange
            var mainScreen = new Screen { Title = "Main Screen" };
            var settingsScreen = new Screen { Title = "Settings Screen", ParentScreen = mainScreen };
            
            _screenManager.RegisterScreen(mainScreen, true);
            _screenManager.RegisterScreen(settingsScreen);
            
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = "back",
                Message = new Message { Chat = new Chat { Id = 123 } }
            };
            
            var messageCount = 0;
            _botClientMock.Setup(m => m.SendTextMessageAsync(
                It.IsAny<ChatId>(), 
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => messageCount++)
            .ReturnsAsync(new Telegram.Bot.Types.Message());
            
            var callbackAnswerCount = 0;
            _botClientMock.Setup(m => m.AnswerCallbackQueryAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<string>(), 
                It.IsAny<int>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => callbackAnswerCount++)
            .ReturnsAsync(true);
            
            // Navigate to settings screen first to set current screen
            await _screenManager.NavigateToScreenAsync(123, settingsScreen.Id);
            
            // Reset counter after setup
            messageCount = 0;
            
            // Act
            await _screenManager.HandleCallbackQueryAsync(callbackQuery);
            
            // Assert
            messageCount.Should().Be(1); // One for navigation back
            callbackAnswerCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_HandleCallbackQuery_InvokesEventHandler()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            var handlerCalled = false;
            
            screen.OnCallback("test_action", async (data) => {
                handlerCalled = true;
                return true;
            });
            
            _screenManager.RegisterScreen(screen);
            
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = "test_action",
                Message = new Message { Chat = new Chat { Id = 123 } }
            };
            
            var messageCount = 0;
            _botClientMock.Setup(m => m.SendTextMessageAsync(
                It.IsAny<ChatId>(), 
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => messageCount++)
            .ReturnsAsync(new Telegram.Bot.Types.Message());
            
            var callbackAnswerCount = 0;
            _botClientMock.Setup(m => m.AnswerCallbackQueryAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<string>(), 
                It.IsAny<int>(), 
                It.IsAny<CancellationToken>()))
            .Callback(() => callbackAnswerCount++)
            .ReturnsAsync(true);
            
            // Navigate to screen first to set current screen
            await _screenManager.NavigateToScreenAsync(123, screen.Id);
            
            // Reset counter after setup
            messageCount = 0;
            
            // Act
            await _screenManager.HandleCallbackQueryAsync(callbackQuery);
            
            // Assert
            handlerCalled.Should().BeTrue();
            callbackAnswerCount.Should().Be(1);
            messageCount.Should().Be(1); // Screen should refresh after handler returns true
        }
    }
} 