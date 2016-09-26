using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATSEngineTool
{
    public partial class TaskForm : Form
    {
        /// <summary>
        /// Gets or Sets whether the task is cancelable
        /// </summary>
        protected bool Cancelable = true;

        /// <summary>
        /// Returns whether the Task form is already open and running
        /// </summary>
        /// <returns></returns>
        public static bool IsOpen
        {
            get { return (Instance != null && !Instance.IsDisposed && Instance.IsHandleCreated); }
        }

        /// <summary>
        /// The task dialog's instance
        /// </summary>
        private static TaskForm Instance;

        /// <summary>
        /// A progress object that is used to report progress to the TaskForm and update the GUI
        /// </summary>
        public static IProgress<TaskProgressUpdate> Progress;

        /// <summary>
        /// The event that is fired when the Cancel button is pressed
        /// </summary>
        public static event CancelEventHandler Cancelled;

        /// <summary>
        /// Private constructor... Use the Show() method rather
        /// </summary>
        private TaskForm()
        {
            InitializeComponent();

            // Create our progress updater. We put this inside the form constructor 
            // to capture the Synchronization Context of this thread
            Progress = new SyncProgress<TaskProgressUpdate>(e => ProgressChanged(e));
        }

        /// <summary>
        /// Updates the TaskForm controls after a Progress.Report() has been recieved
        /// </summary>
        /// <param name="e">The TaskProgressUpdate object with the update information</param>
        private void ProgressChanged(TaskProgressUpdate e)
        {
            // Prevent null exception
            if (!IsOpen) return;

            // Wrap this in an invoke t
            if (e.HeaderText.Length > 0)
                labelInstructionText.Text = e.HeaderText;

            // Update message
            if (e.MessageText.Length > 0)
                labelContent.Text = e.MessageText;

            // Update window title
            if (e.WindowTitle.Length > 0)
                Text = e.WindowTitle;

            // Only increment progress bar if the style is not marguee, and we have progress
            if (progressBar.Style != ProgressBarStyle.Marquee && e.ProgressPercent > 0)
            {
                if (e.IncrementProgress)
                    progressBar.Increment(e.ProgressPercent);
                else 
                    progressBar.ValueFast(e.ProgressPercent);

                progressBar.Update();
            }
        }

        /// <summary>
        /// Open and displays the task form.
        /// </summary>
        /// <param name="Parent">The calling form, so the task form can be centered</param>
        /// <param name="WindowTitle">The task dialog window title</param>
        /// <param name="InstructionText">Instruction text displayed after the info icon. Leave null
        /// to hide the instruction text and icon.</param>
        /// <param name="Style">The progress bar style</param>
        /// <exception cref="Exception">Thrown if the Task form is already open and running. Use the IsOpen property
        /// to determine if the form is already running</exception>
        public static void Show(Form Parent, string WindowTitle, string InstructionText, ProgressBarStyle Style, int Steps)
        {
            Show(Parent, WindowTitle, InstructionText, null, true, Style, Steps);
        }

        /// <summary>
        /// Open and displays the task form.
        /// </summary>
        /// <param name="Parent">The calling form, so the task form can be centered</param>
        /// <param name="WindowTitle">The task dialog window title</param>
        /// <param name="InstructionText">Instruction text displayed after the info icon. Leave null
        /// to hide the instruction text and icon.</param>
        /// <param name="Cancelable">Specifies whether the operation can be canceled</param>
        /// <param name="Style">The progress bar style</param>
        /// <exception cref="Exception">Thrown if the Task form is already open and running. Use the IsOpen property
        /// to determine if the form is already running</exception>
        public static void Show(Form Parent, string WindowTitle, string InstructionText, bool Cancelable, ProgressBarStyle Style, int Steps)
        {
            Show(Parent, WindowTitle, InstructionText, null, Cancelable, Style, Steps);
        }

        /// <summary>
        /// Open and displays the task form.
        /// </summary>
        /// <param name="Parent">The calling form, so the task form can be centered</param>
        /// <param name="WindowTitle">The task dialog window title</param>
        /// <param name="InstructionText">Instruction text displayed after the info icon. Leave null
        /// to hide the instruction text and icon.</param>
        /// <param name="Cancelable">Specifies whether the operation can be canceled</param>
        /// <exception cref="Exception">Thrown if the Task form is already open and running. Use the IsOpen property
        /// to determine if the form is already running</exception>
        public static void Show(Form Parent, string WindowTitle, string InstructionText, bool Cancelable)
        {
            Show(Parent, WindowTitle, InstructionText, null, Cancelable, ProgressBarStyle.Marquee, 0);
        }

        /// <summary>
        /// Open and displays the task form.
        /// </summary>
        /// <param name="Parent">The calling form, so the task form can be centered</param>
        /// <param name="WindowTitle">The task dialog window title</param>
        /// <param name="InstructionText">Instruction text displayed after the info icon. Leave null
        /// to hide the instruction text and icon.</param>
        /// <param name="SubMessage">Detail text that is displayed just above the progress bar</param>
        /// <param name="Cancelable">Specifies whether the operation can be canceled</param>
        /// <param name="Style">The progress bar style</param>
        /// <exception cref="Exception">Thrown if the Task form is already open and running. Use the IsOpen property
        /// to determine if the form is already running</exception>
        public static void Show(Form Parent, string WindowTitle, string InstructionText, string SubMessage, bool Cancelable, ProgressBarStyle Style, int ProgressBarSteps)
        {
            // Make sure we dont have an already active form
            if (Instance != null && !Instance.IsDisposed)
                throw new Exception("Task Form is already being displayed!");

            // Create new instance
            Instance = new TaskForm();
            Instance.Text = WindowTitle;
            Instance.labelInstructionText.Text = InstructionText;
            Instance.labelContent.Text = SubMessage;
            Instance.Cancelable = Cancelable;
            Instance.progressBar.Style = Style;

            // Setup progress bar
            if (ProgressBarSteps > 0)
                Instance.progressBar.Maximum = ProgressBarSteps;

            // Hide Instruction panel if Instruction Text is empty
            if (String.IsNullOrWhiteSpace(InstructionText))
            {
                Instance.panelMain.Hide();
                Instance.labelContent.Location = new Point(10, 15);
                Instance.labelContent.MaximumSize = new Size(410, 0);
                Instance.labelContent.Size = new Size(410, 0);
                Instance.progressBar.Location = new Point(10, 1);
                Instance.progressBar.Size = new Size(410, 18);
            }

            // Hide Cancel
            if (!Cancelable)
            {
                Instance.panelButton.Hide();
                Instance.Padding = new Padding(0, 0, 0, 15);
                Instance.BackColor = Color.White;
            }

            // Set window position to center parent
            double H = Parent.Location.Y + (Parent.Height / 2) - (Instance.Height / 2);
            double W = Parent.Location.X + (Parent.Width / 2) - (Instance.Width / 2);
            Instance.Location = new Point((int)Math.Round(W, 0), (int)Math.Round(H, 0));

            // Display the Instanced Form
            Instance.Show(Parent);

            // Wait until the Instance form is displayed
            while (!Instance.IsHandleCreated) Thread.Sleep(50);
        }

        /// <summary>
        /// Closes the Task dialog, and clears the Cancelled event handle subscriptions
        /// </summary>
        public static void CloseForm()
        {
            // No exception here
            if (Instance == null || Instance.IsDisposed)
                return;

            // Remove all cancellation subs
            if (Cancelled != null)
                Cancelled = (CancelEventHandler) Delegate.RemoveAll(Cancelled, Cancelled);

            try
            {
                Instance.Invoke((Action)delegate()
                {
                    Instance.Close();
                    Instance = null;
                });
            }
            catch { }
        }

        #region Non Static

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            // Call cancel event
            if (Cancelled != null)
                Cancelled(this, null);
        }

        private void TaskForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Instance = null;
        }

        #endregion
    }
}
