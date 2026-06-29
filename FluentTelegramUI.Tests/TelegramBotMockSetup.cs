using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Tests;

internal static class TelegramBotMockSetup
{
    public static void SetupSendMessage(
        this Mock<ITelegramBotClient> mock,
        Action? onSend = null,
        Message? returnMessage = null)
    {
        mock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<Message>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((_, _) => onSend?.Invoke())
            .ReturnsAsync(returnMessage ?? new Message());

        mock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<bool>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    public static void SetupAnswerCallbackQuery(this Mock<ITelegramBotClient> mock, Action? onAnswer = null)
    {
        mock.Setup(m => m.SendRequest(
                It.IsAny<IRequest<bool>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((_, _) => onAnswer?.Invoke())
            .ReturnsAsync(true);
    }
}
