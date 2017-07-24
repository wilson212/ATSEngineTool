using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ATSEngineTool.Database;
using ATSEngineTool.Properties;

namespace ATSEngineTool
{
    public partial class SoundSelectForm : Form
    {
        /// <summary>
        /// Contains the sound package we are selecting a sound for
        /// </summary>
        protected SoundPackage Package { get; set; }

        /// <summary>
        /// Defines, if any, the current selected sound for the attribute we are editing
        /// </summary>
        protected Sound StartingSound { get; set; }

        /// <summary>
        /// Defines, if any, the current selected sound Node for the attribute we are editing
        /// </summary>
        protected TreeNode StartingSoundNode { get; set; }

        /// <summary>
        /// Gets the selected sound path from the form
        /// </summary>
        public string SoundPath { get; private set; }

        public SoundSelectForm(SoundPackage package, Sound currentSound = null)
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
            StartingSound = currentSound;

            FillTree();
        }

        /// <summary>
        /// Redraws the file tree with all of the files found in the sound package's folder.
        /// </summary>
        private void FillTree()
        {
            // Always Clear Nodes first!
            treeView1.Nodes.Clear();

            // Define variables
            string path = Path.Combine(Program.RootPath, "sounds", Package.RelativeSystemPath);
            DirectoryInfo directory = new DirectoryInfo(path);

            // Recusivly load the TreeNodes
            TreeNode node = RecursiveTreeNodes(directory);
            node.Tag = "@SP";

            // Add node to tree
            treeView1.Nodes.Add(node);

            // Select starting sound node if we have one
            if (StartingSoundNode != null)
            {
                ExpandTreeNode(StartingSoundNode);
                treeView1.SelectedNode = StartingSoundNode;
            }
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

            // Add Directories
            DirectoryInfo[] dirs = directory.GetDirectories();
            foreach (var dir in dirs)
            {
                TreeNode subNode = RecursiveTreeNodes(dir);
                subNode.Tag = dir;
                node.Nodes.Add(subNode);
            }

            var location = (directory.Name.Equals("ext") || directory.FullName.Contains("\\ext\\")) 
                ? SoundLocation.Exterior 
                : SoundLocation.Interior;
            string fn = (StartingSound != null) ? Path.GetFileName(StartingSound.FileName.Replace("@", "")) : "";

            // Add files in this directory
            foreach (var file in directory.GetFiles("*.ogg"))
            {
                TreeNode subNode = new TreeNode(file.Name);
                subNode.Tag = file;
                subNode.SelectedImageIndex = 2;
                subNode.ImageIndex = 2;
                node.Nodes.Add(subNode);

                if (StartingSound != null && location == StartingSound.Location)
                {
                    if (file.Name.Equals(fn))
                    {
                        StartingSoundNode = subNode;
                    }
                }
            }

            return node;
        }

        private void ExpandTreeNode(TreeNode node)
        {
            if (node.Nodes.Count > 0)
                node.Expand();

            while (node.Parent != null)
            {
                node = node.Parent;
                node.Expand();
            }
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

        /// <summary>
        /// Changes the folder image to open on expand
        /// </summary>
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        /// <summary>
        /// Changes the folder image to closed on collapse
        /// </summary>
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
            string path = selected.FullPath;
            int index = path.IndexOf('/'); // skip "packageFoldName/"
            SoundPath = parent.Tag.ToString() + path.Substring(index);

            // Close form
            this.DialogResult = DialogResult.OK;
            this.Close();
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

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // first, let .NET draw the Node with its defaults
            e.DrawDefault = true;

            // Now update the highlighting or not
            if (e.State == TreeNodeStates.Selected)
            {
                e.Node.BackColor = SystemColors.Highlight;
                e.Node.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                e.Node.BackColor = ((TreeView)sender).BackColor;
                e.Node.ForeColor = ((TreeView)sender).ForeColor;
            }
        }
    }
}
