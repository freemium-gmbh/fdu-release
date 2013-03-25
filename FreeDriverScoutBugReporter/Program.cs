/*
 * This tool was developed mainly as a bug reporter for the freemium system utilities software, 
 * it was designed specifically to interact with the freemium system utilities webservice, 
 * in order to provide the user with a friendly interface for bug submission.
 */

using System;
using System.Windows.Forms;

namespace BugReporter
{
    static class Program
    {
        public static string Version { get; private set; }
        public static string BugStackTrace { get; private set; }
        public static string BugType { get; private set; }
        public static string BugMessage { get; private set; }
        public static string BugTargetSite { get; private set; }
        public static string BugSource { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Version = args[0];
                BugStackTrace = args[1];
                BugType = args[2];
                BugMessage = args[3];
                BugTargetSite = args[4];
                BugSource = args[5];
            }
            catch (IndexOutOfRangeException)
            { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
