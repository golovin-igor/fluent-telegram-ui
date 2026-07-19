using FluentAssertions;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Telegram.Bot;

namespace FluentTelegramUI.Tests;

public class PerChatScreenViewTests
{
    [Fact]
    public void BuildRenderMessage_UsesPerChatTextOverride()
    {
        const long chatId = 42;
        var stateStore = new StateMachine();
        stateStore.SetState(chatId, PerChatScreenViewKeys.TextKey("jobs"), "Chat-specific body");

        var screen = new Screen
        {
            Id = "jobs",
            Title = "Jobs",
            Content = new Message
            {
                Text = "Shared template body",
                Buttons = [new Button { Text = "Shared", CallbackData = "shared" }],
            },
        };

        var renderer = CreateRenderer(stateStore);
        var rendered = renderer.BuildRenderMessage(chatId, screen);

        rendered.Text.Should().Contain("Chat-specific body");
        rendered.Text.Should().NotContain("Shared template body");
    }

    [Fact]
    public void BuildRenderMessage_UsesPerChatButtonOverride()
    {
        const long chatId = 7;
        var stateStore = new StateMachine();
        stateStore.SetState(
            chatId,
            PerChatScreenViewKeys.ButtonsKey("jobs"),
            """[{"Text":"Apply #1","CallbackData":"apply:1"}]""");

        var screen = new Screen
        {
            Id = "jobs",
            Content = new Message
            {
                Text = "Pick a job",
                Buttons = [new Button { Text = "Shared", CallbackData = "shared" }],
            },
        };

        var renderer = CreateRenderer(stateStore);
        var rendered = renderer.BuildRenderMessage(chatId, screen);

        rendered.Buttons.Should().ContainSingle();
        rendered.Buttons[0].Text.Should().Be("Apply #1");
        rendered.Buttons[0].CallbackData.Should().Be("apply:1");
    }

    [Fact]
    public void BuildRenderMessage_DifferentChatsDoNotShareOverrides()
    {
        var stateStore = new StateMachine();
        stateStore.SetState(1, PerChatScreenViewKeys.TextKey("jobs"), "Chat 1");
        stateStore.SetState(2, PerChatScreenViewKeys.TextKey("jobs"), "Chat 2");

        var screen = new Screen
        {
            Id = "jobs",
            Content = new Message { Text = "Default" },
        };

        var renderer = CreateRenderer(stateStore);

        renderer.BuildRenderMessage(1, screen).Text.Should().Contain("Chat 1");
        renderer.BuildRenderMessage(2, screen).Text.Should().Contain("Chat 2");
    }

    private static ScreenRenderer CreateRenderer(IStateStore stateStore)
    {
        var botClient = new Mock<ITelegramBotClient>().Object;
        return new ScreenRenderer(
            botClient,
            NullLogger.Instance,
            stateStore,
            FluentStyle.Default);
    }
}
