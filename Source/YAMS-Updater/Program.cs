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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ExceptionManager;

namespace YAMS_Updater
{
    class Program
    {
        public static string RootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        [DllImport("kernel32")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        static void Main(string[] args)
        {
            UnhandledExceptionManager.AddHandler();
            
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

            //svcYAMS = new ServiceController("YAMS_Service");

            Console.WriteLine("*** YAMS Updater ***");

            YAMS.Database.init();
            YAMS.Database.AddLog("YAMS-Updater run on local machine");
            
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

                //Have they run the app before?
                if (YAMS.Database.GetSetting("FirstRun", "YAMS") != "true")
                {
                    Application.Run(new frmDependencies());
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
            ServiceController svcYAMS = new ServiceController("YAMS_Service");

            if (svcYAMS.Status.Equals(ServiceControllerStatus.Stopped))
            {
                Console.WriteLine("Service not running");
            }
            else if (svcYAMS.Status.Equals(ServiceControllerStatus.StartPending)) {
                System.Threading.Thread.Sleep(1000);
                StopService();
            }
            else
            {
                svcYAMS.Stop();
                Console.WriteLine("Service stopped");
            }

        }

        public static void StartService()
        {
            ServiceController svcYAMS = new ServiceController("YAMS_Service");

            if (svcYAMS.Status.Equals(ServiceControllerStatus.Running))
            {
                Console.WriteLine("Service already running");
            }
            else if (svcYAMS.Status.Equals(ServiceControllerStatus.StopPending) || svcYAMS.Status.Equals(ServiceControllerStatus.StartPending))
            {
                System.Threading.Thread.Sleep(1000);
                StartService();
            }
            else
            {
                //Special case for a file that shouldn't be where it is
                if (!File.Exists(RootFolder + @"\SharpUPnP.dll") && File.Exists(RootFolder + @"\lib\SharpUPnP.dll")) File.Copy(RootFolder + @"\lib\SharpUPnP.dll", RootFolder + @"\SharpUPnP.dll");
                
                //Apply any updates to core files, these should cope with any other updates
                if (File.Exists(RootFolder + @"\YAMS-Library.dll.UPDATE"))
                {
                    if (File.Exists(RootFolder + @"\YAMS-Library.dll.OLD")) File.Delete(RootFolder + @"\YAMS-Library.dll.OLD");
                    File.Move(RootFolder + @"\YAMS-Library.dll", RootFolder + @"\YAMS-Library.dll.OLD");
                    File.Move(RootFolder + @"\YAMS-Library.dll.UPDATE", RootFolder + @"\YAMS-Library.dll");
                }
                if (File.Exists(RootFolder + @"\YAMS-Service.exe.UPDATE"))
                {
                    if (File.Exists(RootFolder + @"\YAMS-Service.exe.OLD")) File.Delete(RootFolder + @"\YAMS-Service.exe.OLD");
                    File.Move(RootFolder + @"\YAMS-Service.exe", RootFolder + @"\YAMS-Service.exe.OLD");
                    File.Move(RootFolder + @"\YAMS-Service.exe.UPDATE", RootFolder + @"\YAMS-Service.exe");
                }
                if (File.Exists(RootFolder + @"\YAMS-Service.exe.config.UPDATE"))
                {
                    if (File.Exists(RootFolder + @"\YAMS-Service.exe.config.OLD")) File.Delete(RootFolder + @"\YAMS-Service.exe.config.OLD");
                    File.Move(RootFolder + @"\YAMS-Service.exe.config", RootFolder + @"\YAMS-Service.exe.config.OLD");
                    File.Move(RootFolder + @"\YAMS-Service.exe.config.UPDATE", RootFolder + @"\YAMS-Service.exe.config");
                }

                string json = File.ReadAllText(RootFolder + @"\lib\versions.json");
                //Dictionary<string, string> dicVers = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                JObject jVers = JObject.Parse(json);

                foreach (JProperty j in jVers["libs"])
                {
                    if (File.Exists(RootFolder + @"\lib\" + j.Name + ".UPDATE"))
                    {
                        try
                        {
                            if (File.Exists(RootFolder + @"\lib\" + j.Name + ".OLD")) File.Delete(RootFolder + @"\lib\" + j.Name + ".OLD");
                            if (File.Exists(RootFolder + @"\lib\" + j.Name)) File.Delete(RootFolder + @"\lib\" + j.Name);
                            File.Move(RootFolder + @"\lib\" + j.Name + ".UPDATE", RootFolder + @"\lib\" + j.Name);
                        }
                        catch (Exception e)
                        {
                            Database.AddLog("Unable to update " + j.Name + ". Please manually rename " + j.Name + ".UPDATE to " + j.Name + " in the libs folder whilst the service is stopped.", "updater", "error");
                        }
                    } 
                }

                //Restart the service
                svcYAMS.Start();
                Console.WriteLine("Service started");
            }

        }
    }
}
