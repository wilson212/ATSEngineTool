using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ATSEngineTool.Database;
using ATSEngineTool.SiiEntities;
using FreeImageAPI;
using Sii;

namespace ATSEngineTool
{
    public partial class EngineForm : Form
    {
        /// <summary>
        /// Specifies the distance in RPMs that the current horsepower is plotted on the chart
        /// </summary>
        const int HP_POINT_DIST = 25;

        /// <summary>
        /// Our engine WIP
        /// </summary>
        public Engine Engine { get; protected set; }

        /// <summary>
        /// Indicates whether this is a new engine we are creating, or
        /// an engine we are editing
        /// </summary>
        protected bool NewEngine = false;

        /// <summary>
        /// Gets a list of Ratios for this engines torque curve
        /// </summary>
        protected List<TorqueRatio> Ratios { get; set; } = new List<TorqueRatio>();

        /// <summary>
        /// Indicates whether the conflicting transmissions box had a check change
        /// </summary>
        protected bool ConflictsChanged { get; set; } = false;

        /// <summary>
        /// Indicates whether the suitable transmissions box had a check change
        /// </summary>
        protected bool SuitablesChanged { get; set; } = false;

        /// <summary>
        /// Indicates whether the trucks box had a check change
        /// </summary>
        protected bool TrucksChanged { get; set; } = false;

        /// <summary>
        /// Indicates whether the Torque Ratios changed on this engine
        /// </summary>
        protected bool RatiosChanged { get; set; } = false;

        /// <summary>
        /// Icon file path
        /// </summary>
        protected static string MatPath = Path.Combine(Program.RootPath, "graphics");

        public EngineForm(Engine engine = null)
        {
            // Create form controls
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);
            chart1.ChartAreas[0].AxisX.Interval = 300;

            NewEngine = engine == null;
            Engine = engine ?? new Engine();

            // Setup metrics
            if (Program.Config.UnitSystem == UnitSystem.Metric)
            {
                labelTorque.Text = "Newton Metres:";
                labelPeakTorque.Text = "Peak N·m RPM:";
                chart1.ChartAreas[0].AxisY.Title = "Newton Metres";
                maxTorqueLabel.Text = "Max Newton Metres:";
            }

            // Add each sound to the lists
            using (AppDatabase db = new AppDatabase())
            {
                foreach (EngineSeries model in db.EngineSeries.OrderBy(x => x.ToString()))
                {
                    engineModelBox.Items.Add(model);
                    if (!NewEngine && model.Id == Engine.SeriesId)
                        engineModelBox.SelectedIndex = engineModelBox.Items.Count - 1;
                }

                if (!NewEngine)
                {
                    Ratios.AddRange(engine.TorqueRatios);
                }
                else
                {
                    Ratios.Add(new TorqueRatio() { Ratio = 0, RpmLevel = 300 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0.5, RpmLevel = 440 });
                    Ratios.Add(new TorqueRatio() { Ratio = 1, RpmLevel = 1100 });
                    Ratios.Add(new TorqueRatio() { Ratio = 1, RpmLevel = 1400 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0.77, RpmLevel = 1900 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0.5, RpmLevel = 2400 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0, RpmLevel = 2600 });
                }

                // Grab a list of trucks that use this engine
                List<int> trucks = new List<int>();
                if (!NewEngine)
                    trucks.AddRange(engine.ItemOf.Select(x => x.TruckId));

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
            if (!NewEngine)
            {
                // Set form values
                unitNameBox.Text = engine.UnitName;
                engineNameBox.Text = engine.Name;
                unlockBox.Value = engine.Unlock;
                priceBox.Value = engine.Price;
                horsepowerBox.Value = engine.Horsepower;
                peakRPMBox.Value = engine.PeakRpm;
                idleRpmBox.Value = engine.IdleRpm;
                rpmLimitBox.Value = engine.RpmLimit;
                neutralRpmBox.Value = engine.RpmLimitNeutral;
                brakeStrengthBox.Value = engine.BrakeStrength;
                brakePositionsBox.Value = engine.BrakePositions;
                automaticDSCheckBox.Checked = engine.BrakeDownshift;
                rpmRangeBox1.Value = engine.MinRpmRange_LowGear;
                rpmRangeBox2.Value = engine.MaxRpmRange_LowGear;
                rpmRangeBox3.Value = engine.MinRpmRange_HighGear;
                rpmRangeBox4.Value = engine.MaxRpmRange_HighGear;
                rpmRangeBox5.Value = engine.LowRpmRange_PowerBoost;
                rpmRangeBox6.Value = engine.HighRpmRange_PowerBoost;
                fileDefaultsTextBox.Lines = engine.Defaults;
                fileCommentTextBox.Lines = engine.Comment;
                filenameTextBox.Text = engine.FileName;
                conflictsTextBox.Lines = engine.Conflicts;
                engineBrakeLow.Value = engine.LowRpmRange_EngineBrake;
                engineBrakeHigh.Value = engine.HighRpmRange_EngineBrake;
                adBlueConsumption.Value = engine.AdblueConsumption;
                adBlueNoPowerLimit.Value = engine.NoAdbluePowerLimit;

                // Set torque last, to update the chart
                torqueBox.Value = (Program.Config.UnitSystem == UnitSystem.Imperial)
                    ? engine.Torque
                    : engine.NewtonMetres;
            }

            // Fill torque ratios
            PopulateTorqueRatios();

            // Fill Conflicts
            PopulateTransmissions();

            if (NewEngine)
            {
                // Force change to update graph
                torqueBox.Value = (Program.Config.UnitSystem == UnitSystem.Imperial) 
                    ? 1650 : 
                    Metrics.TorqueToNewtonMeters(1650m);
            }
        }

        private void PopulateTransmissions()
        {
            // Clear transmissions
            int groupId = -1;
            ListViewGroup group1 = null;
            ListViewGroup group2 = null;

            using (AppDatabase db = new AppDatabase())
            {
                var conflicts = new List<int>();
                var suits = new List<int>();
                if (!NewEngine)
                {
                    conflicts.AddRange(Engine.TransmissionConflicts.Select(x => x.TransmissionId));
                    suits.AddRange(Engine.SuitableTransmissions.Select(x => x.TransmissionId));
                }

                foreach (var trans in db.Transmissions.OrderBy(x => x.SeriesId).ThenByDescending(x => x.Price))
                {
                    if (trans.SeriesId != groupId)
                    {
                        groupId = trans.SeriesId;
                        var series = trans.Series; // lazy loaded... load just once
                        group1 = new ListViewGroup(series.Name);
                        conflictListView.Groups.Add(group1);
                        group2 = new ListViewGroup(series.Name);
                        suitsListView.Groups.Add(group2);
                    }

                    // Add transmission to conflicts box
                    ListViewItem item = new ListViewItem(trans.Name);
                    item.Checked = conflicts.Contains(trans.Id);
                    item.SubItems.Add(trans.DifferentialRatio.ToString());
                    item.Tag = trans.Id;
                    group1.Items.Add(item);
                    conflictListView.Items.Add(item);

                    // Add transmission to suitibles box
                    item = new ListViewItem(trans.Name);
                    item.Checked = suits.Contains(trans.Id);
                    item.SubItems.Add(trans.DifferentialRatio.ToString());
                    item.Tag = trans.Id;
                    group2.Items.Add(item);
                    suitsListView.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Fills the torque ratio Listview
        /// </summary>
        private void PopulateTorqueRatios()
        {
            // Clear out any old items
            ratioListView.Items.Clear();

            int i = 1;
            foreach (TorqueRatio ratio in Ratios.OrderBy(x => x.RpmLevel))
            {
                ListViewItem item = new ListViewItem(i.ToString());
                item.SubItems.Add(ratio.RpmLevel.ToString());
                item.SubItems.Add(Math.Round(ratio.Ratio * 100, 2).ToString(Program.NumberFormat));
                item.Tag = Ratios.IndexOf(ratio);
                ratioListView.Items.Add(item);
                i++;
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            // Check UnitName
            // Check for a valid identifier string
            if (!SiiFileBuilder.IsValidUnitName(unitNameBox.Text))
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid Engine Sii Unit Name. Tokens must be 1 to 12 characters in length, seperated by a dot, "
                        + "and contain alpha-numeric or underscores only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Check engine name
            if (engineNameBox.Text.Length < 2 || engineNameBox.Text.Contains('"'))
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid Engine Name string. The name must be at least 2 characters long and contain no quotes",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Set new attribute values
            Engine.SeriesId = ((EngineSeries)engineModelBox.SelectedItem).Id;
            Engine.UnitName = unitNameBox.Text.Trim();
            Engine.Name = engineNameBox.Text.Trim();
            Engine.Price = (int)priceBox.Value;
            Engine.Unlock = (int)unlockBox.Value;
            Engine.Horsepower = (int)horsepowerBox.Value;
            Engine.PeakRpm = (int)peakRPMBox.Value;
            Engine.IdleRpm = (int)idleRpmBox.Value;
            Engine.RpmLimit = (int)rpmLimitBox.Value;
            Engine.RpmLimitNeutral = (int)neutralRpmBox.Value;
            Engine.BrakeStrength = brakeStrengthBox.Value;
            Engine.BrakePositions = (int)brakePositionsBox.Value;
            Engine.BrakeDownshift = automaticDSCheckBox.Checked;
            Engine.MinRpmRange_LowGear = (int)rpmRangeBox1.Value;
            Engine.MaxRpmRange_LowGear = (int)rpmRangeBox2.Value;
            Engine.MinRpmRange_HighGear = (int)rpmRangeBox3.Value;
            Engine.MaxRpmRange_HighGear = (int)rpmRangeBox4.Value;
            Engine.LowRpmRange_PowerBoost = (int)rpmRangeBox5.Value;
            Engine.HighRpmRange_PowerBoost = (int)rpmRangeBox6.Value;
            Engine.NoAdbluePowerLimit = adBlueNoPowerLimit.Value;
            Engine.AdblueConsumption = adBlueConsumption.Value;
            Engine.LowRpmRange_EngineBrake = (int)engineBrakeLow.Value;
            Engine.HighRpmRange_EngineBrake = (int)engineBrakeHigh.Value;
            Engine.Defaults = fileDefaultsTextBox.Lines;
            Engine.Comment = fileCommentTextBox.Lines;
            Engine.Conflicts = conflictsTextBox.Lines;

            // Torque metrics
            if (Program.Config.UnitSystem == UnitSystem.Imperial)
                Engine.Torque = (int)torqueBox.Value;
            else
                Engine.NewtonMetres = (int)torqueBox.Value;

            // Figure out the filename
            if (!String.IsNullOrWhiteSpace(filenameTextBox.Text))
                Engine.FileName = filenameTextBox.Text.Trim();

            // Validate and Save
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                // Verify that the series.Id and engine.UnitName are unique
                string query = "SELECT * FROM `Engine` WHERE `SeriesId`=@P0 AND `UnitName`=@P1";
                var engine = db.Query<Engine>(query, Engine.SeriesId, Engine.UnitName).FirstOrDefault();
                if (engine != null && (NewEngine || engine.Id != Engine.Id))
                {
                    // Tell the user this isnt allowed
                    MessageBox.Show(
                        $"The selected Engine Series already contains an engine with the Sii Unit Name of \""
                        + Engine.UnitName + "\"! Please select a different Engine Series or change the Sii "
                        + "Unit Name to something unique.",
                        "Unique Constraint Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    return;
                }

                // Wrap database changes in a try-catch block so we can Rollback on error
                try
                {
                    // Update or Add engine
                    db.Engines.AddOrUpdate(Engine);

                    // If pre-existing engine, delete all changed data
                    if (!NewEngine)
                    {
                        // Delete any and all TorueRatios from thed database
                        if (RatiosChanged)
                        {
                            foreach (TorqueRatio ratio in Engine.TorqueRatios)
                                db.TorqueRatios.Remove(ratio);
                        }

                        // Set Conflicts
                        if (ConflictsChanged)
                        {
                            // Remove old
                            foreach (var conflict in Engine.TransmissionConflicts)
                                db.AccessoryConflicts.Remove(conflict);

                        }

                        // Set Suitables
                        if (SuitablesChanged)
                        {
                            foreach (var item in Engine.SuitableTransmissions)
                                db.SuitableAccessories.Remove(item);

                        }

                        // Set compatible trucks
                        if (TrucksChanged)
                        {
                            foreach (var item in Engine.ItemOf)
                                db.TruckEngines.Remove(item);

                        }
                    }

                    // Add conflicts
                    if ((NewEngine || ConflictsChanged) && conflictListView.CheckedItems.Count > 0)
                    {
                        var ids = conflictListView.CheckedItems.Cast<ListViewItem>().Select(x => (int)x.Tag);
                        foreach (var item in ids)
                        {
                            db.AccessoryConflicts.Add(new AccessoryConflict()
                            {
                                EngineId = Engine.Id,
                                TransmissionId = item
                            });
                        }
                    }

                    // Add suitible fors
                    if ((NewEngine || SuitablesChanged) && suitsListView.CheckedItems.Count > 0)
                    {
                        var ids = suitsListView.CheckedItems.Cast<ListViewItem>().Select(x => (int)x.Tag);
                        foreach (var item in ids)
                        {
                            db.SuitableAccessories.Add(new SuitableAccessory()
                            {
                                EngineId = Engine.Id,
                                TransmissionId = item
                            });
                        }
                    }

                    // Add new truck engines
                    if ((NewEngine || TrucksChanged) && truckListView.CheckedItems.Count > 0)
                    {
                        var ids = truckListView.CheckedItems.Cast<ListViewItem>().Select(x => (int)x.Tag);
                        foreach (var item in ids)
                        {
                            db.TruckEngines.Add(new TruckEngine()
                            {
                                EngineId = Engine.Id,
                                TruckId = item
                            });
                        }
                    }

                    // Add the new torque ratios
                    if (NewEngine || RatiosChanged)
                    {
                        foreach (TorqueRatio ratio in Ratios.OrderBy(x => x.RpmLevel))
                        {
                            ratio.EngineId = Engine.Id;
                            db.TorqueRatios.AddOrUpdate(ratio);
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

        private void engineModelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Unlock all other controls
            if (!engineNameBox.Enabled)
            {
                unitNameBox.Enabled = true;
                engineNameBox.Enabled = true;
                unlockBox.Enabled = true;
                priceBox.Enabled = true;
                torqueBox.Enabled = true;
                horsepowerBox.Enabled = true;
                //peakRPMBox.Enabled = true;
                rpmLimitBox.Enabled = true;
                idleRpmBox.Enabled = true;
                brakePositionsBox.Enabled = true;
                brakeStrengthBox.Enabled = true;
                automaticDSCheckBox.Enabled = true;
                rpmRangeBox1.Enabled = true;
                rpmRangeBox2.Enabled = true;
                rpmRangeBox3.Enabled = true;
                rpmRangeBox4.Enabled = true;
                rpmRangeBox5.Enabled = true;
                rpmRangeBox6.Enabled = true;
                neutralRpmBox.Enabled = true;
                filenameTextBox.Enabled = true;
                adBlueConsumption.Enabled = true;
                adBlueNoPowerLimit.Enabled = true;
                engineBrakeHigh.Enabled = true;
                engineBrakeLow.Enabled = true;
            }

            EngineSeries series = (EngineSeries)engineModelBox.SelectedItem;
            string path = Path.Combine(MatPath, series.EngineIcon);
            if (!path.EndsWith(".dds"))
                path += ".dds";

            // Set data's for engine series
            displacementBox.Value = series.Displacement;
            soundTextBox.Text = series.SoundPackage.Name;

            // Perform cleanup
            if (engineIcon.Image != null)
            {
                engineIcon.Image.Dispose();
                engineIcon.Image = null;
            }

            // Ensure icon exists before proceeding
            if (!File.Exists(path)) return;

            // Attempt to load image as a DDS file... or png if its a mod sometimes
            FREE_IMAGE_FORMAT Format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
            Bitmap MapImage = FreeImage.LoadBitmap(path, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref Format);
            if (MapImage != null)
            {
                engineIcon.Image = new Bitmap(MapImage, 64, 64);
            }
        }

        private void torqueBox_ValueChanged(object sender, EventArgs e)
        {
            // Update label
            labelNM.Text = (Program.Config.UnitSystem == UnitSystem.Imperial)
                ? String.Concat(Metrics.TorqueToNewtonMeters(torqueBox.Value), " (Nm)")
                : String.Concat(Metrics.NewtonMetersToTorque(torqueBox.Value), " (Trq)");

            // disable buttons
            addPointButton.Enabled = false;
            removePointButton.Enabled = false;

            // Clear old chart points
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            DataPoint lastPoint = null;
            TorqueRatio highest = null;
            double torque = 0;
            double horsepower = 0;

            // Fill torque ratios
            foreach (TorqueRatio ratio in Ratios.OrderBy(x => x.RpmLevel))
            {
                // Plot the torque point
                torque = Math.Round((double)torqueBox.Value * ratio.Ratio, 2);
                int index = chart1.Series[0].Points.AddXY(ratio.RpmLevel, torque);
                DataPoint point = chart1.Series[0].Points[index];

                // Set tool tip and covert chart value for later
                if (Program.Config.UnitSystem == UnitSystem.Imperial)
                {
                    point.ToolTip = $"{torque} lb-ft @ {ratio.RpmLevel} RPM";
                }
                else
                {
                    point.ToolTip = $"{torque} Nm @ {ratio.RpmLevel} RPM";
                    torque = Metrics.NewtonMetersToTorque(torque, 2);
                }

                // Set the highest and earliest ratio
                if (highest == null || ratio.Ratio > highest.Ratio)
                    highest = ratio;

                // === Plot Horsepower
                if (lastPoint != null)
                {
                    // Calculate the last plots starting Torque value
                    double torqueAtPoint = lastPoint.YValues[0];
                    // Get torque difference from last point
                    double deltaY = point.YValues[0] - lastPoint.YValues[0];
                    // Get RPM difference from last point
                    double deltaX = point.XValue - lastPoint.XValue;

                    // Convert Nm to Torque if using the Metric System
                    if (Program.Config.UnitSystem == UnitSystem.Metric)
                    {
                        deltaY = Metrics.NewtonMetersToTorque(deltaY, 2);
                        torqueAtPoint = Metrics.NewtonMetersToTorque(torqueAtPoint, 2);
                    }

                    // Calculate torque rise (rpm distance * (torque rise per rpm)),
                    // Then walk from the last point, to right before the current one
                    double torqueRise = HP_POINT_DIST * (deltaY / deltaX);
                    double rpm = lastPoint.XValue + HP_POINT_DIST;
                    double stop = point.XValue;

                    // Now we plot the horsepower points, up until we hit the next torque point
                    for (; rpm < stop; rpm += HP_POINT_DIST)
                    {
                        // increment torque rating by our dermined curve rise
                        torqueAtPoint += torqueRise;

                        // Plot the horsepower point
                        horsepower = Metrics.TorqueToHorsepower(torqueAtPoint, rpm, 0);
                        index = chart1.Series[1].Points.AddXY(rpm, horsepower);
                        chart1.Series[1].Points[index].ToolTip = $"#VALY HP @ {rpm} RPM";
                    }
                }

                // Plot the Horsepower plots
                horsepower = Metrics.TorqueToHorsepower(torque, ratio.RpmLevel, 0);
                index = chart1.Series[1].Points.AddXY(ratio.RpmLevel, horsepower);
                chart1.Series[1].Points[index].ToolTip = $"#VALY HP @ {ratio.RpmLevel} RPM";
                chart1.Series[1].Points[index].MarkerStyle = MarkerStyle.Circle;
                chart1.Series[1].Points[index].MarkerColor = Color.Black;

                // Set the last point
                lastPoint = point;
            }

            // Update labels
            if (chart1.Series[1].Points.Count > 0)
            {
                DataPoint pnt = chart1.Series[1].Points.FindMaxByValue("Y");
                maxHpLabel.Text = $"{pnt.YValues[0]} @ {pnt.XValue} RPM";
                torque = Math.Round((double)torqueBox.Value * highest.Ratio, 0);
                maxTrqLabel.Text = $"{torque} @ {highest.RpmLevel} RPM";
                peakRPMBox.Value = highest?.RpmLevel ?? 1200;
                peakRPMBox.Enabled = false; // no user edit
            }
            else
            {
                peakRPMBox.Enabled = true;
                maxHpLabel.Text = "";
                maxTrqLabel.Text = "";
            }

            // Enable buttons
            addPointButton.Enabled = true;
            removePointButton.Enabled = true;
        }

        private void horsepowerBox_ValueChanged_1(object sender, EventArgs e)
        {
            KwLabel.Text = String.Concat(
                Metrics.HorsepowerToKilowatts(horsepowerBox.Value), " (Kw)"
            );
        }

        private void unitNameBox_TextChanged(object sender, EventArgs e)
        {
            if (!NewEngine || unitNameBox.Text.Length == 0) return;

            string text = unitNameBox.Text.MakeFileNameSafe();
            filenameTextBox.Text = text + ".sii";
        }

        private void ratioListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ratioListView.SelectedItems.Count == 0) return;
            removePointButton.Enabled = true;
        }

        private void removePointButton_Click(object sender, EventArgs e)
        {
            if (ratioListView.SelectedItems.Count == 0) return;

            // Remove torque ratio
            ListViewItem item = ratioListView.SelectedItems[0];
            int index = (int)item.Tag;
            Ratios.RemoveAt(index);

            // Flag change
            RatiosChanged = true;

            // Force Points Redraw
            PopulateTorqueRatios();

            // Force a chart redraw
            torqueBox_ValueChanged(this, EventArgs.Empty);
        }

        private void ratioListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (ratioListView.SelectedItems.Count == 0) return;

            ListViewItem item = ratioListView.SelectedItems[0];
            int index = (int)item.Tag;
            if (e.KeyCode == Keys.Delete)
            {
                // Remove torque ratio
                Ratios.RemoveAt(index);

                // Flag change
                RatiosChanged = true;

                // Force Points Redraw
                PopulateTorqueRatios();

                // Force a chart redraw
                torqueBox_ValueChanged(this, EventArgs.Empty);

                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                TorqueRatio ratio = Ratios[index];
                using (TorqueCurveForm frm = new TorqueCurveForm((int)torqueBox.Value, ratio))
                {
                    // Show form
                    var result = frm.ShowDialog();
                    if (result == DialogResult.Yes)
                    {
                        // Grab Ratio
                        TorqueRatio values = frm.GetRatio();

                        // Create the new Ratio
                        ratio.RpmLevel = values.RpmLevel;
                        ratio.Ratio = values.Ratio;

                        // Flag change
                        RatiosChanged = true;

                        // Force Points Redraw
                        PopulateTorqueRatios();

                        // Force a chart redraw
                        torqueBox_ValueChanged(this, EventArgs.Empty);
                    }
                }

                e.Handled = true;
            }
        }

        private void addPointButton_Click(object sender, EventArgs e)
        {
            using (TorqueCurveForm frm = new TorqueCurveForm((int)torqueBox.Value))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    // Create the new Ratio
                    TorqueRatio ratio = frm.GetRatio();
                    Ratios.Add(ratio);

                    // Flag change
                    RatiosChanged = true;

                    // Force Points Redraw
                    PopulateTorqueRatios();

                    // Force a chart redraw
                    torqueBox_ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        private void resetPointsButton_Click(object sender, EventArgs e)
        {
            if (Ratios.Count > 0)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to reset the torque curve points to the default values? "
                    + "Any changes made to the current power curves will be undone.",
                    "Reset Torque Curve", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                );

                // If yes is not selected, get outta here!
                if (result != DialogResult.Yes) return;

                // Remove all ratios
                Ratios.Clear();

                // Flag change
                RatiosChanged = true;
            }

            // Lock button
            resetPointsButton.Enabled = false;

            // Add default ratios
            Ratios.Add(new TorqueRatio() { Ratio = 0, RpmLevel = 300 });
            Ratios.Add(new TorqueRatio() { Ratio = 0.5, RpmLevel = 440 });
            Ratios.Add(new TorqueRatio() { Ratio = 1, RpmLevel = 1100 });
            Ratios.Add(new TorqueRatio() { Ratio = 1, RpmLevel = 1400 });
            Ratios.Add(new TorqueRatio() { Ratio = 0.77, RpmLevel = 1900 });
            Ratios.Add(new TorqueRatio() { Ratio = 0.5, RpmLevel = 2400 });
            Ratios.Add(new TorqueRatio() { Ratio = 0, RpmLevel = 2600 });

            // Force Points Redraw
            PopulateTorqueRatios();

            // Force a chart redraw
            torqueBox_ValueChanged(this, EventArgs.Empty);

            // Enable Button
            resetPointsButton.Enabled = true;
        }

        private void removeAllButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                    "Are you sure you want to reset the torque curve points to the default values? "
                    + "Any changes made to the current power curves will be undone.",
                    "Reset Torque Curve", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                );

            // If yes is not selected, get outta here!
            if (result != DialogResult.Yes) return;

            // Remove all ratios
            Ratios.Clear();

            // Flag change
            RatiosChanged = true;

            // Clear out any old items
            ratioListView.Items.Clear();

            // Force a chart redraw
            torqueBox_ValueChanged(this, EventArgs.Empty);
        }

        private void ratioListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = ratioListView.HitTest(e.Location).Item;
            if (item != null)
            {
                int index = (int)item.Tag;
                TorqueRatio ratio = Ratios[index];

                using (TorqueCurveForm frm = new TorqueCurveForm((int)torqueBox.Value, ratio))
                {
                    // Show form
                    var result = frm.ShowDialog();
                    if (result == DialogResult.Yes)
                    {
                        Ratios[index] = frm.GetRatio();

                        // Flag change
                        RatiosChanged = true;

                        // Force Points Redraw
                        PopulateTorqueRatios();

                        // Force a chart redraw
                        torqueBox_ValueChanged(this, EventArgs.Empty);
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
            var dialog = new OpenFileDialog();
            dialog.Title = "Engine SII File Import";
            dialog.Filter = "SiiNunit|*.sii";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var document = new SiiDocument(typeof(AccessoryEngineData));

                    using (FileStream stream = File.OpenRead(dialog.FileName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        // Read the file contents
                        string contents = reader.ReadToEnd().Trim();
                        document.Load(contents);

                        // Grab the engine object
                        List<string> objects = new List<string>(document.Definitions.Keys);
                        if (objects.Count == 0)
                        {
                            MessageBox.Show("Unable to find any engine date in this sii document!", 
                                "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning
                            );
                            return;
                        }

                        // Grab the engine
                        var engine = document.GetDefinition<AccessoryEngineData>(objects[0]);

                        // === Set form values
                        int len = objects[0].IndexOf('.');
                        unitNameBox.Text = objects[0].Substring(0, len);
                        engineNameBox.Text = engine.Name;
                        filenameTextBox.Text = Path.GetFileName(dialog.FileName);
                        unlockBox.Value = engine.UnlockLevel;
                        priceBox.Value = engine.Price;
                        neutralRpmBox.Value = engine?.RpmLimitNeutral ?? 2200;
                        rpmLimitBox.Value = engine.RpmLimit;
                        idleRpmBox.Value = engine?.IdleRpm ?? 650;
                        brakeStrengthBox.Value = (decimal)engine.BrakeStrength;
                        brakePositionsBox.Value = engine.BrakePositions;
                        automaticDSCheckBox.Checked = engine.BrakeDownshift == 1;

                        // Misc
                        if (engine.RpmRangeEngineBrake.X > 0f)
                        {
                            engineBrakeLow.Value = (int)engine.RpmRangeEngineBrake.X;
                            engineBrakeHigh.Value = (int)engine.RpmRangeEngineBrake.Y;
                        }
                        adBlueConsumption.Value = (decimal)engine.AdblueConsumption;
                        adBlueNoPowerLimit.Value = (decimal)engine.NoAdbluePowerLimit;
                        conflictsTextBox.Lines = engine?.Conflicts ?? new string[] { };

                        // Tab 3
                        if (engine.RpmRange_LowGear.X > 0f)
                        {
                            rpmRangeBox1.Value = (int)engine.RpmRange_LowGear.X;
                            rpmRangeBox2.Value = (int)engine.RpmRange_LowGear.Y;
                        }
                        if (engine.RpmRange_HighGear.X > 0f)
                        {
                            rpmRangeBox3.Value = (int)engine.RpmRange_HighGear.X;
                            rpmRangeBox4.Value = (int)engine.RpmRange_HighGear.Y;
                        }
                        if (engine.RpmRange_PowerBoost.X > 0f)
                        {
                            rpmRangeBox5.Value = (int)engine.RpmRange_PowerBoost.X;
                            rpmRangeBox6.Value = (int)engine.RpmRange_PowerBoost.Y;
                        }

                        // Parse Horsepower
                        Regex reg = new Regex("^(?<hp>[0-9]+)", RegexOptions.Multiline);
                        if (reg.IsMatch(engine.Info[0]))
                        {
                            horsepowerBox.Value = Int32.Parse(reg.Match(engine.Info[0]).Groups["hp"].Value);
                        }

                        if (engine.TorqueCurves?.Length > 0)
                        {
                            // Clear torque curves
                            chart1.Series[0].Points.Clear();
                            ratioListView.Items.Clear();
                            Ratios.Clear();

                            // Set new torque curves
                            foreach (Vector2 vector in engine.TorqueCurves)
                            {
                                TorqueRatio ratio = new TorqueRatio();
                                ratio.RpmLevel = (int)vector.X;
                                ratio.Ratio = Math.Round(vector.Y, 4);
                                Ratios.Add(ratio);
                            }

                            // Fill ratio view
                            PopulateTorqueRatios();
                        }

                        // Set torque value 
                        torqueBox.Value = (Program.Config.UnitSystem == UnitSystem.Imperial)
                            ? Metrics.NewtonMetersToTorque((decimal)engine.Torque, torqueBox.DecimalPlaces)
                            : Math.Round((decimal)engine.Torque, torqueBox.DecimalPlaces);

                        // Defaults (skip sounds)
                        if (engine.Defaults != null)
                        {
                            fileDefaultsTextBox.Lines = (from x in engine.Defaults where !x.Contains("/sound/") select x).ToArray();
                        }

                        // Alert the user
                        MessageBox.Show(
                           $"Successfully imported the engine \"{engine.Name}\"! You must now select an engine series for this engine.",
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

        private void conflictListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ConflictsChanged = true;
        }

        private void suitsListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SuitablesChanged = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[0].ChartType = (radioButton1.Checked) ? SeriesChartType.Spline : SeriesChartType.Line;
            chart1.Series[1].ChartType = chart1.Series[0].ChartType;
        }

        private void truckListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TrucksChanged = true;
        }
    }
}
