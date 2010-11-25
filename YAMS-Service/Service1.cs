using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;


namespace YAMS_Service
{
    public partial class Service1 : ServiceBase
    {
        public static Process minecraftServer;
        
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ProcessStartInfo psi = new ProcessStartInfo("calc.exe");
            psi.WorkingDirectory = Directory.GetCurrentDirectory();
            psi.UseShellExecute = false;
            //psi.CreateNoWindow = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            minecraftServer = new Process();
            minecraftServer.StartInfo = psi;

            minecraftServer.EnableRaisingEvents = true;
            //minecraftServer.Exited += new EventHandler(minecraftServer_Exited);
            //minecraftServer.OutputDataReceived += new DataReceivedEventHandler(minecraftServer_OutputDataReceived);
            //minecraftServer.ErrorDataReceived += new DataReceivedEventHandler(minecraftServer_ErrorDataReceived);

            minecraftServer.Start();

            minecraftServer.BeginOutputReadLine();
            minecraftServer.BeginErrorReadLine();
        }

        protected override void OnStop()
        {
            minecraftServer.EnableRaisingEvents = false;
            try
            {
                minecraftServer.CancelErrorRead();
            }
            catch { }
            try
            {
                minecraftServer.CancelOutputRead();
            }
            catch { }
            minecraftServer.Kill();
            minecraftServer.WaitForExit();
        }
    }
}
