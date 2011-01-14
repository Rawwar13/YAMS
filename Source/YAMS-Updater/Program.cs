using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using YAMS;

namespace YAMS_Updater
{
    class Program
    {
        private static string RootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        [DllImport("kernel32")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        static void Main(string[] args)
        {
            Console.WriteLine("*** YAMS Updater ***");

            if (args.Contains<string>("/restart"))
            {
                //We're just here to restart the service safely after some updates
                Console.WriteLine("Restarting service...");
                ServiceController scYAMS = new ServiceController("YAMS_Service");
                scYAMS.Stop();
                while (!scYAMS.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    scYAMS.Refresh();
                }
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
            else if (args.Contains<string>("/start"))
            {
                ServiceController scYAMS = new ServiceController("YAMS_Service");
                if (!scYAMS.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    Console.WriteLine("Service already running");
                }
                else
                {
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
                }
                Environment.Exit(0);
            }
            else if (args.Contains<string>("/stop"))
            {
                ServiceController scYAMS = new ServiceController("YAMS_Service");
                if (scYAMS.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    Console.WriteLine("Service not running");
                }
                else
                {
                    scYAMS.Stop();
                    Console.WriteLine("Service stopped");
                }
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    IntPtr conWnd = GetConsoleWindow();
                    if (conWnd != IntPtr.Zero) ShowWindow(conWnd, 0);
                }
                catch { }

                System.Windows.Forms.Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);

                YAMS.Database.init();
                YAMS.Database.AddLog("Updater Starting");

                //Have they run the app before?
                if (YAMS.Database.GetSetting("FirstRun", "YAMS") != "true")
                {
                    Application.Run(new frmFirstRun());
                }

                Application.Run(new frmMain());

                return;
            }
        
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
