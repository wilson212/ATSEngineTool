using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class SoundEditor : Form
    {
        protected bool NewSound { get; set; }

        protected SoundType Type { get; set; }

        protected EngineSound Sound { get; set; }

        protected SoundPackage Package { get; set; }

        public SoundEditor(SoundPackage package, SoundType type)
        {
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            NewSound = true;
            Sound = new EngineSound();
            Package = package;
            Type = type;

            InitializeForm();
        }

        public SoundEditor(EngineSound sound)
        {
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            NewSound = sound == null;
            Sound = sound;
            Package = sound.Package;
            Type = (sound?.Type ?? SoundType.Interior);

            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set title
            if (Type == SoundType.Interior)
                this.Text = "Interior Sound Editor";
            else
                this.Text = "Exterior Sound Editor";

            var existing = new List<SoundAttribute>();
            if (NewSound)
                existing.AddRange(Package.EngineSounds.Where(x => x.Type == Type).Select(x => x.Attribute));

            // Fill selection box
            foreach (var name in Enum.GetNames(typeof(SoundAttribute)))
            {
                // Skip existing sounds for this package that are not array sounds
                var val = (SoundAttribute)Enum.Parse(typeof(SoundAttribute), name);
                if (existing.Contains(val) && !SoundInfo.Attributes[val].IsArray)
                    continue;

                // Add item
                attrType.Items.Add(name);
                if (Sound != null && name == Sound.Attribute.ToString())
                    attrType.SelectedIndex = attrType.Items.Count - 1;
            }

            if (!NewSound)
            {
                attrType.Enabled = false;
                fileNameBox.Text = Sound.FileName;
                volumeBox.Value = (decimal)(Sound.Volume * 100);
                checkBox2D.Checked = Sound.Is2D;
                checkBoxLooped.Checked = Sound.Looped;
                if (Sound.IsEngineSound)
                {
                    pitchBox.Value = Sound.PitchReference;
                    maxRpmBox.Value = Sound.MaxRpm;
                    minRpmBox.Value = Sound.MinRpm;
                }
            }
            
            if (attrType.SelectedIndex == -1)
                attrType.SelectedIndex = 0;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (!PassesValidaion()) return;

            // Set new values
            Sound.Package = Package;
            Sound.FileName = fileNameBox.Text;
            Sound.Volume = (double)(volumeBox.Value / 100);
            Sound.Is2D = checkBox2D.Checked;
            Sound.Looped = checkBoxLooped.Checked;
            if (Sound.IsEngineSound)
            {
                Sound.PitchReference = (int)pitchBox.Value;
                Sound.MaxRpm = (int)maxRpmBox.Value;
                Sound.MinRpm = (int)minRpmBox.Value;
            }

            using (AppDatabase db = new AppDatabase())
            {
                db.EngineSounds.AddOrUpdate(Sound);
            }

            // Close form
            this.DialogResult = DialogResult.OK;
        }

        private void attrType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var attr = (SoundAttribute)Enum.Parse(typeof(SoundAttribute), attrType.SelectedItem.ToString());
            groupBox1.Enabled = SoundInfo.Attributes[attr].IsEngineSound;

            pitchBox.Enabled = groupBox1.Enabled;
            maxRpmBox.Enabled = groupBox1.Enabled;
            minRpmBox.Enabled = groupBox1.Enabled;
        }

        private bool PassesValidaion()
        {
            try
            {
                // Remove bad file system characters
                string file = fileNameBox.Text;

                // Check for empty strings
                if (String.IsNullOrWhiteSpace(file))
                {
                    // Tell the user this isnt allowed
                    MessageBox.Show(
                        "Invalid or no sound filename specified. Please Try again",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                    return false;
                }

                // Add Extensions if they are missing
                if (!Path.HasExtension(file)) file += ".ogg";

                // Set text box values again
                fileNameBox.Text = file;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private void searchButton_Click(object sender, EventArgs e)
        {
            using (SoundSelectForm frm = new SoundSelectForm(Package))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fileNameBox.Text = frm.SoundPath;
                }
            }
        }

        private DialogResult AskUserAboutSwitch(SoundType type)
        {
            string prefix = (Type == SoundType.Exterior) ? "interior" : "exterior";
            string suffix = (Type == SoundType.Exterior) ? "exterior" : "interior";
            return MessageBox.Show(
               $"The selected sound file is an {prefix} sound file. Would you like to create a copy in the {suffix} folder?", 
                "Verification", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question
            );
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
