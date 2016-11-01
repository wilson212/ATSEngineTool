namespace ATSEngineTool
{
    partial class EditGearForm
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gearName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ratioBox = new System.Windows.Forms.NumericUpDown();
            this.labelTorque = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ratioBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.confirmButton);
            this.panel2.Controls.Add(this.cancelButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 142);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(284, 52);
            this.panel2.TabIndex = 3;
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(161, 13);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(100, 25);
            this.confirmButton.TabIndex = 26;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(24, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 25);
            this.cancelButton.TabIndex = 25;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gearName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ratioBox);
            this.panel1.Controls.Add(this.labelTorque);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 142);
            this.panel1.TabIndex = 2;
            // 
            // gearName
            // 
            this.gearName.Location = new System.Drawing.Point(148, 59);
            this.gearName.MaxLength = 5;
            this.gearName.Name = "gearName";
            this.gearName.Size = new System.Drawing.Size(103, 20);
            this.gearName.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Gear Name (optional):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Enter a negative number for reverse gears";
            // 
            // ratioBox
            // 
            this.ratioBox.DecimalPlaces = 2;
            this.ratioBox.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.ratioBox.Location = new System.Drawing.Point(148, 93);
            this.ratioBox.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            131072});
            this.ratioBox.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147418112});
            this.ratioBox.Name = "ratioBox";
            this.ratioBox.Size = new System.Drawing.Size(103, 20);
            this.ratioBox.TabIndex = 19;
            this.ratioBox.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            // 
            // labelTorque
            // 
            this.labelTorque.AutoSize = true;
            this.labelTorque.Location = new System.Drawing.Point(33, 95);
            this.labelTorque.Name = "labelTorque";
            this.labelTorque.Size = new System.Drawing.Size(61, 13);
            this.labelTorque.TabIndex = 18;
            this.labelTorque.Text = "Gear Ratio:";
            // 
            // EditGearForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(284, 194);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditGearForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Gear";
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ratioBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown ratioBox;
        private System.Windows.Forms.Label labelTorque;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox gearName;
        private System.Windows.Forms.Label label2;
    }
}