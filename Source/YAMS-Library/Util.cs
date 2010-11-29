using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;

namespace YAMS
{
    public static class Util
    {
        private static string strRootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        private static string strJRERegKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
        private static string strJDKRegKey = "SOFTWARE\\JavaSoft\\Java Development Kit";

        //Check for the existence to the two JVMs
        public static bool HasJRE()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey(strJRERegKey);
                if (subKey != null) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool HasJDK()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey(strJDKRegKey);
                if (subKey != null) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        
        }

        //Get the Java version from the registry
        public static string JavaVersion(string strType = "jre")
        {
            string strKey = "";
            switch (strType)
            {
                case "jre":
                    strKey = strJRERegKey;
                    break;
                case "jdk":
                    strKey = strJDKRegKey;
                    break;
            }
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey subKey = rk.OpenSubKey(strKey);
            if (subKey != null) return subKey.GetValue("CurrentVersion").ToString();
            else return "";
        }

        //Calculate the path to the Java executable
        public static string JavaPath(string strType = "jre")
        {
            string strKey = "";
            switch (strType)
            {
                case "jre":
                    strKey = strJRERegKey;
                    break;
                case "jdk":
                    strKey = strJDKRegKey;
                    break;
            }
            strKey += "\\" + JavaVersion(strType);
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey subKey = rk.OpenSubKey(strKey);
            if (subKey != null) return subKey.GetValue("JavaHome").ToString() + "\\bin\\";
            else return "";
        }

        //Replaces file 1 with file 2
        public static bool ReplaceFile(string strFileOriginal, string strFileReplacement) {
            try
            {
                if (File.Exists(strFileReplacement))
                {
                    if (File.Exists(strFileOriginal)) File.Delete(strFileOriginal);
                    File.Move(strFileReplacement, strFileOriginal);
                }
                return true;
           }
            catch {
                YAMS.Database.AddLog("Unable to update " + strFileOriginal, "updater", "error");
                return false;
            }
        }

        //Initial Set-up for first run only
        public static void FirstRun()
        {
            //Create directory structure
            if (!Directory.Exists(strRootFolder + @"\config\")) Directory.CreateDirectory(strRootFolder + @"\config\");
            if (!Directory.Exists(strRootFolder + @"\lib\")) Directory.CreateDirectory(strRootFolder + @"\lib\");
            if (!Directory.Exists(strRootFolder + @"\worlds\")) Directory.CreateDirectory(strRootFolder + @"\worlds\");

            //Create default config files
            YAMS.Database.BuildServerProperties();
            if (!File.Exists(strRootFolder + @"\config\banned-ips.txt")) File.Create(strRootFolder + @"\config\banned-ips.txt");
            if (!File.Exists(strRootFolder + @"\config\banned-players.txt")) File.Create(strRootFolder + @"\config\banned-players.txt");
            if (!File.Exists(strRootFolder + @"\config\ops.txt")) File.Create(strRootFolder + @"\config\ops.txt");

            //Grab latest server jar
            YAMS.AutoUpdate.UpdateIfNeeded(YAMS.AutoUpdate.strMCServerURL, strRootFolder + @"\lib\minecraft_server.jar.UPDATE");

            //Set our MC Defaults in the DB
            YAMS.Database.SaveSetting("admin-slot", "true", "MC");
            YAMS.Database.SaveSetting("enable-health", "true", "MC");
            YAMS.Database.SaveSetting("grow-trees", "true", "MC");
            YAMS.Database.SaveSetting("hellworld", "false", "MC");
            YAMS.Database.SaveSetting("level-name", @"..\\worlds\\world1", "MC");
            YAMS.Database.SaveSetting("max-connections", "1", "MC");
            YAMS.Database.SaveSetting("max-players", "20", "MC");
            YAMS.Database.SaveSetting("motd", "Welcome to a YAMS server!", "MC");
            YAMS.Database.SaveSetting("online-mode", "true", "MC");
            YAMS.Database.SaveSetting("public", "false", "MC");
            YAMS.Database.SaveSetting("pvp", "true", "MC");
            YAMS.Database.SaveSetting("server-ip", "127.0.0.1", "MC");
            YAMS.Database.SaveSetting("server-name", "My YAMS MC Server", "MC");
            YAMS.Database.SaveSetting("server-port", "56552", "MC"); //Use an IANA legal internal port 49152 - 65535
            YAMS.Database.SaveSetting("spawn-animals", "true", "MC");
            YAMS.Database.SaveSetting("spawn-monsters", "true", "MC");
            YAMS.Database.SaveSetting("verify-names", "true", "MC");

            //Set our YAMS Defaults
            YAMS.Database.SaveSetting("UpdateJAR", "true", "YAMS");
            YAMS.Database.SaveSetting("UpdateSVC", "false", "YAMS");
            YAMS.Database.SaveSetting("UpdateGUI", "true", "YAMS");
            YAMS.Database.SaveSetting("UpdateWeb", "true", "YAMS");
            YAMS.Database.SaveSetting("UpdateAddons", "true", "YAMS");
            YAMS.Database.SaveSetting("RestartOnJarUpdate", "false", "YAMS");
            YAMS.Database.SaveSetting("RestartOnSVCUpdate", "false", "YAMS");
            YAMS.Database.SaveSetting("Memory", "1024", "YAMS");
            YAMS.Database.SaveSetting("EnableJavaOptimisations", "true", "YAMS");
            YAMS.Database.SaveSetting("ListenPort", "25565", "YAMS");
            YAMS.Database.SaveSetting("ExternalIP", GetExternalIP().ToString(), "YAMS");
            YAMS.Database.SaveSetting("ListenIP", GetListenIP().ToString(), "YAMS");


            //Tell the DB that we've run this
            YAMS.Database.SaveSetting("FirstRun", "true", "YAMS");

        }

        //Uses WhatIsMyIP.com to determine the user's external IP
        public static IPAddress GetExternalIP()
        {
            string strExternalIPChecker = "http://whatismyip.com";
            Regex regGetIP = new Regex(@"(?<=<TITLE>.*)\d*\.\d*\.\d*\.\d*(?=</TITLE>)");
            WebClient wcGetIP = new WebClient();
            UTF8Encoding utf8 = new UTF8Encoding();
            string strResponse = "";
            try
            {
                strResponse = utf8.GetString(wcGetIP.DownloadData(strExternalIPChecker));
            }
            catch (WebException e)
            {
                YAMS.Database.AddLog("Unable to determine external IP: " + e.Data, "utils", "warn");
            }

            Match regMatch = regGetIP.Match(strResponse);
            IPAddress ipExternal = null;
            if (regMatch.Success) ipExternal = IPAddress.Parse(regMatch.Value);
            return ipExternal;
        }

        //Get an IP Address to bind to
        public static IPAddress GetListenIP()
        {
            IPHostEntry ipListen = Dns.GetHostEntry(Dns.GetHostName());
            return ipListen.AddressList[0];
        }
    }
}
