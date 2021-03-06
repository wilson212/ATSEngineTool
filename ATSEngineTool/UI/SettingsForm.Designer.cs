﻿namespace ATSEngineTool
{
    partial class SettingsForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.iUnitRatio = new System.Windows.Forms.RadioButton();
            this.mUnitRadio = new System.Windows.Forms.RadioButton();
            this.updateCheckBox = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.linkCheckBox = new System.Windows.Forms.CheckBox();
            this.ChangeButton = new System.Windows.Forms.Button();
            this.SteamInstallPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bothRadio = new System.Windows.Forms.RadioButton();
            this.transmissionRadio = new System.Windows.Forms.RadioButton();
            this.engineRadio = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.torqueOutput1 = new System.Windows.Forms.RadioButton();
            this.torqueOutput2 = new System.Windows.Forms.RadioButton();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.shadowLabel1 = new System.Windows.Forms.ShadowLabel();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(-1, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(488, 213);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.updateCheckBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(480, 187);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Application";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.iUnitRatio);
            this.groupBox1.Controls.Add(this.mUnitRadio);
            this.groupBox1.Location = new System.Drawing.Point(260, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Unit System";
            // 
            // iUnitRatio
            // 
            this.iUnitRatio.AutoSize = true;
            this.iUnitRatio.Checked = true;
            this.iUnitRatio.Location = new System.Drawing.Point(29, 34);
            this.iUnitRatio.Name = "iUnitRatio";
            this.iUnitRatio.Size = new System.Drawing.Size(88, 17);
            this.iUnitRatio.TabIndex = 29;
            this.iUnitRatio.TabStop = true;
            this.iUnitRatio.Text = "Imperial Units";
            this.iUnitRatio.UseVisualStyleBackColor = true;
            // 
            // mUnitRadio
            // 
            this.mUnitRadio.AutoSize = true;
            this.mUnitRadio.Location = new System.Drawing.Point(29, 58);
            this.mUnitRadio.Name = "mUnitRadio";
            this.mUnitRadio.Size = new System.Drawing.Size(81, 17);
            this.mUnitRadio.TabIndex = 30;
            this.mUnitRadio.TabStop = true;
            this.mUnitRadio.Text = "Metric Units";
            this.mUnitRadio.UseVisualStyleBackColor = true;
            // 
            // updateCheckBox
            // 
            this.updateCheckBox.AutoSize = true;
            this.updateCheckBox.Checked = true;
            this.updateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateCheckBox.Location = new System.Drawing.Point(28, 49);
            this.updateCheckBox.Name = "updateCheckBox";
            this.updateCheckBox.Size = new System.Drawing.Size(169, 17);
            this.updateCheckBox.TabIndex = 28;
            this.updateCheckBox.Text = "Enable program update check";
            this.updateCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.linkCheckBox);
            this.tabPage2.Controls.Add(this.ChangeButton);
            this.tabPage2.Controls.Add(this.SteamInstallPath);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(480, 187);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Steam Integration";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // linkCheckBox
            // 
            this.linkCheckBox.AutoSize = true;
            this.linkCheckBox.Checked = true;
            this.linkCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.linkCheckBox.Location = new System.Drawing.Point(40, 36);
            this.linkCheckBox.Name = "linkCheckBox";
            this.linkCheckBox.Size = new System.Drawing.Size(344, 17);
            this.linkCheckBox.TabIndex = 27;
            this.linkCheckBox.Text = "Integrate with Real Engines and Sounds Mod (Steam path required)";
            this.linkCheckBox.UseVisualStyleBackColor = true;
            // 
            // ChangeButton
            // 
            this.ChangeButton.Location = new System.Drawing.Point(386, 131);
            this.ChangeButton.Name = "ChangeButton";
            this.ChangeButton.Size = new System.Drawing.Size(66, 25);
            this.ChangeButton.TabIndex = 26;
            this.ChangeButton.Text = "Change";
            this.ChangeButton.UseVisualStyleBackColor = true;
            this.ChangeButton.Click += new System.EventHandler(this.ChangeButton_Click);
            // 
            // SteamInstallPath
            // 
            this.SteamInstallPath.Location = new System.Drawing.Point(28, 105);
            this.SteamInstallPath.Name = "SteamInstallPath";
            this.SteamInstallPath.ReadOnly = true;
            this.SteamInstallPath.Size = new System.Drawing.Size(424, 20);
            this.SteamInstallPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Steam Installation Path:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Controls.Add(this.groupBox3);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(480, 187);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Compile Options";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.bothRadio);
            this.groupBox4.Controls.Add(this.transmissionRadio);
            this.groupBox4.Controls.Add(this.engineRadio);
            this.groupBox4.Location = new System.Drawing.Point(239, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(230, 152);
            this.groupBox4.TabIndex = 34;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Suitable For";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(206, 26);
            this.label2.TabIndex = 32;
            this.label2.Text = "Define if Engines or Transmissins will have\r\nthe suitible_for[] array for each ot" +
    "her.";
            // 
            // bothRadio
            // 
            this.bothRadio.AutoSize = true;
            this.bothRadio.Location = new System.Drawing.Point(18, 121);
            this.bothRadio.Name = "bothRadio";
            this.bothRadio.Size = new System.Drawing.Size(199, 17);
            this.bothRadio.TabIndex = 31;
            this.bothRadio.TabStop = true;
            this.bothRadio.Text = "Both (Unexpected results may occur)";
            this.bothRadio.UseVisualStyleBackColor = true;
            // 
            // transmissionRadio
            // 
            this.transmissionRadio.AutoSize = true;
            this.transmissionRadio.Location = new System.Drawing.Point(18, 75);
            this.transmissionRadio.Name = "transmissionRadio";
            this.transmissionRadio.Size = new System.Drawing.Size(115, 17);
            this.transmissionRadio.TabIndex = 29;
            this.transmissionRadio.Text = "Transmissions Only";
            this.transmissionRadio.UseVisualStyleBackColor = true;
            // 
            // engineRadio
            // 
            this.engineRadio.AutoSize = true;
            this.engineRadio.Location = new System.Drawing.Point(18, 98);
            this.engineRadio.Name = "engineRadio";
            this.engineRadio.Size = new System.Drawing.Size(87, 17);
            this.engineRadio.TabIndex = 30;
            this.engineRadio.TabStop = true;
            this.engineRadio.Text = "Engines Only";
            this.engineRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.torqueOutput1);
            this.groupBox3.Controls.Add(this.torqueOutput2);
            this.groupBox3.Location = new System.Drawing.Point(11, 17);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(222, 92);
            this.groupBox3.TabIndex = 33;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Engine Torque Line";
            // 
            // torqueOutput1
            // 
            this.torqueOutput1.AutoSize = true;
            this.torqueOutput1.Location = new System.Drawing.Point(18, 34);
            this.torqueOutput1.Name = "torqueOutput1";
            this.torqueOutput1.Size = new System.Drawing.Size(193, 17);
            this.torqueOutput1.TabIndex = 29;
            this.torqueOutput1.Text = "Imperial Units (2,050 lb·ft @ x RPM)";
            this.torqueOutput1.UseVisualStyleBackColor = true;
            // 
            // torqueOutput2
            // 
            this.torqueOutput2.AutoSize = true;
            this.torqueOutput2.Location = new System.Drawing.Point(18, 57);
            this.torqueOutput2.Name = "torqueOutput2";
            this.torqueOutput2.Size = new System.Drawing.Size(182, 17);
            this.torqueOutput2.TabIndex = 30;
            this.torqueOutput2.TabStop = true;
            this.torqueOutput2.Text = "Metric Units (2779 Nm @ x RPM)";
            this.torqueOutput2.UseVisualStyleBackColor = true;
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Location = new System.Drawing.Point(263, 13);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(100, 25);
            this.ConfirmButton.TabIndex = 26;
            this.ConfirmButton.Text = "Confirm";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
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
            this.headerPanel.TabIndex = 30;
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
            this.shadowLabel1.Text = "Program Settings";
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.DimGray;
            this.footerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Controls.Add(this.ConfirmButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(0, 287);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(484, 50);
            this.footerPanel.TabIndex = 34;
            this.footerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.footerPanel_Paint);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(119, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 25);
            this.cancelButton.TabIndex = 25;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.tabControl1);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 65);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(484, 222);
            this.contentPanel.TabIndex = 35;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(484, 337);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.TextBox SteamInstallPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ChangeButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel shadowLabel1;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.CheckBox linkCheckBox;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox updateCheckBox;
        private System.Windows.Forms.RadioButton mUnitRadio;
        private System.Windows.Forms.RadioButton iUnitRatio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton torqueOutput1;
        private System.Windows.Forms.RadioButton torqueOutput2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton transmissionRadio;
        private System.Windows.Forms.RadioButton engineRadio;
        private System.Windows.Forms.RadioButton bothRadio;
        private System.Windows.Forms.Label label2;
    }
}