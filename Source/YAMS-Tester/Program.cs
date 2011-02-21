using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YAMS;

namespace YAMS_Gui
{
    public static class Program
    {
        public static YAMS.MCServer myServer;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //YAMS.Core.StartUp();
            Database.init();
            Util.FirstRun();
            WebServer.Init();
            WebServer.StartPublic();
            WebServer.StartAdmin();
        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
