using System.Runtime.InteropServices;
using AlreadyEdge.Models;

namespace AlreadyEdge.Core;

/// <summary>
/// Manages Desktop Window Manager (DWM) operations
/// </summary>
public static class DwmManager
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_MICA_EFFECT = 1029;
    private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

    /// <summary>
    /// Apply backdrop effect with advanced configuration
    /// </summary>
    public static bool ApplyBackdrop(IntPtr hwnd, EffectConfig config)
    {
        try
        {
            // Extend frame into client area
            var margins = new MARGINS { cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1 };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);

            // Set backdrop type
            int backdropValue = (int)config.SelectedBackdrop;
            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdropValue, sizeof(int));

            // Set dark mode
            int darkModeValue = config.UseDarkMode ? 1 : 0;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkModeValue, sizeof(int));

            // Set translucent property for Chrome
            SetProp(hwnd, "Chrome.WindowTranslucent", (IntPtr)1);

            // Force window recalculation
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, 
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED | SWP_NOACTIVATE);

            // Force theme reload
            SendMessage(hwnd, WM_THEMECHANGED, IntPtr.Zero, IntPtr.Zero);

            return true;
        }
        catch
        {
            return false;
        }
    }

    #region P/Invoke

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr hData);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint WM_THEMECHANGED = 0x031A;

    #endregion
}
