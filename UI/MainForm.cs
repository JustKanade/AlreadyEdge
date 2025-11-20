using System.Diagnostics;
using AlreadyEdge.Core;
using AlreadyEdge.Localization;
using AlreadyEdge.Models;

namespace AlreadyEdge.UI;

/// <summary>
/// Main application window with tabbed interface
/// </summary>
public partial class MainForm : Form
{
    private readonly EdgeMonitor _monitor;
    private AppConfig _config;

    // Main UI
    private TabControl _tabControl = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _statusLabel = null!;

    // Basic page controls
    private RadioButton _radioAcrylic = null!;
    private RadioButton _radioNone = null!;
    private CheckBox _checkDarkMode = null!;
    private CheckBox _checkAutoApply = null!;
    private CheckBox _checkRunInBackground = null!;
    private CheckBox _checkStartWithWindows = null!;
    private Button _buttonStart = null!;
    private Button _buttonStop = null!;
    private Button _buttonApplyNow = null!;

    // Edge launch controls
    private CheckBox _checkDisableGpu = null!;
    private CheckBox _checkDisableGpuCompositing = null!;
    private CheckBox _checkTransparentVisuals = null!;
    private Button _buttonLaunchEdge = null!;

    // Language combo
    private ComboBox _comboLanguage = null!;

    // About page
    private AboutPage _aboutPage = null!;

    private ToolTip _toolTip = null!;

    public MainForm()
    {
        _config = ConfigManager.Load();
        LocalizationManager.Initialize();
        LocalizationManager.SetLanguage(_config.Language);

        _monitor = new EdgeMonitor();
        _monitor.StatusChanged += OnMonitorStatusChanged;
        _monitor.WindowsApplied += OnWindowsApplied;

        InitializeComponent();
        LoadConfiguration();

        if (_config.AutoApply)
            _monitor.Start();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Initialize tooltip
        _toolTip = new ToolTip();

        // Window properties
        Text = LocalizationManager.Get("AppTitle");
        ClientSize = new Size(550, 620);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        // TabControl
        _tabControl = new TabControl
        {
            Location = new Point(10, 10),
            Size = new Size(530, 540),
            Dock = DockStyle.Top
        };

        // Create tab pages
        var basicTab = new TabPage(LocalizationManager.Get("TabBasic"));
        var aboutTab = new TabPage(LocalizationManager.Get("TabAbout"));

        InitializeBasicPage(basicTab);
        InitializeAboutTab(aboutTab);

        _tabControl.TabPages.Add(basicTab);
        _tabControl.TabPages.Add(aboutTab);

        Controls.Add(_tabControl);

        // Action buttons
        int btnY = _tabControl.Bottom + 10;
        _buttonStart = new Button
        {
            Text = LocalizationManager.Get("ButtonStart"),
            Location = new Point(20, btnY),
            Size = new Size(165, 35)
        };
        _buttonStart.Click += OnStartClick;
        Controls.Add(_buttonStart);

        _buttonStop = new Button
        {
            Text = LocalizationManager.Get("ButtonStop"),
            Location = new Point(195, btnY),
            Size = new Size(165, 35),
            Enabled = false
        };
        _buttonStop.Click += OnStopClick;
        Controls.Add(_buttonStop);

        _buttonApplyNow = new Button
        {
            Text = LocalizationManager.Get("ButtonApplyNow"),
            Location = new Point(370, btnY),
            Size = new Size(165, 35)
        };
        _buttonApplyNow.Click += OnApplyNowClick;
        Controls.Add(_buttonApplyNow);

        // Status strip
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel(LocalizationManager.Get("StatusReady"));
        _statusStrip.Items.Add(_statusLabel);
        Controls.Add(_statusStrip);

        // Event handlers
        FormClosing += OnFormClosing;

        ResumeLayout(false);
        PerformLayout();
    }

    private void InitializeBasicPage(TabPage page)
    {
        int yPos = 20;

        // Backdrop effect
        var groupBackdrop = new GroupBox
        {
            Text = LocalizationManager.Get("GroupBackdrop"),
            Location = new Point(20, yPos),
            Size = new Size(470, 70)
        };

        _radioAcrylic = new RadioButton
        {
            Text = LocalizationManager.Get("RadioAcrylic"),
            Location = new Point(20, 25),
            AutoSize = true
        };
        _radioNone = new RadioButton
        {
            Text = LocalizationManager.Get("RadioNone"),
            Location = new Point(200, 25),
            AutoSize = true
        };

        _toolTip.SetToolTip(_radioAcrylic, LocalizationManager.Get("TooltipAcrylic"));

        groupBackdrop.Controls.AddRange(new Control[] { _radioAcrylic, _radioNone });
        page.Controls.Add(groupBackdrop);
        yPos += 85;

        // Settings
        var groupSettings = new GroupBox
        {
            Text = LocalizationManager.Get("GroupSettings"),
            Location = new Point(20, yPos),
            Size = new Size(470, 130)
        };

        _checkDarkMode = new CheckBox
        {
            Text = LocalizationManager.Get("CheckDarkMode"),
            Location = new Point(20, 25),
            AutoSize = true
        };
        _checkAutoApply = new CheckBox
        {
            Text = LocalizationManager.Get("CheckAutoApply"),
            Location = new Point(20, 55),
            AutoSize = true
        };
        _checkRunInBackground = new CheckBox
        {
            Text = LocalizationManager.Get("CheckRunInBackground"),
            Location = new Point(20, 85),
            AutoSize = true
        };
        _checkStartWithWindows = new CheckBox
        {
            Text = LocalizationManager.Get("CheckStartWithWindows"),
            Location = new Point(250, 25),
            AutoSize = true
        };

        groupSettings.Controls.AddRange(new Control[] { _checkDarkMode, _checkAutoApply, _checkRunInBackground, _checkStartWithWindows });
        page.Controls.Add(groupSettings);
        yPos += 145;

        // Edge launch options
        var groupEdge = new GroupBox
        {
            Text = LocalizationManager.Get("GroupEdgeLaunch"),
            Location = new Point(20, yPos),
            Size = new Size(470, 160)
        };

        _checkDisableGpu = new CheckBox
        {
            Text = LocalizationManager.Get("CheckDisableGpu"),
            Location = new Point(20, 25),
            AutoSize = true
        };
        _checkDisableGpuCompositing = new CheckBox
        {
            Text = LocalizationManager.Get("CheckDisableGpuCompositing"),
            Location = new Point(20, 55),
            AutoSize = true
        };
        _checkTransparentVisuals = new CheckBox
        {
            Text = LocalizationManager.Get("CheckTransparentVisuals"),
            Location = new Point(20, 85),
            AutoSize = true
        };

        _toolTip.SetToolTip(_checkDisableGpu, LocalizationManager.Get("TooltipDisableGpu"));

        _buttonLaunchEdge = new Button
        {
            Text = LocalizationManager.Get("ButtonLaunchEdge"),
            Location = new Point(20, 120),
            Size = new Size(430, 30)
        };
        _buttonLaunchEdge.Click += OnLaunchEdgeClick;

        groupEdge.Controls.AddRange(new Control[] { _checkDisableGpu, _checkDisableGpuCompositing, _checkTransparentVisuals, _buttonLaunchEdge });
        page.Controls.Add(groupEdge);
        yPos += 175;

        // Language
        var groupLang = new GroupBox
        {
            Text = LocalizationManager.Get("GroupLanguage"),
            Location = new Point(20, yPos),
            Size = new Size(470, 60)
        };

        var labelLang = new Label
        {
            Text = LocalizationManager.Get("LabelLanguage"),
            Location = new Point(20, 28),
            AutoSize = true
        };
        _comboLanguage = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(200, 25),
            Size = new Size(250, 25)
        };
        _comboLanguage.Items.AddRange(new object[] { "English", "简体中文" });
        _comboLanguage.SelectedIndexChanged += OnLanguageChanged;

        groupLang.Controls.AddRange(new Control[] { labelLang, _comboLanguage });
        page.Controls.Add(groupLang);
    }

    private void InitializeAboutTab(TabPage page)
    {
        _aboutPage = new AboutPage { Dock = DockStyle.Fill };
        page.Controls.Add(_aboutPage);
    }

    private void LoadConfiguration()
    {
        // Backdrop
        if (_config.Effect.SelectedBackdrop == BackdropType.Acrylic)
            _radioAcrylic.Checked = true;
        else
            _radioNone.Checked = true;

        // Settings
        _checkDarkMode.Checked = _config.Effect.UseDarkMode;
        _checkAutoApply.Checked = _config.AutoApply;
        _checkRunInBackground.Checked = _config.RunInBackground;
        _checkStartWithWindows.Checked = _config.StartWithWindows;

        // Edge options
        _checkDisableGpu.Checked = _config.DisableGpu;
        _checkDisableGpuCompositing.Checked = _config.DisableGpuCompositing;
        _checkTransparentVisuals.Checked = _config.EnableTransparentVisuals;

        // Language
        _comboLanguage.SelectedIndex = _config.Language == "zh-CN" ? 1 : 0;
    }

    private void SaveConfiguration()
    {
        // Backdrop
        _config.Effect.SelectedBackdrop = _radioAcrylic.Checked ? BackdropType.Acrylic : BackdropType.None;

        // Settings
        _config.Effect.UseDarkMode = _checkDarkMode.Checked;
        _config.AutoApply = _checkAutoApply.Checked;
        _config.RunInBackground = _checkRunInBackground.Checked;
        _config.StartWithWindows = _checkStartWithWindows.Checked;

        // Edge options
        _config.DisableGpu = _checkDisableGpu.Checked;
        _config.DisableGpuCompositing = _checkDisableGpuCompositing.Checked;
        _config.EnableTransparentVisuals = _checkTransparentVisuals.Checked;

        ConfigManager.Save(_config);
        ConfigManager.SetStartupEnabled(_config.StartWithWindows);
    }

    private void OnStartClick(object? sender, EventArgs e)
    {
        SaveConfiguration();
        _monitor.Start();
        _buttonStart.Enabled = false;
        _buttonStop.Enabled = true;
        UpdateStatus(LocalizationManager.Get("StatusMonitoring"));
    }

    private void OnStopClick(object? sender, EventArgs e)
    {
        _monitor.Stop();
        _buttonStart.Enabled = true;
        _buttonStop.Enabled = false;
        UpdateStatus(LocalizationManager.Get("StatusStopped"));
    }

    private void OnApplyNowClick(object? sender, EventArgs e)
    {
        SaveConfiguration();
        var count = _monitor.ApplyToAll(_config.Effect);
        UpdateStatus(LocalizationManager.GetFormatted("StatusApplied", count));
    }

    private void OnLaunchEdgeClick(object? sender, EventArgs e)
    {
        SaveConfiguration();
        
        var edgePath = FindEdgePath();
        if (string.IsNullOrEmpty(edgePath))
        {
            MessageBox.Show("Edge executable not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var args = new List<string>();
        if (_config.EnableTransparentVisuals) args.Add("--enable-transparent-visuals");
        if (_config.DisableGpu) args.Add("--disable-gpu");
        if (_config.DisableGpuCompositing) args.Add("--disable-gpu-compositing");

        try
        {
            Process.Start(edgePath, string.Join(" ", args));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to launch Edge: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string? FindEdgePath()
    {
        var paths = new[]
        {
            @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
            @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
        };
        return paths.FirstOrDefault(File.Exists);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (_comboLanguage.SelectedIndex < 0) return;

        var lang = _comboLanguage.SelectedIndex == 0 ? "en-US" : "zh-CN";
        
        // Only show message if language actually changed
        if (_config.Language == lang) return;
        
        _config.Language = lang;
        LocalizationManager.SetLanguage(lang);

        // Update about page language info
        _aboutPage.UpdateLanguageInfo();

        // Reload UI text
        MessageBox.Show(
            LocalizationManager.Get("StatusLanguageChanged"),
            LocalizationManager.Get("AppTitle"),
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );

        SaveConfiguration();
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_config.RunInBackground && _monitor.IsRunning && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            return;
        }

        SaveConfiguration();
        _monitor.Dispose();
        _toolTip?.Dispose();
    }

    private void OnMonitorStatusChanged(object? sender, string status)
    {
        if (InvokeRequired)
        {
            Invoke(() => UpdateStatus(status));
        }
        else
        {
            UpdateStatus(status);
        }
    }

    private void OnWindowsApplied(object? sender, int count)
    {
        if (InvokeRequired)
        {
            Invoke(() => UpdateStatus(LocalizationManager.GetFormatted("StatusApplied", count)));
        }
        else
        {
            UpdateStatus(LocalizationManager.GetFormatted("StatusApplied", count));
        }
    }

    private void UpdateStatus(string message)
    {
        _statusLabel.Text = message;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _monitor?.Dispose();
            _toolTip?.Dispose();
        }
        base.Dispose(disposing);
    }
}
