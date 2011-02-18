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

        public static Dictionary<int, MCServer> Servers = new Dictionary<int, MCServer> { };

        private static Timer timUpdate;

        public static void StartUp()
        {
            //Clear ouy old files if they exist, if it doesn't work we'll just do it on next startup.
            try { if (File.Exists(RootFolder + @"\YAMS-Library.dll.OLD")) File.Delete(RootFolder + @"\YAMS-Library.dll.OLD"); }
            catch { };
            try { if (File.Exists(RootFolder + @"\YAMS-Service.exe.OLD")) File.Delete(RootFolder + @"\YAMS-Service.exe.OLD"); }
            catch { };

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
            int UpdateTick = (60 * 60 * 1000);
            if (Database.GetSetting("UpdateBranch", "YAMS") == "dev") UpdateTick = (1 * 60 * 1000);
            timUpdate = new Timer(new TimerCallback(timUpdate_Tick), null, 0, UpdateTick);

            //Load any servers
            SqlCeDataReader readerServers = YAMS.Database.GetServers();
            while (readerServers.Read())
            {
                MCServer myServer = new MCServer(Convert.ToInt32(readerServers["ServerID"]));
                if (Convert.ToBoolean(readerServers["ServerAutostart"])) myServer.Start();
                Servers.Add(Convert.ToInt32(readerServers["ServerID"]), myServer);
            }

            //Start Webserver
            WebServer.Init();
            WebServer.Start();

        }

        public static void ShutDown()
        {
            foreach (KeyValuePair<int, MCServer> kvp in Core.Servers)
            {
                kvp.Value.Stop();
            }
            YAMS.Database.AddLog("Shutting Down");
        }

        public static void timUpdate_Tick(object t)
        {
            AutoUpdate.CheckUpdates();
        }
    }
}
