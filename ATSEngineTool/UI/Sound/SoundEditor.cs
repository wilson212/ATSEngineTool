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

        protected Sound Sound { get; set; }

        protected SoundLocation Type { get; set; }

        protected SoundPackage Package { get; set; }

        public SoundEditor(SoundPackage package, SoundLocation type)
        {
            // Create controls and style the header
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            // Set internals
            NewSound = true;
            Package = package;
            Type = type;

            // Create new sound
            switch (package.SoundType)
            {
                case SoundType.Engine:
                    Sound = new EngineSound();
                    break;
                case SoundType.Truck:
                    Sound = new TruckSound();
                    break;
                default:
                    throw new Exception("Invalid sound type");
            }

            InitializeForm();
        }

        public SoundEditor(Sound sound, SoundPackage package)
        {
            // Create controls and style the header
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            // Set internals
            NewSound = false;
            Sound = sound;
            Package = package;
            Type = sound.Location;

            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set title
            this.Text = (Type == SoundLocation.Interior) ? "Interior Sound Editor" : "Exterior Sound Editor";

            // Get a list of existing sound attributes already in the sound package
            var existing = new List<SoundAttribute>();
            if (NewSound)
            {
                switch (Package.SoundType)
                {
                    case SoundType.Engine:
                        var ep = (EngineSoundPackage)Package;
                        existing.AddRange(ep.EngineSounds.Where(x => x.Location == Type).Select(x => x.Attribute));
                        break;
                    case SoundType.Truck:
                        var tp = (TruckSoundPackage)Package;
                        existing.AddRange(tp.TruckSounds.Where(x => x.Location == Type).Select(x => x.Attribute));
                        break;
                    default:
                        throw new Exception("Invalid sound type");
                }
            }

            // Fill selection box
            foreach (var name in Enum.GetNames(typeof(SoundAttribute)))
            {
                // Fetch sound info for this attribute
                var val = (SoundAttribute)Enum.Parse(typeof(SoundAttribute), name);
                SoundInfo si = SoundInfo.Attributes[val];

                // Skip existing sounds for this package that are not array sounds
                if (si.SoundType != Package.SoundType || (existing.Contains(val) && !si.IsArray))
                {
                    continue;
                }

                // Add item
                attrType.Items.Add(val);
                if (Sound != null && val == Sound.Attribute)
                    attrType.SelectedIndex = attrType.Items.Count - 1;
            }

            // If not a new sound, fill form data
            if (!NewSound)
            {
                attrType.Enabled = false;
                fileNameBox.Text = Sound.FileName;
                volumeBox.Value = (decimal)(Sound.Volume * 100);
                checkBox2D.Checked = Sound.Is2D;
                checkBoxLooped.Checked = Sound.Looped;
                if (Package.SoundType == SoundType.Engine)
                {
                    var es = (EngineSound)Sound;
                    pitchBox.Value = es.PitchReference;
                    maxRpmBox.Value = es.MaxRpm;
                    minRpmBox.Value = es.MinRpm;
                }
            }

            // Enable or Disable engine specific inputs
            bool enabled = Package.SoundType == SoundType.Engine;
            groupBox1.Enabled = enabled;
            pitchBox.Enabled = enabled;
            maxRpmBox.Enabled = enabled;
            minRpmBox.Enabled = enabled;

            // Set default attribute index
            if (attrType.SelectedIndex == -1)
                attrType.SelectedIndex = 0;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (!PassesValidaion()) return;

            // Set new values
            Sound.Attribute = (SoundAttribute)attrType.SelectedItem;
            Sound.Location = Type;
            Sound.PackageId = Package.Id;
            Sound.FileName = fileNameBox.Text;
            Sound.Volume = (double)(volumeBox.Value / 100);
            Sound.Is2D = checkBox2D.Checked;
            Sound.Looped = checkBoxLooped.Checked;

            // Save sound to database
            using (AppDatabase db = new AppDatabase())
            {
                switch (Package.SoundType)
                {
                    case SoundType.Engine:
                        var es = (EngineSound)Sound;
                        es.PitchReference = (int)pitchBox.Value;
                        es.MaxRpm = (int)maxRpmBox.Value;
                        es.MinRpm = (int)minRpmBox.Value;
                        db.EngineSounds.AddOrUpdate(es);
                        break;
                    case SoundType.Truck:
                        var ts = (TruckSound)Sound;
                        db.TruckSounds.AddOrUpdate(ts);
                        break;
                    default:
                        throw new Exception("Invalid sound type");
                }
            }

            // Close form
            this.DialogResult = DialogResult.OK;
        }

        private void attrType_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            using (SoundSelectForm frm = new SoundSelectForm(Package, Sound))
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fileNameBox.Text = frm.SoundPath;
                }
            }
        }

        private DialogResult AskUserAboutSwitch(SoundLocation type)
        {
            string prefix = (Type == SoundLocation.Exterior) ? "interior" : "exterior";
            string suffix = (Type == SoundLocation.Exterior) ? "exterior" : "interior";
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
