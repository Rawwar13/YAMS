using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace YAMS
{
    public static class Core
    {
        public static string RootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        public static List<MCServer> Servers = new List<MCServer> { };

        public static void StartUp()
        {
            //Start DB Connection
            YAMS.Database.init();

            //Is this the first run?
            if (YAMS.Database.GetSetting("FirstRun", "YAMS") != "true") YAMS.Util.FirstRun();

            //Check for updates
            YAMS.AutoUpdate.CheckUpdates();

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
    }
}
