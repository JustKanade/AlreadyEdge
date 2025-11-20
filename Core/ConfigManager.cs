using System.Text.Json;
using AlreadyEdge.Models;

namespace AlreadyEdge.Core;

/// <summary>
/// Manages application configuration persistence
/// </summary>
public static class ConfigManager
{
    private static readonly string _configPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AlreadyEdge",
        "config.json"
    );

    /// <summary>
    /// Load configuration from file
    /// </summary>
    public static AppConfig Load()
    {
        try
        {
            if (!File.Exists(_configPath))
                return new AppConfig();

            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
        catch
        {
            return new AppConfig();
        }
    }

    /// <summary>
    /// Save configuration to file
    /// </summary>
    public static void Save(AppConfig config)
    {
        try
        {
            var directory = Path.GetDirectoryName(_configPath);
            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
        catch
        {
            // Silent fail
        }
    }

    /// <summary>
    /// Configure startup registry entry
    /// </summary>
    public static void SetStartupEnabled(bool enabled)
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            
            if (key == null) return;

            if (enabled)
            {
                var exePath = Environment.ProcessPath;
                if (exePath != null)
                    key.SetValue("AlreadyEdge", $"\"{exePath}\"");
            }
            else
            {
                key.DeleteValue("AlreadyEdge", false);
            }
        }
        catch
        {
            // Silent fail
        }
    }
}
