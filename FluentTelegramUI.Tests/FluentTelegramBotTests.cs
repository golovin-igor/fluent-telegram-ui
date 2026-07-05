using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Xunit;
using Message = Telegram.Bot.Types.Message;

namespace FluentTelegramUI.Tests
{
    public class FluentTelegramBotTests
    {
        private static FluentTelegramBot BuildBot(Mock<ITelegramBotClient> clientMock)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ITelegramBotClient>(clientMock.Object);
            services.AddLogging();
            services.AddFluentTelegramUICoreForTesting();
            return new FluentTelegramBot(services.BuildServiceProvider());
        }

        [Fact]
        public async Task SendMessageAsync_SendsTextThroughClient()
        {
            var clientMock = new Mock<ITelegramBotClient>();
            string? capturedText = null;
            clientMock.Setup(m => m.SendRequest(
                    It.IsAny<IRequest<Message>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<Message>, CancellationToken>((req, _) =>
                {
                    capturedText = req.GetType().GetProperty("Text")?.GetValue(req) as string;
                })
                .ReturnsAsync(new Message());

            var bot = BuildBot(clientMock);

            await bot.SendMessageAsync(123, new Models.Message { Text = "Hello, bot!" });

            capturedText.Should().Be("Hello, bot!");
        }

        [Fact]
        public void RegisterScreen_And_TryGetScreen_RoundTrip()
        {
            var bot = BuildBot(new Mock<ITelegramBotClient>());
            var screen = new Screen { Title = "Settings" };

            bot.RegisterScreen(screen);

            bot.TryGetScreen(screen.Id, out var found).Should().BeTrue();
            found.Should().BeSameAs(screen);
        }

        [Fact]
        public void SetMainScreen_ExposesMainScreenProperty()
        {
            var bot = BuildBot(new Mock<ITelegramBotClient>());
            var screen = new Screen { Title = "Main" };

            bot.SetMainScreen(screen);

            bot.MainScreen.Should().BeSameAs(screen);
            bot.ScreenManager.Should().NotBeNull();
        }

        [Fact]
        public async Task NavigateToScreenAsync_DisplaysTargetScreen()
        {
            var clientMock = new Mock<ITelegramBotClient>();
            var sendCount = 0;
            clientMock.SetupSendMessage(() => sendCount++);

            var bot = BuildBot(clientMock);
            var main = new Screen { Id = "main", Title = "Main", Content = new Models.Message { Text = "Main" } };
            var details = new Screen { Id = "details", Title = "Details", Content = new Models.Message { Text = "Details" } };
            bot.RegisterScreen(main, isMainScreen: true);
            bot.RegisterScreen(details);

            await bot.NavigateToScreenAsync(42, "details");

            sendCount.Should().Be(1);
        }

        [Fact]
        public async Task NavigateToMainScreenAsync_DisplaysMainScreen()
        {
            var clientMock = new Mock<ITelegramBotClient>();
            var sendCount = 0;
            clientMock.SetupSendMessage(() => sendCount++);

            var bot = BuildBot(clientMock);
            var main = new Screen { Id = "main", Title = "Main", Content = new Models.Message { Text = "Main" } };
            bot.RegisterScreen(main, isMainScreen: true);

            await bot.NavigateToMainScreenAsync(7);

            sendCount.Should().Be(1);
        }

        [Fact]
        public void MainScreen_And_ScreenManager_PropertiesAreReadOnly()
        {
            var botType = typeof(FluentTelegramBot);
            botType.GetProperty("MainScreen")!.CanWrite.Should().BeFalse();
            botType.GetProperty("ScreenManager")!.CanWrite.Should().BeFalse();
        }
    }

    internal static class FluentTelegramBotTestServiceRegistration
    {
        public static IServiceCollection AddFluentTelegramUICoreForTesting(this IServiceCollection services)
        {
            services.Configure<DependencyInjection.FluentTelegramUIOptions>(o => o.BotToken = "test");
            services.AddSingleton<StateMachine>();
            services.AddSingleton<IStateStore>(sp => sp.GetRequiredService<StateMachine>());
            services.AddSingleton<Resources.ILocalizationService, Resources.LocalizationService>();
            services.AddSingleton<ScreenManager>();
            return services;
        }
    }
}
