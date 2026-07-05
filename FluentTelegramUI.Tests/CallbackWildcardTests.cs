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
    public class CallbackWildcardTests
    {
        private static (ScreenManager, Screen) BuildWithRatingHandler(out string handledData)
        {
            var clientMock = new Mock<ITelegramBotClient>();
            clientMock.SetupSendMessage();
            clientMock.SetupAnswerCallbackQuery();
            var logger = new Mock<ILogger<ScreenManager>>().Object;
            var manager = new ScreenManager(clientMock.Object, logger);

            string captured = string.Empty;
            var screen = new Screen { Id = "rate-screen", Title = "Rate" };
            screen.OnCallback("rate:*", async (data, context) =>
            {
                captured = data;
                return true;
            });
            manager.RegisterScreen(screen);
            handledData = captured;
            return (manager, screen);
        }

        [Fact]
        public async Task Wildcard_Handler_Matches_PrefixedCallback()
        {
            var (manager, screen) = BuildWithRatingHandler(out var captured);
            await manager.NavigateToScreenAsync(1, screen.Id);

            var handled = await manager.HandleCallbackQueryAsync(new CallbackQuery
            {
                Id = "cb",
                Data = "rate:4",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 1 } },
                From = new User { Id = 9 }
            }, CancellationToken.None);

            handled.Should().BeTrue();
        }

        [Fact]
        public async Task Wildcard_Handler_DoesNotMatch_UnrelatedCallback()
        {
            var (manager, screen) = BuildWithRatingHandler(out _);
            await manager.NavigateToScreenAsync(1, screen.Id);

            var handled = await manager.HandleCallbackQueryAsync(new CallbackQuery
            {
                Id = "cb",
                Data = "other:thing",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 1 } },
                From = new User { Id = 9 }
            }, CancellationToken.None);

            handled.Should().BeFalse();
        }

        [Fact]
        public async Task Exact_Handler_TakesPrecedence_Over_Wildcard()
        {
            var clientMock = new Mock<ITelegramBotClient>();
            clientMock.SetupSendMessage();
            clientMock.SetupAnswerCallbackQuery();
            var manager = new ScreenManager(clientMock.Object, new Mock<ILogger<ScreenManager>>().Object);

            string? matched = null;
            var screen = new Screen { Id = "s", Title = "S" };
            screen.OnCallback("rate:*", async (data, ctx) => { matched = "wild"; return true; });
            screen.OnCallback("rate:5", async (data, ctx) => { matched = "exact"; return true; });
            manager.RegisterScreen(screen);
            await manager.NavigateToScreenAsync(1, screen.Id);

            await manager.HandleCallbackQueryAsync(new CallbackQuery
            {
                Id = "cb",
                Data = "rate:5",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 1 } },
                From = new User { Id = 9 }
            }, CancellationToken.None);

            matched.Should().Be("exact");
        }
    }
}
