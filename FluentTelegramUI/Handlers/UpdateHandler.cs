using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FluentTelegramUI.Handlers
{
    /// <summary>
    /// Default update handler for processing Telegram updates
    /// </summary>
    public class UpdateHandler : IUpdateHandler
    {
        private readonly Func<ITelegramBotClient, Update, CancellationToken, Task> _updateHandler;
        private readonly Func<ITelegramBotClient, Exception, CancellationToken, Task> _errorHandler;
        
        /// <summary>
        /// Initializes a new instance of the UpdateHandler class
        /// </summary>
        /// <param name="updateHandler">The function to handle updates</param>
        /// <param name="errorHandler">The function to handle errors</param>
        public UpdateHandler(
            Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler,
            Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler)
        {
            _updateHandler = updateHandler ?? throw new ArgumentNullException(nameof(updateHandler));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }
        
        /// <summary>
        /// Handles a Telegram update
        /// </summary>
        /// <param name="botClient">The bot client</param>
        /// <param name="update">The update to handle</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await _updateHandler(botClient, update, cancellationToken);
        }
        
        /// <summary>
        /// Handles an error that occurred during update processing
        /// </summary>
        /// <param name="botClient">The bot client</param>
        /// <param name="exception">The exception that occurred</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await _errorHandler(botClient, exception, cancellationToken);
        }
    }
} 