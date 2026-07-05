namespace FluentTelegramUI.Models;

/// <summary>
/// Per-chat rendering context for screen controls.
/// </summary>
public sealed class ScreenRenderContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenRenderContext"/> class.
    /// </summary>
    /// <param name="chatId">The chat identifier.</param>
    /// <param name="screen">The screen being rendered.</param>
    /// <param name="getState">Function that reads persisted control state.</param>
    public ScreenRenderContext(long chatId, Screen screen, Func<string, object?, object?> getState)
    {
        ChatId = chatId;
        Screen = screen;
        GetState = getState;
    }

    /// <summary>The chat identifier.</summary>
    public long ChatId { get; }

    /// <summary>The screen being rendered.</summary>
    public Screen Screen { get; }

    private Func<string, object?, object?> GetState { get; }

    /// <summary>
    /// Gets persisted state for a control property, falling back to the template default.
    /// </summary>
    public T GetControlState<T>(UIControl control, string property, T defaultValue)
    {
        var key = ControlStateKey(Screen.Id, control.Id, property);
        var value = GetState(key, defaultValue);
        return value is T typed ? typed : defaultValue;
    }

    /// <summary>
    /// Builds a stable storage key for control state.
    /// </summary>
    public static string ControlStateKey(string screenId, string controlId, string property)
        => $"{StateKeys.ControlPrefix}{screenId}:{controlId}:{property}";
}
