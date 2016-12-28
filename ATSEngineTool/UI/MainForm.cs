using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATSEngineTool.Database;
using ATSEngineTool.Properties;
using BrightIdeasSoftware;

namespace ATSEngineTool
{
    public partial class MainForm : Form
    {
        private MultipleListViewColumnSorter ListViewSorter { get; set; }

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

            // Enable sorting behavior on certain listviews
            ListViewSorter = new MultipleListViewColumnSorter();
            ListViewSorter.AddListView(truckListView2, SortOrder.Ascending);
            ListViewSorter.AddListView(seriesListView, SortOrder.Ascending);
            ListViewSorter.AddListView(transSeriesListView, SortOrder.Ascending);
            ListViewSorter.AddListView(packageListView, SortOrder.Ascending);

            // Fill form fields under 1 database connection
            using (AppDatabase db = new AppDatabase())
            {
                // Fill trucks
                FillTrucks(db);

                // Fill Engines
                FillEnginesSeries(db);

                // Fill Transmissions
                FillTransmissionSeries(db);

                // Fill Sound Packages
                FillSoundPackages(db);

                // Set label text
                dbVersionLabel.Text = AppDatabase.DatabaseVersion.ToString();
            }

            // Set sync enabled
            syncCheckBox.Enabled = Program.Config.IntegrateWithMod;
            var imp = (Program.Config.UnitSystem == UnitSystem.Imperial);
            engineListView.Columns[3].Text = (imp) ? "Torque" : "N·m";

            // Setup the TreeListView (sounds)
            soundListView.CanExpandGetter = model => ((SoundWrapper)model).ChildCount > 0;
            soundListView.ChildrenGetter = model => ((SoundWrapper)model).Children;

            // Check for updates?
            if (Program.Config.UpdateCheck)
            {
                ProgramUpdater.CheckCompleted += Updater_CheckCompleted;
                ProgramUpdater.CheckForUpdateAsync();
            }
        }

        #region Functions

        private void FillSoundPackages(AppDatabase db)
        {
            // Clear old junk
            packageListView.Items.Clear();
            soundListView.Items.Clear();

            // Fill in packages
            foreach (var package in db.SoundPackages)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = package;
                item.Text = package.ToString();
                packageListView.Items.Add(item);
            }

            // Sorting
            packageListView.Sort();
        }

        private void FillTransmissionSeries(AppDatabase db)
        {
            transSeriesListView.Items.Clear();
            transmissionListView.Items.Clear();

            // Fill in transmission series
            foreach (var series in db.TransmissionSeries)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = series;
                item.Text = series.ToString();
                transSeriesListView.Items.Add(item);
            }

            // Sorting
            transSeriesListView.Sort();

            // Disable engine buttons
            removeTransButton.Enabled = false;
            editTransButton.Enabled = false;
        }

        private void FillEnginesSeries(AppDatabase db)
        {
            seriesListView.Items.Clear();
            engineListView.Items.Clear();

            // Fill in trucks
            foreach (EngineSeries series in db.EngineSeries)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = series;
                item.Text = series.ToString();
                seriesListView.Items.Add(item);
            }

            // Sorting
            seriesListView.Sort();

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
            engineListView.Items.Clear();

            foreach (Engine engine in series.Engines.OrderByDescending(x => x.Horsepower))
            {
                ListViewItem item = new ListViewItem(engine.Name);
                item.SubItems.Add(engine.Price.ToString());
                item.SubItems.Add(engine.Horsepower.ToString());
                if (Program.Config.UnitSystem == UnitSystem.Imperial)
                    item.SubItems.Add(engine.Torque.ToString());
                else
                    item.SubItems.Add(engine.NewtonMetres.ToString());
                item.Tag = engine;
                engineListView.Items.Add(item);
            }
        }

        private void FillTransmissions(AppDatabase db)
        {
            // If we have no selected trucks, skimp out here
            if (transSeriesListView.SelectedItems.Count == 0) return;

            ListViewItem selected = transSeriesListView.SelectedItems[0];
            var series = selected.Tag as TransmissionSeries;

            // Clear transmissions
            transmissionListView.Items.Clear();

            foreach (var trans in series.Transmissions.OrderByDescending(x => x.Price))
            {
                var gears = trans.Gears.ToList();
                var forward = gears.Where(x => !x.IsReverse).Count();
                var reverse = gears.Count - forward;

                ListViewItem item = new ListViewItem(trans.Name);
                item.SubItems.Add(forward.ToString());
                item.SubItems.Add(reverse.ToString());
                item.SubItems.Add(trans.DifferentialRatio.ToString());
                item.Tag = trans;
                transmissionListView.Items.Add(item);
            }
        }

        #endregion Functions

        /// <summary>
        /// Event fired once the program update check has finished
        /// </summary>
        private void Updater_CheckCompleted(object sender, EventArgs e)
        {
            if (ProgramUpdater.UpdateAvailable)
            {
                appVersionLabel.ForeColor = Color.OrangeRed;
                updatePicture.Image = Resources.updateAvailable;
                updatePicture.Click += updatePicture_Click;
                toolTip1.SetToolTip(updatePicture, "Update Available! Click to open Download Page.");
            }
            else
            {
                updatePicture.Image = Resources.check;
                toolTip1.SetToolTip(updatePicture, "Program is up to date.");
            }
        }

        #region Truck Management Tab

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

        private async void truckListView2_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (truckListView2.SelectedItems.Count == 0) return;

            // Clear out existing data
            truckItemListView.BeginUpdate();
            truckItemListView.Items.Clear();
            truckItemListView.Groups.Clear();

            // Setup local variables
            ListViewItem selected = truckListView2.SelectedItems[0];
            Truck truck = selected.Tag as Truck;
            ListViewGroup group = new ListViewGroup() { Tag = -1 };
            Dictionary<int, ListViewGroup> groups = new Dictionary<int, ListViewGroup>();

            if (enginesToolStripMenuItem.Checked)
            {
                // DO this in a new thread, because its slow
                IEnumerable<Engine> engines = await Task.Run(() =>
                {
                    // Load engines from the database
                    using (AppDatabase db = new AppDatabase())
                    {
                        // Grab series and create a group for each one
                        foreach (var series in db.EngineSeries.OrderBy(x => x.ToString()))
                        {
                            group = new ListViewGroup(series.ToString());
                            group.Tag = series.Id;
                            groups.Add(series.Id, group);
                        }

                        // Grab the trucks engines, ordered by Brand, then by Horsepower
                        return truck.TruckEngines.Select(x => x.Engine).OrderByDescending(x => x.Horsepower);
                    }
                });

                // Add Groups
                truckItemListView.Groups.AddRange(groups.Values.ToArray());

                // Add truck items
                foreach (Engine eng in engines)
                {
                    // Setup group?
                    ListViewItem item = new ListViewItem();
                    item.Tag = eng;
                    item.Text = eng.Name;
                    groups[eng.SeriesId].Items.Add(item);
                    truckItemListView.Items.Add(item);
                }
            }
            else
            {
                // DO this in a new thread, because its slow
                var transmissions = await Task.Run(() =>
                {
                    // Load engines from the database
                    using (AppDatabase db = new AppDatabase())
                    {
                        // Grab series and create a group for each one
                        foreach (var series in db.TransmissionSeries.OrderBy(x => x.ToString()))
                        {
                            group = new ListViewGroup(series.ToString());
                            group.Tag = series.Id;
                            groups.Add(series.Id, group);
                        }

                        // Grab the trucks transmissions, ordered by Price
                        return truck.TruckTransmissions.Select(x => x.Transmission).OrderByDescending(x => x.Price);
                    }
                });

                // Add Groups
                truckItemListView.Groups.AddRange(groups.Values.ToArray());

                // Add truck items
                foreach (var trans in transmissions)
                {
                    ListViewItem item = new ListViewItem();
                    item.Tag = trans;
                    item.Text = trans.Name;
                    groups[trans.SeriesId].Items.Add(item);
                    truckItemListView.Items.Add(item);
                }
            }

            truckItemListView.EndUpdate();
            truckListView2.Focus();
        }

        private void enginesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (enginesToolStripMenuItem.Checked)
                return;

            enginesToolStripMenuItem.Checked = true;
            transmissionsToolStripMenuItem.Checked = false;
            truckItemListView.Columns[0].Text = "Selected Truck's Engines";
            truckListView2_ItemSelectionChanged(sender, null);
        }

        private void transmissionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (transmissionsToolStripMenuItem.Checked)
                return;

            enginesToolStripMenuItem.Checked = false;
            transmissionsToolStripMenuItem.Checked = true;
            truckItemListView.Columns[0].Text = "Selected Truck's Transmissions";
            truckListView2_ItemSelectionChanged(sender, null);
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
            truckItemListView.Items.Clear();
            truckItemListView.Groups.Clear();
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (truckListView2.SelectedItems.Count == 0) return;

            // Disable button spam 
            modifyButton.Enabled = false;

            ListViewItem selected = truckListView2.SelectedItems[0];
            Truck truck = selected.Tag as Truck;

            if (enginesToolStripMenuItem.Checked)
            {
                using (var frm = new EngineListEditor(truck))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        truckListView2_ItemSelectionChanged(this, null);
                    }
                }
            }
            else
            {
                using (var frm = new TransmissionListEditor(truck))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        truckListView2_ItemSelectionChanged(this, null);
                    }
                }
            }

            // Enable button
            modifyButton.Enabled = true;
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton1.ShowDropDown();
        }

        #endregion Truck Management Tab

        #region Engine Management

        private void seriesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (seriesListView.SelectedItems.Count == 0)
            {
                // Disable engine buttons
                deleteEngineButton.Enabled = false;
                editEngineButton.Enabled = false;
                engineListView.Items.Clear();
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

        private void engineListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = engineListView.HitTest(e.Location).Item;
            if (item != null)
            {
                ListViewItem selected = engineListView.SelectedItems[0];
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
            if (engineListView.SelectedItems.Count == 0) return;

            // Always verify that this isnt a mistake
            var engine = (Engine)engineListView.SelectedItems[0].Tag;
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
            if (engineListView.SelectedItems.Count == 0) return;

            ListViewItem selected = engineListView.SelectedItems[0];
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

        #endregion Engine Management

        #region Misc Events

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

        #endregion Misc Events

        #region Main Page Events

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

        private void updatePicture_MouseEnter(object sender, EventArgs e)
        {
            if (ProgramUpdater.UpdateAvailable)
            {
                updatePicture.SizeMode = PictureBoxSizeMode.Zoom;
                updatePicture.Cursor = Cursors.Hand;
            }
        }

        private void updatePicture_MouseLeave(object sender, EventArgs e)
        {
            if (ProgramUpdater.UpdateAvailable)
            {
                updatePicture.SizeMode = PictureBoxSizeMode.CenterImage;
                updatePicture.Cursor = Cursors.Default;
            }
        }

        private void updatePicture_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/wilson212/ATSEngineTool/releases/latest");
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
            TaskForm.Show(this, "Compiling Def Files", "Compiling Mod Files", true);
            TaskProgressUpdate update;
            this.Enabled = false;
            try
            {
                // Run this in a task to prevent GUI lockup
                await Task.Run(() => 
                {
                    // Clean out old compiled files
                    if (cleanCompCheckBox.Checked)
                    {
                        Mod.CleanCompileDirectory(TaskForm.Progress);
                    }

                    // Show Update
                    update = new TaskProgressUpdate();
                    update.MessageText = "Creating Def Files...";
                    TaskForm.Progress.Report(update);

                    // Compile Mod
                    var usedSounds = Mod.Compile(trucks, TaskForm.Progress);

                    // Are we sync'ing the Compiled and Mod folders?
                    if (syncCheckBox.Checked)
                    {
                        Mod.Sync(
                            usedSounds,
                            Program.Config.IntegrateWithMod && cleanModCheckBox.Checked,
                            Program.Config.IntegrateWithMod && cleanSoundsCheckBox.Checked,
                            TaskForm.Progress
                        );
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
            catch (Exception ex)
            {
                TaskForm.CloseForm();
                MessageBox.Show(ex.Message, "An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                if (TaskForm.IsOpen)
                {
                    TaskForm.CloseForm();
                }
            }
        }

        private void syncCheckBox_EnabledChanged(object sender, EventArgs e)
        {
            cleanModCheckBox.Enabled = syncCheckBox.Enabled;
            cleanSoundsCheckBox.Enabled = syncCheckBox.Enabled;
        }

        private void syncCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            cleanSoundsCheckBox.Enabled = syncCheckBox.Checked;
            cleanModCheckBox.Enabled = syncCheckBox.Checked;
        }

        #endregion Main Page Events

        #region Menu Events

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsForm frm = new SettingsForm())
            {
                frm.ShowDialog();
                syncCheckBox.Enabled = Program.Config.IntegrateWithMod;

                var imp = (Program.Config.UnitSystem == UnitSystem.Imperial);
                engineListView.Columns[3].Text = (imp) ? "Torque" : "N·m";
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

        private void integrityMenuItem_Click(object sender, EventArgs e)
        {
            using (AppDatabase db = new AppDatabase())
            {
                // Perform integrity check
                var wizard = new MigrationWizard(db);
                int issues = wizard.PerformIntegrityCheck();

                // Alert the user
                if (issues == 0)
                {
                    MessageBox.Show("No database integrity issues were found!", 
                        "Integrity Check", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show($"There are {issues} integrity errors in the database! "
                        + "Review the \"/errors/IntegrityErrors.log\" file for more information.", 
                        "Integrity Check", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                }
            }
        }

        private void vacuumMenuItem_Click(object sender, EventArgs e)
        {
            using (AppDatabase db = new AppDatabase())
            {
                // Perform integrity check
                var wizard = new MigrationWizard(db);
                wizard.VacuumDatabase();

                // Alert the user
                MessageBox.Show("Successfully vacuumed the database!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
        }

        private async void clearMenuItem_Click(object sender, EventArgs e)
        {
            // Verify that this is what the user wants to do!
            var result = MessageBox.Show(
                "Are you sure you want to clear the database of all data? This process cannot be reversed!",
                "Clear Database", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                // Show task form
                TaskForm.Show(this, "Clearing Database", "Clearing Database", "please wait...", false);
                this.Enabled = false;

                // Open database and clear
                using (AppDatabase db = new AppDatabase())
                using (var trans = db.BeginTransaction())
                {
                    try
                    {
                        // Run in a new thread
                        await Task.Run(() =>
                        {
                            // Clear tables, in order to prevent a foreignkey exception
                            db.TruckEngines.Clear();
                            db.TruckTransmissions.Clear();
                            db.TransmissionGears.Clear();
                            db.AccessoryConflicts.Clear();
                            db.SuitableAccessories.Clear();
                            db.Transmissions.Clear();
                            db.TransmissionSeries.Clear();
                            db.TorqueRatios.Clear();
                            db.Engines.Clear();
                            db.EngineSeries.Clear();
                            db.EngineSounds.Clear();
                            db.SoundPackages.Clear();
                            db.Trucks.Clear();

                            // Apply changes
                            trans.Commit();

                            // Compress database
                            db.Execute("VACUUM;");
                        });

                        // Clear form
                        truckListView1.Items.Clear();
                        truckListView2.Items.Clear();
                        seriesListView.Items.Clear();
                        engineListView.Items.Clear();
                        transSeriesListView.Items.Clear();
                        transmissionListView.Items.Clear();
                        packageListView.Items.Clear();
                        soundListView.Items.Clear();

                        // Close task form
                        TaskForm.CloseForm();
                        this.Enabled = true;

                        // Alert the user
                        MessageBox.Show("Successfully cleared the database of all data!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information
                        );
                    }
                    catch
                    {
                        this.Enabled = true;
                        TaskForm.CloseForm();
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion Menu Events

        #region Transmission Management

        private void transSeriesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we have no selected series, skimp out here
            if (transSeriesListView.SelectedItems.Count == 0)
            {
                // Disable transmission buttons
                removeTransButton.Enabled = false;
                editTransButton.Enabled = false;
                transmissionListView.Items.Clear();
            }
            else
            {
                // Re-fill the transmission list
                using (AppDatabase db = new AppDatabase())
                {
                    FillTransmissions(db);
                }

                // Enable transmission buttons
                removeTransButton.Enabled = true;
                editTransButton.Enabled = true;
            }
        }

        private void transSeriesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = transSeriesListView.HitTest(e.Location).Item;
            if (item != null)
            {
                ListViewItem selected = transSeriesListView.SelectedItems[0];
                var transmission = selected.Tag as TransmissionSeries;
                using (var frm = new TransSeriesEditForm(transmission))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        using (AppDatabase db = new AppDatabase())
                        {
                            FillTransmissionSeries(db);
                        }
                    }
                }
            }
        }

        private void addTransSeriesButton_Click(object sender, EventArgs e)
        {
            using (var frm = new TransSeriesEditForm())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                        FillTransmissionSeries(db);
                }
            }
        }

        private void removeTransSeriesButton_Click(object sender, EventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (transSeriesListView.SelectedItems.Count == 0) return;

            ListViewItem selected = transSeriesListView.SelectedItems[0];
            var series = selected.Tag as TransmissionSeries;

            var result = MessageBox.Show(
                $"Are you sure you want to the transmission series \"{series.ToString()}\"? "
                    + "Any and all transmissions that use this series will also be deleted in the process. "
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
                db.TransmissionSeries.Remove(series);

                FillTransmissionSeries(db);
            }
        }

        private void newTransButton_Click(object sender, EventArgs e)
        {
            using (var frm = new TransmissionForm())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                    {
                        FillTransmissions(db);
                    }
                }
            }
        }

        private void removeTransButton_Click(object sender, EventArgs e)
        {
            if (transmissionListView.SelectedItems.Count == 0) return;

            // Always verify that this isnt a mistake
            var trans = (Transmission)transmissionListView.SelectedItems[0].Tag;
            var result = MessageBox.Show(
                $"Are you sure you want to delete transmission \"{trans.Name}\"? This action cannot be undone!",
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
                if (!db.Transmissions.Remove(trans))
                {
                    MessageBox.Show(
                        $"Failed to delete transmission \"{trans.Name}\" from the database!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                else
                {
                    FillTransmissions(db);
                }
            }
        }

        private void editTransButton_Click(object sender, EventArgs e)
        {
            if (transmissionListView.SelectedItems.Count == 0) return;

            ListViewItem selected = transmissionListView.SelectedItems[0];
            var transmission = selected.Tag as Transmission;
            using (var frm = new TransmissionForm(transmission))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                    {
                        FillTransmissions(db);
                    }
                }
            }
        }

        private void transmissionListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = transmissionListView.HitTest(e.Location).Item;
            if (item != null)
            {
                ListViewItem selected = transmissionListView.SelectedItems[0];
                var transmission = selected.Tag as Transmission;
                using (var frm = new TransmissionForm(transmission))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        using (AppDatabase db = new AppDatabase())
                        {
                            FillTransmissions(db);
                        }
                    }
                }
            }
        }

        #endregion Transmission Management

        private void packageListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packageListView.SelectedItems.Count == 0) return;

            // Always verify that this isnt a mistake
            var package = (SoundPackage)packageListView.SelectedItems[0].Tag;
            var soundList = new List<EngineSound>(package.EngineSounds.OrderBy(x => x.Attribute));
            SoundWrapper wrapper = null;

            // Create tree Roots
            var objectList = new List<SoundWrapper>()
            {
                new SoundWrapper() { SoundName = "Interior", Sound = new EngineSound() { Type = SoundType.Interior } },
                new SoundWrapper() { SoundName = "Exterior", Sound = new EngineSound() { Type = SoundType.Exterior }},
            };

            // Add sounds to the tree roots
            wrapper = objectList[0];
            foreach (var sound in soundList.Where(x => x.Type == SoundType.Interior))
            {
                wrapper = AddSoundToList(sound, wrapper, objectList[0]);
            }

            wrapper = objectList[1];
            foreach (var sound in soundList.Where(x => x.Type == SoundType.Exterior))
            {
                wrapper = AddSoundToList(sound, wrapper, objectList[1]);
            }

            soundListView.Roots = objectList;
            //soundListView.Expand(objectList[0]);
        }

        private SoundWrapper AddSoundToList(EngineSound sound, SoundWrapper parent, SoundWrapper top)
        {
            SoundWrapper wrapper = parent;
            // If changine sound type or attribute
            if (parent == null || parent.Sound.Attribute != sound.Attribute)
            {
                if (sound.IsSoundArray)
                {
                    wrapper = new SoundWrapper() { Sound = sound, SoundName = sound.Attribute.ToString(), Parent = parent };
                    wrapper.Children.Add(new SoundWrapper()
                    {
                        Parent = wrapper,
                        SoundName = sound.Attribute.ToString(),
                        Sound = sound
                    });
                }
                else
                {
                    wrapper = new SoundWrapper()
                    {
                        Parent = parent,
                        SoundName = sound.Attribute.ToString(),
                        Sound = sound,
                    };
                }

                top.Children.Add(wrapper);
            }
            else
            {
                wrapper.Children.Add(new SoundWrapper()
                {
                    Parent = parent,
                    SoundName = sound.Attribute.ToString(),
                    Sound = sound
                });
            }

            return wrapper;
        }

        private void packageListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (packageListView.SelectedItems.Count == 0) return;
            SoundPackage package = packageListView.SelectedItems[0].Tag as SoundPackage;

            using (SoundPackageEditor frm = new SoundPackageEditor(package))
            {
                frm.ShowDialog();
            }
        }

        private void newPackageButton_Click(object sender, EventArgs e)
        {
            using (SoundPackageEditor frm = new SoundPackageEditor())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (AppDatabase db = new AppDatabase())
                    {
                        FillSoundPackages(db);
                    }
                }
            }
        }

        private void removePackageButton_Click(object sender, EventArgs e)
        {
            // If we have no selected packages, skimp out here
            if (packageListView.SelectedItems.Count == 0) return;

            ListViewItem selected = packageListView.SelectedItems[0];
            var package = selected.Tag as SoundPackage;
            var items = package.Series.ToList();

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
            else
            {
                // Tell the use they are making a mistake!
                var result = MessageBox.Show("Are you sure you want to remove this sound pack?",
                    "Verification", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                // phew... /wipesSweatOffBrow
                if (result != DialogResult.Yes) return;
            }

            // ReLoad engine series from the database
            using (AppDatabase db = new AppDatabase())
            {
                db.SoundPackages.Remove(package);

                FillSoundPackages(db);
            }

            // Finally delete sound folder
            DirectoryExt.Delete(Path.Combine(Program.RootPath, "sounds", "engine", package.FolderName));
        }

        private void soundListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // If we have no selected packages, skimp out here
            if (packageListView.SelectedItems.Count == 0) return;

            var selected = (OLVListItem)soundListView.HitTest(e.Location).Item;
            if (selected != null)
            {
                var wrapper = selected.RowObject as SoundWrapper;
                if (wrapper.ChildCount > 0)
                {
                    if (soundListView.IsExpanded(wrapper))
                        soundListView.Collapse(wrapper);
                    else
                        soundListView.Expand(wrapper);
                    return;
                }

                using (SoundEditor frm = new SoundEditor(wrapper.Sound))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        int index = soundListView.IndexOf(wrapper);
                        soundListView.RedrawItems(index, index, false);
                    }
                }
            }
        }

        private void soundListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            editSoundButton.Enabled = (
                soundListView.SelectedItem != null
                && ((SoundWrapper)soundListView.SelectedItem.RowObject).ChildCount == 0
            );
            removeSoundButton.Enabled = editSoundButton.Enabled;
            newSoundButton.Enabled = soundListView.SelectedItem != null;
        }

        private void editSoundButton_Click(object sender, EventArgs e)
        {
            var selected = soundListView.SelectedItem;
            if (selected == null) return;

            var wrapper = selected.RowObject as SoundWrapper;
            if (wrapper.ChildCount > 0) return;

            using (SoundEditor frm = new SoundEditor(wrapper.Sound))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    int index = soundListView.IndexOf(wrapper);
                    soundListView.RedrawItems(index, index, false);
                }
            }

        }

        private void removeSoundButton_Click(object sender, EventArgs e)
        {
            var selected = soundListView.SelectedItem;
            if (selected == null) return;

            var wrapper = selected.RowObject as SoundWrapper;
            if (wrapper.ChildCount > 0) return;

            // Tell the use they are making a mistake!
            var result = MessageBox.Show($"Are you sure you want to remove the sound \"{wrapper.SoundName}\"?",
                "Validation", MessageBoxButtons.YesNo, MessageBoxIcon.Question
            );

            // phew... /wipesSweatOffBrow
            if (result != DialogResult.Yes) return;

            // Open database
            using (AppDatabase db = new AppDatabase())
            {
                // Remove
                db.EngineSounds.Remove(wrapper.Sound);

                // Refill Sounds
                packageListView_SelectedIndexChanged(sender, e);
            }
        }

        private void newSoundButton_Click(object sender, EventArgs e)
        {
            // If we have no selected packages, skimp out here
            var item = soundListView.SelectedItem;
            if (item == null) return;

            var wrapper = item.RowObject as SoundWrapper;
            ListViewItem selected = packageListView.SelectedItems[0];
            var package = selected.Tag as SoundPackage;

            using (SoundEditor frm = new SoundEditor(package, wrapper.Catagory))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Refill Sounds
                    packageListView_SelectedIndexChanged(sender, e);
                }
            }
        }
    }
}
