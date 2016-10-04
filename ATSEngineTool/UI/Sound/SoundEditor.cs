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

        public SoundEditor(SoundPackage package, EngineSound sound = null)
        {
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            NewSound = sound == null;
            Sound = sound ?? new EngineSound();
            Package = package;
            Type = (sound?.Type ?? SoundType.Interior);

            // Set title
            if (Type == SoundType.Interior)
                this.Text = "Interior Sound Editor";
            else
                this.Text = "Exterior Sound Editor";

            // Fill selection box
            int i = 0;
            foreach (var name in Enum.GetNames(typeof(SoundAttribute)))
            {
                attrType.Items.Add(name);
                if (sound != null && name == sound.Attribute.ToString())
                    attrType.SelectedIndex = attrType.Items.Count - 1;
                i++;
            }

            if (Sound != null)
            {
                string prefix = (Type == SoundType.Exterior) ? "ext/" : "int/";
                fileNameBox.Text = prefix + sound.FileName;
                volumeBox.Value = (decimal)(sound.Volume * 100);
                checkBox2D.Checked = sound.Is2D;
                checkBoxLooped.Checked = sound.Looped;
                if (sound.IsEngineSound)
                {
                    pitchBox.Value = (int)sound.PitchReference;
                    maxRpmBox.Value = (int)sound.MaxRpm;
                    minRpmBox.Value = (int)sound.MinRpm;
                }
            }
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
            groupBox1.Enabled = EngineSound.IsEngineSoundType(attr);

            pitchBox.Enabled = groupBox1.Enabled;
            maxRpmBox.Enabled = groupBox1.Enabled;
            minRpmBox.Enabled = groupBox1.Enabled;
        }

        private bool PassesValidaion()
        {
            // Remove bad file system characters
            string file = fileNameBox.Text.MakeFileNameSafe();

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
        
        private void searchButton_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(Program.RootPath, "sounds", "engine", Package.FolderName);
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Sound File";
            dialog.Filter = "Ogg Vorbis Sound|*.ogg";
            dialog.InitialDirectory = folderPath;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileName(dialog.FileName);
                string directory = Path.GetFileName(Path.GetDirectoryName(dialog.FileName));

                // Sound import?
                if (!dialog.FileName.StartsWith(folderPath))
                {
                    return;
                }

                // Switching?
                if (Type == SoundType.Interior && directory.StartsWith("ext"))
                {
                    // Ask the user what they are doing
                    var result = AskUserAboutSwitch(Type);
                    if (result != DialogResult.Yes) return;

                    // copy file over
                    File.Copy(dialog.FileName, Path.Combine(folderPath, "int", fileName));
                    fileNameBox.Text = "int/" + fileName;
                }
                else if (Type == SoundType.Exterior && directory.StartsWith("int"))
                {
                    // Ask the user what they are doing
                    var result = AskUserAboutSwitch(Type);
                    if (result != DialogResult.Yes) return;

                    // copy file over
                    File.Copy(dialog.FileName, Path.Combine(folderPath, "ext", fileName));
                    fileNameBox.Text = "ext/" + fileName;
                }
                else
                {
                    // just select the file
                    string prefix = (Type == SoundType.Exterior) ? "ext/" : "int/";
                    fileNameBox.Text = prefix + fileName;
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
