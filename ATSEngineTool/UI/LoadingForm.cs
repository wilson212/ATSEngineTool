using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public partial class LoadingForm : Form
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        /// <summary>
        /// Gets or Sets whether the User is able to move the window
        /// </summary>
        protected bool AllowDrag;

        /// <summary>
        /// Our isntance of the update form
        /// </summary>
        private static LoadingForm Instance;

        /// <summary>
        /// Main calling method. Opens a new instance of the form, and displays it
        /// </summary>
        /// <param name="Parent">The parent form, that will be used to center this form over</param>
        /// <param name="AllowDrag">Sets whether this window will be allowed to be moved by the user</param>
        /// <param name="WindowTitle">The text in the window title bar</param>
        public static void ShowScreen(Form Parent, bool AllowDrag = false, string WindowTitle = "Loading... Please Wait")
        {
            // Make sure it is currently not open and running.
            if (Instance != null && !Instance.IsDisposed)
                return;

            // Set window position to center parent
            Instance = new LoadingForm();
            Instance.Text = WindowTitle;
            Instance.AllowDrag = AllowDrag;
            double H = Parent.Location.Y + (Parent.Height / 2) - (Instance.Height / 2);
            double W = Parent.Location.X + (Parent.Width / 2) - (Instance.Width / 2);
            Instance.Location = new Point((int)Math.Round(W, 0), (int)Math.Round(H, 0));

            // Display the Instanced Form
            Instance.Show(Parent);

            // Wait until the Instance form is displayed
            while (!Instance.IsHandleCreated) Thread.Sleep(50);
        }

        /// <summary>
        /// Method called to close the update form
        /// </summary>
        public static void CloseForm()
        {
            // No exception here
            if (Instance == null || Instance.IsDisposed)
                return;

            try
            {
                Instance.Invoke((Action)delegate
                {
                    Instance.Close();
                    Instance = null;
                });
            }
            catch { }
        }

        private LoadingForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prevents the form from being dragable
        /// </summary>
        /// <param name="message"></param>
        protected override void WndProc(ref Message message)
        {
            if (!AllowDrag)
            {
                switch (message.Msg)
                {
                    case WM_SYSCOMMAND:
                        int command = message.WParam.ToInt32() & 0xfff0;
                        if (command == SC_MOVE)
                            return;
                        break;
                }
            }

            base.WndProc(ref message);
        }
    }
}
