using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace YAMS
{
    public static class AutoUpdate
    {
        //Settings
        public static bool bolUpdateApp = true;
        public static bool bolUpdateServer = true;
        public static bool bolUpdateClient = true;
        public static bool bolUpdateAddons = true;

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
        public static string strYAMSDLLURL = "http://richardbenson.github.com/YAMS/download/YAMS-Library.dll";
        public static string strYAMSServiceURL = "http://richardbenson.github.com/YAMS/download/YAMS-Service.exe";
        public static string strYAMSWebURL = "http://richardbenson.github.com/YAMS/download/web.zip";
        public static string strYAMSVersionsURL = "http://richardbenson.githib.com/YAMS/versions.json";

        //Third party URLS
        public static string strOverviewerURL = "https://github.com/downloads/brownan/Minecraft-Overviewer/Overviewer-xxx.zip";
        public static string strC10tx86URL = "https://github.com/downloads/udoprog/c10t/c10t-xxx-windows-x86.zip";
        public static string strC10tx64URL = "https://github.com/downloads/udoprog/c10t/c10t-xxx-windows-x86_64.zip";

        //Default versions
        private static string strOverviewerVer = "0.0.4";
        private static string strC10tVer = "1.3";

        //Checks for available updates
        public static void CheckUpdates()
        {
            //Check Minecraft server first
            if (bolUpdateServer) bolServerUpdateAvailable = UpdateIfNeeded(strMCServerURL, YAMS.Core.RootFolder + @"\lib\minecraft_server.jar.UPDATE");

            //Now update self
            if (bolUpdateApp)
            {
                bolDllUpdateAvailable = UpdateIfNeeded(strYAMSDLLURL, YAMS.Core.RootFolder + @"\YAMS-Library.dll.UPDATE");
                bolServiceUpdateAvailable = UpdateIfNeeded(strMCServerURL, YAMS.Core.RootFolder + @"\YAMS-Service.exe.UPDATE");
                bolWebUpdateAvailable = UpdateIfNeeded(strMCServerURL, YAMS.Core.RootFolder + @"\web\web.zip");
            }

            //Check our managed updates
            if (bolUpdateAddons && UpdateIfNeeded(strYAMSVersionsURL, YAMS.Core.RootFolder + @"\lib\versions.json"))
            {
                //There is an update somewhere, extract versions and compare


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
                request.Headers[HttpRequestHeader.IfNoneMatch] = strETag;

                //Grab the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Save the etag
                YAMS.Database.SaveEtag(strURL, response.Headers[HttpResponseHeader.ETag]);

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
