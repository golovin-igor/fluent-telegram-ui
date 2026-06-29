using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Handlers;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class ScreenUpdateHandlerTests
    {
        private readonly Mock<ILogger<ScreenUpdateHandler>> _loggerMock;
        private readonly Mock<ITelegramBotClient> _botClientMock;
        private readonly ScreenManager _screenManager;
        private readonly ScreenUpdateHandler _handler;
        
        public ScreenUpdateHandlerTests()
        {
            _loggerMock = new Mock<ILogger<ScreenUpdateHandler>>();
            _botClientMock = new Mock<ITelegramBotClient>();
            _botClientMock.SetupSendMessage();
            var screenManagerLogger = new Mock<ILogger<ScreenManager>>();
            _screenManager = new ScreenManager(_botClientMock.Object, screenManagerLogger.Object);
            _handler = new ScreenUpdateHandler(_loggerMock.Object, _screenManager);
        }
        
        [Fact]
        public void ScreenUpdateHandler_Constructor_InitializesProperties()
        {
            _handler.Should().BeAssignableTo<IFluentUpdateHandler>();
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandleTextMessageAsync_NavigatesToMainScreen_ForStartCommand()
        {
            var mainScreen = new Screen { Title = "Main" };
            _screenManager.RegisterScreen(mainScreen, true);

            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);

            var message = new Telegram.Bot.Types.Message
            {
                Text = "/start",
                Chat = new Chat { Id = 123 }
            };
            var cancellationToken = CancellationToken.None;
            
            await _handler.HandleTextMessageAsync(_botClientMock.Object, message, cancellationToken);
            
            messageCount.Should().Be(1);
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandleTextMessageAsync_DoesNotNavigate_ForOtherMessages()
        {
            var messageCount = 0;
            _botClientMock.SetupSendMessage(() => messageCount++);

            var message = new Telegram.Bot.Types.Message
            {
                Text = "Hello, world!",
                Chat = new Chat { Id = 123 }
            };
            var cancellationToken = CancellationToken.None;
            
            await _handler.HandleTextMessageAsync(_botClientMock.Object, message, cancellationToken);
            
            messageCount.Should().Be(0);
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandleCallbackQueryAsync_AnswersCallbackQuery()
        {
            var botClientMock = new Mock<ITelegramBotClient>();
            var callbackQuery = new CallbackQuery
            {
                Id = "callback-id",
                Data = "test_callback",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 123 } }
            };
            var cancellationToken = CancellationToken.None;
            
            botClientMock.SetupAnswerCallbackQuery();
            
            await _handler.HandleCallbackQueryAsync(botClientMock.Object, callbackQuery, cancellationToken);
            
            botClientMock.Verify(m => m.SendRequest(
                It.IsAny<IRequest<bool>>(),
                cancellationToken
            ), Times.Once);
        }
        
        [Fact]
        public async Task ScreenUpdateHandler_HandlePollingErrorAsync_LogsError()
        {
            var botClientMock = new Mock<ITelegramBotClient>();
            var exception = new Exception("Test error");
            var cancellationToken = CancellationToken.None;
            
            await _handler.HandlePollingErrorAsync(botClientMock.Object, exception, cancellationToken);
            
            _loggerMock.Invocations.Should().NotBeEmpty();
        }
    }
}
