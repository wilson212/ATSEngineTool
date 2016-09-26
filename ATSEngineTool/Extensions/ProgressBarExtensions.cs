using System.Windows.Forms;

namespace ATSEngineTool
{
    public static class ProgressBarExtensions
    {
        /// <summary>
        /// Hack method to make the progress bar immediately update to the desired position
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/977278/how-can-i-make-the-progress-bar-update-fast-enough"/>
        /// <param name="progressBar"></param>
        /// <param name="value"></param>
        public static void ValueFast(this ProgressBar progressBar, int value)
        {
            progressBar.Value = value;

            if (value > 0)    // prevent ArgumentException error on value = 0
            {
                progressBar.Value = value - 1;
                progressBar.Value = value;
            }

        }
    }
}
