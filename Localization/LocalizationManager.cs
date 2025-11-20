using System.Globalization;
using System.Resources;

namespace AlreadyEdge.Localization;

/// <summary>
/// Manages application localization
/// </summary>
public static class LocalizationManager
{
    private static ResourceManager? _resourceManager;
    private static CultureInfo _currentCulture = CultureInfo.GetCultureInfo("en");

    /// <summary>
    /// Initialize localization system
    /// </summary>
    public static void Initialize()
    {
        _resourceManager = new ResourceManager("AlreadyEdge.Localization.Strings", typeof(LocalizationManager).Assembly);
    }

    /// <summary>
    /// Set current language
    /// </summary>
    public static void SetLanguage(string languageCode)
    {
        try
        {
            _currentCulture = CultureInfo.GetCultureInfo(languageCode);
            CultureInfo.CurrentUICulture = _currentCulture;
        }
        catch
        {
            _currentCulture = CultureInfo.GetCultureInfo("en");
        }
    }

    /// <summary>
    /// Get localized string by key
    /// </summary>
    public static string Get(string key)
    {
        if (_resourceManager == null)
            Initialize();

        return _resourceManager?.GetString(key, _currentCulture) ?? key;
    }

    /// <summary>
    /// Get formatted localized string
    /// </summary>
    public static string GetFormatted(string key, params object[] args)
    {
        var format = Get(key);
        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return format;
        }
    }
}
