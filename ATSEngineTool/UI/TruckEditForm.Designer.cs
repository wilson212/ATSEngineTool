namespace ATSEngineTool
{
    partial class TruckEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TruckEditForm));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.shadowLabel1 = new System.Windows.Forms.ShadowLabel();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.truckSoundPackageBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.truckNameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.unitNameBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.engineSoundPackageBox = new System.Windows.Forms.ComboBox();
            this.headerPanel.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.DimGray;
            this.headerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.shadowLabel1);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(489, 65);
            this.headerPanel.TabIndex = 29;
            this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.headerPanel_Paint);
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
            this.shadowLabel1.Text = "Add New Truck";
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.DimGray;
            this.footerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.footerPanel.Controls.Add(this.confirmButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(0, 372);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(489, 50);
            this.footerPanel.TabIndex = 33;
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(263, 13);
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
            this.cancelButton.Location = new System.Drawing.Point(126, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 25);
            this.cancelButton.TabIndex = 25;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.groupBox2);
            this.contentPanel.Controls.Add(this.groupBox1);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 65);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(489, 307);
            this.contentPanel.TabIndex = 32;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.truckSoundPackageBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.truckNameBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.unitNameBox);
            this.groupBox2.Location = new System.Drawing.Point(19, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(450, 163);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Truck Info";
            // 
            // truckSoundPackageBox
            // 
            this.truckSoundPackageBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.truckSoundPackageBox.FormattingEnabled = true;
            this.truckSoundPackageBox.ItemHeight = 13;
            this.truckSoundPackageBox.Location = new System.Drawing.Point(191, 107);
            this.truckSoundPackageBox.Name = "truckSoundPackageBox";
            this.truckSoundPackageBox.Size = new System.Drawing.Size(221, 21);
            this.truckSoundPackageBox.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Truck Sound Package:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(38, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Truck Name:";
            // 
            // truckNameBox
            // 
            this.truckNameBox.Location = new System.Drawing.Point(191, 34);
            this.truckNameBox.Name = "truckNameBox";
            this.truckNameBox.Size = new System.Drawing.Size(221, 20);
            this.truckNameBox.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Sii Unit Name:";
            // 
            // unitNameBox
            // 
            this.unitNameBox.Location = new System.Drawing.Point(191, 70);
            this.unitNameBox.Name = "unitNameBox";
            this.unitNameBox.Size = new System.Drawing.Size(221, 20);
            this.unitNameBox.TabIndex = 29;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.engineSoundPackageBox);
            this.groupBox1.Location = new System.Drawing.Point(19, 193);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(450, 101);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(10, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(230, 17);
            this.checkBox1.TabIndex = 34;
            this.checkBox1.Text = "Override Default Engine and Truck Sounds";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(38, 44);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(160, 13);
            this.label15.TabIndex = 31;
            this.label15.Text = "Default Engine Sound Package:";
            // 
            // engineSoundPackageBox
            // 
            this.engineSoundPackageBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.engineSoundPackageBox.Enabled = false;
            this.engineSoundPackageBox.FormattingEnabled = true;
            this.engineSoundPackageBox.ItemHeight = 13;
            this.engineSoundPackageBox.Location = new System.Drawing.Point(209, 41);
            this.engineSoundPackageBox.Name = "engineSoundPackageBox";
            this.engineSoundPackageBox.Size = new System.Drawing.Size(203, 21);
            this.engineSoundPackageBox.TabIndex = 30;
            // 
            // TruckEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(489, 422);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TruckEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Truck Editor";
            this.headerPanel.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ShadowLabel shadowLabel1;
        private System.Windows.Forms.Panel headerPanel; 
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox truckNameBox;
        private System.Windows.Forms.TextBox unitNameBox;
        private System.Windows.Forms.ComboBox engineSoundPackageBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox truckSoundPackageBox;
        private System.Windows.Forms.Label label2;
    }
}