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
            _screenManager.MainScreen.Should().BeNull();
        }
        
        [Fact]
        public void ScreenManager_RegisterScreen_AddsScreenToCollection()
        {
            var screen = new Screen { Title = "Test Screen" };
            
            _screenManager.RegisterScreen(screen);
            
            bool success = _screenManager.GetScreenById(screen.Id, out var retrievedScreen);
            success.Should().BeTrue();
            retrievedScreen.Should().BeSameAs(screen);
        }
        
        [Fact]
        public void ScreenManager_RegisterScreen_WithMainScreen_SetsMainScreen()
        {
            var screen = new Screen { Title = "Main Screen" };
            
            _screenManager.RegisterScreen(screen, true);
            
            _screenManager.MainScreen.Should().BeSameAs(screen);
        }
        
        [Fact]
        public void ScreenManager_SetMainScreen_UpdatesMainScreen()
        {
            var screen1 = new Screen { Title = "Screen 1" };
            var screen2 = new Screen { Title = "Screen 2" };
            
            _screenManager.RegisterScreen(screen1, true);
            _screenManager.SetMainScreen(screen2);
            
            _screenManager.MainScreen.Should().BeSameAs(screen2);
        }
        
        [Fact]
        public void ScreenManager_SetMainScreen_RegistersScreenIfNotAlreadyRegistered()
        {
            var screen = new Screen { Title = "Main Screen" };
            
            _screenManager.SetMainScreen(screen);
            
            _screenManager.MainScreen.Should().BeSameAs(screen);
            
            bool success = _screenManager.GetScreenById(screen.Id, out var retrievedScreen);
            success.Should().BeTrue();
            retrievedScreen.Should().BeSameAs(screen);
        }
        
        [Fact]
        public async Task ScreenManager_NavigateToMainScreen_NavigatesToMainScreen()
        {
            var mainScreen = new Screen { Title = "Main Screen" };
            _screenManager.RegisterScreen(mainScreen, true);
            
            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);
            
            await _screenManager.NavigateToMainScreenAsync(123);
            
            messageCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_NavigateToScreen_NavigatesToSpecifiedScreen()
        {
            var screen = new Screen { Title = "Test Screen" };
            _screenManager.RegisterScreen(screen);
            
            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);
            
            await _screenManager.NavigateToScreenAsync(123, screen.Id);
            
            messageCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_NavigateToScreen_LogsError_WhenScreenNotFound()
        {
            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);
            
            await _screenManager.NavigateToScreenAsync(123, "non-existent-id");
            
            messageCount.Should().Be(0);
            _loggerMock.Invocations.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task ScreenManager_HandleCallbackQuery_HandlesScreenNavigation()
        {
            var mainScreen = new Screen { Title = "Main Screen" };
            var settingsScreen = new Screen { Title = "Settings Screen" };
            
            _screenManager.RegisterScreen(mainScreen, true);
            _screenManager.RegisterScreen(settingsScreen);
            
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = $"screen:{settingsScreen.Id}",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 123 } }
            };
            
            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);
            var callbackAnswerCount = 0;
            _botClientMock.SetupAnswerCallbackQuery(() => callbackAnswerCount++);
            
            await _screenManager.NavigateToScreenAsync(123, mainScreen.Id);
            messageCount = 0;
            
            await _screenManager.HandleCallbackQueryAsync(callbackQuery);
            
            messageCount.Should().Be(1);
            callbackAnswerCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_HandleCallbackQuery_HandlesBackNavigation()
        {
            var mainScreen = new Screen { Title = "Main Screen" };
            var settingsScreen = new Screen { Title = "Settings Screen", ParentScreen = mainScreen };
            
            _screenManager.RegisterScreen(mainScreen, true);
            _screenManager.RegisterScreen(settingsScreen);
            
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = "back",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 123 } }
            };
            
            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);
            var callbackAnswerCount = 0;
            _botClientMock.SetupAnswerCallbackQuery(() => callbackAnswerCount++);
            
            await _screenManager.NavigateToScreenAsync(123, settingsScreen.Id);
            messageCount = 0;
            
            await _screenManager.HandleCallbackQueryAsync(callbackQuery);
            
            messageCount.Should().Be(1);
            callbackAnswerCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenManager_HandleCallbackQuery_InvokesEventHandler()
        {
            var screen = new Screen { Title = "Test Screen" };
            var handlerCalled = false;
            Dictionary<string, object>? contextReceived = null;
            
            screen.OnCallback("test_action", async (data, context) => {
                handlerCalled = true;
                contextReceived = context;
                return true;
            });
            
            _screenManager.RegisterScreen(screen);
            
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = "test_action",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 123 } },
                From = new User { Id = 456, Username = "testuser", FirstName = "Test", LastName = "User" }
            };
            
            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);
            var callbackAnswerCount = 0;
            _botClientMock.SetupAnswerCallbackQuery(() => callbackAnswerCount++);
            
            await _screenManager.NavigateToScreenAsync(123, screen.Id);
            messageCount = 0;
            
            await _screenManager.HandleCallbackQueryAsync(callbackQuery);
            
            handlerCalled.Should().BeTrue();
            callbackAnswerCount.Should().Be(1);
            messageCount.Should().Be(1);
            
            contextReceived.Should().NotBeNull();
            contextReceived!["chatId"].Should().Be(123L);
            contextReceived["userId"].Should().Be(456L);
            contextReceived["username"].Should().Be("testuser");
            contextReceived["firstName"].Should().Be("Test");
            contextReceived["lastName"].Should().Be("User");
            contextReceived["callbackQuery"].Should().BeSameAs(callbackQuery);
        }
        
        [Fact]
        public async Task ScreenManager_HandleCallbackQuery_PassesContextToEventHandler()
        {
            var screen = new Screen { Title = "Test Screen" };
            Dictionary<string, object>? capturedContext = null;
            
            screen.OnCallback("test_action", async (data, context) => {
                capturedContext = context;
                return true;
            });
            
            _screenManager.RegisterScreen(screen);
            
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = "test_action",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 123 } },
                From = new User { 
                    Id = 456, 
                    Username = "testuser", 
                    FirstName = "Test", 
                    LastName = "User" 
                }
            };
            
            _botClientMock.SetupSendMessage();
            
            await _screenManager.NavigateToScreenAsync(123, screen.Id);
            await _screenManager.HandleCallbackQueryAsync(callbackQuery);
            
            capturedContext.Should().NotBeNull();
            capturedContext.Should().ContainKey("chatId");
            capturedContext.Should().ContainKey("userId");
            capturedContext.Should().ContainKey("username");
            capturedContext.Should().ContainKey("firstName");
            capturedContext.Should().ContainKey("lastName");
            capturedContext.Should().ContainKey("messageId");
            capturedContext.Should().ContainKey("callbackQuery");
            
            capturedContext!["chatId"].Should().Be(123L);
            capturedContext["userId"].Should().Be(456L);
            capturedContext["username"].Should().Be("testuser");
            capturedContext["firstName"].Should().Be("Test");
            capturedContext["lastName"].Should().Be("User");
            capturedContext["callbackQuery"].Should().BeSameAs(callbackQuery);
        }
    }
}
