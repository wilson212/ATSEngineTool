using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using ATSEngineTool.Properties;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class SoundSelectForm : Form
    {
        protected SoundPackage Package { get; set; }

        public string SoundPath { get; private set; }

        public SoundSelectForm(SoundPackage package)
        {
            // Create controls and style the header
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);

            // Create treeView image list
            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.Add(Resources.folder2);
            treeView1.ImageList.Images.Add(Resources.folder_open2);
            treeView1.ImageList.Images.Add(Resources.music);

            // Fill the tree
            Package = package;
            FillTree();
        }

        private void FillTree()
        {
            // Always Clear Nodes first!
            treeView1.Nodes.Clear();

            // Define paths
            string commonPath = Path.Combine(Program.RootPath, "sounds", "common");
            var directory = new DirectoryInfo(commonPath);

            // Add common sounds
            TreeNode node = RecursiveTreeNodes(directory);
            node.Tag = "@CP";
            treeView1.Nodes.Add(node);

            // Engines for this package only
            string enginePath = Path.Combine(Program.RootPath, "sounds", "engine", Package.FolderName);
            directory = new DirectoryInfo(enginePath);
            node = RecursiveTreeNodes(directory);
            node.Text = "engine/" + node.Text;
            node.Tag = "@EP";
            treeView1.Nodes.Add(node);
        }

        /// <summary>
        /// Recursivly adds tree nodes for each folder and file within the 
        /// specified directory.
        /// </summary>
        private TreeNode RecursiveTreeNodes(DirectoryInfo directory)
        {
            TreeNode node = new TreeNode(directory.Name);
            node.ImageIndex = 0;
            node.SelectedImageIndex = 0;

            DirectoryInfo[] dirs = directory.GetDirectories();
            foreach (var dir in dirs)
            {
                TreeNode subNode = RecursiveTreeNodes(dir);
                subNode.Tag = dir;
                node.Nodes.Add(subNode);
            }

            foreach (var file in directory.GetFiles("*.ogg"))
            {
                TreeNode subNode = new TreeNode(file.Name);
                subNode.Tag = file;
                subNode.SelectedImageIndex = 2;
                subNode.ImageIndex = 2;
                node.Nodes.Add(subNode);
            }

            return node;
        }

        /// <summary>
        /// Gets the root TreeNode for the specified TreeNode
        /// </summary>
        private TreeNode GetRootNode(TreeNode node)
        {
            while (node.Parent != null)
                node = node.Parent;

            return node;
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
            => confirmButton_Click(sender, EventArgs.Empty);

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Nodes.Count > 0) return;

            var selected = treeView1.SelectedNode;
            var parent = GetRootNode(selected);

            // Grab the sound path with the prefix
            string path = (selected.FullPath.StartsWith("engine/")) ? selected.FullPath.Substring(7) : selected.FullPath;
            int index = path.IndexOf('/'); // skip "engine/"
            SoundPath = parent.Tag.ToString() + path.Substring(index);

            // Close form
            this.DialogResult = DialogResult.OK;
        }

        private void importSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Sound File";
            dialog.Filter = "Ogg Vorbis Sound|*.ogg";
            dialog.InitialDirectory = Path.Combine(Program.RootPath, "sounds");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = Path.GetFileName(dialog.FileName);
                var selected = treeView1.SelectedNode;

                try
                {
                    // copy file over
                    File.Copy(dialog.FileName, Path.Combine((selected.Tag as DirectoryInfo).FullName, fileName));

                    MessageBox.Show(
                        $"Successfully copied over the sound file {fileName}!",
                        "File Copy Success", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );

                    FillTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"failed to copy over the sound file {fileName}!"
                         + Environment.NewLine + Environment.NewLine
                         + "Error: " + ex.Message,
                        "File Copy Failed", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                }
            }
        }

        private void deleteSoundFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the use they are making a mistake!
            var result = MessageBox.Show("Are you sure you want to delete this sound file?",
                "Verification", MessageBoxButtons.YesNo, MessageBoxIcon.Question
            );

            // phew... /wipesSweatOffBrow
            if (result != DialogResult.Yes) return;
            string fileName = (treeView1.SelectedNode.Tag as FileInfo).FullName;

            try
            {
                // Delete file
                File.Delete(fileName);

                MessageBox.Show(
                    $"Successfully delete the sound file {Path.GetFileName(fileName)}!",
                    "File Copy Success", MessageBoxButtons.OK, MessageBoxIcon.Information
                );

                FillTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"failed to delete the sound file {Path.GetFileName(fileName)}!"
                     + Environment.NewLine + Environment.NewLine
                     + "Error: " + ex.Message,
                    "File delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }

            
        }

        /// <summary>
        /// Only show context menu for sound sub folders
        /// </summary>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true; // never show
            return;
            /*
            var node = treeView1.SelectedNode;
            if (node != null)
            {
                importSoundToolStripMenuItem.Enabled = (node.Parent != null && node.Tag is DirectoryInfo);
                deleteSoundFileToolStripMenuItem.Enabled = node.Nodes.Count == 0 && node.Tag is FileInfo;
            }
            else
            {
                e.Cancel = true;
            }
            */
        }

        /// <summary>
        /// Right click selects in this treeview!
        /// </summary>
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
            }
        }
    }
}
