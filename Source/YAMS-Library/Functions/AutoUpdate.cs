using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
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

        //Update booleans
        public static bool bolServerUpdateAvailable = false;
        public static bool bolDllUpdateAvailable = false;
        public static bool bolServiceUpdateAvailable = false;
        public static bool bolWebUpdateAvailable = false;
        public static bool bolOverviewerUpdateAvailable = false;
        public static bool bolC10tUpdateAvailable = false;

        //Minecraft URLs
        public static string strMCServerURL = "http://minecraft.net/download/minecraft_server.jar";
        public static string strMCClientURL = "http://minecraft.net/download/Minecraft.jar";

        //YAMS URLs
        public static string strYAMSDLLURL = "http://richardbenson.github.com/YAMS/downloads/YAMS-Library.dll";
        public static string strYAMSServiceURL = "http://richardbenson.github.com/YAMS/downloads/YAMS-Service.exe";
        public static string strYAMSWebURL = "http://richardbenson.github.com/YAMS/downloads/web.zip";
        public static string strYAMSVersionsURL = "http://richardbenson.github.com/YAMS/versions.json";

        //Third party URLS
        public static string strOverviewerURL = "https://github.com/downloads/brownan/Minecraft-Overviewer/Overviewer-xxx.zip";
        public static string strC10tx86URL = "http://toolchain.eu/minecraft/c10t/releases/c10t-xxx-windows-x86.zip";
        public static string strC10tx64URL = "http://toolchain.eu/minecraft/c10t/releases/c10t-xxx-windows-x86_64.zip";

        //Default versions
        private static string strOverviewerVer = "0.0.5";
        private static string strC10tVer = "1.4";

        //Checks for available updates
        public static void CheckUpdates()
        {
            //Check Minecraft server first
            if (bolUpdateJAR) bolServerUpdateAvailable = UpdateIfNeeded(strMCServerURL, YAMS.Core.RootFolder + @"\lib\minecraft_server.jar.UPDATE");

            //Now update self
            if (bolUpdateGUI)
            {
                bolDllUpdateAvailable = UpdateIfNeeded(strYAMSDLLURL, YAMS.Core.RootFolder + @"\YAMS-Library.dll.UPDATE");
                bolServiceUpdateAvailable = UpdateIfNeeded(strMCServerURL, YAMS.Core.RootFolder + @"\YAMS-Service.exe.UPDATE");
                bolWebUpdateAvailable = UpdateIfNeeded(strMCServerURL, YAMS.Core.RootFolder + @"\web\web.zip");
            }

            //Check our managed updates
            if (bolUpdateAddons && UpdateIfNeeded(strYAMSVersionsURL, YAMS.Core.RootFolder + @"\lib\versions.json"))
            {
                //There is an update somewhere, extract versions and compare
                string json = File.ReadAllText(YAMS.Core.RootFolder + @"\lib\versions.json");
                Dictionary<string, string> dicVers = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                strOverviewerVer = dicVers["overviewer"];
                if (UpdateIfNeeded(GetExternalURL("overviewer", strOverviewerVer), YAMS.Core.RootFolder + @"\apps\overviewer.zip"))
                {
                    bolOverviewerUpdateAvailable = true;
                    ExtractZip(YAMS.Core.RootFolder + @"\apps\overviewer.zip", YAMS.Core.RootFolder + @"\apps\");
                    File.Delete(YAMS.Core.RootFolder + @"\apps\overviewer.zip");
                    if (Directory.Exists(YAMS.Core.RootFolder + @"\apps\overviewer\")) Directory.Delete(YAMS.Core.RootFolder + @"\apps\overviewer\", true);
                    Directory.Move(YAMS.Core.RootFolder + @"\apps\Overviewer-" + strOverviewerVer, YAMS.Core.RootFolder + @"\apps\overviewer");
                }

                strC10tVer = dicVers["c10t"];
                if (UpdateIfNeeded(GetExternalURL("c10t", strC10tVer), YAMS.Core.RootFolder + @"\apps\c10t.zip"))
                {
                    bolC10tUpdateAvailable = true;
                    ExtractZip(YAMS.Core.RootFolder + @"\apps\c10t.zip", YAMS.Core.RootFolder + @"\apps\");
                    File.Delete(YAMS.Core.RootFolder + @"\apps\c10t.zip");
                    if (Directory.Exists(YAMS.Core.RootFolder + @"\apps\c10t\")) Directory.Delete(YAMS.Core.RootFolder + @"\apps\c10t\", true);
                    Directory.Move(YAMS.Core.RootFolder + @"\apps\c10t-" + strC10tVer, YAMS.Core.RootFolder + @"\apps\c10t");
                }


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
                case "overviewer":
                    strReturn = strOverviewerURL;
                    break;
                case "c10t":
                    switch (YAMS.Util.GetBitness())
                    {
                        case "x86":
                            strReturn = strC10tx86URL;
                            break;
                        case "x64":
                            strReturn = strC10tx64URL;
                            break;
                    }
                    break;

            }
            strReturn = strReturn.Replace("xxx", strVersion);
            return strReturn;
        }

        public static bool UpdateIfNeeded(string strURL, string strFile)
        {
            //Get our stored eTag for this URL
            string strETag = "";
            strETag = YAMS.Database.GetEtag(strURL);
            
            try
            {
                //Set up a request and include our eTag
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(strURL);
                request.Method = "GET";
                if (strETag != null) request.Headers[HttpRequestHeader.IfModifiedSince] = strETag;

                //Grab the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Save the etag
                if (response.Headers[HttpResponseHeader.LastModified] != null) YAMS.Database.SaveEtag(strURL, response.Headers[HttpResponseHeader.LastModified]);

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
                            YAMS.Database.AddLog(strFile + " is up to date", "updater");
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
        }
    }
}
