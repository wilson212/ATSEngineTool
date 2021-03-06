﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ATSEngineTool.Database;
using ATSEngineTool.SiiEntities;
using FreeImageAPI;
using Sii;

namespace ATSEngineTool
{
    public partial class TransmissionForm: Form
    {
        const int MAX_RPM = 2000;
        const int SHIFTING_RPM = 1500;

        /// <summary>
        /// Internal testing points to 39.5 inches tires in game
        /// </summary>
        const decimal TIRE_DIAMETER = 39.5m;

        /// <summary>
        /// Inches in a mile / (60 min/hr * PI)
        /// </summary>
        const decimal DISTANCE_MILES = 336.13m;

        /// <summary>
        /// Inches in a kilometer / (60 min/hr * PI)
        /// </summary>
        const decimal DISTANCE_KILOS = 208.97m;

        /// <summary>
        /// Meteric Affix
        /// </summary>
        public string Affix = (Program.Config.UnitSystem == UnitSystem.Imperial) ? "Mph" : "Kph";

        /// <summary>
        /// Our engine WIP
        /// </summary>
        public Transmission Transmission { get; protected set; }

        /// <summary>
        /// Indicates whether this is a new engine we are creating, or
        /// an engine we are editing
        /// </summary>
        protected bool NewTransmission = false;

        /// <summary>
        /// Gets a list of all reverse gears
        /// </summary>
        protected List<TransmissionGear> ReverseGears { get; set; } = new List<TransmissionGear>();

        /// <summary>
        /// Gets a list of all forward gears
        /// </summary>
        protected List<TransmissionGear> ForwardGears { get; set; } = new List<TransmissionGear>();

        /// <summary>
        /// Indicates whether the selected items on the Conflicts tab have changed
        /// </summary>
        protected bool ConflictsChanged { get; set; } = false;

        /// <summary>
        /// Indicates whether the selected items on the Suitables tab have changed
        /// </summary>
        protected bool SuitablesChanged { get; set; } = false;

        /// <summary>
        /// Indicates whether the selected items on the Trucks tab have changed
        /// </summary>
        protected bool TrucksChanged { get; set; } = false;

        /// <summary>
        /// Indicates whether the any gearing data has changed
        /// </summary>
        protected bool GearsChanged { get; set; } = false;

        /// <summary>
        /// Icon file path
        /// </summary>
        protected static string MatPath = Path.Combine(Program.RootPath, "graphics");

        /// <summary>
        /// Provides sorting functionality to the truck list view
        /// </summary>
        protected ListViewColumnSorter Sorter { get; set; }

        public TransmissionForm(Transmission transmission = null)
        {
            // Create form controls
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);
            chart1.ChartAreas[0].AxisX.Interval = 10;
            chart1.MouseWheel += Chart1_MouseWheel;

            NewTransmission = transmission == null;
            Transmission = transmission ?? new Transmission();
            Sorter = new ListViewColumnSorter(truckListView) { Order = SortOrder.Ascending };

            // Setup metrics
            if (Program.Config.UnitSystem == UnitSystem.Metric)
            {
                chart1.ChartAreas[0].AxisX.Title = "Speed (Kph)";
                conflictListView.Columns[2].Text = "N·m";
                suitsListView.Columns[2].Text = "N·m";
            }

            // Add each sound to the lists
            using (AppDatabase db = new AppDatabase())
            {
                foreach (var model in db.TransmissionSeries.OrderBy(x => x.ToString()))
                {
                    seriesModelBox.Items.Add(model);
                    if (!NewTransmission && model.Id == Transmission.SeriesId)
                        seriesModelBox.SelectedIndex = seriesModelBox.Items.Count - 1;
                }

                if (!NewTransmission)
                {
                    var gears = transmission.Gears.ToList();
                    ForwardGears.AddRange(gears.Where(x => !x.IsReverse));
                    ReverseGears.AddRange(gears.Where(x => x.IsReverse));
                }
                else
                {
                    ReverseGears.Add(new TransmissionGear() { Ratio = -5.55m });
                    ForwardGears.AddRange(new[]
                    {
                        new TransmissionGear() { Ratio = 4.7m },
                        new TransmissionGear() { Ratio = 2.21m },
                        new TransmissionGear() { Ratio = 1.53m },
                        new TransmissionGear() { Ratio = 1.0m },
                        new TransmissionGear() { Ratio = 0.76m },
                        new TransmissionGear() { Ratio = 0.67m }
                    });
                }

                // Grab a list of trucks that use this transmission
                List<int> trucks = new List<int>();
                if (!NewTransmission)
                    trucks.AddRange(transmission.ItemOf.Select(x => x.TruckId));

                // Add trucks to the truck list
                foreach (var truck in db.Trucks)
                {
                    ListViewItem item = new ListViewItem(truck.Name);
                    item.Tag = truck.Id;
                    item.Checked = trucks.Contains(truck.Id);
                    truckListView.Items.Add(item);
                }
            }

            // Are we editing an engine?
            if (!NewTransmission)
            {
                // Set form values
                unitNameBox.Text = transmission.UnitName;
                transNameBox.Text = transmission.Name;
                unlockBox.Value = transmission.Unlock;
                priceBox.Value = transmission.Price;
                diffRatio.Value = transmission.DifferentialRatio;
                if (transmission.StallTorqueRatio > 0m)
                {
                    stallRatio.Value = transmission.StallTorqueRatio;
                    hasTorqueConverter.Checked = true;
                }
                if (transmission.Retarder > 0)
                {
                    retardPositions.Value = transmission.Retarder;
                    hasRetarder.Checked = true;
                }
                fileDefaultsTextBox.Lines = transmission.Defaults;
                fileCommentTextBox.Lines = transmission.Comment;
                filenameTextBox.Text = transmission.FileName;
                conflictsTextBox.Lines = transmission.Conflicts;
                suitablesTextBox.Lines = transmission.SuitableFor;
            }

            // Fill torque ratios
            PopulateGears();

            PopulateEngines();
        }

        /// <summary>
        /// Fills the torque ratio Listview
        /// </summary>
        private void PopulateGears()
        {
            // Clear out any old items
            gearListView.Items.Clear();

            // Reorder
            ReverseGears = ReverseGears.OrderBy(x => x.Ratio).ToList();
            ForwardGears = ForwardGears.OrderByDescending(x => x.Ratio).ToList();

            // Reverse Gears
            int i = 0;
            foreach (var gear in ReverseGears)
            {
                gear.GearIndex = i;
                AddGear(gear, i++);
            }

            // Forward Gears
            i = 0;
            foreach (var gear in ForwardGears)
            {
                gear.GearIndex = i;
                AddGear(gear, i++);
            }
        }

        private void PopulateEngines()
        {
            // Clear transmissions
            int groupId = -1;
            ListViewGroup group1 = null;
            ListViewGroup group2 = null;

            using (AppDatabase db = new AppDatabase())
            {
                var conflicts = new List<int>();
                var suits = new List<int>();
                if (!NewTransmission)
                {
                    conflicts.AddRange(Transmission.EngineConflicts.Select(x => x.EngineId));
                    suits.AddRange(Transmission.SuitableEngines.Select(x => x.EngineId));
                }

                foreach (var engine in db.Engines.OrderBy(x => x.SeriesId).ThenByDescending(x => x.Price))
                {
                    if (engine.SeriesId != groupId)
                    {
                        groupId = engine.SeriesId;
                        var series = engine.Series; // lazy loaded... load just once
                        group1 = new ListViewGroup(series.Name);
                        conflictListView.Groups.Add(group1);
                        group2 = new ListViewGroup(series.Name);
                        suitsListView.Groups.Add(group2);
                    }

                    // Add transmission to conflicts box
                    ListViewItem item = new ListViewItem(engine.Name);
                    item.Checked = conflicts.Contains(engine.Id);
                    item.SubItems.Add(engine.Horsepower.ToString());
                    if (Program.Config.UnitSystem == UnitSystem.Imperial)
                        item.SubItems.Add(engine.Torque.ToString());
                    else
                        item.SubItems.Add(engine.NewtonMetres.ToString());
                    item.Tag = engine.Id;
                    group1.Items.Add(item);
                    conflictListView.Items.Add(item);

                    // Add transmission to suitibles box
                    item = new ListViewItem(engine.Name);
                    item.Checked = suits.Contains(engine.Id);
                    item.SubItems.Add(engine.Horsepower.ToString());
                    if (Program.Config.UnitSystem == UnitSystem.Imperial)
                        item.SubItems.Add(engine.Torque.ToString());
                    else
                        item.SubItems.Add(engine.NewtonMetres.ToString());
                    item.Tag = engine.Id;
                    group2.Items.Add(item);
                    suitsListView.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Adds a gear to the Gear List View
        /// </summary>
        /// <param name="gear"></param>
        /// <param name="index"></param>
        private void AddGear(TransmissionGear gear, int index)
        {
            string name = (showPlaceholdersBox.Checked) ? GetGearNameAtIndex(index, gear) : gear.Name;

            ListViewItem item = new ListViewItem((index + 1).ToString());
            item.SubItems.Add(name);
            item.SubItems.Add($"{gear.Ratio}:1");
            item.Tag = index;

            index = (gear.IsReverse) ? 0 : 1;
            gearListView.Groups[index].Items.Add(item);
            gearListView.Items.Add(item);
        }

        /// <summary>
        /// Method called whenever a gear is modified. This method will move the gear
        /// to the Forward or Reverse gear depending.
        /// </summary>
        /// <param name="oldGear">The gear before the changes</param>
        /// <param name="newGear">The new gear values</param>
        /// <param name="index">The index the oldGear is at in its corresponding list</param>
        /// <returns></returns>
        private bool OnGearEdit(TransmissionGear oldGear, TransmissionGear newGear, int index)
        {
            // Are we switching from forward to reverse (or vise-versa)?
            if (oldGear.IsReverse != newGear.IsReverse)
            {
                if (newGear.IsReverse)
                {
                    ReverseGears.Add(newGear);
                    ForwardGears.RemoveAt(index);
                }
                else
                {
                    ForwardGears.Add(newGear);
                    ReverseGears.RemoveAt(index);
                }
            }
            else
            {
                var list = (newGear.IsReverse) ? ReverseGears : ForwardGears;
                list[index] = newGear;
            }

            return true;
        }

        /// <summary>
        /// Gets the gear name, or if the gear does not have a name, generates a name based off 
        /// of the gear index.
        /// </summary>
        /// <param name="index">The index of the gear in the list (sorted by ratio desc)</param>
        /// <param name="gear">The gear we are fetching the name for</param>
        /// <returns></returns>
        private string GetGearNameAtIndex(int index, TransmissionGear gear)
        {
            if (gear.IsReverse)
                return Transmission.GetGearNameAtIndex(index, gear, ReverseGears);
            else
                return Transmission.GetGearNameAtIndex(index, gear, ForwardGears);
        }

        /// <summary>
        /// Gets the truck speed based off of Engine RPM, and a few other variables.
        /// </summary>
        /// <param name="rpm">The current engine RPM</param>
        /// <param name="gear">The current gear the truck is in</param>
        /// <returns>The truck speed in MPH or KPH depending on the <see cref="UnitSystem"/> configured</returns>
        private int GetSpeedFromRpm(int rpm, TransmissionGear gear)
        {
            decimal axelRatio = diffRatio.Value;
            decimal distance = (Program.Config.UnitSystem == UnitSystem.Imperial)
                ? DISTANCE_MILES
                : DISTANCE_KILOS;

            // Now the real math starts
            distance = (distance * axelRatio * gear.Ratio) / TIRE_DIAMETER;
            return (int)Math.Round(rpm / distance, 0);
        }

        /// <summary>
        /// Gets the engine RPM based on truck speed, and other variables.
        /// </summary>
        /// <param name="speed">
        /// The truck speed in MPH or KPH depending on the <see cref="UnitSystem"/> configured
        /// </param>
        /// <param name="gear">The current gear the truck is in</param>
        /// <returns></returns>
        private int GetRpmFromSpeed(int speed, TransmissionGear gear)
        {
            decimal axelRatio = diffRatio.Value;
            decimal distance = (Program.Config.UnitSystem == UnitSystem.Imperial)
                ? DISTANCE_MILES
                : DISTANCE_KILOS;

            return (int)Math.Round((axelRatio * speed * gear.Ratio * distance) / TIRE_DIAMETER, 0);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            // Check UnitName
            // Check for a valid identifier string
            if (!SiiFileBuilder.IsValidUnitName(unitNameBox.Text))
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid Transmission Sii Unit Name. Tokens must be 1 to 12 characters in length, seperated by a dot, "
                        + "and contain alpha-numeric or underscores only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Check engine name
            if (transNameBox.Text.Length < 2 || transNameBox.Text.Contains('"'))
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid Transmission Name string! The name must be at least 2 characters long and contain no quotes",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Set new attribute values
            Transmission.SeriesId = ((TransmissionSeries)seriesModelBox.SelectedItem).Id;
            Transmission.UnitName = unitNameBox.Text.Trim();
            Transmission.Name = transNameBox.Text.Trim();
            Transmission.Price = (int)priceBox.Value;
            Transmission.Unlock = (int)unlockBox.Value;
            Transmission.DifferentialRatio = diffRatio.Value;
            Transmission.Defaults = fileDefaultsTextBox.Lines;
            Transmission.Comment = fileCommentTextBox.Lines;
            Transmission.Conflicts = conflictsTextBox.Lines;
            Transmission.SuitableFor = suitablesTextBox.Lines;
            Transmission.Retarder = (hasRetarder.Checked) ? (int)retardPositions.Value : 0;
            Transmission.StallTorqueRatio = (hasTorqueConverter.Checked) ? stallRatio.Value : 0.0m;

            // Figure out the filename
            if (!String.IsNullOrWhiteSpace(filenameTextBox.Text))
                Transmission.FileName = filenameTextBox.Text.Trim();

            // Validate and Save
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                // Verify that the series.Id and engine.UnitName are unique
                string query = "SELECT * FROM `Transmission` WHERE `SeriesId`=@P0 AND `UnitName`=@P1";
                var eTrans = db.Query<Transmission>(query, Transmission.SeriesId, Transmission.UnitName).FirstOrDefault();
                if (eTrans != null && (NewTransmission || eTrans.Id != Transmission.Id))
                {
                    // Tell the user this isnt allowed
                    MessageBox.Show(
                        $"The selected Transmission Series already contains a transmission with the Sii Unit Name of \""
                        + Transmission.UnitName + "\"! Please select a different Transmission Series or change the Sii "
                        + "Unit Name to something unique.",
                        "Unique Constraint Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    return;
                }

                // Wrap database changes in a try-catch block so we can Rollback on error
                try
                {
                    // Update the current engine
                    db.Transmissions.AddOrUpdate(Transmission);

                    // If pre-existing transmission, delete all changed data
                    if (!NewTransmission)
                    {
                        // Delete any and all TorueRatios from the database
                        if (GearsChanged)
                        {
                            foreach (var gear in Transmission.Gears)
                                db.TransmissionGears.Remove(gear);
                        }

                        // Set Conflicts
                        if (ConflictsChanged)
                        {
                            foreach (var conflict in Transmission.EngineConflicts)
                                db.AccessoryConflicts.Remove(conflict);
                        }

                        // Set Suitables
                        if (SuitablesChanged)
                        {
                            foreach (var item in Transmission.SuitableEngines)
                                db.SuitableAccessories.Remove(item);
                        }

                        // Set Conflicts
                        if (TrucksChanged)
                        {
                            foreach (var item in Transmission.ItemOf)
                                db.TruckTransmissions.Remove(item);
                        }
                    }

                    // Add conflicts if any
                    if ((NewTransmission || ConflictsChanged) && conflictListView.CheckedItems.Count > 0)
                    {
                        var ids = conflictListView.CheckedItems.Cast<ListViewItem>().Select(x => (int)x.Tag);
                        foreach (var item in ids)
                        {
                            db.AccessoryConflicts.Add(new AccessoryConflict()
                            {
                                EngineId = item,
                                TransmissionId = Transmission.Id
                            });
                        }
                    }

                    // Add suitible fors if any
                    if ((NewTransmission || SuitablesChanged) && suitsListView.CheckedItems.Count > 0)
                    {
                        var ids = suitsListView.CheckedItems.Cast<ListViewItem>().Select(x => (int)x.Tag);
                        foreach (var item in ids)
                        {
                            db.SuitableAccessories.Add(new SuitableAccessory()
                            {
                                EngineId = item,
                                TransmissionId = Transmission.Id
                            });
                        }
                    }

                    // Add trucks if any
                    if ((NewTransmission || TrucksChanged) && truckListView.CheckedItems.Count > 0)
                    {
                        var ids = truckListView.CheckedItems.Cast<ListViewItem>().Select(x => (int)x.Tag);
                        foreach (var item in ids)
                        {
                            db.TruckTransmissions.Add(new TruckTransmission()
                            {
                                TruckId = item,
                                TransmissionId = Transmission.Id
                            });
                        }
                    }

                    // Add the new torque ratios
                    if (NewTransmission || GearsChanged)
                    {
                        int i = 0;
                        foreach (var gear in ReverseGears.OrderBy(x => x.Ratio))
                        {
                            gear.TransmissionId = Transmission.Id;
                            gear.GearIndex = i++;
                            db.TransmissionGears.Add(gear);
                        }
                        foreach (var gear in ForwardGears.OrderByDescending(x => x.Ratio))
                        {
                            gear.TransmissionId = Transmission.Id;
                            gear.GearIndex = i++;
                            db.TransmissionGears.Add(gear);
                        }
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    // Tell the user about the failed validation error
                    MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void seriesModelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Unlock all other controls
            if (!transNameBox.Enabled)
            {
                unitNameBox.Enabled = true;
                transNameBox.Enabled = true;
                unlockBox.Enabled = true;
                priceBox.Enabled = true;
                filenameTextBox.Enabled = true;
                hasRetarder.Enabled = true;
                hasTorqueConverter.Enabled = true;
                diffRatio.Enabled = true;
                stallRatio.Enabled = true;
                retardPositions.Enabled = true;
                confirmButton.Enabled = true;
            }

            // Perform cleanup
            if (seriesIcon.Image != null)
            {
                seriesIcon.Image.Dispose();
                seriesIcon.Image = null;
            }

            var series = seriesModelBox.SelectedItem as TransmissionSeries;
            if (series == null)
                return;

            // Load image
            string path = Path.Combine(MatPath, series.Icon);
            if (!path.EndsWith(".dds"))
                path += ".dds";

            // Ensure icon exists before proceeding
            if (!File.Exists(path)) return;

            // Attempt to load image as a DDS file... or png if its a mod sometimes
            FREE_IMAGE_FORMAT Format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
            Bitmap MapImage = FreeImage.LoadBitmap(path, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref Format);
            if (MapImage != null)
            {
                seriesIcon.Image = new Bitmap(MapImage, 256, 64);
            }
        }

        /// <summary>
        /// Plots the points (Rpm / speed) on the chart
        /// </summary>
        private void diffRatio_ValueChanged(object sender, EventArgs e)
        {
            // Clear old chart points
            chart1.Series[0].Points.Clear();

            int count = ForwardGears.Count - 1;
            int currentRpm = 0;
            int currentSpd = 0;

            // Fill speeds at RPM intervals for each gear
            int i = 0;
            foreach (var gear in ForwardGears)
            {
                // Get our gear name
                string name = GetGearNameAtIndex(i, gear);

                // After a shift, get the new RPM for our truck speed
                currentRpm = GetRpmFromSpeed(currentSpd, gear);

                // Low Plot
                int index = chart1.Series[0].Points.AddXY(currentSpd, currentRpm);
                DataPoint point = chart1.Series[0].Points[index];
                point.ToolTip = $"[{name}] {currentSpd} {Affix} @ {currentRpm} RPM";

                // Now @ shifting RPM, get our new speed
                int rpm = (i == count) ? MAX_RPM : SHIFTING_RPM;
                currentSpd = GetSpeedFromRpm(SHIFTING_RPM, gear);

                // High Plot
                index = chart1.Series[0].Points.AddXY(currentSpd, SHIFTING_RPM);
                point = chart1.Series[0].Points[index];
                point.ToolTip = $"[{name}] {currentSpd} {Affix} @ {SHIFTING_RPM} RPM";
                point.Label = name;

                // Final Gear?
                if (i == count)
                {
                    // Post Max Speed
                    currentSpd = GetSpeedFromRpm(MAX_RPM, gear);
                    index = chart1.Series[0].Points.AddXY(currentSpd, MAX_RPM);
                    point = chart1.Series[0].Points[index];
                    point.ToolTip = $"[{name}] {currentSpd} {Affix} @ {MAX_RPM} RPM";
                    point.Label = name;
                }
                i++;
            }
        }

        private void unitNameBox_TextChanged(object sender, EventArgs e)
        {
            if (unitNameBox.Text.Length == 0) return;

            string text = unitNameBox.Text.MakeFileNameSafe();
            filenameTextBox.Text = text + ".sii";
        }

        private void gearListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gearListView.SelectedItems.Count == 0) return;
            removeGearButton.Enabled = true;
        }

        private void removePointButton_Click(object sender, EventArgs e)
        {
            if (gearListView.SelectedItems.Count == 0) return;

            // Remove torque ratio
            ListViewItem item = gearListView.SelectedItems[0];
            bool isReverse = gearListView.Groups[0].Items.Contains(item);
            int index = (int)item.Tag;

            // Remove gear
            var list = (isReverse) ? ReverseGears : ForwardGears;
            list.RemoveAt(index);

            // Force Points Redraw
            PopulateGears();

            // Flag 
            GearsChanged = true;

            // Force a chart redraw
            diffRatio_ValueChanged(this, EventArgs.Empty);
        }

        private void gearListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (gearListView.SelectedItems.Count == 0) return;

            ListViewItem item = gearListView.SelectedItems[0];
            bool isReverse = gearListView.Groups[0].Items.Contains(item);
            var list = (isReverse) ? ReverseGears : ForwardGears;
            int index = (int)item.Tag;

            // Process input
            if (e.KeyCode == Keys.Delete)
            {
                // Remove gear
                list.RemoveAt(index);

                // Force Points Redraw
                PopulateGears();

                // Flag 
                GearsChanged = true;

                // Force a chart redraw
                diffRatio_ValueChanged(this, EventArgs.Empty);

                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                var gear = list[index];
                using (var frm = new EditGearForm(gear))
                {
                    // Show form
                    var result = frm.ShowDialog();
                    if (result == DialogResult.Yes)
                    {
                        // Process the modifcation
                        if (OnGearEdit(gear, frm.GetGear(), index))
                        {
                            // Force Points Redraw
                            PopulateGears();

                            // Flag 
                            GearsChanged = true;

                            // Force a chart redraw
                            diffRatio_ValueChanged(this, EventArgs.Empty);
                        }
                    }
                }

                e.Handled = true;
            }
        }

        private void addPointButton_Click(object sender, EventArgs e)
        {
            using (var frm = new EditGearForm())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    // Create the new Ratio
                    var gear = frm.GetGear();

                    // Add the gear to the correct list
                    var list = (gear.IsReverse) ? ReverseGears : ForwardGears;
                    list.Add(gear);

                    // Flag 
                    GearsChanged = true;

                    // Force Points Redraw
                    PopulateGears();

                    // Force a chart redraw
                    diffRatio_ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        private void gearListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = gearListView.HitTest(e.Location).Item;
            if (item != null)
            {
                bool isReverse = gearListView.Groups[0].Items.Contains(item);
                int index = (int)item.Tag;
                var gear = (isReverse) ? ReverseGears[index] : ForwardGears[index];

                using (var frm = new EditGearForm(gear))
                {
                    // Show form
                    var result = frm.ShowDialog();
                    if (result == DialogResult.Yes)
                    {
                        // Process the modifcation
                        if (OnGearEdit(gear, frm.GetGear(), index))
                        {
                            // Force Points Redraw
                            PopulateGears();

                            // Flag 
                            GearsChanged = true;

                            // Force a chart redraw
                            diffRatio_ValueChanged(this, EventArgs.Empty);
                        }
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

        private void importButton_Click(object sender, EventArgs e)
        {
            // Request the user supply the steam library path
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Title = "Transmission SII File Import";
            Dialog.Filter = "SiiNunit|*.sii";
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Create document
                    var document = new SiiDocument(typeof(AccessoryTransmissionData), typeof(TransmissionNames));

                    using (FileStream stream = File.OpenRead(Dialog.FileName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        // Read the file contents
                        string contents = reader.ReadToEnd().Trim();
                        document.Load(contents);

                        // Grab the engine object
                        List<string> objects = new List<string>(document.Definitions.Keys);
                        if (objects.Count == 0)
                        {
                            MessageBox.Show("Unable to find any transmission data in this sii document!",
                                "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning
                            );
                            return;
                        }

                        // Grab the transmission
                        var transmission = document.GetDefinition<AccessoryTransmissionData>(objects[0]);

                        // === Set form values
                        unitNameBox.Text = Path.GetFileNameWithoutExtension(Dialog.FileName);
                        transNameBox.Text = transmission.Name;
                        unlockBox.SetValueInRange(transmission.UnlockLevel);
                        priceBox.SetValueInRange(transmission.Price);
                        diffRatio.SetValueInRange(transmission.DifferentialRatio);
                        if (transmission.StallTorqueRatio > 0m)
                        {
                            stallRatio.SetValueInRange(transmission.StallTorqueRatio);
                            hasTorqueConverter.Checked = true;
                        }
                        if (transmission.Retarder > 0)
                        {
                            retardPositions.SetValueInRange(transmission.Retarder);
                            hasRetarder.Checked = true;
                        }
                        fileDefaultsTextBox.Lines = transmission.Defaults;
                        filenameTextBox.Text = Path.GetFileName(Dialog.FileName);
                        conflictsTextBox.Lines = transmission.Conflicts;
                        suitablesTextBox.Lines = transmission.Suitables;

                        // Clear chart points and gears
                        chart1.Series[0].Points.Clear();
                        gearListView.Items.Clear();
                        ForwardGears.Clear();
                        ReverseGears.Clear();

                        // Set gear ratios
                        var nameList = transmission.GearNames?.Reverse?.ToList();
                        int i = 0;
                        foreach (var item in transmission.ReverseRatios)
                        {
                            string name = (nameList != null && i < nameList.Count) ? nameList[i] : string.Empty;
                            ReverseGears.Add(new TransmissionGear()
                            {
                                Name = name,
                                Ratio = item,
                                GearIndex = i++
                            });
                        }

                        nameList = transmission.GearNames?.Forward?.ToList();
                        i = 0;
                        foreach (var item in transmission.ForwardRatios)
                        {
                            string name = (nameList != null && i < nameList.Count) ? nameList[i] : string.Empty;
                            ForwardGears.Add(new TransmissionGear()
                            {
                                Name = name,
                                Ratio = item,
                                GearIndex = i++
                            });
                        }

                        // Fill ratio view
                        PopulateGears();

                        // Flag 
                        GearsChanged = true;

                        // Defaults (skip sounds)
                        if (transmission.Defaults != null)
                        {
                            fileDefaultsTextBox.Lines = transmission.Defaults;
                        }

                        // Alert the user
                        MessageBox.Show(
                            $"Successfully imported the transmission \"{transmission.Name}\"! You must now select a series for this transmission.",
                            "Import Successful", MessageBoxButtons.OK, MessageBoxIcon.Information
                        );
                    }
                }
                catch (SiiSyntaxException ex)
                {
                    StringBuilder builder = new StringBuilder("A Syntax error occured while parsing the sii file!");
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendLine($"Message: {ex.Message.Replace("\0", "\\0")}");
                    builder.AppendLine();
                    builder.AppendLine($"Line: {ex.Span.Start.Line}");
                    builder.AppendLine($"Column: {ex.Span.Start.Column}");
                    MessageBox.Show(builder.ToString(), "Sii Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (SiiException ex)
                {
                    StringBuilder builder = new StringBuilder("Failed to parse the sii file.");
                    builder.AppendLine();
                    builder.AppendLine($"Message: {ex.Message}");
                    MessageBox.Show(builder.ToString(), "Sii Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }

                if (e.Delta > 0)
                {
                    double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 2;
                    double posXFinish = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 2;
                    double posYStart = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 2;
                    double posYFinish = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 2;

                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void Chart1_MouseLeave(object sender, EventArgs e)
        {
            if (chart1.Focused)
                chart1.Parent.Focus();
        }

        private void Chart1_MouseEnter(object sender, EventArgs e)
        {
            if (!chart1.Focused)
                chart1.Focus();
        }

        private void suitsListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SuitablesChanged = true;
        }

        private void conflictListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ConflictsChanged = true;
        }

        private void truckListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TrucksChanged = true;
        }

        private void showPlaceholdersBox_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGears();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check all items
            foreach (var item in truckListView.Items.Cast<ListViewItem>())
            {
                item.Checked = true;
            }
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Un-Check all items
            foreach (var item in truckListView.Items.Cast<ListViewItem>())
            {
                item.Checked = false;
            }
        }
    }
}
