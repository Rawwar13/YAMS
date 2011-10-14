using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YAMS;
using System.Reflection;
using System.IO;
using System.Data.SqlServerCe;

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

            //Start DB Connection
            Database.init();
            Database.AddLog("Starting Up");

            //Is this the first run?
            if (Database.GetSetting("FirstRun", "YAMS") != "true") YAMS.Util.FirstRun();
            Database.SaveSetting("AdminPassword", "password");

            SqlCeDataReader readerServers = YAMS.Database.GetServers();
            while (readerServers.Read())
            {
                Database.AddLog("Starting Server " + readerServers["ServerID"]);
                MCServer myServer = new MCServer(Convert.ToInt32(readerServers["ServerID"]));
                if (Convert.ToBoolean(readerServers["ServerAutostart"])) myServer.Start();
                Core.Servers.Add(Convert.ToInt32(readerServers["ServerID"]), myServer);
            }

            //Start job engine
            JobEngine.Init();

            //Start Webserver
            WebServer.Init();
        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
