using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ATSEngineTool
{
    /// <summary>
    /// A simple object to handle exceptions thrown during runtime
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Handles an exception on the main thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="t"></param>
        public static void OnThreadException(object sender, ThreadExceptionEventArgs t)
        {
            // Create Trace Log
            string FileName = GenerateFileName();
            try
            {
                // Try to generate a trace log
                GenerateExceptionLog(FileName, t.Exception);
            }
            catch { }

            // Display the Exception Form
            using (ExceptionForm EForm = new ExceptionForm(t.Exception, true))
            {
                EForm.Message = "An unhandled exception was thrown while trying to preform the requested task.\r\n"
                    + "If you click Continue, the application will attempt to ignore this error, and continue. "
                    + "If you click Quit, the application will close immediatly.";
                EForm.TraceLog = FileName;
                DialogResult Result = EForm.ShowDialog();

                // Kill the form on abort
                if (Result == DialogResult.Abort)
                    Application.Exit();
            }
        }

        /// <summary>
        /// Handles cross thread exceptions, that are unrecoverable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Create Trace Log
            string FileName = GenerateFileName();
            Exception Ex = e.ExceptionObject as Exception;
            using (ExceptionForm EForm = new ExceptionForm(Ex, false))
            {
                try
                {
                    // Try to generate a trace log
                    GenerateExceptionLog(FileName, Ex);

                    // Display the Exception Form
                    EForm.Message = "An unhandled exception was thrown while trying to preform the requested task.\r\n"
                        + "A trace log was generated under the \"<Editor Path>/errors\" folder, to "
                        + "assist with debugging, and getting help with this error.";
                    EForm.TraceLog = FileName;
                }
                catch (Exception ex)
                {
                    EForm.Message = "An unhandled exception was thrown while trying to preform the requested task.\r\n"
                        + "A trace log was unable to be generated because that threw another exception :(. "
                        + Environment.NewLine + Environment.NewLine
                        + "Message: " + ex.Message
                        ;
                }
                finally
                {
                    EForm.ShowDialog();
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// Generates a trace log for an exception. If an exception is thrown here, The error
        /// will automatically be logged in the programs error log
        /// </summary>
        /// <param name="E">The exception we are logging</param>
        public static void GenerateExceptionLog(Exception E)
        {
            string FileName = GenerateFileName();
            GenerateExceptionLog(FileName, E);
        }

        /// <summary>
        /// Generates a trace log for an exception. If an exception is thrown here, The error
        /// will automatically be logged in the programs error log
        /// </summary>
        /// <param name="E">The exception we are logging</param>
        /// <param name="FileName">Provides the full file path and name where this exception log is created at.</param>
        public static void GenerateExceptionLog(Exception E, out string FileName)
        {
            FileName = GenerateFileName();
            GenerateExceptionLog(FileName, E);
        }

        /// <summary>
        /// Generates a trace log for an exception. If an exception is thrown here, The error
        /// will automatically be logged in the programs error log
        /// </summary>
        /// <param name="FileName">The tracelog filepath (Must not exist yet)</param>
        /// <param name="E">The exception to log</param>
        public static void GenerateExceptionLog(string FileName, Exception E)
        {
            // Try to write to the log
            try
            {
                // Generate the tracelog
                using (StreamWriter Log = new StreamWriter(File.Open(FileName, FileMode.Create, FileAccess.Write)))
                {
                    // Write the header data
                    Log.WriteLine("-------- ATSEngineTool Exception Trace Log --------");
                    Log.WriteLine("Exception Date: " + DateTime.Now.ToString());
                    Log.WriteLine("Program Version: " + Program.Version.ToString());
                    Log.WriteLine("Os Version: " + Environment.OSVersion.VersionString);
                    Log.WriteLine("Architecture: " + ((Environment.Is64BitOperatingSystem) ? "x64" : "x86"));
                    Log.WriteLine("RunAs Admin: " + ((Program.IsAdministrator) ? "True" : "False"));
                    Log.WriteLine();
                    Log.WriteLine("-------- Exception --------");

                    // Log each exception
                    int i = 0;
                    while (true)
                    {
                        // Create a stack trace
                        StackTrace trace = new StackTrace(E, true);
                        StackFrame frame = trace.GetFrame(0);

                        // Log the current exception
                        Log.WriteLine("Type: " + E.GetType().FullName);
                        Log.WriteLine("Message: " + E.Message.Replace("\n", "\n\t"));
                        Log.WriteLine("Target Method: " + frame.GetMethod().Name);
                        Log.WriteLine("File: " + frame.GetFileName());
                        Log.WriteLine("Line: " + frame.GetFileLineNumber());
                        Log.WriteLine("StackTrace:");
                        Log.WriteLine(E.StackTrace.TrimEnd());

                        // If we have no more inner exceptions, end the logging
                        if (E.InnerException == null)
                            break;

                        // Prepare next inner exception data
                        Log.WriteLine();
                        Log.WriteLine("-------- Inner Exception ({0}) --------", i++);
                        E = E.InnerException;
                    }

                    Log.Flush();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generates a filename to be used for logging
        /// </summary>
        /// <returns></returns>
        private static string GenerateFileName()
        {
            // Ensure the errors folder is created
            string folder = Path.Combine(Program.RootPath, "errors");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Create initial filepath
            string dateFormat = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = Path.Combine(folder, "ExceptionLog_" + dateFormat + ".txt");

            // If the file already exists, then we try to create a duplicate with a numerical extension "_(1)"
            if (File.Exists(filePath))
            {
                filePath = Path.Combine(folder, "ExceptionLog_" + dateFormat + "_({0}).txt");

                // We will only try and create up to 3 exceptions during this timestamp
                int maxIndex = Enumerable.Range(1, 3)
                    .SkipWhile(x => File.Exists(String.Format(filePath, x)))
                    .FirstOrDefault();

                filePath = String.Format(filePath, maxIndex);
            }

            return filePath;
        }
    }
}
