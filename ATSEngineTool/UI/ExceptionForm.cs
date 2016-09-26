using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ATSEngineTool
{
    public partial class ExceptionForm : Form
    {
        const int WS_SYSMENU = 0x80000;

        /// <summary>
        /// Gets or sets whether the details are hidden or shown when the form is displayed
        /// </summary>
        public bool DetailsExpanded
        {
            get { return panelDetails.Visible; }
            set
            {
                if (value == true)
                    panelDetails.Show();
                else
                    panelDetails.Hide();
            }
        }

        /// <summary>
        /// Gets or sets the Header text of the window
        /// </summary>
        public string HeaderText
        {
            get { return InstructionText.Text; }
            set { InstructionText.Text = value; }
        }

        /// <summary>
        /// Gets or sets the message to be displayed in the form
        /// </summary>
        public string Message
        {
            get { return labelContent.Text; }
            set { labelContent.Text = value; }
        }

        /// <summary>
        /// Gets or Sets the window title
        /// </summary>
        public string WindowTitle
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        /// <summary>
        /// Gets or sets the Error Icon
        /// </summary>
        public Bitmap ImgIcon
        {
            get { return ErrorIcon.Image as Bitmap; }
            set { ErrorIcon.Image = value; }
        }

        /// <summary>
        /// The exception
        /// </summary>
        protected Exception ExceptionObj;

        protected string _logFile = "";
        /// <summary>
        /// Full path to the generated trace file
        /// </summary>
        public string TraceLog
        {
            get { return _logFile; }
            set
            {
                this._logFile = value;
                if (!String.IsNullOrWhiteSpace(value))
                    buttonViewLog.Show();
                else
                    buttonViewLog.Hide();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="E">The exception object</param>
        /// <param name="Recoverable">Defines if the exception is recoverable</param>
        public ExceptionForm(Exception E, bool Recoverable)
        {
            InitializeComponent();

            // Hide details at the start
            panelDetails.Hide();
            ExceptionDetails.Text = "StackTrace: " + Environment.NewLine + E.StackTrace;
            ExceptionObj = E;

            // Preform form layout based on recoverability
            if (!Recoverable)
            {
                buttonContinue.Hide();
                buttonViewLog.Location = new Point(225, 12);
            }

            // Reset log button
            TraceLog = _logFile;
        }

        new public DialogResult ShowDialog()
        {
            // Append Exception Message
            labelContent.Text = String.Concat(labelContent.Text, Environment.NewLine, Environment.NewLine, ExceptionObj.Message);
            return base.ShowDialog();
        }

        new public DialogResult ShowDialog(IWin32Window owner)
        {
            // Append Exception Message
            labelContent.Text = String.Concat(labelContent.Text, Environment.NewLine, Environment.NewLine, ExceptionObj.Message);
            return base.ShowDialog(owner);
        }

        public DialogResult ShowDialog(bool AppendExceptionMessage)
        {
            if (AppendExceptionMessage)
                return this.ShowDialog();
            else
                return base.ShowDialog();
        }

        /// <summary>
        /// Expands / Hides the exception details panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDetails_Click(object sender, EventArgs e)
        {
            DetailsExpanded = !panelDetails.Visible;
        }

        /// <summary>
        /// Opens the Trace Log in a text editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonViewLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(_logFile))
                Process.Start(_logFile);
            else
                MessageBox.Show("The program was unable to create the debug log!", "Error");
        }

        /// <summary>
        /// Hides the Close, Minimize, and Maximize buttons
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }
    }
}
