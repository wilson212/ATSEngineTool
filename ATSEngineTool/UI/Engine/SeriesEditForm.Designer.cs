namespace ATSEngineTool
{
    partial class SeriesEditForm
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
            this.contentPanel = new System.Windows.Forms.Panel();
            this.soundBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.displacementBox = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.seriesNameBox = new System.Windows.Forms.TextBox();
            this.iconBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.manuNameBox = new System.Windows.Forms.TextBox();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.shadowLabel1 = new System.Windows.Forms.ShadowLabel();
            this.engineIcon = new System.Windows.Forms.PictureBox();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displacementBox)).BeginInit();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.engineIcon)).BeginInit();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.soundBox);
            this.contentPanel.Controls.Add(this.label3);
            this.contentPanel.Controls.Add(this.displacementBox);
            this.contentPanel.Controls.Add(this.label4);
            this.contentPanel.Controls.Add(this.label2);
            this.contentPanel.Controls.Add(this.seriesNameBox);
            this.contentPanel.Controls.Add(this.iconBox);
            this.contentPanel.Controls.Add(this.label10);
            this.contentPanel.Controls.Add(this.label1);
            this.contentPanel.Controls.Add(this.manuNameBox);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 75);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(434, 237);
            this.contentPanel.TabIndex = 35;
            // 
            // soundBox
            // 
            this.soundBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.soundBox.FormattingEnabled = true;
            this.soundBox.ItemHeight = 13;
            this.soundBox.Location = new System.Drawing.Point(194, 191);
            this.soundBox.Name = "soundBox";
            this.soundBox.Size = new System.Drawing.Size(203, 21);
            this.soundBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "Truck Sound Package:";
            // 
            // displacementBox
            // 
            this.displacementBox.DecimalPlaces = 1;
            this.displacementBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.displacementBox.Location = new System.Drawing.Point(194, 152);
            this.displacementBox.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            65536});
            this.displacementBox.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            65536});
            this.displacementBox.Name = "displacementBox";
            this.displacementBox.Size = new System.Drawing.Size(103, 20);
            this.displacementBox.TabIndex = 4;
            this.displacementBox.Value = new decimal(new int[] {
            129,
            0,
            0,
            65536});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Displacement (Litres):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(74, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Series Name:    ";
            // 
            // seriesNameBox
            // 
            this.seriesNameBox.Location = new System.Drawing.Point(194, 113);
            this.seriesNameBox.Name = "seriesNameBox";
            this.seriesNameBox.Size = new System.Drawing.Size(203, 20);
            this.seriesNameBox.TabIndex = 3;
            // 
            // iconBox
            // 
            this.iconBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.iconBox.FormattingEnabled = true;
            this.iconBox.ItemHeight = 13;
            this.iconBox.Location = new System.Drawing.Point(194, 34);
            this.iconBox.Name = "iconBox";
            this.iconBox.Size = new System.Drawing.Size(203, 21);
            this.iconBox.TabIndex = 1;
            this.iconBox.SelectedIndexChanged += new System.EventHandler(this.iconBox_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(89, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 31;
            this.label10.Text = "Engine Icon:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Manufacturer:    ";
            // 
            // manuNameBox
            // 
            this.manuNameBox.Location = new System.Drawing.Point(194, 74);
            this.manuNameBox.Name = "manuNameBox";
            this.manuNameBox.Size = new System.Drawing.Size(203, 20);
            this.manuNameBox.TabIndex = 2;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.DimGray;
            this.headerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.headerPanel.Controls.Add(this.shadowLabel1);
            this.headerPanel.Controls.Add(this.engineIcon);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(434, 75);
            this.headerPanel.TabIndex = 34;
            this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.headerPanel_Paint);
            // 
            // shadowLabel1
            // 
            this.shadowLabel1.BackColor = System.Drawing.Color.Transparent;
            this.shadowLabel1.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shadowLabel1.ForeColor = System.Drawing.SystemColors.Control;
            this.shadowLabel1.Location = new System.Drawing.Point(111, 18);
            this.shadowLabel1.Name = "shadowLabel1";
            this.shadowLabel1.ShadowDirection = 90;
            this.shadowLabel1.ShadowOpacity = 225;
            this.shadowLabel1.ShadowSoftness = 3F;
            this.shadowLabel1.Size = new System.Drawing.Size(297, 39);
            this.shadowLabel1.TabIndex = 0;
            this.shadowLabel1.Text = "New Engine Series";
            // 
            // engineIcon
            // 
            this.engineIcon.BackColor = System.Drawing.Color.Transparent;
            this.engineIcon.Location = new System.Drawing.Point(26, 5);
            this.engineIcon.Name = "engineIcon";
            this.engineIcon.Size = new System.Drawing.Size(64, 64);
            this.engineIcon.TabIndex = 32;
            this.engineIcon.TabStop = false;
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.DimGray;
            this.footerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.footerPanel.Controls.Add(this.confirmButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(0, 312);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(434, 50);
            this.footerPanel.TabIndex = 36;
            this.footerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.footerPanel_Paint);
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(236, 13);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(100, 25);
            this.confirmButton.TabIndex = 6;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(99, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 25);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // SeriesEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(434, 362);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.footerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SeriesEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Engine Series Editor";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displacementBox)).EndInit();
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.engineIcon)).EndInit();
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ShadowLabel shadowLabel1;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox manuNameBox;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox engineIcon;
        private System.Windows.Forms.ComboBox iconBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox seriesNameBox;
        private System.Windows.Forms.NumericUpDown displacementBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox soundBox;
        private System.Windows.Forms.Label label3;
    }
}