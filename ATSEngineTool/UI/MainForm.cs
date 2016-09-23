using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            // Create form controls
            InitializeComponent();

            // Add version to title bar
            this.Text += " - " + Program.Version;
            appVersionLabel.Text = Program.Version.ToString();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            // Define steam and ATS installation path variables
            string steamPath = Program.Config.SteamPath;
            string atsPath = Path.Combine(steamPath ?? "temp", "SteamApps", "common",
                "American Truck Simulator", "bin", "win_x64", "amtrucks.exe");

            // If we are integrating with Real Engines and Sounds, Check for ATS installation
            if (Program.Config.IntegrateWithMod && (String.IsNullOrWhiteSpace(steamPath) || !File.Exists(atsPath)))
            {
                DialogResult res = MessageBox.Show(
                    "To take advantage of mod integration with the Real Engines and Sounds Mod, a path to your Steam Library Installation "
                        + "where American Truck Simulator is installed is required. Would you like to enable mod integration with "
                        + "Real Engines and Sounds? This setting can be changed later if you change your mind. ",
                    "Steam Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                // Quit here if the user does not want to show us where ATS is isntalled
                if (res == DialogResult.Yes)
                {
                    // Our loop back, incase the user selects the wrong Library
                    LocateInstall:
                    {
                        // Request the user supply the steam library path
                        OpenFileDialog Dialog = new OpenFileDialog();
                        Dialog.Title = "Steam Library Path where American Truck Simulator is Installed";
                        Dialog.FileName = "Steam.dll";
                        Dialog.Filter = "Steam Library|Steam.exe;Steam.dll";
                        if (Dialog.ShowDialog() == DialogResult.OK)
                        {
                            // Set paths
                            steamPath = Path.GetDirectoryName(Dialog.FileName);
                            atsPath = Path.Combine(steamPath, "SteamApps", "common",
                                "American Truck Simulator", "bin", "win_x64", "amtrucks.exe");

                            // If Ats is not installed here...
                            if (!File.Exists(atsPath))
                            {
                                // Alert the user that they are wrong...
                                res = MessageBox.Show(
                                    "Sadly, American Truck Simulator is not installed in this Steam Library. "
                                        + "Would you like to try another location?",
                                    "Steam Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                                );

                                // Start over ?
                                if (res == DialogResult.Yes)
                                {
                                    goto LocateInstall;
                                }
                                else
                                {
                                    Program.Config.IntegrateWithMod = false;
                                }
                            }

                            // Save the location
                            Program.Config.SteamPath = steamPath;
                            Program.Config.Save();
                        }
                        else
                        {
                            Program.Config.IntegrateWithMod = false;
                        }
                    }
                }
                else
                {
                    Program.Config.IntegrateWithMod = false;
                }
            }

            // Load database
            using (AppDatabase db = new AppDatabase())
            {
                FillTrucks(db);

                // Fill Engines
                FillEnginesSeries(db);

                // Set label text
                dbVersionLabel.Text = AppDatabase.DatabaseVersion.ToString();
            }

            // Set sync enabled
            syncCheckBox.Enabled = Program.Config.IntegrateWithMod;
        }

        private void FillEnginesSeries(AppDatabase db)
        {
            seriesListView.Items.Clear();
            engineListView2.Items.Clear();

            // Fill in trucks
            foreach (EngineSeries series in db.EngineSeries)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = series;
                item.Text = series.ToString();
                seriesListView.Items.Add(item);
            }

            // Disable engine buttons
            deleteEngineButton.Enabled = false;
            editEngineButton.Enabled = false;
        }

        private void FillTrucks(AppDatabase db)
        {
            truckListView2.Items.Clear();
            truckListView1.Items.Clear();

            // Fill in trucks
            foreach (Truck truck in db.Trucks)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = truck;
                item.Text = truck.Name;
                truckListView2.Items.Add(item);

                item = new ListViewItem();
                item.Tag = truck;
                item.Text = truck.Name;
                item.Checked = true;
                truckListView1.Items.Add(item);
            }
        }

        /// <summary>
        /// Fills the engine list using any filters set from the database
        /// </summary>
        /// <param name="db"></param>
        private void FillEngines(AppDatabase db)
        {
            // If we have no selected trucks, skimp out here
            if (seriesListView.SelectedItems.Count == 0) return;

            ListViewItem selected = seriesListView.SelectedItems[0];
            EngineSeries series = selected.Tag as EngineSeries;

            // Clear engines
            engineListView2.Items.Clear();

            foreach (Engine engine in series.Engines.OrderByDescending(x => x.Horsepower))
            {
                ListViewItem item = new ListViewItem(engine.Name);
                item.SubItems.Add(engine.Price.ToString());
                item.SubItems.Add(engine.Horsepower.ToString());
                item.SubItems.Add(engine.Torque.ToString());
                item.Tag = engine;
                engineListView2.Items.Add(item);
            }
        }

        private void truckListView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = truckListView2.HitTest(e.Location).Item;
            if (item != null)
            {
                ListViewItem selected = truckListView2.SelectedItems[0];
                Truck truck = selected.Tag as Truck;
                using (TruckEditForm frm = new TruckEditForm(truck))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        using (AppDatabase db = new AppDatabase())
                            FillTrucks(db);
                    }
                }
            }
        }

        private void truckListView2_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // Clear out existing data
            engineListView.Items.Clear();
            engineListView.Groups.Clear();

            // If we have no selected trucks, skimp out here
            if (truckListView2.SelectedItems.Count == 0) return;

            // Setup local variables
            ListViewItem selected = truckListView2.SelectedItems[0];
            Truck truck = selected.Tag as Truck;
            ListViewGroup group = new ListViewGroup() { Tag = -1 };

            // Load engines from the database
            using (AppDatabase db = new AppDatabase())
            {
                // Grab the trucks engines, ordered by Brand
                var engines = from x in truck.TruckEngines
                              orderby x.Engine.Series.ToString() ascending, 
                                      x.Engine.Horsepower descending
                              select x.Engine;

                // Fill in trucks
                foreach (Engine eng in engines)
                {
                    // Setup group?
                    if (((int)group.Tag) != eng.SeriesId)
                    {
                        group = new ListViewGroup(eng.Series.ToString());
                        group.Tag = eng.SeriesId;
                        engineListView.Groups.Add(group);
                    }

                    ListViewItem item = new ListViewItem();
                    item.Tag = eng;
                    item.Text = eng.Name;
                    group.Items.Add(item);
                    engineListView.Items.Add(item);
                }
            }
        }


        private void addTruckButton_Click(object sender, EventArgs e)
        {
            using (TruckEditForm frm = new TruckEditForm())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                        FillTrucks(db);
                }
            }
        }

        private void removeTruckButton_Click(object sender, EventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (truckListView2.SelectedItems.Count == 0) return;

            ListViewItem selected = truckListView2.SelectedItems[0];
            Truck sTruck = selected.Tag as Truck;

            var result = MessageBox.Show(
                $"Are you sure you want to delete truck \"{sTruck.Name}\"? This action cannot be undone!",
                "Verification",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // If the user changed his mind, return
            if (result != DialogResult.Yes)
                return;

            // Load engines from the database
            using (AppDatabase db = new AppDatabase())
            {
                db.Trucks.Remove(sTruck);

                // Fill in trucks
                FillTrucks(db);
            }

            // Clear out existing data
            engineListView.Items.Clear();
            engineListView.Groups.Clear();
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (truckListView2.SelectedItems.Count == 0) return;

            ListViewItem selected = truckListView2.SelectedItems[0];
            Truck truck = selected.Tag as Truck;

            using (EngineListEditor frm = new EngineListEditor(truck))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    truckListView2_ItemSelectionChanged(this, null);
                }
            }
        }

        private void seriesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (seriesListView.SelectedItems.Count == 0)
            {
                // Disable engine buttons
                deleteEngineButton.Enabled = false;
                editEngineButton.Enabled = false;
                engineListView2.Items.Clear();
            }
            else
            {
                // Re-fill the engine list
                using (AppDatabase db = new AppDatabase())
                {
                    FillEngines(db);
                }

                // Enable engine buttons
                deleteEngineButton.Enabled = true;
                editEngineButton.Enabled = true;
            }
        }

        private void seriesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = seriesListView.HitTest(e.Location).Item;
            if (item != null)
            {
                ListViewItem selected = seriesListView.SelectedItems[0];
                EngineSeries engine = selected.Tag as EngineSeries;
                using (SeriesEditForm frm = new SeriesEditForm(engine))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        using (AppDatabase db = new AppDatabase())
                        {
                            FillEnginesSeries(db);
                        }
                    }
                }
            }
        }

        private void engineListView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = engineListView2.HitTest(e.Location).Item;
            if (item != null)
            {
                ListViewItem selected = engineListView2.SelectedItems[0];
                Engine engine = selected.Tag as Engine;
                using (EngineForm frm = new EngineForm(engine))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        using (AppDatabase db = new AppDatabase())
                        {
                            FillEngines(db);
                        }
                    }
                }
            }
        }

        private void addSeriesButton_Click(object sender, EventArgs e)
        {
            using (SeriesEditForm frm = new SeriesEditForm())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                        FillEnginesSeries(db);
                }
            }
        }

        private void deleteSeriesButton_Click(object sender, EventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (seriesListView.SelectedItems.Count == 0) return;

            ListViewItem selected = seriesListView.SelectedItems[0];
            EngineSeries series = selected.Tag as EngineSeries;

            var result = MessageBox.Show(
                $"Are you sure you want to the engine series \"{series.ToString()}\"? "
                    + "Any and all engines that use this series will also be deleted in the process. "
                    + "This action cannot be undone!",
                "Verification",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // If the user changed his mind, return
            if (result != DialogResult.Yes)
                return;

            // ReLoad engine series from the database
            using (AppDatabase db = new AppDatabase())
            {
                db.EngineSeries.Remove(series);

                FillEnginesSeries(db);
            }
        }

        private void newEngineButton_Click(object sender, EventArgs e)
        {
            using (EngineForm frm = new EngineForm())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                    {
                        FillEngines(db);
                    }
                }
            }
        }

        private void deleteEngineButton_Click(object sender, EventArgs e)
        {
            if (engineListView2.SelectedItems.Count == 0) return;

            // Always verify that this isnt a mistake
            var engine = (Engine)engineListView2.SelectedItems[0].Tag;
            var result = MessageBox.Show(
                $"Are you sure you want to delete engine \"{engine.Name}\"? This action cannot be undone!",
                "Verification",
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning
            );

            // If the user changed his mind, return
            if (result != DialogResult.Yes)
                return;

            // Proceed to remove the engine from the database
            using (AppDatabase db = new AppDatabase())
            {
                // Alert the user only if there is an error
                if (!db.Engines.Remove(engine))
                {
                    MessageBox.Show(
                        $"Failed to delete engine \"{engine.Name}\" from the database!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                else
                {
                    FillEngines(db);
                }
            }
        }

        private void editEngineButton_Click(object sender, EventArgs e)
        {
            if (engineListView2.SelectedItems.Count == 0) return;

            ListViewItem selected = engineListView2.SelectedItems[0];
            Engine engine = selected.Tag as Engine;
            using (EngineForm frm = new EngineForm(engine))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                    {
                        FillEngines(db);
                    }
                }
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

        private void steamAppLink_Click(object sender, EventArgs e)
        {
            Process.Start($"http://store.steampowered.com/app/{SteamAppLabel.Text}/");
        }

        private void steamAppFolder_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(
                Program.Config.SteamPath, "SteamApps", "common",
                "American Truck Simulator"
            );

            // Ensure the folder exists before trying to open
            if (!Directory.Exists(path))
            {
                MessageBox.Show(
                    "The game folder appears to be missing! Make sure you have the game installed.",
                    "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
            else
            {
                Process.Start(path);
            }
        }

        private void workshopLink_Click(object sender, EventArgs e)
        {
            Process.Start($"http://steamcommunity.com/sharedfiles/filedetails/?id={WorkshopIdLabel.Text}/");
        }

        private void workshopModFolder_Click(object sender, EventArgs e)
        {
            // Ensure the folder exists before trying to open
            if (!Directory.Exists(Mod.ModPath))
            {
                MessageBox.Show(
                    "The mod folder appears to be missing! Make sure you are subscribed to the mod.",
                    "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
            else
            {
                Process.Start(Mod.ModPath);
            }
        }

        private void appLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/wilson212/ATSEngineTool");
        }

        private void appFolder_Click(object sender, EventArgs e)
        {
            Process.Start(Program.RootPath);
        }

        private void appLink_MouseEnter(object sender, EventArgs e)
        {
            appLink.SizeMode = PictureBoxSizeMode.Zoom;
            appLink.Cursor = Cursors.Hand;
        }

        private void appLink_MouseLeave(object sender, EventArgs e)
        {
            appLink.SizeMode = PictureBoxSizeMode.CenterImage;
            appLink.Cursor = Cursors.Default;
        }

        private void appFolder_MouseEnter(object sender, EventArgs e)
        {
            appFolder.SizeMode = PictureBoxSizeMode.Zoom;
            appFolder.Cursor = Cursors.Hand;
        }

        private void appFolder_MouseLeave(object sender, EventArgs e)
        {
            appFolder.SizeMode = PictureBoxSizeMode.CenterImage;
            appFolder.Cursor = Cursors.Default;
        }

        private void steamAppLink_MouseEnter(object sender, EventArgs e)
        {
            steamAppLink.SizeMode = PictureBoxSizeMode.Zoom;
            steamAppLink.Cursor = Cursors.Hand;
        }

        private void steamAppLink_MouseLeave(object sender, EventArgs e)
        {
            steamAppLink.SizeMode = PictureBoxSizeMode.CenterImage;
            steamAppLink.Cursor = Cursors.Default;
        }

        private void steamAppFolder_MouseEnter(object sender, EventArgs e)
        {
            steamAppFolder.SizeMode = PictureBoxSizeMode.Zoom;
            steamAppFolder.Cursor = Cursors.Hand;
        }

        private void steamAppFolder_MouseLeave(object sender, EventArgs e)
        {
            steamAppFolder.SizeMode = PictureBoxSizeMode.CenterImage;
            steamAppFolder.Cursor = Cursors.Default;
        }

        private void workshopLink_MouseEnter(object sender, EventArgs e)
        {
            workshopLink.SizeMode = PictureBoxSizeMode.Zoom;
            workshopLink.Cursor = Cursors.Hand;
        }

        private void workshopLink_MouseLeave(object sender, EventArgs e)
        {
            workshopLink.SizeMode = PictureBoxSizeMode.CenterImage;
            workshopLink.Cursor = Cursors.Default;
        }

        private void workshopModFolder_MouseEnter(object sender, EventArgs e)
        {
            workshopModFolder.SizeMode = PictureBoxSizeMode.Zoom;
            workshopModFolder.Cursor = Cursors.Hand;
        }

        private void workshopModFolder_MouseLeave(object sender, EventArgs e)
        {
            workshopModFolder.SizeMode = PictureBoxSizeMode.CenterImage;
            workshopModFolder.Cursor = Cursors.Default;
        }

        private async void syncButton_Click(object sender, EventArgs e)
        {
            // Grab Trucks
            List<Truck> trucks = new List<Truck>();
            foreach (ListViewItem item in truckListView1.Items)
            {
                if (item.Checked)
                {
                    Truck truck = item.Tag as Truck;
                    trucks.Add(truck);
                }
            }

            // Ensure we have a truck selected
            if (trucks.Count == 0)
            {
                MessageBox.Show("You must select at at least 1 truck before proceeding.", 
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Show loading form and lock this one
            LoadingForm.ShowScreen(this, false);
            this.Enabled = false;
            try
            {
                // Run this in a task to prevent GUI lockup
                await Task.Run(() => 
                { 
                    // Clean out old compiled files
                    if (cleanCompCheckBox.Checked)
                        Mod.CleanCompileDirectory();

                    // Compile Mod
                    Mod.Compile(trucks);

                    // Are we sync'ing the Compiled and Mod folders?
                    if (syncCheckBox.Checked)
                    {
                        Mod.Sync(
                            Program.Config.IntegrateWithMod && cleanModCheckBox.Checked,
                            Program.Config.IntegrateWithMod && cleanSoundsCheckBox.Checked
                        );
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                LoadingForm.CloseForm();
            }
        }

        private void syncCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            cleanSoundsCheckBox.Enabled = syncCheckBox.Checked;
            cleanModCheckBox.Enabled = syncCheckBox.Checked;
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsForm frm = new SettingsForm())
            {
                frm.ShowDialog();
                syncCheckBox.Enabled = Program.Config.IntegrateWithMod;
            }
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm frm = new AboutForm())
            {
                frm.ShowDialog();
            }
        }

        private void soundMenuItem_Click(object sender, EventArgs e)
        {
            using (SoundRegistryForm frm = new SoundRegistryForm())
            {
                frm.ShowDialog();
            }
        }

        private void syncCheckBox_EnabledChanged(object sender, EventArgs e)
        {
            cleanModCheckBox.Enabled = syncCheckBox.Enabled;
            cleanSoundsCheckBox.Enabled = syncCheckBox.Enabled;
        }
    }
}
