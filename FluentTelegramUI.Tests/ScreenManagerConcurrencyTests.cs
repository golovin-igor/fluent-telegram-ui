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
    public class ScreenManagerConcurrencyTests
    {
        private static ScreenManager BuildManager(out Mock<ITelegramBotClient> clientMock)
        {
            clientMock = new Mock<ITelegramBotClient>();
            clientMock.SetupSendMessage();
            clientMock.SetupAnswerCallbackQuery();
            return new ScreenManager(clientMock.Object, new Mock<ILogger<ScreenManager>>().Object);
        }

        [Fact]
        public async Task Parallel_Navigation_AcrossManyChats_DoesNotThrowAndTracksState()
        {
            var manager = BuildManager(out _);
            var main = new Screen { Id = "main", Title = "Main", Content = new Models.Message { Text = "Main" } };
            manager.RegisterScreen(main, isMainScreen: true);

            var chatIds = Enumerable.Range(1, 50).Select(i => (long)i).ToList();

            var tasks = chatIds.Select(chatId => Task.Run(async () =>
            {
                await manager.NavigateToScreenAsync(chatId, "main");
                for (int i = 0; i < 5; i++)
                {
                    await manager.HandleCallbackQueryAsync(new CallbackQuery
                    {
                        Id = $"{chatId}-{i}",
                        Data = "noop",
                        Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = chatId } },
                        From = new User { Id = chatId }
                    }, CancellationToken.None);
                }
            }));

            await Task.WhenAll(tasks);

            foreach (var chatId in chatIds)
            {
                manager.StateMachine.GetCurrentScreen(chatId).Should().Be("main");
            }
        }

        [Fact]
        public async Task Parallel_Updates_ForSameChat_SerializeWithoutCorruption()
        {
            var manager = BuildManager(out _);
            var main = new Screen { Id = "main", Title = "Main", Content = new Models.Message { Text = "Main" } };
            manager.RegisterScreen(main, isMainScreen: true);

            const long chatId = 999;
            await manager.NavigateToScreenAsync(chatId, "main");

            var tasks = Enumerable.Range(0, 30).Select(i => Task.Run(async () =>
            {
                await manager.HandleCallbackQueryAsync(new CallbackQuery
                {
                    Id = $"cb-{i}",
                    Data = "noop",
                    Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = chatId } },
                    From = new User { Id = chatId }
                }, CancellationToken.None);
                manager.StateMachine.SetState(chatId, $"key-{i}", i);
            }));

            await Task.WhenAll(tasks);

            // Every write should be present; no key lost to races.
            for (int i = 0; i < 30; i++)
            {
                manager.StateMachine.GetState<int>(chatId, $"key-{i}", -1).Should().Be(i);
            }
        }
    }
}
