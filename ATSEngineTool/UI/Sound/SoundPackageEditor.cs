using System;
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

        public SoundPackageEditor(SoundPackage package = null)
        {
            // Create controls and style the header
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            // Set internals
            NewPackage = package == null;
            Package = package ?? new SoundPackage();

            // Set form input field values if existing package
            if (!NewPackage)
            {
                labelAuthor.Text = Package.Author;
                labelVersion.Text = Package.Version.ToString();
                packageNameBox.Enabled = true;
                packageNameBox.Text = Package.Name;
                packageNameBox.SelectionStart = packageNameBox.Text.Length;
                unitNameBox.Enabled = true;
                unitNameBox.Text = Package.UnitName;
                intFilenameBox.Enabled = true;
                intFilenameBox.Text = Package.InteriorFileName;
                extFilenameBox.Enabled = true;
                extFilenameBox.Text = Package.ExteriorFileName;
                folderNameBox.Text = Package.FolderName;

                // Enable buttons
                confirmButton.Enabled = true;
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            // Request the user supply the sound package path
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Sound Package Import";
            dialog.Filter = "Sound Package|*.espack";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Try and mount the sound package
                using (var reader = new SoundPackageReader(dialog.FileName))
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
                        Interior = reader.GetSoundFile(SoundType.Interior);
                        Exterior = reader.GetSoundFile(SoundType.Exterior);
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
                    int index = name.IndexOf('.');
                    labelAuthor.Text = manifest.Author;
                    labelVersion.Text = manifest.Version;
                    unitNameBox.Text = name.Substring(0, (index == -1) ? name.Length : index);
                    packageNameBox.Text = manifest.Name;
                    intFilenameBox.Text = manifest.InteriorName;
                    extFilenameBox.Text = manifest.ExteriorName;

                    // Set internal
                    Imported = true;
                    PackageFilePath = dialog.FileName;

                    // Enable controls
                    packageNameBox.Enabled = true;
                    unitNameBox.Enabled = true;
                    intFilenameBox.Enabled = true;
                    extFilenameBox.Enabled = true;
                    folderNameBox.Enabled = true;
                    confirmButton.Enabled = true;
                }
            }
        }

        private async void confirmButton_Click(object sender, System.EventArgs e)
        {
            // Validate!
            if (!PassesValidaion()) return;

            // Add or update the package details
            Package.Name = packageNameBox.Text;
            Package.Author = labelAuthor.Text;
            Package.Version = Decimal.Parse(labelVersion.Text, CultureInfo.InvariantCulture);
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
                    // Add or update the existing package
                    db.SoundPackages.AddOrUpdate(Package);
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
            string folderPath = Path.Combine(Program.RootPath, "sounds", "engine", folderName);

            // Delete old data
            if (!NewPackage)
            {
                foreach (var sound in Package.EngineSounds)
                    db.EngineSounds.Remove(sound);
            }

            // Delete existing data if it is there.
            if (Directory.Exists(folderPath) && NewPackage)
            {
                // Fetch existing sound from database
                var existing = db.Query<SoundPackage>(
                    "SELECT * FROM `SoundPackage` WHERE `FolderName` = @P0",
                    folderNameBox.Text
                ).FirstOrDefault();

                // Ask the user
                if (existing != null)
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
                    db.SoundPackages.Remove(existing);
                }
            }

            // Wrap in a transaction
            using (var trans = db.BeginTransaction())
            {
                try
                {
                    // Add or update the existing package
                    db.SoundPackages.AddOrUpdate(Package);

                    // Re-open the Sound package ZipFile
                    using (var reader = new SoundPackageReader(PackageFilePath))
                    {
                        // Extract data
                        reader.ExtractToDirectory(folderPath, true);

                        // Save sounds in the database
                        reader.ImportSounds(db, Package, Interior, SoundType.Interior);
                        reader.ImportSounds(db, Package, Exterior, SoundType.Exterior);
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
    }
}
