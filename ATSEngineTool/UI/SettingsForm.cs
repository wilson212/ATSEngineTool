using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ATSEngineTool
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            SteamInstallPath.Text = Program.Config.SteamPath;
            linkCheckBox.Checked = Program.Config.IntegrateWithMod;
            updateCheckBox.Checked = Program.Config.UpdateCheck;
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            // Our goto
            LocateInstall:
            {

                // Request the user supply the steam path
                OpenFileDialog Dialog = new OpenFileDialog();
                Dialog.Title = "Steam Library Path where American Truck Simulator is Installed";
                Dialog.FileName = "Steam.dll";
                Dialog.Filter = "Steam Library|Steam.exe;Steam.dll";
                Dialog.InitialDirectory = Program.Config.SteamPath;
                if (Dialog.ShowDialog() == DialogResult.OK)
                {
                    string steamPath = Path.GetDirectoryName(Dialog.FileName);
                    string atsPath = Path.Combine(steamPath, "SteamApps", "common",
                        "American Truck Simulator", "bin", "win_x64", "amtrucks.exe");

                    // If Ats is not installed here...
                    if (!File.Exists(atsPath))
                    {
                        // Alert the user that they are wrong...
                        DialogResult res = MessageBox.Show(
                            "Sadly, American Truck Simulator is not installed in this Steam Library location. "
                                + "Would you like to try another location?",
                            "Steam Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                        );

                        // Quit here
                        if (res != DialogResult.Yes)
                            return;

                        // Start over...
                        goto LocateInstall;
                    }

                    // Save the location
                    SteamInstallPath.Text = Path.GetDirectoryName(Dialog.FileName);
                }
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            Program.Config.UpdateCheck = updateCheckBox.Checked;
            Program.Config.SteamPath = SteamInstallPath.Text;
            Program.Config.IntegrateWithMod = linkCheckBox.Checked;
            Program.Config.Save();
            this.Close();
        }

        /// <summary>
        /// Adds the darker border line color between the header panel and the contents
        /// panel
        /// </summary>
        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(36, 36, 36), 1);
            Pen greyPen = new Pen(Color.FromArgb(62, 62, 62), 1);

            // Create points that define line.
            Point point1 = new Point(0, headerPanel.Height - 3);
            Point point2 = new Point(headerPanel.Width, headerPanel.Height - 3);
            e.Graphics.DrawLine(greyPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 2);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 2);
            e.Graphics.DrawLine(blackPen, point1, point2);

            // Create points that define line.
            point1 = new Point(0, headerPanel.Height - 1);
            point2 = new Point(headerPanel.Width, headerPanel.Height - 1);
            e.Graphics.DrawLine(greyPen, point1, point2);
        }

        /// <summary>
        /// Adds the darker border line color between the footer panel and the contents
        /// panel
        /// </summary>
        private void footerPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.FromArgb(62, 62, 62), 1);
            Pen greyPen = new Pen(Color.FromArgb(82, 82, 82), 1);

            // Create points that define line.
            Point point1 = new Point(0, 0);
            Point point2 = new Point(footerPanel.Width, 0);
            e.Graphics.DrawLine(greyPen, point1, point2);
        }
    }
}
