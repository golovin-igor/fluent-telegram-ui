using System.Collections.Generic;
using System.Linq;
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
    public class ScreenManagerResetTests
    {
        private static ScreenManager BuildManager(out Mock<ITelegramBotClient> clientMock)
        {
            clientMock = new Mock<ITelegramBotClient>();
            clientMock.SetupSendMessage();
            clientMock.SetupAnswerCallbackQuery();
            return new ScreenManager(clientMock.Object, new Mock<ILogger<ScreenManager>>().Object);
        }

        [Fact]
        public async Task ResetChat_ClearsCurrentScreenAndNavigation()
        {
            var manager = BuildManager(out _);
            var main = new Screen { Id = "main", Title = "Main", Content = new Models.Message { Text = "Main" } };
            manager.RegisterScreen(main, isMainScreen: true);

            await manager.NavigateToScreenAsync(5, "main");
            manager.StateMachine.GetCurrentScreen(5).Should().Be("main");

            manager.ResetChat(5);

            manager.StateMachine.GetCurrentScreen(5).Should().BeNull();
            manager.StateMachine.GetAllState(5).Should().BeEmpty();
        }

        [Fact]
        public async Task RefreshCurrentScreen_AfterReset_DoesNothing()
        {
            var manager = BuildManager(out var clientMock);
            var sendCount = 0;
            clientMock.SetupSendMessage(() => sendCount++);

            var main = new Screen { Id = "main", Title = "Main", Content = new Models.Message { Text = "Main" } };
            manager.RegisterScreen(main, isMainScreen: true);

            await manager.NavigateToScreenAsync(5, "main");
            var countBeforeReset = sendCount;
            manager.ResetChat(5);

            await manager.RefreshCurrentScreenAsync(5);

            sendCount.Should().Be(countBeforeReset);
        }

        [Fact]
        public async Task StartCommand_ResetsBeforeNavigating()
        {
            // Simulate prior chat state (e.g. a stored culture) then /start resets it.
            var stateMachine = new StateMachine();
            var clientMock = new Mock<ITelegramBotClient>();
            clientMock.SetupSendMessage();
            clientMock.SetupAnswerCallbackQuery();
            var manager = new ScreenManager(clientMock.Object, new Mock<ILogger<ScreenManager>>().Object, stateMachine);
            var main = new Screen { Id = "main", Title = "Main", Content = new Models.Message { Text = "Main" } };
            manager.RegisterScreen(main, isMainScreen: true);

            stateMachine.SetState(11, "culture", "de");
            stateMachine.SetCurrentScreen(11, "somewhere");

            manager.ResetChat(11);
            stateMachine.SetState(11, StateKeys.Workflow, "initial");
            await manager.NavigateToMainScreenAsync(11);

            stateMachine.GetState<string>(11, "culture", "en").Should().Be("en");
            stateMachine.GetCurrentScreen(11).Should().Be("main");
        }
    }
}
