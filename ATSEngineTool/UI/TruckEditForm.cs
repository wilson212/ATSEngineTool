using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class TruckEditForm : Form
    {
        /// <summary>
        /// Internal variable to contain the truck being edited
        /// </summary>
        protected Truck Truck { get; set; }

        protected TruckSoundSetting Setting { get; set; }

        public TruckEditForm(Truck truck = null)
        {
            // Create controls and add back color to header
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            // Set internals
            Truck = truck;
            Setting = truck?.SoundSetting?.FirstOrDefault();

            // Add sound packages to the drop down box
            using (AppDatabase db = new AppDatabase())
            {
                foreach (var package in db.EngineSoundPackages)
                {
                    engineSoundPackageBox.Items.Add(package);
                    if (Setting != null && Setting.EngineSoundPackageId == package.Id)
                        engineSoundPackageBox.SelectedIndex = engineSoundPackageBox.Items.Count - 1;
                }

                foreach (var package in db.TruckSoundPackages)
                {
                    truckSoundPackageBox.Items.Add(package);
                    if (Truck != null && Truck.SoundPackageId == package.Id)
                        truckSoundPackageBox.SelectedIndex = truckSoundPackageBox.Items.Count - 1;
                }

                // Set default truck sound
                if (truckSoundPackageBox.SelectedIndex == -1 && truckSoundPackageBox.Items.Count > 0)
                    truckSoundPackageBox.SelectedIndex = 0;
            }

            // Fill data fields
            if (truck == null)
            {
                shadowLabel1.Text = "Add New Truck";
            }
            else
            {
                shadowLabel1.Text = "Modify Truck";
                truckNameBox.Text = truck.Name;
                unitNameBox.Text = truck.UnitName;
            }

            // Check box?
            checkBox1.Checked = (Setting != null);
        }

        /// <summary>
        /// Confirm button click callback
        /// </summary>
        private void confirmButton_Click(object sender, EventArgs e)
        {
            // Check for a valid identifier string
            if (!Regex.Match(unitNameBox.Text, @"^[a-z0-9_\.]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show("Invalid Sii Unit Name. Please use alpha-numeric, or underscores only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Check engine name
            if (!Regex.Match(truckNameBox.Text, @"^[a-z0-9_.,\-\s\t]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid truck name string. Please use alpha-numeric, period, underscores, dashes or spaces only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Grab sound packages selected
            SoundPackage enginePackage = engineSoundPackageBox.SelectedItem as SoundPackage;
            SoundPackage truckPackage = truckSoundPackageBox.SelectedItem as SoundPackage;

            // We need a sound package on a truck!
            if (truckSoundPackageBox.SelectedIndex < 0)
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "No truck sound package selected! Please select a sound package and try again.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                return;
            }

            // Engine sound override?
            if (checkBox1.Checked)
            {
                if (engineSoundPackageBox.SelectedIndex < 0)
                {
                    // Tell the user this isnt allowed
                    MessageBox.Show(
                        "No engine sound package selected! Please select a sound package and try again.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            try
            {
                // Add or update the truck in the database
                using (AppDatabase db = new AppDatabase())
                {
                    if (Truck == null)
                    {
                        Truck = new Truck()
                        {
                            Name = truckNameBox.Text.Trim(),
                            UnitName = unitNameBox.Text.Trim(),
                            SoundPackageId = truckPackage.Id
                        };
                        db.Trucks.Add(Truck);
                    }
                    else
                    {
                        Truck.Name = truckNameBox.Text.Trim();
                        Truck.UnitName = unitNameBox.Text.Trim();
                        Truck.SoundPackageId = truckPackage.Id;
                        db.Trucks.Update(Truck);
                    }

                    // Sync sound settings
                    if (Setting != null)
                    {
                        if (checkBox1.Checked)
                        {
                            var newSetting = new TruckSoundSetting()
                            {
                                TruckId = Truck.Id,
                                EngineSoundPackageId = enginePackage.Id
                            };

                            // If something changed
                            if (!newSetting.IsDuplicateOf(Setting))
                                db.TruckSoundSettings.Update(newSetting);
                        }
                        else
                        {
                            db.TruckSoundSettings.Remove(Setting);
                        }
                    }
                    else if (checkBox1.Checked)
                    {
                        // Alert the user if this is an SCS engine
                        if (Truck?.IsScsTruck ?? false)
                        {
                            var result = MessageBox.Show(
                                "You are attempting to change the default sounds of an SCS created truck. "
                                + "If you decide to revert these changes later, you will experience the \"No Sound Bug\". "
                                + "Are you sure you wish to continue?",
                                "Verification", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                            );

                            if (result != DialogResult.Yes) return;
                        }

                        // Appy setting
                        var newSetting = new TruckSoundSetting()
                        {
                            TruckId = Truck.Id,
                            EngineSoundPackageId = enginePackage.Id
                        };
                        db.TruckSoundSettings.Add(newSetting);
                    }
                }
            }
            catch (Exception ex)
            {
                // Tell the user about the failed validation error
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Truck = null;
                return;
            }

            // Close the form
            this.DialogResult = DialogResult.OK;
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = checkBox1.Checked;
            engineSoundPackageBox.Enabled = enabled;
        }
    }
}
