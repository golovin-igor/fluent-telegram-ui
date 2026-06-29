namespace FluentTelegramUI.Resources;

/// <summary>
/// Resolves localized strings for screens and commands.
/// </summary>
public interface ILocalizationService
{
    /// <summary>Default culture when a chat has no culture stored.</summary>
    string DefaultCulture { get; set; }

    /// <summary>Gets a string for the specified culture.</summary>
    string GetString(string key, string? cultureName = null);

    /// <summary>Gets a string using the culture stored for the chat.</summary>
    string GetString(long chatId, string key);

    /// <summary>Gets a formatted string using the culture stored for the chat.</summary>
    string GetString(long chatId, string key, params object[] args);

    /// <summary>Stores the preferred culture for a chat.</summary>
    void SetCulture(long chatId, string cultureName);

    /// <summary>Gets the culture stored for a chat, or <see cref="DefaultCulture"/>.</summary>
    string GetCulture(long chatId);
}
