using System.IO;
using Microsoft.Win32;

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

            //Tell the DB that we've run this
            YAMS.Database.SaveSetting("FirstRun", "true", "YAMS");

        }
    }
}
