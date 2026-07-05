using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentTelegramUI.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI.Models;

/// <summary>
/// Renders screens into Telegram messages and handles the edit-vs-send fallback.
/// Extracted from <see cref="ScreenManager"/> to separate rendering concerns
/// from navigation and callback dispatch.
/// </summary>
public sealed class ScreenRenderer
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger _logger;
    private readonly IStateStore _stateStore;
    private readonly FluentStyle _defaultStyle;
    private readonly ILocalizationService? _localization;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenRenderer"/> class.
    /// </summary>
    public ScreenRenderer(
        ITelegramBotClient botClient,
        ILogger logger,
        IStateStore stateStore,
        FluentStyle defaultStyle,
        ILocalizationService? localization = null)
    {
        _botClient = botClient;
        _logger = logger;
        _stateStore = stateStore;
        _defaultStyle = defaultStyle;
        _localization = localization;
    }

    /// <summary>
    /// Displays a screen for a chat, editing the tracked message when possible
    /// and falling back to sending a new message when the edit fails.
    /// </summary>
    public async Task<Telegram.Bot.Types.Message> DisplayAsync(
        long chatId,
        Screen screen,
        int lastMessageId,
        bool forceNewMessage,
        CancellationToken cancellationToken)
    {
        var renderMessage = BuildRenderMessage(chatId, screen);
        var markup = renderMessage.ToInlineKeyboardMarkup();

        if (!forceNewMessage && lastMessageId != 0)
        {
            try
            {
                if (renderMessage.HasImage)
                {
                    return await _botClient.SendPhoto(
                        chatId: chatId,
                        photo: renderMessage.ImageUrl,
                        caption: renderMessage.GetEffectiveImageCaption(),
                        parseMode: renderMessage.ParseMarkdown ? ParseMode.Html : ParseMode.None,
                        replyMarkup: markup as InlineKeyboardMarkup,
                        cancellationToken: cancellationToken);
                }

                return await _botClient.EditMessageText(
                    chatId: chatId,
                    messageId: lastMessageId,
                    text: renderMessage.Text,
                    parseMode: renderMessage.ParseMarkdown ? ParseMode.Html : ParseMode.None,
                    replyMarkup: markup as InlineKeyboardMarkup,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Edit failed for chat {ChatId}, sending a new message.", chatId);
            }
        }

        if (renderMessage.HasImage)
        {
            return await _botClient.SendPhoto(
                chatId: chatId,
                photo: renderMessage.ImageUrl,
                caption: renderMessage.GetEffectiveImageCaption(),
                parseMode: renderMessage.ParseMarkdown ? ParseMode.Html : ParseMode.None,
                replyMarkup: markup as InlineKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        return await _botClient.SendMessage(
            chatId: chatId,
            text: renderMessage.Text,
            parseMode: renderMessage.ParseMarkdown ? ParseMode.Html : ParseMode.None,
            replyMarkup: markup,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Builds the rendered message for a screen without sending it.
    /// </summary>
    public Message BuildRenderMessage(long chatId, Screen screen)
    {
        var mainMessage = screen.Content;
        var style = mainMessage.Style == FluentStyle.Default ? _defaultStyle : mainMessage.Style;
        var renderContext = new ScreenRenderContext(
            chatId,
            screen,
            (key, defaultValue) => _stateStore.GetState(chatId, key, defaultValue) ?? defaultValue!);

        var bodyParts = new List<string>();
        var allButtons = new List<Button>(mainMessage.Buttons);

        foreach (var control in screen.Controls)
        {
            var controlMsg = control.ToMessage(renderContext);
            if (!string.IsNullOrEmpty(controlMsg.Text))
            {
                bodyParts.Add(controlMsg.Text);
            }

            allButtons.AddRange(controlMsg.Buttons);
        }

        var body = ResolveContentText(chatId, mainMessage.Text, screen.ContentResourceKey);
        if (bodyParts.Count > 0)
        {
            body = string.IsNullOrEmpty(body)
                ? string.Join("\n\n", bodyParts)
                : $"{body}\n\n{string.Join("\n\n", bodyParts)}";
        }

        body = FluentStyleTemplates.ApplyBody(style, body);
        var title = FluentStyleTemplates.ApplyTitle(style, ResolveTitleText(chatId, screen));
        var text = !string.IsNullOrEmpty(title) ? $"<b>{title}</b>\n\n{body}" : body;

        return new Message
        {
            Text = text,
            ParseMarkdown = true,
            Style = style,
            Buttons = allButtons,
            ButtonsPerRow = mainMessage.ButtonsPerRow,
            ImageUrl = mainMessage.ImageUrl,
            ImageCaption = mainMessage.ImageCaption
        };
    }

    private string ResolveTitleText(long chatId, Screen screen)
    {
        if (!string.IsNullOrEmpty(screen.TitleResourceKey) && _localization != null)
        {
            return _localization.GetString(chatId, screen.TitleResourceKey);
        }

        return screen.Title;
    }

    private string ResolveContentText(long chatId, string? text, string? resourceKey)
    {
        if (!string.IsNullOrEmpty(resourceKey) && _localization != null)
        {
            return _localization.GetString(chatId, resourceKey);
        }

        return text ?? string.Empty;
    }
}
