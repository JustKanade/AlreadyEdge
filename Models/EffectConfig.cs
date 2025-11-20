namespace AlreadyEdge.Models;

/// <summary>
/// Effect configuration
/// </summary>
public class EffectConfig
{
    public BackdropType SelectedBackdrop { get; set; } = BackdropType.Acrylic;
    public bool UseDarkMode { get; set; } = true;
}
