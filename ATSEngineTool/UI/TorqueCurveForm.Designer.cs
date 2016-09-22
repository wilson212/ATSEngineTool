namespace ATSEngineTool
{
    partial class TorqueCurveForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.torqueLevelBox = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.rpmLevelBox = new System.Windows.Forms.NumericUpDown();
            this.label33 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.torqueLevelBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpmLevelBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.torqueLevelBox);
            this.panel1.Controls.Add(this.label26);
            this.panel1.Controls.Add(this.rpmLevelBox);
            this.panel1.Controls.Add(this.label33);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 123);
            this.panel1.TabIndex = 0;
            // 
            // torqueLevelBox
            // 
            this.torqueLevelBox.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.torqueLevelBox.Location = new System.Drawing.Point(148, 71);
            this.torqueLevelBox.Name = "torqueLevelBox";
            this.torqueLevelBox.Size = new System.Drawing.Size(103, 20);
            this.torqueLevelBox.TabIndex = 19;
            this.torqueLevelBox.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(33, 73);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(102, 13);
            this.label26.TabIndex = 18;
            this.label26.Text = "Torque Percentage:";
            // 
            // rpmLevelBox
            // 
            this.rpmLevelBox.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.rpmLevelBox.Location = new System.Drawing.Point(148, 33);
            this.rpmLevelBox.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.rpmLevelBox.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.rpmLevelBox.Name = "rpmLevelBox";
            this.rpmLevelBox.Size = new System.Drawing.Size(103, 20);
            this.rpmLevelBox.TabIndex = 17;
            this.rpmLevelBox.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(33, 35);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(61, 13);
            this.label33.TabIndex = 16;
            this.label33.Text = "Rpm Level:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.confirmButton);
            this.panel2.Controls.Add(this.cancelButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 123);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(284, 53);
            this.panel2.TabIndex = 1;
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(161, 14);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(100, 25);
            this.confirmButton.TabIndex = 26;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(24, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 25);
            this.cancelButton.TabIndex = 25;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // TorqueCurveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(284, 176);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TorqueCurveForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Torque Curve Setting";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.torqueLevelBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpmLevelBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label33;
        public System.Windows.Forms.NumericUpDown torqueLevelBox;
        public System.Windows.Forms.NumericUpDown rpmLevelBox;
    }
}