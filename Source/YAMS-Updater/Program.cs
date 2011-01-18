using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using YAMS;
using System.Security.Principal;

namespace YAMS_Updater
{
    class Program
    {
        private static string RootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        public static ServiceController svcYAMS;

        [DllImport("kernel32")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        static void Main(string[] args)
        {
            //We need Admin for almost everything in here
            // Needs UAC elevation for webmin to run
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!hasAdministrativeRight)
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = Application.ExecutablePath;
                if (args.Length > 0 ) processInfo.Arguments = args.ToString();
                try
                {
                    Process.Start(processInfo);
                }
                catch
                {
                    Environment.Exit(0);
                }
                Environment.Exit(0);
            }

            svcYAMS = new ServiceController("YAMS_Service");

            Console.WriteLine("*** YAMS Updater ***");

            if (args.Contains<string>("/restart"))
            {
                //We're just here to restart the service safely after some updates
                Console.WriteLine("Restarting service...");
                StopService();
                StartService();
                Environment.Exit(0);
            }
            else if (args.Contains<string>("/start"))
            {
                StartService();
                Environment.Exit(0);
            }
            else if (args.Contains<string>("/stop"))
            {
                StopService();
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

                YAMS.Database.init();
                YAMS.Database.AddLog("Updater Starting");

                //Have they run the app before?
                if (YAMS.Database.GetSetting("FirstRun", "YAMS") != "true")
                {
                    Application.Run(new frmFirstRun());
                }
                else
                {
                    Application.Run(new frmMain());
                }

                return;
            }
        
        }

        public static void StopService()
        {
            if (svcYAMS.Status.Equals(ServiceControllerStatus.Stopped))
            {
                Console.WriteLine("Service not running");
            }
            else
            {
                svcYAMS.Stop();
                Console.WriteLine("Service stopped");
            }

        }

        public static void StartService()
        {
            if (!svcYAMS.Status.Equals(ServiceControllerStatus.Stopped))
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
                svcYAMS.Start();
                Console.WriteLine("Service started");
            }

        }
    }
}
