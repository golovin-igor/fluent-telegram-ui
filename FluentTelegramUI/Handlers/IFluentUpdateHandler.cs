using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Handlers
{
    /// <summary>
    /// Interface for handling Telegram updates in a fluent way
    /// </summary>
    public interface IFluentUpdateHandler
    {
        /// <summary>
        /// Handles a text message update
        /// </summary>
        /// <param name="botClient">The bot client</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task HandleTextMessageAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken cancellationToken);
        
        /// <summary>
        /// Handles a callback query update
        /// </summary>
        /// <param name="botClient">The bot client</param>
        /// <param name="callbackQuery">The callback query</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken);
        
        /// <summary>
        /// Handles a polling error
        /// </summary>
        /// <param name="botClient">The bot client</param>
        /// <param name="exception">The exception that occurred</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
    }
} 