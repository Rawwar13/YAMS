using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Threading;
using System.IO;
using YAMS;

namespace YAMS
{
    public static class Core
    {
        public static string RootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        public static List<MCServer> Servers = new List<MCServer> { };

        private static Timer timUpdate;

        public static void StartUp()
        {
            //Clear ouy old files if they exist
            if (File.Exists(RootFolder + @"\YAMS-Library.dll.OLD")) File.Delete(RootFolder + @"\YAMS-Library.dll.OLD");
            if (File.Exists(RootFolder + @"\YAMS-Service.exe.OLD")) File.Delete(RootFolder + @"\YAMS-Service.exe.OLD");

            //Start DB Connection
            Database.init();
            Database.AddLog("Starting Up");

            //Is this the first run?
            if (Database.GetSetting("FirstRun", "YAMS") != "true") YAMS.Util.FirstRun();

            //Fill up some vars
            AutoUpdate.bolUpdateAddons = Convert.ToBoolean(Database.GetSetting("UpdateAddons", "YAMS"));
            AutoUpdate.bolUpdateGUI = Convert.ToBoolean(Database.GetSetting("UpdateGUI", "YAMS"));
            AutoUpdate.bolUpdateJAR = Convert.ToBoolean(Database.GetSetting("UpdateJAR", "YAMS"));
            AutoUpdate.bolUpdateSVC = Convert.ToBoolean(Database.GetSetting("UpdateSVC", "YAMS"));
            AutoUpdate.bolUpdateWeb = Convert.ToBoolean(Database.GetSetting("UpdateWeb", "YAMS"));

            //Check for updates and start a timer to do it automatically
            timUpdate = new Timer(new TimerCallback(timUpdate_Tick), null, 0, 5*60*1000);

            //Load any servers
            SqlCeDataReader readerServers = YAMS.Database.GetServers();
            while (readerServers.Read())
            {
                MCServer myServer = new MCServer(Convert.ToInt32(readerServers["ServerID"]));
                if (Convert.ToBoolean(readerServers["ServerAutostart"])) myServer.Start();
                Servers.Add(myServer);
            }

            //Start Webserver
            WebServer.Init();
            WebServer.Start();

        }

        public static void ShutDown()
        {
            Core.Servers.ForEach(delegate(MCServer s)
            {
               s.Stop();
            });
            YAMS.Database.AddLog("Shutting Down");
        }

        public static void timUpdate_Tick(object t)
        {
            AutoUpdate.CheckUpdates();
        }
    }
}
