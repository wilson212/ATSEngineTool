namespace ATSEngineTool
{
    partial class TaskForm
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.labelInstructionText = new System.Windows.Forms.Label();
            this.InstructionIcon = new System.Windows.Forms.PictureBox();
            this.panelMessage = new System.Windows.Forms.Panel();
            this.labelContent = new System.Windows.Forms.Label();
            this.panelProgressBar = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panelButton = new System.Windows.Forms.Panel();
            this.labelFooterText = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InstructionIcon)).BeginInit();
            this.panelMessage.SuspendLayout();
            this.panelProgressBar.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.SystemColors.Window;
            this.panelMain.Controls.Add(this.labelInstructionText);
            this.panelMain.Controls.Add(this.InstructionIcon);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(434, 50);
            this.panelMain.TabIndex = 1;
            // 
            // labelInstructionText
            // 
            this.labelInstructionText.AutoSize = true;
            this.labelInstructionText.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstructionText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.labelInstructionText.Location = new System.Drawing.Point(50, 15);
            this.labelInstructionText.Name = "labelInstructionText";
            this.labelInstructionText.Size = new System.Drawing.Size(170, 19);
            this.labelInstructionText.TabIndex = 1;
            this.labelInstructionText.Text = "Loading, Please Wait...";
            // 
            // InstructionIcon
            // 
            this.InstructionIcon.Image = global::ATSEngineTool.Properties.Resources.vistaInfo;
            this.InstructionIcon.Location = new System.Drawing.Point(8, 8);
            this.InstructionIcon.Name = "InstructionIcon";
            this.InstructionIcon.Size = new System.Drawing.Size(32, 32);
            this.InstructionIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.InstructionIcon.TabIndex = 0;
            this.InstructionIcon.TabStop = false;
            // 
            // panelMessage
            // 
            this.panelMessage.AutoSize = true;
            this.panelMessage.BackColor = System.Drawing.SystemColors.Window;
            this.panelMessage.Controls.Add(this.labelContent);
            this.panelMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMessage.Location = new System.Drawing.Point(0, 50);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.panelMessage.Size = new System.Drawing.Size(434, 20);
            this.panelMessage.TabIndex = 2;
            // 
            // labelContent
            // 
            this.labelContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelContent.AutoSize = true;
            this.labelContent.Location = new System.Drawing.Point(50, 1);
            this.labelContent.MaximumSize = new System.Drawing.Size(370, 0);
            this.labelContent.MinimumSize = new System.Drawing.Size(370, 0);
            this.labelContent.Name = "labelContent";
            this.labelContent.Size = new System.Drawing.Size(370, 13);
            this.labelContent.TabIndex = 1;
            this.labelContent.Text = "Message";
            // 
            // panelProgressBar
            // 
            this.panelProgressBar.BackColor = System.Drawing.SystemColors.Window;
            this.panelProgressBar.Controls.Add(this.progressBar);
            this.panelProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelProgressBar.Location = new System.Drawing.Point(0, 70);
            this.panelProgressBar.Name = "panelProgressBar";
            this.panelProgressBar.Size = new System.Drawing.Size(434, 35);
            this.panelProgressBar.TabIndex = 3;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(50, 8);
            this.progressBar.MarqueeAnimationSpeed = 20;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(370, 18);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // panelButton
            // 
            this.panelButton.BackColor = System.Drawing.SystemColors.Control;
            this.panelButton.Controls.Add(this.labelFooterText);
            this.panelButton.Controls.Add(this.CancelBtn);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButton.Location = new System.Drawing.Point(0, 105);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(434, 40);
            this.panelButton.TabIndex = 4;
            // 
            // labelFooterText
            // 
            this.labelFooterText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFooterText.AutoSize = true;
            this.labelFooterText.Location = new System.Drawing.Point(50, 9);
            this.labelFooterText.MaximumSize = new System.Drawing.Size(280, 0);
            this.labelFooterText.MinimumSize = new System.Drawing.Size(280, 0);
            this.labelFooterText.Name = "labelFooterText";
            this.labelFooterText.Size = new System.Drawing.Size(280, 13);
            this.labelFooterText.TabIndex = 2;
            this.labelFooterText.Text = "Footer Message";
            this.labelFooterText.Visible = false;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(347, 9);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 0;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // TaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(434, 145);
            this.ControlBox = false;
            this.Controls.Add(this.panelButton);
            this.Controls.Add(this.panelProgressBar);
            this.Controls.Add(this.panelMessage);
            this.Controls.Add(this.panelMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 38);
            this.Name = "TaskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Task";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TaskForm_FormClosed);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InstructionIcon)).EndInit();
            this.panelMessage.ResumeLayout(false);
            this.panelMessage.PerformLayout();
            this.panelProgressBar.ResumeLayout(false);
            this.panelButton.ResumeLayout(false);
            this.panelButton.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelInstructionText;
        private System.Windows.Forms.PictureBox InstructionIcon;
        private System.Windows.Forms.Panel panelMessage;
        private System.Windows.Forms.Label labelContent;
        private System.Windows.Forms.Panel panelProgressBar;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label labelFooterText;
    }
}