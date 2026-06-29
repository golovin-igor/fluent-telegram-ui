namespace FluentTelegramUI.Models;

/// <summary>
/// Applies visual templates based on <see cref="FluentStyle"/>.
/// </summary>
public static class FluentStyleTemplates
{
    /// <summary>
    /// Applies a style prefix and suffix to screen body text.
    /// </summary>
    public static string ApplyBody(FluentStyle style, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return style switch
        {
            FluentStyle.Light => $"🌤 {text}",
            FluentStyle.Dark => $"🌙 {text}",
            FluentStyle.Colorful => $"🎨 {text}",
            FluentStyle.Modern => $"✨ {text}",
            FluentStyle.Minimalist => text,
            FluentStyle.Professional => $"📋 {text}",
            FluentStyle.Fun => $"🎉 {text}",
            FluentStyle.Technical => $"⚙️ {text}",
            _ => text
        };
    }

    /// <summary>
    /// Applies a style prefix to a screen title.
    /// </summary>
    public static string ApplyTitle(FluentStyle style, string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return title;
        }

        return style switch
        {
            FluentStyle.Professional => $"📌 {title}",
            FluentStyle.Fun => $"🚀 {title}",
            FluentStyle.Technical => $"🔧 {title}",
            FluentStyle.Modern => $"▸ {title}",
            _ => title
        };
    }
}
