using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Zartex
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static Main main;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            main = new Main();
            Application.Run(main);
        }
    }
}
