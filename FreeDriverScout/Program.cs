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
            wrapper.Run(args);
        }

      
    }
}
