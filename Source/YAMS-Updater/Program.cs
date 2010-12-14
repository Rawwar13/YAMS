using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;

namespace YAMS_Updater
{
    class Program
    {
        private static string RootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
        private static string strYAMSDLLURL = "https://github.com/richardbenson/YAMS/raw/master/Binaries/YAMS-Library.dll";
        private static string strYAMSServiceURL = "https://github.com/richardbenson/YAMS/raw/master/Binaries/YAMS-Service.exe";

        private static string strHttpURL = "https://github.com/richardbenson/YAMS/raw/master/Binaries/HttpServer.dll";
        private static string strZipURL = "https://github.com/richardbenson/YAMS/raw/master/Binaries/ICSharpCode.SharpZipLib.dll";
        private static string strJsonURL = "https://github.com/richardbenson/YAMS/raw/master/Binaries/Newtonsoft.Json.dll";


        static void Main(string[] args)
        {
            Console.WriteLine("*** YAMS Updater ***");

            if (args.Contains<string>("restart"))
            {
                //We're just here to restart the service safely after some updates
                Console.WriteLine("Restarting service...");
                ServiceController scYAMS = new ServiceController("YAMS-Service");
                scYAMS.Stop();
                Console.WriteLine("Service stopped");

                //Apply any updates to core files, these should cope with any other updates
                if (File.Exists(RootFolder + @"\YAMS-Library.dll.UPDATE"))
                {
                    File.Move(RootFolder + @"\YAMS-Library.dll", RootFolder + @"\YAMS-Library.dll.OLD");
                    File.Move(RootFolder + @"\YAMS-Library.dll.UPDATE", RootFolder + @"\YAMS-Library.dll");
                    File.Delete(RootFolder + @"\YAMS-Library.dll.OLD");
                }
                if (File.Exists(RootFolder + @"\YAMS-Service.exe.UPDATE"))
                {
                    File.Move(RootFolder + @"\YAMS-Service.exe", RootFolder + @"\YAMS-Service.exe.OLD");
                    File.Move(RootFolder + @"\YAMS-Service.exe.UPDATE", RootFolder + @"\YAMS-Service.exe");
                    File.Delete(RootFolder + @"\Service.exe.OLD");
                }

                //Restart the service
                scYAMS.Start();
                Console.WriteLine("Service started");

                Environment.Exit(0);
            }

            //Otherwise we're here to get the files needed to install the app
            DownloadURLToFile(strYAMSDLLURL, RootFolder + @"\YAMS-Library.dll");
            DownloadURLToFile(strYAMSServiceURL, RootFolder + @"\YAMS-Service.exe");

            DownloadURLToFile(strHttpURL, RootFolder + @"\HttpServer.dll");
            DownloadURLToFile(strZipURL, RootFolder + @"\ICSharpCode.SharpZipLib.dll");
            DownloadURLToFile(strJsonURL, RootFolder + @"\Newtonsoft.Json.dll");

            //Install and start the service

            
            ServiceController scYAMS2 = new ServiceController("YAMS-Service");
            scYAMS2.Start();
            Console.WriteLine("Service started");

            Console.WriteLine("-----------------------------------\nYAMS installed and started, press any key to open a browser and start configuring...");
            Console.ReadLine();

            Process.Start("http://localhost:56552/");
        
        }

        private static bool DownloadURLToFile(string strURL, string strFile)
        {
            try
            {
                //Set up a request and include our eTag
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(strURL);
                request.Method = "GET";

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

                Console.WriteLine("Downloaded: " + strURL);

                return true;
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Response != null)
                {
                    using (HttpWebResponse response = ex.Response as HttpWebResponse)
                    {
                        Console.WriteLine(string.Format("Failed to fetch " + strURL + ". Error Code: {0}", response.StatusCode));
                        return false;
                    }
                }
                else return false;
            }
        }
    }
}
