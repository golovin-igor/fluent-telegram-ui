using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI;
using FluentTelegramUI.Models;
using FluentTelegramUI.Resources;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Xunit;

namespace FluentTelegramUI.Tests;

public class TelegramBotBuilderIntegrationTests
{
    [Fact]
    public async Task Build_WithAddScreen_RegistersMainScreenAndProcessesStartCommand()
    {
        var botClientMock = new Mock<ITelegramBotClient>();
        var sendCount = 0;
        botClientMock.SetupSendMessage(() => sendCount++);

        var bot = new TelegramBotBuilder()
            .WithToken("123456789:ABCdefGHIjklMNOpqrsTUVwxyz")
            .WithBotClient(botClientMock.Object)
            .AddScreen("Main Menu", builder => builder
                .WithId("main")
                .WithContent("Hello from integration test"), isMainScreen: true)
            .Build();

        bot.MainScreen.Should().NotBeNull();
        bot.MainScreen!.Id.Should().Be("main");
        bot.TryGetScreen("main", out var screen).Should().BeTrue();
        screen.Should().NotBeNull();

        await bot.ProcessUpdateAsync(botClientMock.Object, new Update
        {
            Message = new Telegram.Bot.Types.Message
            {
                Text = "/start",
                Chat = new Chat { Id = 42 }
            }
        }, CancellationToken.None);

        sendCount.Should().Be(1);
    }

    [Fact]
    public async Task Build_WithNavigationButton_NavigatesBetweenScreens()
    {
        var botClientMock = new Mock<ITelegramBotClient>();
        var sendCount = 0;
        botClientMock.SetupSendMessage(() => sendCount++);

        var bot = new TelegramBotBuilder()
            .WithToken("123456789:ABCdefGHIjklMNOpqrsTUVwxyz")
            .WithBotClient(botClientMock.Object)
            .AddScreen("Menu", builder => builder
                .WithId("menu")
                .WithContent("Pick a screen")
                .AddNavigationButton("Details", "details"), isMainScreen: true)
            .AddScreen("Details", builder => builder
                .WithId("details")
                .WithContent("Details screen")
                .AddNavigationButton("Back", "menu"))
            .Build();

        await bot.ProcessUpdateAsync(botClientMock.Object, new Update
        {
            Message = new Telegram.Bot.Types.Message
            {
                Text = "/start",
                Chat = new Chat { Id = 7 }
            }
        }, CancellationToken.None);

        await bot.ProcessUpdateAsync(botClientMock.Object, new Update
        {
            CallbackQuery = new CallbackQuery
            {
                Id = "cb-1",
                Data = "screen:details",
                Message = new Telegram.Bot.Types.Message { Chat = new Chat { Id = 7 } }
            }
        }, CancellationToken.None);

        sendCount.Should().BeGreaterThanOrEqualTo(1);
        botClientMock.Verify(m => m.SendRequest(
            It.IsAny<IRequest<Telegram.Bot.Types.Message>>(),
            It.IsAny<CancellationToken>()), Times.AtLeast(2));
        botClientMock.Verify(m => m.SendRequest(
            It.IsAny<IRequest<bool>>(),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Build_WithLocalizedScreen_RendersGermanAfterCultureChange()
    {
        var botClientMock = new Mock<ITelegramBotClient>();
        string? capturedText = null;
        botClientMock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<Telegram.Bot.Types.Message>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Telegram.Bot.Types.Message>, CancellationToken>((request, _) =>
            {
                var textProperty = request.GetType().GetProperty("Text");
                capturedText = textProperty?.GetValue(request) as string;
            })
            .ReturnsAsync(new Telegram.Bot.Types.Message());

        botClientMock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<bool>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var bot = new TelegramBotBuilder()
            .WithToken("123456789:ABCdefGHIjklMNOpqrsTUVwxyz")
            .WithBotClient(botClientMock.Object)
            .AddScreen("Home", builder => builder
                .WithId("home")
                .WithLocalizedTitle("WelcomeMessage")
                .WithLocalizedContent("SettingsMessage"), isMainScreen: true)
            .Build();

        await bot.ProcessUpdateAsync(botClientMock.Object, new Update
        {
            Message = new Telegram.Bot.Types.Message
            {
                Text = "/start",
                Chat = new Chat { Id = 99 }
            }
        }, CancellationToken.None);

        bot.StateMachine.SetState(99, LocalizationKeys.Culture, "de");
        await bot.RefreshCurrentScreenAsync(99, CancellationToken.None);

        capturedText.Should().Contain("Willkommen bei Fluent Telegram UI!");
        capturedText.Should().Contain("Bitte wählen Sie eine Einstellung zum Konfigurieren");
    }
}
