namespace FluentTelegramUI.Models;

/// <summary>
/// Matches callback data against exact and pattern-based handlers.
/// </summary>
internal static class CallbackMatcher
{
    /// <summary>
    /// Tries to resolve a callback handler for the given callback data.
    /// </summary>
    public static bool TryResolveHandler(
        Screen screen,
        string callbackData,
        out Func<string, Dictionary<string, object>, Task<bool>> handler)
    {
        if (screen.EventHandlers.TryGetValue(callbackData, out handler!))
        {
            return true;
        }

        foreach (var entry in screen.EventHandlers)
        {
            if (MatchesPattern(entry.Key, callbackData))
            {
                handler = entry.Value;
                return true;
            }
        }

        handler = null!;
        return false;
    }

    private static bool MatchesPattern(string pattern, string callbackData)
    {
        if (!pattern.Contains('*'))
        {
            return false;
        }

        if (pattern.EndsWith('*'))
        {
            var prefix = pattern[..^1];
            return callbackData.StartsWith(prefix, StringComparison.Ordinal);
        }

        var starIndex = pattern.IndexOf('*');
        if (starIndex < 0)
        {
            return false;
        }

        var before = pattern[..starIndex];
        var after = pattern[(starIndex + 1)..];
        return callbackData.StartsWith(before, StringComparison.Ordinal)
            && callbackData.EndsWith(after, StringComparison.Ordinal)
            && callbackData.Length >= before.Length + after.Length;
    }
}
