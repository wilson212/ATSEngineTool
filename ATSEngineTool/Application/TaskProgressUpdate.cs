using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSEngineTool
{
    /// <summary>
    /// This class is used to post updated progress to the TaskForm, allowing
    /// the form's progressbar and text fields to be changed as progress updates.
    /// </summary>
    public class TaskProgressUpdate
    {
        /// <summary>
        /// Sets the Window Title of the TaskForm
        /// </summary>
        public string WindowTitle = String.Empty;

        /// <summary>
        /// Sets the Instruction Text of the TaskForm
        /// </summary>
        public string HeaderText = String.Empty;

        /// <summary>
        /// Sets the Content Message Text of the TaskForm
        /// </summary>
        public string MessageText = String.Empty;

        /// <summary>
        /// Gets or Sets whether the progress bar is set, or incremented from its current value
        /// </summary>
        public bool IncrementProgress = false;

        /// <summary>
        /// Sets the Progress Bar Percentage of the TaskForm
        /// </summary>
        public int ProgressPercent = 0;

        /// <summary>
        /// Creates a new instance of TaskProgressUpdate
        /// </summary>
        public TaskProgressUpdate() { }

        /// <summary>
        /// Creates a new instance of TaskProgressUpdate
        /// </summary>
        /// <param name="MessageText">Sets the content status message of the TaskForm</param>
        public TaskProgressUpdate(string MessageText)
        {
            this.MessageText = MessageText;
        }

        /// <summary>
        /// Creates a new instance of TaskProgressUpdate
        /// </summary>
        /// <param name="ProgressPercent">Sets the progress percentage of the TaskForm's progress bar</param>
        /// <param name="IncrementPercent">
        ///   If set to true, then <paramref name="ProgressPercent"/> is Incremented onto the progressbar's current percentage
        /// </param>
        public TaskProgressUpdate(int ProgressPercent, bool IncrementPercent = false)
        {
            this.IncrementProgress = IncrementPercent;
            this.ProgressPercent = ProgressPercent;
        }

        /// <summary>
        /// Creates a new instance of TaskProgressUpdate
        /// </summary>
        /// <param name="MessageText">Sets the content status message of the TaskForm</param>
        /// <param name="ProgressPercent">Sets the progress percentage of the TaskForm's progress bar</param>
        /// <param name="IncrementPercent">
        ///   If set to true, then <paramref name="ProgressPercent"/> is Incremented onto the progressbar's current percentage
        /// </param>
        public TaskProgressUpdate(string MessageText, int ProgressPercent, bool IncrementPercent = false)
        {
            this.MessageText = MessageText;
            this.IncrementProgress = IncrementPercent;
            this.ProgressPercent = ProgressPercent;
        }

        /// <summary>
        /// Creates a new instance of TaskProgressUpdate
        /// </summary>
        /// <param name="MessageText">Sets the content status message of the TaskForm</param>
        /// <param name="HeaderText">Sets the Instruction (header) text of the TaskForm</param>
        public TaskProgressUpdate(string MessageText, string HeaderText)
        {
            this.HeaderText = HeaderText;
            this.MessageText = MessageText;
        }

        /// <summary>
        /// Creates a new instance of TaskProgressUpdate
        /// </summary>
        /// <param name="MessageText">Sets the content status message of the TaskForm</param>
        /// <param name="HeaderText">Sets the Instruction (header) text of the TaskForm</param>
        /// <param name="ProgressPercent">Sets the progress percentage of the TaskForm's progress bar</param>
        /// <param name="IncrementPercent">
        ///   If set to true, then <paramref name="ProgressPercent"/> is Incremented onto the progressbar's current percentage
        /// </param>
        public TaskProgressUpdate(string MessageText, string HeaderText, int ProgressPercent, bool IncrementPercent = false)
        {
            this.HeaderText = HeaderText;
            this.MessageText = MessageText;
            this.IncrementProgress = IncrementPercent;
            this.ProgressPercent = ProgressPercent;
        }
    }
}
