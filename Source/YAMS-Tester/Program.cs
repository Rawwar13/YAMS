using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YAMS;

namespace YAMS_Gui
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            YAMS.Database.init();
            YAMS.Database.AddLog("tester started");

        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
