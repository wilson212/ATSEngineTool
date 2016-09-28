using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATSEngineTool.Database;
using FreeImageAPI;

namespace ATSEngineTool
{
    public partial class SeriesEditForm : Form
    {
        protected EngineSeries Series { get; set; }
        public bool NewSeries { get; private set; }

        /// <summary>
        /// Icon file path
        /// </summary>
        protected static string MatPath = Path.Combine(Program.RootPath, "graphics");

        public SeriesEditForm(EngineSeries series = null)
        {
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);
            shadowLabel1.Text = (series == null) ? "New Engine Series" : "Modify Engine Series";
            Series = series;
            NewSeries = series == null;

            // Add engine icons
            var images = Directory.GetFiles(MatPath, "*.dds");
            foreach (string image in images)
            {
                string fn = Path.GetFileNameWithoutExtension(image);
                iconBox.Items.Add(fn);
                if (series != null && fn.Equals(series.EngineIcon, StringComparison.OrdinalIgnoreCase))
                    iconBox.SelectedIndex = iconBox.Items.Count - 1;
            }

            // Add each sound to the lists
            using (AppDatabase db = new AppDatabase())
            {
                foreach (SoundPackage sound in db.EngineSounds)
                {
                    soundBox.Items.Add(sound);
                    if (Series != null && sound.Id == series.SoundId)
                        soundBox.SelectedIndex = soundBox.Items.Count - 1;
                }
            }

            if (iconBox.SelectedIndex == -1)
                iconBox.SelectedIndex = 0;

            if (soundBox.SelectedIndex == -1)
                soundBox.SelectedIndex = 0;

            // Set texts
            if (series != null)
            {
                manuNameBox.Text = series.Manufacturer;
                seriesNameBox.Text = series.Name;
                displacementBox.Value = series.Displacement;
                iconBox.Focus();
            }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            // Check for a valid identifier string
            if (!Regex.Match(manuNameBox.Text, @"^[a-z0-9_.,\-\s\t()]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show("Invalid Manufacturer Name. Please use alpha-numeric, period, comma, underscores, dashes or spaces only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return;
            }

            // Check engine name
            if (!Regex.Match(seriesNameBox.Text, @"^[a-z0-9_.,\-\s\t()]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid Series Name string. Please use alpha-numeric, period, underscores, dashes or spaces only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
            }

             
            // Save or update the engine series
            try
            {
                // Add or update the truck in the database
                using (AppDatabase db = new AppDatabase())
                {
                    if (NewSeries)
                    {
                        Series = new EngineSeries()
                        {
                            Name = seriesNameBox.Text.Trim(),
                            Manufacturer = manuNameBox.Text.Trim(),
                            Displacement = displacementBox.Value,
                            EngineIcon = iconBox.SelectedItem.ToString(),
                            SoundPackage = ((SoundPackage)soundBox.SelectedItem)
                        };
                        db.EngineSeries.Add(Series);
                    }
                    else
                    {
                        Series.Name = seriesNameBox.Text.Trim();
                        Series.Manufacturer = manuNameBox.Text.Trim();
                        Series.Displacement = displacementBox.Value;
                        Series.EngineIcon = iconBox.SelectedItem.ToString();
                        Series.SoundPackage = ((SoundPackage)soundBox.SelectedItem);
                        db.EngineSeries.Update(Series);
                    }
                }
            }
            catch (Exception ex)
            {
                // Tell the user about the failed validation error
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void iconBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = Path.Combine(MatPath, iconBox.SelectedItem.ToString());
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
