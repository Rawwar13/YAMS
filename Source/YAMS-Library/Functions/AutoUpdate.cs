using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace YAMS
{
    public static class AutoUpdate
    {
        //Settings
        public static bool bolUpdateGUI = false;
        public static bool bolUpdateJAR = false;
        public static bool bolUpdateClient = false;
        public static bool bolUpdateAddons = false;
        public static bool bolUpdateSVC = false;
        public static bool bolUpdateWeb = false;
        public static bool UpdatePaused = false;

        //Update booleans
        public static bool bolServerUpdateAvailable = false;
        public static bool bolPreUpdateAvailable = false;
        public static bool bolDllUpdateAvailable = false;
        public static bool bolServiceUpdateAvailable = false;
        public static bool bolGUIUpdateAvailable = false;
        public static bool bolWebUpdateAvailable = false;
        public static bool bolOverviewerUpdateAvailable = false;
        public static bool bolC10tUpdateAvailable = false;
        public static bool bolTectonicusUpdateAvailable = false;
        public static bool bolRestartNeeded = false;
        public static bool bolBukkitUpdateAvailable = false;
        public static bool bolLibUpdateAvailable = false;

        //Minecraft URLs
        public static string strMCServerURL = "http://www.minecraft.net/download/minecraft_server.jar";
        public static string strMCClientURL = "http://minecraft.net/download/Minecraft.jar";

        //Bukkit URLs
        public static string strBukkitServerURL = "http://ci.bukkit.org/job/dev-CraftBukkit/promotion/latest/Recommended/artifact/target/craftbukkit-0.0.1-SNAPSHOT.jar";

        //YAMS URLs
        public static Dictionary<string, string> strYAMSUpdatePath = new Dictionary<string, string>()
        {
            { "live", "https://github.com/richardbenson/YAMS/raw/updater"},
            { "dev", "https://github.com/richardbenson/YAMS/raw/updater/development" }
        };

        //Third party URLS
        public static Dictionary<string, string> dicAddOnURLS = new Dictionary<string, string>
        {
            { "tectonicus", "http://www.triangularpixels.com/Tectonicus/Tectonicus_vxxx.jar" },
            { "overviewer-x86", "https://github.com/downloads/overviewer/Minecraft-Overviewer/win86_32-vxxx.zip" },
            { "overviewer-x64", "https://github.com/downloads/overviewer/Minecraft-Overviewer/win86_64-vxxx.zip" },
            { "biome-extractor", "http://dl.dropbox.com/u/107712/MCMap/Minecraft-Biome-Extractor-vxxx.zip" },
            { "c10t-x86", "http://toolchain.eu/minecraft/c10t/releases/c10t-xxx-windows-x86.zip" },
            { "c10t-x64", "http://toolchain.eu/minecraft/c10t/releases/c10t-xxx-windows-x86_64.zip" }
        };

        //Default versions
        private static string strOverviewerVer = "0.3.0";
        private static string strC10tVer = "1.7";
        private static string strBiomeExtractorVer = "080";
        private static string strTectonicusVer = "1.38";

        //Checks for available updates
        public static void CheckUpdates(bool bolForce = false)
        {
            YAMS.Database.AddLog("Running update check", "updater");

            if (!UpdatePaused)
            {
                //What branch are we on?
                string strBranch = Database.GetSetting("UpdateBranch", "YAMS");
                string strYPath = strYAMSUpdatePath[strBranch];

                //Grab latest version file if it needs updating
                UpdateIfNeeded(strYPath + @"/versions.json", YAMS.Core.RootFolder + @"\lib\versions.json");
                string json = File.ReadAllText(YAMS.Core.RootFolder + @"\lib\versions.json");
                //Dictionary<string, string> dicVers = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                JObject jVers = JObject.Parse(json);

                //Reset all the JAR etags so we re-download them
                if (bolForce)
                {
                    YAMS.Database.AddLog("Forced re-download of JAR files", "updater", "warn");
                    YAMS.Database.SaveEtag(strMCServerURL, "");
                    YAMS.Database.SaveEtag((string)jVers["pre"], "");
                    YAMS.Database.SaveEtag(strBukkitServerURL, "");
                }
                
                //Check Minecraft server first
                if (bolUpdateJAR)
                {
                    bolServerUpdateAvailable = UpdateIfNeeded(strMCServerURL, YAMS.Core.RootFolder + @"\lib\minecraft_server.jar.UPDATE");
                    UpdateIfNeeded(strYPath + @"/properties.json", YAMS.Core.RootFolder + @"\lib\properties.json");
                    bolPreUpdateAvailable = UpdateIfNeeded((string)jVers["pre"], YAMS.Core.RootFolder + @"\lib\minecraft_server_pre.jar.UPDATE");
                }

                //Have they opted for bukkit? If so, update that too
                if (Convert.ToBoolean(Database.GetSetting("BukkitInstalled", "YAMS")))
                {
                    bolBukkitUpdateAvailable = UpdateIfNeeded(strBukkitServerURL, Core.RootFolder + @"\lib\craftbukkit.jar", "modified");
                }

                //Now update self
                if (bolUpdateSVC)
                {
                    bolDllUpdateAvailable = UpdateIfNeeded(strYPath + @"/YAMS-Library.dll", YAMS.Core.RootFolder + @"\YAMS-Library.dll.UPDATE");
                    if (UpdateIfNeeded(strYPath + @"/YAMS-Service.exe", YAMS.Core.RootFolder + @"\YAMS-Service.exe.UPDATE") || UpdateIfNeeded(strYPath + @"/YAMS-Service.exe.config", YAMS.Core.RootFolder + @"\YAMS-Service.exe.config.UPDATE"))
                    {
                        bolServiceUpdateAvailable = true;
                    }
                    bolWebUpdateAvailable = UpdateIfNeeded(strYPath + @"/web.zip", YAMS.Core.RootFolder + @"\web.zip");
                    bolGUIUpdateAvailable = UpdateIfNeeded(strYPath + @"/YAMS-Updater.exe", YAMS.Core.RootFolder + @"\YAMS-Updater.exe");

                    //Update External libs
                    foreach (JProperty j in jVers["libs"])
                    {
                        if (UpdateIfNeeded(strYAMSUpdatePath[strBranch] + @"/lib/" + j.Name, Core.RootFolder + @"\lib\" + j.Name + ".UPDATE")) bolLibUpdateAvailable = true;
                    }
                }

                if (bolUpdateAddons)
                {
                    //Update add-ons if they have elected to have them
                    //Update overviewer
                    if (Convert.ToBoolean(Database.GetSetting("OverviewerInstalled", "YAMS"))) {
                        strOverviewerVer = (string)jVers["apps"]["overviewer"];
                        if (UpdateIfNeeded(GetExternalURL("overviewer", strOverviewerVer), YAMS.Core.RootFolder + @"\apps\overviewer.zip"))
                        {
                            bolOverviewerUpdateAvailable = true;
                            if (!Directory.Exists(YAMS.Core.RootFolder + @"\apps\overviewer-" + strOverviewerVer + "\\")) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\apps\overviewer-" + strOverviewerVer + "\\");
                            ExtractZip(YAMS.Core.RootFolder + @"\apps\overviewer.zip", YAMS.Core.RootFolder + @"\apps\overviewer-" + strOverviewerVer + "\\");
                            File.Delete(YAMS.Core.RootFolder + @"\apps\overviewer.zip");
                            if (Directory.Exists(YAMS.Core.RootFolder + @"\apps\overviewer\")) Directory.Delete(YAMS.Core.RootFolder + @"\apps\overviewer\", true);
                            Directory.Move(YAMS.Core.RootFolder + @"\apps\overviewer-" + strOverviewerVer, YAMS.Core.RootFolder + @"\apps\overviewer");
                        }
                    }

                    //Grab the biome extractor too
                    if (Convert.ToBoolean(Database.GetSetting("BiomeExtractorInstalled", "YAMS"))) {
                        strBiomeExtractorVer = (string)jVers["apps"]["biomeextractor"];
                        if (UpdateIfNeeded(GetExternalURL("biome-extractor", strBiomeExtractorVer), YAMS.Core.RootFolder + @"\apps\biome-extractor.zip"))
                        {
                            ExtractZip(YAMS.Core.RootFolder + @"\apps\biome-extractor.zip", YAMS.Core.RootFolder + @"\apps\");
                            File.Delete(YAMS.Core.RootFolder + @"\apps\biome-extractor.zip");
                            if (Directory.Exists(YAMS.Core.RootFolder + @"\apps\biome-extractor\")) Directory.Delete(YAMS.Core.RootFolder + @"\apps\biome-extractor\", true);
                            Directory.Move(YAMS.Core.RootFolder + @"\apps\Minecraft Biome Extractor", YAMS.Core.RootFolder + @"\apps\biome-extractor");
                        }
                    }

                    //Update c10t
                    if (Convert.ToBoolean(Database.GetSetting("C10tInstalled", "YAMS")))
                    {
                        strC10tVer = (string)jVers["apps"]["c10t"];
                        if (UpdateIfNeeded(GetExternalURL("c10t", strC10tVer), YAMS.Core.RootFolder + @"\apps\c10t.zip", "modified"))
                        {
                            bolC10tUpdateAvailable = true;
                            ExtractZip(YAMS.Core.RootFolder + @"\apps\c10t.zip", YAMS.Core.RootFolder + @"\apps\");
                            File.Delete(YAMS.Core.RootFolder + @"\apps\c10t.zip");
                            if (Directory.Exists(YAMS.Core.RootFolder + @"\apps\c10t\")) Directory.Delete(YAMS.Core.RootFolder + @"\apps\c10t\", true);
                            string strC10tDir = YAMS.Core.RootFolder + @"\apps\c10t-" + strC10tVer + "-windows-x86";
                            if (YAMS.Util.GetBitness() == "x64") strC10tDir += "_64";
                            Directory.Move(strC10tDir, YAMS.Core.RootFolder + @"\apps\c10t");
                        }
                    }

                    //Update Tectonicus
                    if (Convert.ToBoolean(Database.GetSetting("TectonicusInstalled", "YAMS")))
                    {
                        if (!Directory.Exists(YAMS.Core.RootFolder + @"\apps\tectonicus\")) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\apps\tectonicus\");
                        strTectonicusVer = (string)jVers["apps"]["tectonicus"];
                        if (UpdateIfNeeded(GetExternalURL("tectonicus", strTectonicusVer), YAMS.Core.RootFolder + @"\apps\tectonicus\tectonicus.jar.update", "modified"))
                        {
                            bolTectonicusUpdateAvailable = true;
                            try
                            {
                                if (File.Exists(YAMS.Core.RootFolder + @"\apps\tectonicus\tectonicus.jar")) File.Delete(YAMS.Core.RootFolder + @"\apps\tectonicus\tectonicus.jar");
                                File.Move(YAMS.Core.RootFolder + @"\apps\tectonicus\tectonicus.jar.update", YAMS.Core.RootFolder + @"\apps\tectonicus\tectonicus.jar");
                            }
                            catch (IOException e)
                            {
                                YAMS.Database.AddLog("Unable to update Tectonicus: " + e.Message, "updater", "warn");
                            }
                        }
                    }
                }

                //Now check if we can auto-restart anything
                if ((bolDllUpdateAvailable || bolServiceUpdateAvailable || bolWebUpdateAvailable || bolRestartNeeded || bolLibUpdateAvailable) && Convert.ToBoolean(Database.GetSetting("RestartOnSVCUpdate", "YAMS")))
                {
                    //Check there are no players on the servers
                    bool bolPlayersOn = false;
                    foreach (KeyValuePair<int, MCServer> kvp in Core.Servers)
                    {
                        if (kvp.Value.Players.Count > 0) bolPlayersOn = true;
                    }
                    if (bolPlayersOn)
                    {
                        Database.AddLog("Deferring update until free");
                        bolRestartNeeded = true;
                    }
                    else
                    {
                        Database.AddLog("Restarting Service for updates");
                        System.Diagnostics.Process.Start(YAMS.Core.RootFolder + @"\YAMS-Updater.exe", "/restart");
                    }
                }

                //Restart individual servers?
                if ((bolServerUpdateAvailable || bolBukkitUpdateAvailable || bolPreUpdateAvailable) && Convert.ToBoolean(Database.GetSetting("RestartOnJarUpdate", "YAMS")))
                {
                    foreach (KeyValuePair<int, MCServer> kvp in Core.Servers)
                    {
                        if (((kvp.Value.ServerType == "vanilla" && bolServerUpdateAvailable) || (kvp.Value.ServerType == "bukkit" && bolBukkitUpdateAvailable)))
                        {
                            kvp.Value.RestartIfEmpty();
                        }
                    }
                }
            }
            else
            {
                Database.AddLog("Updating Paused", "updater", "warn");
            }
        }

        public static void ExtractZip(string strZipFile, string strPath)
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(strZipFile)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(strPath + directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(strPath + theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }

        //Swaps out the version number from the third party URLs where needed
        public static string GetExternalURL(string strApp, string strVersion)
        {
            var strReturn = "";
            switch (strApp)
            {
                case "c10t":
                case "overviewer":
                    strReturn = dicAddOnURLS[strApp + "-" + YAMS.Util.GetBitness()];
                    break;
                default:
                    strReturn = dicAddOnURLS[strApp];
                    break;
            }
            strReturn = strReturn.Replace("xxx", strVersion);
            return strReturn;
        }

        public static bool UpdateIfNeeded(string strURL, string strFile, string strType = "etag")
        {
            //Get our stored eTag for this URL
            string strETag = "";
            strETag = YAMS.Database.GetEtag(strURL);

            try
            {
                //Set up a request and include our eTag
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(strURL);
                request.Method = "GET";
                if (strETag != "")
                {
                    if (strType == "etag")
                    {
                        request.Headers[HttpRequestHeader.IfNoneMatch] = strETag;
                    }
                    else
                    {
                        try
                        {
                            strETag = strETag.Replace("UTC", "GMT"); //Fix for weird servers not sending correct formate datetime
                            request.IfModifiedSince = Convert.ToDateTime(strETag);
                        }
                        catch (Exception e) { Database.AddLog("Unable to set modified date for URL: " + strURL + "; " + e.Message, "updater", "warn"); }
                    }
                }
                //if (strETag != null) request.Headers[HttpRequestHeader.IfModifiedSince] = strETag;

                //Grab the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Stream the file
                Stream strm = response.GetResponseStream();
                FileStream fs = new FileStream(strFile, FileMode.Create, FileAccess.Write, FileShare.None);
                const int ArrSize = 10000;
                Byte[] barr = new Byte[ArrSize];
                while (true)
                {
                    int Result = strm.Read(barr, 0, ArrSize);
                    if (Result == -1 || Result == 0)
                        break;
                    fs.Write(barr, 0, Result);
                }
                fs.Flush();
                fs.Close();
                strm.Close();
                response.Close();

                //Save the etag
                if (strType == "etag")
                {
                    if (response.Headers[HttpResponseHeader.ETag] != null) YAMS.Database.SaveEtag(strURL, response.Headers[HttpResponseHeader.ETag]);
                }
                else
                {
                    if (response.Headers[HttpResponseHeader.LastModified] != null) YAMS.Database.SaveEtag(strURL, response.Headers[HttpResponseHeader.LastModified]);
                }

                YAMS.Database.AddLog(strFile + " update downloaded", "updater");

                return true;
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Response != null)
                {
                    using (HttpWebResponse response = ex.Response as HttpWebResponse)
                    {
                        if (response.StatusCode == HttpStatusCode.NotModified)
                        {
                            //304 means there is no update available
                            //YAMS.Database.AddLog(strFile + " is up to date", "updater");
                            return false;
                        }
                        else
                        {
                            // Wasn't a 200, and wasn't a 304 so let the log know
                            YAMS.Database.AddLog(string.Format("Failed to check " + strURL + ". Error Code: {0}", response.StatusCode), "updater", "error");
                            return false;
                        }
                    }
                }
                else return false;
            }
            catch (Exception e)
            {
                YAMS.Database.AddLog(string.Format("Failed to update " + strFile + ". Error: {0}", e.Message), "updater", "error");
                return false;
            }
        }
    }
}
