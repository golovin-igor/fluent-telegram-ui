namespace FluentTelegramUI.Models;

/// <summary>
/// Abstraction for persisting per-chat bot state.
/// </summary>
public interface IStateStore
{
    /// <summary>Gets a state value for a chat.</summary>
    T GetState<T>(long chatId, string key, T defaultValue = default!);

    /// <summary>Sets a state value for a chat.</summary>
    void SetState<T>(long chatId, string key, T value);

    /// <summary>Clears all state for a chat.</summary>
    void ClearState(long chatId);
}
