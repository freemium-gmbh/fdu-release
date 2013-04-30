using System;
using System.Diagnostics;

namespace FreeDriverScout
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            WindowsFormsApp wrapper = new WindowsFormsApp();
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            wrapper.Run(args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
