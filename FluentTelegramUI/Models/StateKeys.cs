namespace FluentTelegramUI.Models;

/// <summary>
/// Centralized state key constants used across the framework.
/// </summary>
public static class StateKeys
{
    /// <summary>Workflow state name key (e.g. "awaiting_email").</summary>
    public const string Workflow = "state";

    /// <summary>Last text input received from the user.</summary>
    public const string LastInput = "last_input";

    /// <summary>Prefix for per-control state keys: <c>ctrl:{screen}:{control}:{property}</c>.</summary>
    public const string ControlPrefix = "ctrl:";
}

/// <summary>
/// Centralized callback data prefixes and constants. Telegram callback data
/// is limited to 64 bytes, so keeping these short and centralized matters.
/// </summary>
public static class CallbackPrefixes
{
    /// <summary>Navigation callback prefix. Data format: <c>screen:{screenId}</c>.</summary>
    public const string Screen = "screen:";

    /// <summary>Back-navigation callback data.</summary>
    public const string Back = "back";

    /// <summary>Carousel callback prefix. Data format: <c>carousel:{id}:{action}</c>.</summary>
    public const string Carousel = "carousel:";

    /// <summary>Accordion callback prefix. Data format: <c>accordion:{id}:{action}</c>.</summary>
    public const string Accordion = "accordion:";

    /// <summary>Text-input handler key prefix: <c>text_input:{state}</c>.</summary>
    public const string TextInput = "text_input:";

    /// <summary>Language switch callback prefix. Data format: <c>lang:{culture}</c>.</summary>
    public const string Language = "lang:";
}

/// <summary>Carousel callback action segments.</summary>
public static class CarouselActions
{
    public const string Previous = "prev";
    public const string Next = "next";
    public const string Info = "info";
}

/// <summary>Accordion callback action segments.</summary>
public static class AccordionActions
{
    public const string Expand = "expand";
    public const string Collapse = "collapse";
}

/// <summary>Toggle callback action segments.</summary>
public static class ToggleActions
{
    public const string On = "on";
    public const string Off = "off";
}
