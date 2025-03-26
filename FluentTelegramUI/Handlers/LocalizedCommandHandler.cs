using System.Text;
using FluentTelegramUI.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluentTelegramUI.Handlers
{
    public interface ICommandHandler
    {
        Task HandleCommandAsync(Message message);
    }

    public class LocalizedCommandHandler : ICommandHandler
    {
        private readonly LocalizationService _localization;
        private readonly ITelegramBotClient _botClient;

        public LocalizedCommandHandler(ITelegramBotClient botClient)
        {
            _localization = LocalizationService.Instance;
            _botClient = botClient;
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
            string welcomeMessage = _localization.GetString("WelcomeMessage");
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: welcomeMessage
            );
        }

        private async Task HandleHelpCommand(Message message)
        {
            var helpText = new StringBuilder();
            helpText.AppendLine(_localization.GetString("AvailableCommands"));
            helpText.AppendLine($"/start - {_localization.GetString("StartCommand")}");
            helpText.AppendLine($"/help - {_localization.GetString("HelpCommand")}");
            helpText.AppendLine($"/settings - {_localization.GetString("SettingsCommand")}");
            helpText.AppendLine($"/language - {_localization.GetString("LanguageCommand")}");

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: helpText.ToString()
            );
        }

        private async Task HandleSettingsCommand(Message message)
        {
            var settingsKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { _localization.GetString("LanguageButton") },
                new KeyboardButton[] { _localization.GetString("ThemeButton") }
            })
            {
                ResizeKeyboard = true
            };

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localization.GetString("SettingsMessage"),
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

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localization.GetString("SelectLanguage"),
                replyMarkup: languageKeyboard
            );
        }

        private async Task HandleInvalidCommand(Message message)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localization.GetString("InvalidCommand")
            );
        }
    }
} 