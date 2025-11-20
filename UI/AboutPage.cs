using AlreadyEdge.Localization;
using System.Diagnostics;

namespace AlreadyEdge.UI;

/// <summary>
/// About page following DWMBlurGlass style
/// </summary>
public class AboutPage : UserControl
{
    private const string GitHubUrl = "https://github.com/JustKanade/AlreadyEdge";

    public AboutPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        BackColor = Color.White;
        Dock = DockStyle.Fill;
        Padding = new Padding(20, 30, 20, 20);

        // Container panel
        var container = new Panel
        {
            Location = new Point(20, 30),
            Size = new Size(440, 400),
            AutoScroll = false
        };

        int yPos = 0;

        // About section title with icon
        var titlePanel = new Panel
        {
            Location = new Point(0, yPos),
            Size = new Size(440, 30),
            BackColor = Color.Transparent
        };

        var iconBox = new PictureBox
        {
            Location = new Point(0, 5),
            Size = new Size(20, 20),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = SystemIcons.Information.ToBitmap()
        };
        titlePanel.Controls.Add(iconBox);

        var lblTitle = new Label
        {
            Text = LocalizationManager.Get("TabAbout"),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Location = new Point(25, 5),
            Size = new Size(100, 20),
            ForeColor = Color.Black,
            BackColor = Color.Transparent
        };
        titlePanel.Controls.Add(lblTitle);
        container.Controls.Add(titlePanel);
        yPos += 40;

        // Author info
        var lblAuthor = new Label
        {
            Text = LocalizationManager.Get("AboutInfo"),
            Font = new Font("Segoe UI", 9),
            Location = new Point(0, yPos),
            Size = new Size(440, 20),
            ForeColor = Color.FromArgb(64, 64, 64),
            BackColor = Color.Transparent
        };
        container.Controls.Add(lblAuthor);
        yPos += 30;

        // GitHub link
        var linkGitHub = new LinkLabel
        {
            Text = GitHubUrl,
            Font = new Font("Segoe UI", 9),
            Location = new Point(0, yPos),
            Size = new Size(440, 20),
            LinkColor = Color.FromArgb(30, 144, 255),
            BackColor = Color.Transparent,
            LinkBehavior = LinkBehavior.HoverUnderline
        };
        linkGitHub.LinkClicked += (s, e) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo(GitHubUrl) { UseShellExecute = true });
            }
            catch { }
        };
        container.Controls.Add(linkGitHub);

        Controls.Add(container);
        ResumeLayout(false);
    }

    public void UpdateLanguageInfo()
    {
        // No-op, kept for compatibility
    }
}
