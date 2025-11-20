namespace AlreadyEdge.Models;

/// <summary>
/// Application configuration model
/// </summary>
public class AppConfig
{
    // Effect configuration
    public EffectConfig Effect { get; set; } = new();
    
    // Application behavior
    public bool AutoApply { get; set; } = true;
    public bool StartWithWindows { get; set; } = false;
    public bool RunInBackground { get; set; } = false; // Keep monitoring when window closed
    public string Language { get; set; } = "en";
    
    // Edge launch parameters
    public bool DisableGpu { get; set; } = true;
    public bool DisableGpuCompositing { get; set; } = true;
    public bool EnableTransparentVisuals { get; set; } = true;
}
