using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class TruckEditForm : Form
    {
        protected Truck Truck { get; set; }

        public TruckEditForm(Truck truck = null)
        {
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            if (truck == null)
            {
                shadowLabel1.Text = "Add New Truck";
            }
            else
            {
                shadowLabel1.Text = "Modify Truck";
                EngineNameBox.Text = truck.Name;
                UnitNameBox.Text = truck.UnitName;
            }

            Truck = truck;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            // Check for a valid identifier string
            if (!Regex.Match(UnitNameBox.Text, @"^[a-z0-9_\.]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show("Invalid Engine Sii Unit Name. Please use alpha-numeric, or underscores only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Check engine name
            if (!Regex.Match(EngineNameBox.Text, @"^[a-z0-9_.,\-\s\t]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid Engine Name string. Please use alpha-numeric, period, underscores, dashes or spaces only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
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
                            Name = EngineNameBox.Text.Trim(),
                            UnitName = UnitNameBox.Text.Trim()
                        };
                        db.Trucks.Add(Truck);
                    }
                    else
                    {
                        Truck.Name = EngineNameBox.Text.Trim();
                        Truck.UnitName = UnitNameBox.Text.Trim();
                        db.Trucks.Update(Truck);
                    }
                }
            }
            catch (Exception ex)
            {
                // Tell the user about the failed validation error
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
