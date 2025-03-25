using shortid;
using shortid.Configuration;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Utility class for generating short IDs
    /// </summary>
    internal static class IdGenerator
    {
        /// <summary>
        /// Generates a short ID with a maximum length of 7 characters
        /// </summary>
        /// <returns>A short ID string</returns>
        public static string GenerateShortId()
        {
            // Generate a short ID and truncate to 7 characters if needed
            var id = ShortId.Generate();
            return id.Length > 7 ? id[..7] : id;
        }
    }
} 