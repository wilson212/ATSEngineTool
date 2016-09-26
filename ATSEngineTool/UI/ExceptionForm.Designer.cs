namespace ATSEngineTool
{
    partial class ExceptionForm
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
            this.InstructionText = new System.Windows.Forms.Label();
            this.ErrorIcon = new System.Windows.Forms.PictureBox();
            this.panelMessage = new System.Windows.Forms.Panel();
            this.labelContent = new System.Windows.Forms.Label();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.buttonViewLog = new System.Windows.Forms.Button();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonDetails = new System.Windows.Forms.Button();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.ExceptionDetails = new System.Windows.Forms.TextBox();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).BeginInit();
            this.panelMessage.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.SystemColors.Window;
            this.panelMain.Controls.Add(this.InstructionText);
            this.panelMain.Controls.Add(this.ErrorIcon);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(444, 51);
            this.panelMain.TabIndex = 0;
            // 
            // InstructionText
            // 
            this.InstructionText.AutoSize = true;
            this.InstructionText.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstructionText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.InstructionText.Location = new System.Drawing.Point(50, 15);
            this.InstructionText.Name = "InstructionText";
            this.InstructionText.Size = new System.Drawing.Size(313, 19);
            this.InstructionText.TabIndex = 1;
            this.InstructionText.Text = "ATSEngineTool Has Encountered An Error!";
            // 
            // ErrorIcon
            // 
            this.ErrorIcon.Image = global::ATSEngineTool.Properties.Resources.vistaerror;
            this.ErrorIcon.Location = new System.Drawing.Point(8, 8);
            this.ErrorIcon.Name = "ErrorIcon";
            this.ErrorIcon.Size = new System.Drawing.Size(32, 32);
            this.ErrorIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ErrorIcon.TabIndex = 0;
            this.ErrorIcon.TabStop = false;
            // 
            // panelMessage
            // 
            this.panelMessage.AutoSize = true;
            this.panelMessage.BackColor = System.Drawing.SystemColors.Window;
            this.panelMessage.Controls.Add(this.labelContent);
            this.panelMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMessage.Location = new System.Drawing.Point(0, 51);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.panelMessage.Size = new System.Drawing.Size(444, 24);
            this.panelMessage.TabIndex = 1;
            // 
            // labelContent
            // 
            this.labelContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelContent.AutoSize = true;
            this.labelContent.Location = new System.Drawing.Point(50, 1);
            this.labelContent.MaximumSize = new System.Drawing.Size(380, 0);
            this.labelContent.MinimumSize = new System.Drawing.Size(380, 0);
            this.labelContent.Name = "labelContent";
            this.labelContent.Size = new System.Drawing.Size(380, 13);
            this.labelContent.TabIndex = 1;
            // 
            // buttonAbort
            // 
            this.buttonAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonAbort.Location = new System.Drawing.Point(362, 12);
            this.buttonAbort.Margin = new System.Windows.Forms.Padding(5);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonAbort.TabIndex = 2;
            this.buttonAbort.Text = "&Quit";
            this.buttonAbort.UseVisualStyleBackColor = true;
            // 
            // buttonViewLog
            // 
            this.buttonViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonViewLog.Location = new System.Drawing.Point(150, 12);
            this.buttonViewLog.Margin = new System.Windows.Forms.Padding(5);
            this.buttonViewLog.Name = "buttonViewLog";
            this.buttonViewLog.Size = new System.Drawing.Size(125, 23);
            this.buttonViewLog.TabIndex = 3;
            this.buttonViewLog.Text = "&View Debug Log";
            this.buttonViewLog.UseVisualStyleBackColor = true;
            this.buttonViewLog.Click += new System.EventHandler(this.buttonViewLog_Click);
            // 
            // buttonContinue
            // 
            this.buttonContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonContinue.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.buttonContinue.Location = new System.Drawing.Point(281, 12);
            this.buttonContinue.Margin = new System.Windows.Forms.Padding(5);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(75, 23);
            this.buttonContinue.TabIndex = 4;
            this.buttonContinue.Text = "&Continue";
            this.buttonContinue.UseVisualStyleBackColor = true;
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.SystemColors.Control;
            this.panelButtons.Controls.Add(this.buttonDetails);
            this.panelButtons.Controls.Add(this.buttonContinue);
            this.panelButtons.Controls.Add(this.buttonAbort);
            this.panelButtons.Controls.Add(this.buttonViewLog);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(0, 75);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(444, 50);
            this.panelButtons.TabIndex = 5;
            // 
            // buttonDetails
            // 
            this.buttonDetails.Location = new System.Drawing.Point(9, 12);
            this.buttonDetails.Name = "buttonDetails";
            this.buttonDetails.Size = new System.Drawing.Size(75, 23);
            this.buttonDetails.TabIndex = 5;
            this.buttonDetails.Text = "&Details";
            this.buttonDetails.UseVisualStyleBackColor = true;
            this.buttonDetails.Click += new System.EventHandler(this.buttonDetails_Click);
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.ExceptionDetails);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(0, 125);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new System.Windows.Forms.Padding(5);
            this.panelDetails.Size = new System.Drawing.Size(444, 200);
            this.panelDetails.TabIndex = 6;
            // 
            // ExceptionDetails
            // 
            this.ExceptionDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ExceptionDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.ExceptionDetails.Location = new System.Drawing.Point(5, 5);
            this.ExceptionDetails.Multiline = true;
            this.ExceptionDetails.Name = "ExceptionDetails";
            this.ExceptionDetails.ReadOnly = true;
            this.ExceptionDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ExceptionDetails.Size = new System.Drawing.Size(434, 198);
            this.ExceptionDetails.TabIndex = 0;
            this.ExceptionDetails.WordWrap = false;
            // 
            // ExceptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(444, 332);
            this.Controls.Add(this.panelDetails);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelMessage);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 38);
            this.Name = "ExceptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Error";
            this.TopMost = true;
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).EndInit();
            this.panelMessage.ResumeLayout(false);
            this.panelMessage.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelMessage;
        private System.Windows.Forms.Label labelContent;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Button buttonViewLog;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonDetails;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.TextBox ExceptionDetails;
        private System.Windows.Forms.Label InstructionText;
        private System.Windows.Forms.PictureBox ErrorIcon;
    }
}