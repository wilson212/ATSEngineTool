using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using ATSEngineTool.Database;
using ATSEngineTool.SiiEntities;
using Sii;

namespace ATSEngineTool
{
    public partial class SoundPackageEditor : Form
    {
        public SoundPackageEditor(SoundPackage package = null)
        {
            InitializeComponent();
        }

        private void importButton_Click(object sender, System.EventArgs e)
        {
            // Request the user supply the steam library path
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Sound Package Import";
            dialog.Filter = "Sound Package|*.espack";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream zipFileToOpen = new FileStream(dialog.FileName, FileMode.Open))
                using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Read))
                {
                    // Load the manifest, interior and exterior files
                    var entry = archive.GetEntry("manifest.sii");
                    var interior = archive.GetEntry("interior.sii");
                    var exterior = archive.GetEntry("exterior.sii");

                    // Ensure manifest
                    if (entry == null)
                    {
                        MessageBox.Show("Sound package is missing its manifest.sii file.",
                            "Manifest Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        return;
                    }
                    else if (interior == null || exterior == null)
                    {
                        MessageBox.Show("Sound package is missing its interior.sii or exterior.sii file.",
                            "Sound Package Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        return;
                    }

                    // Parse the manifiest file
                    var document = new SiiDocument(typeof(SoundPackManifest));
                    using (StreamReader reader = new StreamReader(entry.Open()))
                    {
                        document.Load(reader.ReadToEnd().Trim());
                    }

                    // Grab the manifest object
                    var keys = new List<string>(document.Definitions.Keys);
                    var manifest = document.GetDefinition<SoundPackManifest>(keys[0]);

                    // Set form values
                    labelAuthor.Text = manifest.Author;
                    labelVersion.Text = manifest.Version;
                    packageNameBox.Text = manifest.Name;
                    intFilenameBox.Text = manifest.InteriorName;
                    extFilenameBox.Text = manifest.ExteriorName;
                }
            }
        }
    }
}
