using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATSEngineTool.Database;
using ATSEngineTool.SiiEntities;

namespace ATSEngineTool
{
    public partial class SoundPackageEditor : Form
    {
        /// <summary>
        /// The interior sound data accessory
        /// </summary>
        private AccessorySoundData Interior { get; set; }

        /// <summary>
        /// The exterior sound data accessory
        /// </summary>
        private AccessorySoundData Exterior { get; set; }

        /// <summary>
        /// The current sound package being edited or created
        /// </summary>
        private SoundPackage Package { get; set; }

        /// <summary>
        /// Indicates whether this is a new or existing sound package
        /// </summary>
        private bool NewPackage { get; set; }

        /// <summary>
        /// If the user decides to import an espack, then this turns true
        /// </summary>
        private bool Imported { get; set; } = false;

        /// <summary>
        /// Gets the imported espacks full file path
        /// </summary>
        protected string PackageFilePath { get; set; }

        public SoundPackageEditor(SoundType type) : this()
        {
            NewPackage = true;
            switch (type)
            {
                case SoundType.Engine:
                    Package = new EngineSoundPackage();
                    shadowLabel1.Text = "Engine Sound Package Editor";
                    break;
                case SoundType.Truck:
                    Package = new TruckSoundPackage();
                    shadowLabel1.Text = "Truck Sound Package Editor";
                    intFilenameBox.Enabled = false;
                    extFilenameBox.Enabled = false;
                    break;
                default:
                    throw new Exception("Invalid sound type");
            }
            youtubeLinkLabel.Enabled = false;
            youtubeLinkLabel.Visible = false;
            youtubeIcon.Visible = false;
        }

        public SoundPackageEditor(SoundPackage package) : this()
        {
            // Set internals
            NewPackage = false;
            Package = package;

            // Set title label text
            switch (Package.SoundType)
            {
                case SoundType.Engine:
                    shadowLabel1.Text = "Engine Sound Package Editor";
                    break;
                case SoundType.Truck:
                    shadowLabel1.Text = "Truck Sound Package Editor";
                    break;
                default:
                    throw new Exception("Invalid sound type");
            }

            // Set form input field values if existing package
            labelAuthor.Text = Package.Author;
            labelVersion.Text = Package.Version.ToString();
            packageNameBox.Enabled = true;
            packageNameBox.Text = Package.Name;
            packageNameBox.SelectionStart = packageNameBox.Text.Length;
            unitNameBox.Enabled = true;
            unitNameBox.Text = Package.UnitName;
            folderNameBox.Enabled = true;
            folderNameBox.Text = Package.FolderName;
            if (package.SoundType == SoundType.Engine)
            {
                intFilenameBox.Enabled = true;
                intFilenameBox.Text = Package.InteriorFileName;
                extFilenameBox.Enabled = true;
                extFilenameBox.Text = Package.ExteriorFileName;
            }

            if (String.IsNullOrWhiteSpace(Package.YoutubeVideoId))
            {
                youtubeLinkLabel.Enabled = false;
                youtubeLinkLabel.Visible = false;
                youtubeIcon.Visible = false;
            }

            // Change Import button text and size
            importButton.Size = new Size(125, 25);
            importButton.Text = "Import Update";

            // Enable buttons
            confirmButton.Enabled = true;
        }

        private SoundPackageEditor()
        {
            // Create controls and style the header
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);
        }

        /// <summary>
        /// Import Button Click Event
        /// </summary>
        private void importButton_Click(object sender, EventArgs e)
        {
            // Request the user supply the sound package path
            OpenFileDialog dialog = new OpenFileDialog();
            switch (Package.SoundType)
            {
                case SoundType.Engine:
                    dialog.Title = "Engine Sound Package Import";
                    dialog.Filter = "Engine Sound Pack|*.espack";
                    break;
                case SoundType.Truck:
                    dialog.Title = "Truck Sound Package Import";
                    dialog.Filter = "Truck Sound Pack|*.tspack";
                    break;
                default:
                    throw new Exception("Invalid sound type");
            }

            // If the user selects a file
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Try and mount the sound package
                using (var reader = new SoundPackageReader(dialog.FileName, Package.SoundType))
                {
                    // Load the manifest, interior and exterior files
                    string name = string.Empty;
                    SoundPackManifest manifest = null;
                    Interior = null; // Reset
                    Exterior = null; // Reset

                    // Try and parse the internal sii files
                    try
                    {
                        manifest = reader.GetManifest(out name);
                        Interior = reader.GetSoundFile(SoundLocation.Interior);
                        Exterior = reader.GetSoundFile(SoundLocation.Exterior);
                    }
                    catch (Exception ex)
                    {
                        // Be fancy and generate a good error message
                        StringBuilder builder = new StringBuilder();
                        if (manifest == null)
                            builder.AppendLine("Failed to parse the package manifest file!");
                        else
                            builder.AppendLineIf(Interior == null, "Failed to parse the interior.sii file!", "Failed to parse the exterior.sii file!");
                        builder.AppendLine();
                        builder.Append("Error: ").AppendLine(ex.Message);

                        if (ex is Sii.SiiSyntaxException)
                        {
                            var siiEx = ex as Sii.SiiSyntaxException;
                            builder.AppendLine("Line: " + siiEx.Span.Start.Line);
                            builder.AppendLine("Column: " + siiEx.Span.Start.Column);
                        }

                        // Alert the user
                        MessageBox.Show(builder.ToString().TrimEnd(), "Sii Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Ensure manifest
                    if (manifest == null)
                    {
                        MessageBox.Show("Sound package is missing its manifest.sii file.",
                            "Manifest Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        return;
                    }
                    else if (Interior == null || Exterior == null)
                    {
                        MessageBox.Show("Sound package is missing its interior.sii or exterior.sii file.",
                            "Sound Package Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        return;
                    }

                    // Set form values
                    labelAuthor.Text = manifest.Author;
                    labelVersion.Text = manifest.Version;
                    if (NewPackage)
                    {
                        int index = name.IndexOf('.');
                        unitNameBox.Text = name.Substring(0, (index == -1) ? name.Length : index);
                        packageNameBox.Text = manifest.Name;
                        intFilenameBox.Text = manifest.InteriorName;
                        extFilenameBox.Text = manifest.ExteriorName;
                    }

                    // Set internal
                    Imported = true;
                    PackageFilePath = dialog.FileName;

                    // Enable controls
                    packageNameBox.Enabled = true;
                    unitNameBox.Enabled = true;
                    folderNameBox.Enabled = true;
                    confirmButton.Enabled = true;
                    if (Package.SoundType == SoundType.Engine)
                    {
                        intFilenameBox.Enabled = true;
                        extFilenameBox.Enabled = true;
                    }

                    if (String.IsNullOrWhiteSpace(manifest.YoutubeVideoId))
                    {
                        youtubeLinkLabel.Enabled = false;
                        youtubeLinkLabel.Visible = false;
                        youtubeIcon.Visible = false;
                    }
                    else
                    {
                        Package.YoutubeVideoId = manifest.YoutubeVideoId;
                        youtubeLinkLabel.Enabled = true;
                        youtubeLinkLabel.Visible = true;
                        youtubeIcon.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Confirm Button Click Event
        /// </summary>
        private async void confirmButton_Click(object sender, System.EventArgs e)
        {
            // Validate user input!
            if (!PassesValidaion()) return;

            // Add or update the package details
            Package.Name = packageNameBox.Text;
            Package.Author = labelAuthor.Text;
            Package.Version = labelVersion.Text;
            Package.UnitName = unitNameBox.Text;
            Package.FolderName = folderNameBox.Text;
            Package.InteriorFileName = intFilenameBox.Text;
            Package.ExteriorFileName = extFilenameBox.Text;

            // Open the database connection and lets go!
            using (AppDatabase db = new AppDatabase())
            {
                // Did we import new data?
                if (!Imported)
                {
                    switch (Package.SoundType)
                    {
                        // Add or update the existing package
                        case SoundType.Engine:
                            db.EngineSoundPackages.AddOrUpdate((EngineSoundPackage)Package);
                            break;
                        case SoundType.Truck:
                            db.TruckSoundPackages.AddOrUpdate((TruckSoundPackage)Package);
                            break;
                    }
                }
                else
                {
                    // Else, we imported. We do this in a seperate task
                    try
                    {
                        // Show task form
                        TaskForm.Show(this,
                            "Importing Sound Package",
                            "Importing Sound Package",
                            "Please wait while the sound package is installed..."
                        );

                        // Import sound
                        await Task.Run(() => ImportSoundPack(db));

                        // Close task form
                        TaskForm.CloseForm();
                    }
                    catch (Exception ex)
                    {
                        TaskForm.CloseForm();
                        ExceptionHandler.GenerateExceptionLog(ex);
                        MessageBox.Show(ex.Message, "Sound Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            // Close the form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ImportSoundPack(AppDatabase db)
        {
            // Define folder path
            string folderName = folderNameBox.Text.MakeFileNameSafe();
            string folderPath = Path.Combine(Program.RootPath, "sounds", Package.PackageTypeFolderName, folderName);

            // Delete old data
            if (!NewPackage)
            {
                // Delete the old sounds
                switch (Package.SoundType)
                {
                    case SoundType.Engine:
                        var ep = (EngineSoundPackage)Package;
                        foreach (var sound in ep.EngineSounds)
                            db.EngineSounds.Remove(sound);
                        break;
                    case SoundType.Truck:
                        var tp = (TruckSoundPackage)Package;
                        foreach (var sound in tp.TruckSounds)
                            db.TruckSounds.Remove(sound);
                        break;
                    default:
                        throw new Exception("Invalid sound type");
                }
            }

            // Delete existing data if it is there.
            if (Directory.Exists(folderPath))
            {
                string query = String.Empty;
                SoundPackage existing;

                // Fetch existing sound from database
                switch (Package.SoundType)
                {
                    // Add or update the existing package
                    case SoundType.Engine:
                        query = "SELECT * FROM `EngineSoundPackage` WHERE `FolderName` = @P0";
                        existing = db.Query<EngineSoundPackage>(query, folderNameBox.Text).FirstOrDefault();
                        break;
                    case SoundType.Truck:
                        query = "SELECT * FROM `TruckSoundPackage` WHERE `FolderName` = @P0";
                        existing = db.Query<TruckSoundPackage>(query, folderNameBox.Text).FirstOrDefault();
                        break;
                    default:
                        throw new Exception("Invalid sound type");
                }

                // If the folder name is already in use, and (is new package OR the existing package ID does not match the current)
                if (existing != null && (NewPackage || Package.Id != existing.Id))
                {
                    var result = MessageBox.Show(
                        $"The sound folder chosen belongs to the sound package \"{existing?.Name}\"! "
                            + "Do you want to replace the existing sound package with this one?",
                        "Verification", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                    );

                    // Quit if the user see's the error in his ways
                    if (result != DialogResult.Yes)
                        return false;

                    // Delete the old sound
                    switch (Package.SoundType)
                    {
                        // Add or update the existing package
                        case SoundType.Engine:
                            db.EngineSoundPackages.Remove((EngineSoundPackage)existing);
                            break;
                        case SoundType.Truck:
                            db.TruckSoundPackages.Remove((TruckSoundPackage)existing);
                            break;
                        default:
                            throw new Exception("Invalid sound type");
                    }
                }
            }

            // Wrap in a transaction
            using (var trans = db.BeginTransaction())
            {
                try
                {
                    // Add or update the existing package
                    switch (Package.SoundType)
                    {
                        // Add or update the existing package
                        case SoundType.Engine:
                            db.EngineSoundPackages.AddOrUpdate((EngineSoundPackage)Package);
                            break;
                        case SoundType.Truck:
                            db.TruckSoundPackages.AddOrUpdate((TruckSoundPackage)Package);
                            break;
                        default:
                            throw new Exception("Invalid sound type");
                    }

                    // Re-open the Sound package ZipFile
                    using (var reader = new SoundPackageReader(PackageFilePath, Package.SoundType))
                    {
                        // Save sounds in the database
                        reader.InstallPackage(db, Package, folderPath, true);
                    }

                    // Commit and return
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }

            return true;
        }

        private bool PassesValidaion()
        {
            // Remove bad file system characters
            string intF = intFilenameBox.Text.MakeFileNameSafe();
            string extF = extFilenameBox.Text.MakeFileNameSafe();
            string folder = folderNameBox.Text.MakeFileNameSafe();

            if (Package.SoundType == SoundType.Engine)
            {
                // Check for empty strings
                if (String.IsNullOrWhiteSpace(intF) || String.IsNullOrWhiteSpace(extF))
                {
                    // Tell the user this isnt allowed
                    MessageBox.Show(
                        "One or both filename inputs failed to pass validation. Please Try again",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                    return false;
                }
            }

            // Check for empty strings
            if (String.IsNullOrWhiteSpace(folder) || String.IsNullOrWhiteSpace(folder))
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid or no foldername specified. Please Try again",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return false;
            }

            // Add Extensions if they are missing
            if (!Path.HasExtension(intF)) intF += ".sii";
            if (!Path.HasExtension(extF)) extF += ".sii";

            // Check sound package name
            if (!Regex.Match(packageNameBox.Text, @"^[a-z0-9_.,\-\s\t()]+$", RegexOptions.IgnoreCase).Success)
            {
                // Tell the user this isnt allowed
                MessageBox.Show(
                    "Invalid sound package name string. Please use alpha-numeric, period, underscores, dashes or spaces only",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                return false;
            }

            // Set text box values again
            intFilenameBox.Text = intF;
            extFilenameBox.Text = extF;
            folderNameBox.Text = folder;
            return true;
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

        private void YoutubeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start($"https://www.youtube.com/watch?v={Package.YoutubeVideoId}");
        }
    }
}
