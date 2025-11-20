using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AlreadyEdge.Models;

namespace AlreadyEdge.Core;

/// <summary>
/// Monitors and manages Edge browser windows
/// </summary>
public class EdgeMonitor : IDisposable
{
    private readonly System.Threading.Timer _timer;
    private readonly HashSet<IntPtr> _processedWindows = new();
    private bool _isRunning;

    public event EventHandler<string>? StatusChanged;
    public event EventHandler<int>? WindowsApplied;

    public bool IsRunning => _isRunning;

    public EdgeMonitor()
    {
        _timer = new System.Threading.Timer(ScanEdgeWindows, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Start monitoring Edge windows
    /// </summary>
    public void Start()
    {
        if (_isRunning) return;

        _isRunning = true;
        _timer.Change(0, 2000); // Scan every 2 seconds
        StatusChanged?.Invoke(this, "Monitoring started");
    }

    /// <summary>
    /// Stop monitoring
    /// </summary>
    public void Stop()
    {
        if (!_isRunning) return;

        _isRunning = false;
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        _processedWindows.Clear();
        StatusChanged?.Invoke(this, "Monitoring stopped");
    }

    /// <summary>
    /// Apply effect to all current Edge windows
    /// </summary>
    public int ApplyToAll(EffectConfig config)
    {
        int count = 0;
        EnumWindows((hwnd, lParam) =>
        {
            if (IsEdgeWindow(hwnd))
            {
                if (DwmManager.ApplyBackdrop(hwnd, config))
                {
                    _processedWindows.Add(hwnd);
                    count++;
                }
            }
            return true;
        }, IntPtr.Zero);

        WindowsApplied?.Invoke(this, count);
        return count;
    }

    private void ScanEdgeWindows(object? state)
    {
        var config = ConfigManager.Load();
        if (!config.AutoApply) return;

        EnumWindows((hwnd, lParam) =>
        {
            if (IsEdgeWindow(hwnd) && !_processedWindows.Contains(hwnd))
            {
                if (DwmManager.ApplyBackdrop(hwnd, config.Effect))
                {
                    _processedWindows.Add(hwnd);
                    StatusChanged?.Invoke(this, $"Applied to window: {hwnd:X}");
                }
            }
            return true;
        }, IntPtr.Zero);

        // Clean up invalid windows
        _processedWindows.RemoveWhere(hwnd => GetWindowThreadProcessId(hwnd, out _) == 0);
    }

    private static bool IsEdgeWindow(IntPtr hwnd)
    {
        var className = new StringBuilder(256);
        GetClassName(hwnd, className, className.Capacity);

        if (className.ToString() != "Chrome_WidgetWin_1") return false;

        GetWindowThreadProcessId(hwnd, out uint processId);
        try
        {
            var process = Process.GetProcessById((int)processId);
            return process.ProcessName.Equals("msedge", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        Stop();
        _timer?.Dispose();
    }

    #region P/Invoke

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    #endregion
}
