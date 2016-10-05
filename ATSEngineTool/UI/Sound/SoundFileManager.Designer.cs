namespace ATSEngineTool
{
    partial class SoundFileManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importSoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.shadowLabel1 = new System.Windows.Forms.ShadowLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteSoundFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentPanel.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.treeView1);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 65);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(484, 397);
            this.contentPanel.TabIndex = 38;
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Location = new System.Drawing.Point(32, 21);
            this.treeView1.Name = "treeView1";
            this.treeView1.PathSeparator = "/";
            this.treeView1.Size = new System.Drawing.Size(420, 350);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCollapse);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importSoundToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteSoundFileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(166, 76);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // importSoundToolStripMenuItem
            // 
            this.importSoundToolStripMenuItem.Name = "importSoundToolStripMenuItem";
            this.importSoundToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.importSoundToolStripMenuItem.Text = "Import Sound";
            this.importSoundToolStripMenuItem.Click += new System.EventHandler(this.importSoundToolStripMenuItem_Click);
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.DimGray;
            this.footerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.footerPanel.Controls.Add(this.confirmButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(0, 462);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(484, 50);
            this.footerPanel.TabIndex = 39;
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(245, 13);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(100, 25);
            this.confirmButton.TabIndex = 26;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(139, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 25);
            this.cancelButton.TabIndex = 25;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.DimGray;
            this.headerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.shadowLabel1);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(484, 65);
            this.headerPanel.TabIndex = 37;
            // 
            // shadowLabel1
            // 
            this.shadowLabel1.BackColor = System.Drawing.Color.Transparent;
            this.shadowLabel1.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shadowLabel1.ForeColor = System.Drawing.SystemColors.Control;
            this.shadowLabel1.Location = new System.Drawing.Point(37, 14);
            this.shadowLabel1.Name = "shadowLabel1";
            this.shadowLabel1.ShadowDirection = 90;
            this.shadowLabel1.ShadowOpacity = 225;
            this.shadowLabel1.ShadowSoftness = 3F;
            this.shadowLabel1.Size = new System.Drawing.Size(297, 39);
            this.shadowLabel1.TabIndex = 0;
            this.shadowLabel1.Text = "Sound File Selector";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
            // 
            // deleteSoundFileToolStripMenuItem
            // 
            this.deleteSoundFileToolStripMenuItem.Name = "deleteSoundFileToolStripMenuItem";
            this.deleteSoundFileToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.deleteSoundFileToolStripMenuItem.Text = "Delete Sound File";
            this.deleteSoundFileToolStripMenuItem.Click += new System.EventHandler(this.deleteSoundFileToolStripMenuItem_Click);
            // 
            // SoundFileManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(484, 512);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoundFileManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Sound File";
            this.contentPanel.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel shadowLabel1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem importSoundToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteSoundFileToolStripMenuItem;
    }
}