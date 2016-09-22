using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ATSEngineTool.Database;
using FreeImageAPI;

namespace ATSEngineTool
{
    public partial class EngineForm : Form
    {
        /// <summary>
        /// Our engine WIP
        /// </summary>
        public Engine Engine { get; protected set; }

        /// <summary>
        /// Indicates whether this is a new engine we are creating, or
        /// an engine we are editing
        /// </summary>
        protected bool NewEngine = false;

        protected List<TorqueRatio> Ratios { get; set; } = new List<TorqueRatio>();

        /// <summary>
        /// English culture for numbers
        /// </summary>
        protected CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Icon file path
        /// </summary>
        protected static string MatPath = Path.Combine(Program.RootPath, "graphics");

        public EngineForm(Engine engine = null)
        {
            // Create form controls
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            NewEngine = engine == null;
            Engine = engine ?? new Engine();

            // Setup sounds
            //soundBox.Items.Add("Please Select A Sound Package");
            //soundBox.SelectedIndex = 0;

            // Add each sound to the lists
            using (AppDatabase db = new AppDatabase())
            {
                foreach (EngineSeries model in db.EngineSeries)
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
                    Ratios.Add(new TorqueRatio() { Ratio = 0m, RpmLevel = 300 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0.5m, RpmLevel = 440 });
                    Ratios.Add(new TorqueRatio() { Ratio = 1m, RpmLevel = 1100 });
                    Ratios.Add(new TorqueRatio() { Ratio = 1m, RpmLevel = 1400 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0.77m, RpmLevel = 1900 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0.5m, RpmLevel = 2400 });
                    Ratios.Add(new TorqueRatio() { Ratio = 0m, RpmLevel = 2600 });
                }
            }

            // Are we editing an engine?
            if (!NewEngine)
            {
                // Lock the identify box!
                unitNameBox.Enabled = false;
                engineModelBox.Enabled = false;

                // Set form values
                unitNameBox.Text = engine.UnitName;
                engineNameBox.Text = engine.Name;
                unlockBox.Value = engine.Unlock;
                priceBox.Value = engine.Price;
                horsepowerBox.Value = engine.Horsepower;
                torqueBox.Value = engine.Torque;
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
                fileDefaultsTextBox.Lines = Engine.Defaults;
                fileCommentTextBox.Lines = Engine.Comment;
                filenameTextBox.Text = Engine.FileName;
            }
            else
            {
                // Force change to update graph
                torqueBox.Value = 1650;
            }

            // Fill torque ratios
            PopulateTorqueRatios();
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
                item.SubItems.Add(Math.Round(ratio.Ratio * 100, 0).ToString());
                item.Tag = Ratios.IndexOf(ratio);
                ratioListView.Items.Add(item);
                i++;
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            // Check UnitName
            // Check for a valid identifier string
            if (!Regex.Match(unitNameBox.Text, @"^[a-z0-9_]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show("Invalid Engine Sii Unit Name. Please use alpha-numeric, or underscores only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Check engine name
            if (!Regex.Match(engineNameBox.Text, @"^[a-z0-9_.,\-\s\t()]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid Engine Name string. Please use alpha-numeric, period, underscores, dashes or spaces only",
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
            Engine.Torque = (int)torqueBox.Value;
            Engine.BrakeStrength = brakeStrengthBox.Value;
            Engine.BrakePositions = (int)brakePositionsBox.Value;
            Engine.BrakeDownshift = automaticDSCheckBox.Checked;
            Engine.MinRpmRange_LowGear = (int)rpmRangeBox1.Value;
            Engine.MaxRpmRange_LowGear = (int)rpmRangeBox2.Value;
            Engine.MinRpmRange_HighGear = (int)rpmRangeBox3.Value;
            Engine.MaxRpmRange_HighGear = (int)rpmRangeBox4.Value;
            Engine.LowRpmRange_PowerBoost = (int)rpmRangeBox5.Value;
            Engine.HighRpmRange_PowerBoost = (int)rpmRangeBox6.Value;
            Engine.Defaults = fileDefaultsTextBox.Lines;
            Engine.Comment = fileCommentTextBox.Lines;

            // Figure out the filename
            if (!String.IsNullOrWhiteSpace(filenameTextBox.Text))
            {
                Engine.FileName = filenameTextBox.Text.Trim();
            }

            // Validate and Save
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    if (NewEngine)
                    {
                        // Ensure this engine doesnt exist already!
                        if (db.Engines.Contains(Engine))
                        {
                            trans.Dispose();

                            // Tell the user this isnt allowed
                            MessageBox.Show("The unique engine identifier already exists!",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                            );

                            return;
                        }

                        // Add the engine
                        db.Engines.Add(Engine);
                    }
                    else
                    {
                        // Update the current engine
                        db.Engines.Update(Engine);

                        // Delete any and all TorueRatios from thed database
                        List<TorqueRatio> ratios = new List<TorqueRatio>(Engine.TorqueRatios);
                        if (ratios.Count > 0)
                        {
                            foreach (TorqueRatio ratio in ratios)
                                db.TorqueRatios.Remove(ratio);
                        }
                    }

                    // Add the new torque ratios
                    foreach (TorqueRatio ratio in Ratios.OrderBy(x => x.RpmLevel))
                    {
                        ratio.EngineId = Engine.Id;
                        db.TorqueRatios.AddOrUpdate(ratio);
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
                peakRPMBox.Enabled = true;
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
            }

            EngineSeries series = (EngineSeries)engineModelBox.SelectedItem;
            string path = Path.Combine(MatPath, series.EngineIcon);
            if (!path.EndsWith(".dds"))
                path += ".dds";

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

            displacementBox.Value = series.Displacement;
            soundTextBox.Text = series.SoundPackage.Name;
        }

        private void torqueBox_ValueChanged(object sender, EventArgs e)
        {
            // Update label
            NmLabel.Text = String.Concat(Engine.TorqueToNm(torqueBox.Value), " (Nm)");

            // Clear old chart points
            chart1.Series[0].Points.Clear();

            // Fill torque ratios
            foreach (TorqueRatio ratio in Ratios.OrderBy(x => x.RpmLevel))
            {
                double torque = (double)Math.Round(torqueBox.Value * ratio.Ratio, 0);
                int index = chart1.Series[0].Points.AddXY(ratio.RpmLevel, torque);
                DataPoint point = chart1.Series[0].Points[index];
                point.ToolTip = $"{torque} lb-ft @ {ratio.RpmLevel} RPM";
            }
        }

        private void horsepowerBox_ValueChanged_1(object sender, EventArgs e)
        {
            KwLabel.Text = String.Concat(
                Engine.HorsepowerToKilowatts(horsepowerBox.Value), " (Kw)"
            );
        }

        private void unitNameBox_TextChanged(object sender, EventArgs e)
        {
            if (unitNameBox.Text.Length == 0) return;

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

            // Force Points Redraw
            PopulateTorqueRatios();

            // Force a chart redraw
            torqueBox_ValueChanged(this, EventArgs.Empty);
        }

        private void addPointButton_Click(object sender, EventArgs e)
        {
            using (TorqueCurveForm frm = new TorqueCurveForm())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    // Prevent too large of a number
                    decimal r = (int)frm.torqueLevelBox.Value;
                    if (r > 100m) r = 100m;

                    // Create the new Ratio
                    TorqueRatio ratio = new TorqueRatio();
                    ratio.RpmLevel = (int)frm.rpmLevelBox.Value;
                    ratio.Ratio = Math.Round(r / 100, 2);
                    Ratios.Add(ratio);

                    // Force Points Redraw
                    PopulateTorqueRatios();

                    // Force a chart redraw
                    torqueBox_ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        private void ratioListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = ratioListView.HitTest(e.Location).Item;
            if (item != null)
            {
                int index = (int)item.Tag;
                TorqueRatio ratio = Ratios[index];

                using (TorqueCurveForm frm = new TorqueCurveForm())
                {
                    // Set form values
                    frm.rpmLevelBox.Value = ratio.RpmLevel;
                    frm.torqueLevelBox.Value = Math.Round(ratio.Ratio * 100, 0);

                    // Show form
                    var result = frm.ShowDialog();
                    if (result == DialogResult.Yes)
                    {
                        // Prevent too large of a number
                        decimal r = (int)frm.torqueLevelBox.Value;
                        if (r > 100m) r = 100m;

                        // Create the new Ratio
                        ratio.RpmLevel = (int)frm.rpmLevelBox.Value;
                        ratio.Ratio = Math.Round(r / 100, 2);

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
    }
}
