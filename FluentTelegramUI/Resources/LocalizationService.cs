using System.Globalization;
using System.Resources;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FluentTelegramUI.Resources;

/// <summary>
/// Service for managing localization resources. Register via dependency
/// injection (<c>AddFluentTelegramUI</c> or <c>TelegramBotBuilder.Build</c>)
/// so it is bound to the shared <see cref="IStateStore"/>.
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private readonly ResourceManager _resourceManager;
    private readonly ILogger _logger;
    private IStateStore? _stateStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class.
    /// </summary>
    public LocalizationService(ILogger<LocalizationService>? logger = null)
    {
        _resourceManager = new ResourceManager(
            "FluentTelegramUI.Resources.Strings",
            typeof(LocalizationService).Assembly);
        _logger = logger ?? NullLogger<LocalizationService>.Instance;
        DefaultCulture = CultureInfo.CurrentUICulture.Name;
    }

    /// <inheritdoc />
    public string DefaultCulture { get; set; }

    /// <summary>
    /// Binds the state store used for per-chat culture.
    /// </summary>
    public void BindStateStore(IStateStore stateStore) => _stateStore = stateStore;

    /// <inheritdoc />
    public string GetCulture(long chatId) =>
        _stateStore?.GetState(chatId, LocalizationKeys.Culture, DefaultCulture) ?? DefaultCulture;

    /// <inheritdoc />
    public void SetCulture(long chatId, string cultureName) =>
        _stateStore?.SetState(chatId, LocalizationKeys.Culture, cultureName);

    /// <inheritdoc />
    public string GetString(long chatId, string key) => GetString(key, GetCulture(chatId));

    /// <inheritdoc />
    public string GetString(long chatId, string key, params object[] args)
    {
        var template = GetString(chatId, key);
        try
        {
            return string.Format(template, args);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Failed to format localized string '{Key}'.", key);
            return template;
        }
    }

    /// <inheritdoc />
    public string GetString(string key, string? cultureName = null)
    {
        try
        {
            var culture = CreateCulture(cultureName ?? DefaultCulture);
            return _resourceManager.GetString(key, culture) ?? key;
        }
        catch (Exception ex) when (ex is CultureNotFoundException or MissingManifestResourceException)
        {
            _logger.LogWarning(ex, "Failed to resolve localized string '{Key}' for culture '{Culture}'.", key, cultureName);
            return key;
        }
    }

    private static CultureInfo CreateCulture(string cultureName)
    {
        try
        {
            return new CultureInfo(cultureName);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}
