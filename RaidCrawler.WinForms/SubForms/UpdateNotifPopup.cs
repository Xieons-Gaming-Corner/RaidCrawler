namespace RaidCrawler.WinForms.SubForms;

public partial class UpdateNotifPopup : Form
{
    private Version cv;
    private Version nv;
    /// <summary>
    /// Initializes a new instance of the UpdateNotifPopup form configured with the specified current and new versions.
    /// </summary>
    /// <param name="currentVersion">The application's current version.</param>
    /// <param name="newVersion">The available new version to notify about.</param>
    public UpdateNotifPopup(Version currentVersion, Version newVersion)
    {
        cv = currentVersion;
        nv = newVersion;
        InitializeComponent();
    }

    /// <summary>
    /// Prepares the update notification UI when the form is loaded.
    /// </summary>
    /// <param name="sender">The source of the load event.</param>
    /// <param name="e">Event data for the load event.</param>
    private void UpdateNotifPopup_Load(object sender, EventArgs e)
    {
        L_Version.Text = $"Current: v{cv.Major}.{cv.Minor}.{cv.Build} | New: v{nv.Major}.{nv.Minor}.{nv.Build}";
        B_Download.Focus();
        CenterToScreen();
    }
}
