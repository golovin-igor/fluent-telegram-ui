using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Xunit;
using Message = Telegram.Bot.Types.Message;

namespace FluentTelegramUI.Tests;

public class ScreenRendererRichMessageTests
{
    [Fact]
    public void BuildRenderMessage_PreservesRichHtmlWithoutTitleWrapper()
    {
        var html = "<h1>Build with fluent-telegram-ui</h1>";
        var renderer = CreateRenderer();
        var screen = new Screen
        {
            Title = "Pitch",
            Content = new Models.Message { RichHtml = html },
        };

        var rendered = renderer.BuildRenderMessage(1, screen);

        rendered.RichHtml.Should().Be(html);
        rendered.Text.Should().BeEmpty();
        rendered.HasRichContent.Should().BeTrue();
    }

    [Fact]
    public async Task DisplayAsync_SendsRichMessageWhenHtmlIsSet()
    {
        SendRichMessageRequest? captured = null;
        var clientMock = new Mock<ITelegramBotClient>();
        clientMock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<Message>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((req, _) =>
            {
                captured = req as SendRichMessageRequest;
            })
            .ReturnsAsync(new Message());

        var renderer = CreateRenderer(clientMock.Object);
        var screen = new Screen
        {
            Content = new Models.Message { RichHtml = "<p>hello</p>" },
        };

        await renderer.DisplayAsync(42, screen, lastMessageId: 0, forceNewMessage: true, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.RichMessage.Html.Should().Be("<p>hello</p>");
    }

    [Fact]
    public async Task DisplayAsync_EditsWithRichMessageWhenLastMessageIdIsSet()
    {
        EditMessageTextRequest? captured = null;
        var clientMock = new Mock<ITelegramBotClient>();
        clientMock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<Message>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((req, _) =>
            {
                captured = req as EditMessageTextRequest;
            })
            .ReturnsAsync(new Message());

        var renderer = CreateRenderer(clientMock.Object);
        var screen = new Screen
        {
            Content = new Models.Message { RichHtml = "<p>updated</p>" },
        };

        await renderer.DisplayAsync(42, screen, lastMessageId: 11, forceNewMessage: false, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.RichMessage.Should().NotBeNull();
        captured.RichMessage!.Html.Should().Be("<p>updated</p>");
    }

    [Fact]
    public async Task DisplayAsync_SendsPlainTextWhenRichContentIsAbsent()
    {
        object? captured = null;
        var clientMock = new Mock<ITelegramBotClient>();
        clientMock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<Message>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(new Message());

        var renderer = CreateRenderer(clientMock.Object);
        var screen = new Screen
        {
            Content = new Models.Message { Text = "plain" },
        };

        await renderer.DisplayAsync(7, screen, lastMessageId: 0, forceNewMessage: true, CancellationToken.None);

        captured.Should().NotBeOfType<SendRichMessageRequest>();
    }

    private static ScreenRenderer CreateRenderer(ITelegramBotClient? botClient = null)
    {
        return new ScreenRenderer(
            botClient ?? new Mock<ITelegramBotClient>().Object,
            NullLogger.Instance,
            new StateMachine(),
            FluentStyle.Default);
    }
}
