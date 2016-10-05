namespace ATSEngineTool
{
    partial class SoundEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoundEditor));
            this.contentPanel = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.attrType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fileNameBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxLooped = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox2D = new System.Windows.Forms.CheckBox();
            this.volumeBox = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.minRpmBox = new System.Windows.Forms.NumericUpDown();
            this.pitchBox = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.maxRpmBox = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.footerPanel = new System.Windows.Forms.Panel();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.shadowLabel1 = new System.Windows.Forms.ShadowLabel();
            this.contentPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minRpmBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxRpmBox)).BeginInit();
            this.footerPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.groupBox3);
            this.contentPanel.Controls.Add(this.groupBox2);
            this.contentPanel.Controls.Add(this.groupBox1);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 65);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(509, 299);
            this.contentPanel.TabIndex = 35;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.searchButton);
            this.groupBox3.Controls.Add(this.attrType);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.fileNameBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(19, 17);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(471, 105);
            this.groupBox3.TabIndex = 45;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sound Information";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ATSEngineTool.Properties.Resources.question_button;
            this.pictureBox1.Location = new System.Drawing.Point(384, 59);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 32;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, resources.GetString("pictureBox1.ToolTip"));
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(354, 59);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(27, 20);
            this.searchButton.TabIndex = 31;
            this.searchButton.Text = "...";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // attrType
            // 
            this.attrType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.attrType.FormattingEnabled = true;
            this.attrType.Location = new System.Drawing.Point(178, 22);
            this.attrType.Name = "attrType";
            this.attrType.Size = new System.Drawing.Size(203, 21);
            this.attrType.TabIndex = 30;
            this.attrType.SelectedIndexChanged += new System.EventHandler(this.attrType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(86, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Sound Filename:";
            // 
            // fileNameBox
            // 
            this.fileNameBox.Location = new System.Drawing.Point(178, 59);
            this.fileNameBox.Name = "fileNameBox";
            this.fileNameBox.Size = new System.Drawing.Size(170, 20);
            this.fileNameBox.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(89, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Sound Attribute:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.checkBoxLooped);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.checkBox2D);
            this.groupBox2.Controls.Add(this.volumeBox);
            this.groupBox2.Location = new System.Drawing.Point(19, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 150);
            this.groupBox2.TabIndex = 44;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "All Sounds";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Sound Volume:";
            // 
            // checkBoxLooped
            // 
            this.checkBoxLooped.AutoSize = true;
            this.checkBoxLooped.Location = new System.Drawing.Point(64, 79);
            this.checkBoxLooped.Name = "checkBoxLooped";
            this.checkBoxLooped.Size = new System.Drawing.Size(106, 17);
            this.checkBoxLooped.TabIndex = 31;
            this.checkBoxLooped.Text = "Sound is Looped";
            this.checkBoxLooped.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(196, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 36;
            this.label3.Text = "%";
            // 
            // checkBox2D
            // 
            this.checkBox2D.AutoSize = true;
            this.checkBox2D.Location = new System.Drawing.Point(64, 102);
            this.checkBox2D.Name = "checkBox2D";
            this.checkBox2D.Size = new System.Drawing.Size(84, 17);
            this.checkBox2D.TabIndex = 32;
            this.checkBox2D.Text = "Sound is 2D";
            this.checkBox2D.UseVisualStyleBackColor = true;
            // 
            // volumeBox
            // 
            this.volumeBox.Location = new System.Drawing.Point(106, 35);
            this.volumeBox.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.volumeBox.Name = "volumeBox";
            this.volumeBox.Size = new System.Drawing.Size(84, 20);
            this.volumeBox.TabIndex = 35;
            this.volumeBox.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.minRpmBox);
            this.groupBox1.Controls.Add(this.pitchBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.maxRpmBox);
            this.groupBox1.Location = new System.Drawing.Point(260, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 150);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Engine Sounds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Pitch Reference:";
            // 
            // minRpmBox
            // 
            this.minRpmBox.Location = new System.Drawing.Point(120, 114);
            this.minRpmBox.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.minRpmBox.Name = "minRpmBox";
            this.minRpmBox.Size = new System.Drawing.Size(84, 20);
            this.minRpmBox.TabIndex = 42;
            this.minRpmBox.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // pitchBox
            // 
            this.pitchBox.Location = new System.Drawing.Point(120, 35);
            this.pitchBox.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.pitchBox.Name = "pitchBox";
            this.pitchBox.Size = new System.Drawing.Size(84, 20);
            this.pitchBox.TabIndex = 38;
            this.pitchBox.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(60, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "Min RPM:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(57, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Max RPM:";
            // 
            // maxRpmBox
            // 
            this.maxRpmBox.Location = new System.Drawing.Point(120, 75);
            this.maxRpmBox.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.maxRpmBox.Name = "maxRpmBox";
            this.maxRpmBox.Size = new System.Drawing.Size(84, 20);
            this.maxRpmBox.TabIndex = 40;
            this.maxRpmBox.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 2000;
            this.toolTip1.AutoPopDelay = 20000;
            this.toolTip1.InitialDelay = 200;
            this.toolTip1.ReshowDelay = 400;
            this.toolTip1.ShowAlways = true;
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.DimGray;
            this.footerPanel.BackgroundImage = global::ATSEngineTool.Properties.Resources.mainPattern;
            this.footerPanel.Controls.Add(this.confirmButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(0, 364);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(509, 50);
            this.footerPanel.TabIndex = 36;
            this.footerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.footerPanel_Paint);
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(260, 13);
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
            this.cancelButton.Location = new System.Drawing.Point(154, 13);
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
            this.headerPanel.Size = new System.Drawing.Size(509, 65);
            this.headerPanel.TabIndex = 34;
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
            this.shadowLabel1.Text = "Sound Editor";
            // 
            // SoundEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(509, 414);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoundEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SoundEditor";
            this.contentPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minRpmBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxRpmBox)).EndInit();
            this.footerPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fileNameBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ShadowLabel shadowLabel1;
        private System.Windows.Forms.ComboBox attrType;
        private System.Windows.Forms.CheckBox checkBox2D;
        private System.Windows.Forms.CheckBox checkBoxLooped;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown volumeBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown minRpmBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown maxRpmBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown pitchBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}