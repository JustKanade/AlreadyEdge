using AlreadyEdge.Localization;
using AlreadyEdge.UI;

namespace AlreadyEdge;

/// <summary>
/// Application entry point
/// </summary>
internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ApplicationConfiguration.Initialize();
        
        // Initialize localization system
        LocalizationManager.Initialize();
        
        Application.Run(new MainForm());
    }
}