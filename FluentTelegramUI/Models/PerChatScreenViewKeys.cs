namespace FluentTelegramUI.Models;

/// <summary>
/// State keys for per-chat screen content overrides (avoids mutating shared <see cref="Screen"/> instances).
/// </summary>
public static class PerChatScreenViewKeys
{
    public const string TextPrefix = "__pcv_text__:";
    public const string ButtonsPrefix = "__pcv_buttons__:";

    public static string TextKey(string screenId) => $"{TextPrefix}{screenId}";

    public static string ButtonsKey(string screenId) => $"{ButtonsPrefix}{screenId}";

    public static void Clear(IStateStore store, long chatId, string screenId)
    {
        store.SetState(chatId, TextKey(screenId), string.Empty);
        store.SetState(chatId, ButtonsKey(screenId), string.Empty);
    }
}
