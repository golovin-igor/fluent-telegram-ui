using System.Globalization;
using System.Resources;
using FluentTelegramUI.Models;

namespace FluentTelegramUI.Resources;

/// <summary>
/// Service for managing localization resources.
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private static LocalizationService? _instance;
    private readonly ResourceManager _resourceManager;
    private IStateStore? _stateStore;

    /// <summary>
    /// Gets the singleton instance of the localization service.
    /// </summary>
    public static LocalizationService Instance => _instance ??= new LocalizationService();

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class.
    /// </summary>
    public LocalizationService()
    {
        _resourceManager = new ResourceManager(
            "FluentTelegramUI.Resources.Strings",
            typeof(LocalizationService).Assembly);
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
        catch
        {
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
        catch
        {
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
