using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class SoundRegistryForm : Form
    {
        public SoundRegistryForm()
        {
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            using (AppDatabase db = new AppDatabase())
            {
                FillListView(db);
            }
        }

        private void FillListView(AppDatabase db)
        {
            // Clear the old
            soundListView.Items.Clear();
            soundListView.Groups[0].Items.Clear();
            soundListView.Groups[1].Items.Clear();

            List<string> installed = new List<string>();
            foreach (SoundPackage sound in db.EngineSounds)
            {
                installed.Add(sound.FolderName);
                ListViewItem item = new ListViewItem(sound.FolderName);
                item.SubItems.Add(sound.Name);
                item.Tag = sound;
                soundListView.Items.Add(item);
                soundListView.Groups[0].Items.Add(item);
            }

            // Grab sounds that are not installed
            string path = Path.Combine(Program.RootPath, "sounds", "engine");
            var folders = from x in Directory.GetDirectories(path)
                          where !installed.Contains(Path.GetFileName(x))
                          select x;

            // We only add packages that have both an interior and exterior file
            foreach (string folder in folders)
            {
                string ifile = Path.Combine(folder, "interior.sii");
                string efile = Path.Combine(folder, "exterior.sii");
                if (File.Exists(ifile) && File.Exists(efile))
                {
                    ListViewItem item = new ListViewItem(Path.GetFileName(folder));
                    item.SubItems.Add("");
                    soundListView.Items.Add(item);
                    soundListView.Groups[1].Items.Add(item);
                }
            }
        }

        private void soundListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (soundListView.SelectedItems.Count == 0)
            {
                packageNameBox.Text = "";
                intFilenameBox.Text = "";
                extFilenameBox.Text = "";
                removeButton.Enabled = false;
                updateButton.Enabled = false;
                return;
            }
            else
            {
                updateButton.Enabled = true;
            }

            ListViewItem item = soundListView.SelectedItems[0];
            bool installed = soundListView.Groups[0].Items.Contains(item);

            if (installed)
            {
                SoundPackage sound = (SoundPackage)item.Tag;
                packageNameBox.Text = sound.Name;
                intFilenameBox.Text = sound.InteriorFileName;
                extFilenameBox.Text = sound.ExteriorFileName;
                removeButton.Enabled = true;
                updateButton.Text = "Update";
            }
            else
            {
                packageNameBox.Text = "";
                intFilenameBox.Text = "";
                extFilenameBox.Text = "";
                removeButton.Enabled = false;
                updateButton.Text = "Install";
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (soundListView.SelectedItems.Count == 0) return;

            // Validate!
            if (!PassesValidaion()) return;

            ListViewItem item = soundListView.SelectedItems[0];
            bool installed = soundListView.Groups[0].Items.Contains(item);

            // Editing Sound
            if (installed)
            {
                SoundPackage sound = (SoundPackage)item.Tag;
                sound.Name = packageNameBox.Text;
                sound.InteriorFileName = intFilenameBox.Text;
                sound.ExteriorFileName = extFilenameBox.Text;
                item.SubItems[1].Text = sound.Name;

                // Update the sound in the database
                using (AppDatabase db = new AppDatabase())
                {
                    db.EngineSounds.Update(sound);
                }

                // Update the listview
                int i = soundListView.SelectedItems[0].Index;
                soundListView.RedrawItems(i, i + 1, false);
            }
            else // Adding new Sound
            {
                // Get our sound object
                SoundPackage sound = (SoundPackage)item.Tag;
                if (sound == null)
                    sound = new SoundPackage();

                // Set values
                sound.Name = packageNameBox.Text;
                sound.FolderName = item.SubItems[0].Text;
                sound.InteriorFileName = intFilenameBox.Text;
                sound.ExteriorFileName = extFilenameBox.Text;

                // Add the sound to the database
                try
                {
                    using (AppDatabase db = new AppDatabase())
                    {
                        db.EngineSounds.Add(sound);
                    }

                    // Change Groups
                    soundListView.Groups[1].Items.Remove(item);
                    soundListView.Groups[0].Items.Add(item);

                    // Update Buttons
                    removeButton.Enabled = true;
                    updateButton.Text = "Update";
                }
                catch (Exception ex)
                {
                    // Tell the user about the failed validation error
                    MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private bool PassesValidaion()
        {
            // Remove bad file system characters
            string intF = intFilenameBox.Text.MakeFileNameSafe();
            string extF = extFilenameBox.Text.MakeFileNameSafe();

            // Check for empty strings
            if (String.IsNullOrWhiteSpace(intF) || String.IsNullOrWhiteSpace(extF))
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "One or both filename inputs failed to pass validation. Please Try again",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return false;
            }

            // Add Extensions if they are missing
            if (!Path.HasExtension(intF)) intF += ".sii";
            if (!Path.HasExtension(extF)) extF += ".sii";

            // Check sound package name
            if (!Regex.Match(packageNameBox.Text, @"^[a-z0-9_.,\-\s\t()]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid sound package name string. Please use alpha-numeric, period, underscores, dashes or spaces only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return false;
            }

            // Set text box values again
            intFilenameBox.Text = intF;
            extFilenameBox.Text = extF;
            return true;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (soundListView.SelectedItems.Count == 0) return;

            ListViewItem item = soundListView.SelectedItems[0];
            SoundPackage sound = (SoundPackage)item.Tag;
            var items = sound.Series.ToList();

            if (items.Count > 0)
            {
                // Tell the use they are making a mistake!
                var result = MessageBox.Show($"This sound package is being use by \"{items[0]}\". If you proceed, all "
                    + "engines that use this sound package will also be deleted. Are you sure you want to do this?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                );

                // phew... /wipesSweatOffBrow
                if (result != DialogResult.Yes) return;
            }

            // Ok then... remove it!
            using (AppDatabase db = new AppDatabase())
            {
                db.EngineSounds.Remove(sound);

                // Set values
                packageNameBox.Text = "";
                intFilenameBox.Text = "";
                extFilenameBox.Text = "";
                removeButton.Enabled = false;
                updateButton.Text = "Install";

                // Fill List
                FillListView(db);
            }
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
