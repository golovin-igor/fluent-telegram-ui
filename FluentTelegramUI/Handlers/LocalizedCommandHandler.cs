using System.Text;
using FluentTelegramUI.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI.Handlers
{
    /// <summary>
    /// Handles localized slash commands (/start, /help, /settings, /language).
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>Handles a command message.</summary>
        Task HandleCommandAsync(Message message);
    }

    /// <summary>
    /// A localized command handler that resolves strings per chat from
    /// <see cref="ILocalizationService"/>.
    /// </summary>
    public class LocalizedCommandHandler : ICommandHandler
    {
        private readonly ILocalizationService _localization;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<LocalizedCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedCommandHandler"/> class.
        /// </summary>
        public LocalizedCommandHandler(
            ITelegramBotClient botClient,
            ILocalizationService localization,
            ILogger<LocalizedCommandHandler>? logger = null)
        {
            _botClient = botClient;
            _localization = localization;
            _logger = logger ?? NullLogger<LocalizedCommandHandler>.Instance;
        }

        public async Task HandleCommandAsync(Message message)
        {
            if (message.Text == null) return;

            switch (message.Text.ToLower())
            {
                case "/start":
                    await HandleStartCommand(message);
                    break;
                case "/help":
                    await HandleHelpCommand(message);
                    break;
                case "/settings":
                    await HandleSettingsCommand(message);
                    break;
                case "/language":
                    await HandleLanguageCommand(message);
                    break;
                default:
                    await HandleInvalidCommand(message);
                    break;
            }
        }

        private async Task HandleStartCommand(Message message)
        {
            string welcomeMessage = _localization.GetString(message.Chat.Id, "WelcomeMessage");
            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: welcomeMessage
            );
        }

        private async Task HandleHelpCommand(Message message)
        {
            var chatId = message.Chat.Id;
            var helpText = new StringBuilder();
            helpText.AppendLine(_localization.GetString(chatId, "AvailableCommands"));
            helpText.AppendLine($"/start - {_localization.GetString(chatId, "StartCommand")}");
            helpText.AppendLine($"/help - {_localization.GetString(chatId, "HelpCommand")}");
            helpText.AppendLine($"/settings - {_localization.GetString(chatId, "SettingsCommand")}");
            helpText.AppendLine($"/language - {_localization.GetString(chatId, "LanguageCommand")}");

            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: helpText.ToString()
            );
        }

        private async Task HandleSettingsCommand(Message message)
        {
            var chatId = message.Chat.Id;
            var settingsKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { _localization.GetString(chatId, "LanguageButton") },
                new KeyboardButton[] { _localization.GetString(chatId, "ThemeButton") }
            })
            {
                ResizeKeyboard = true
            };

            await _botClient.SendMessage(
                chatId: chatId,
                text: _localization.GetString(chatId, "SettingsMessage"),
                replyMarkup: settingsKeyboard
            );
        }

        private async Task HandleLanguageCommand(Message message)
        {
            var languageKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("English", "lang_en"),
                    InlineKeyboardButton.WithCallbackData("Deutsch", "lang_de")
                }
            });

            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: _localization.GetString(message.Chat.Id, "SelectLanguage"),
                replyMarkup: languageKeyboard
            );
        }

        private async Task HandleInvalidCommand(Message message)
        {
            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: _localization.GetString(message.Chat.Id, "InvalidCommand")
            );
        }
    }
} 