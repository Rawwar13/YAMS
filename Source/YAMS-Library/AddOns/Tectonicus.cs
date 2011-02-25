using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using YAMS;

namespace YAMS.AddOns
{
    class Tectonicus : App
    {
        public Tectonicus(MCServer s, string strParams = "")
            : base(s, "tectonicus", @"tectonicus.jar", "Tectonicus", true, strParams) {}

        public override void DoWork()
        {
            //Force a server save and turn off level saving
            this.Server.Save();
            this.Server.DisableSaving();

            string ServerRoot = this.Server.ServerDirectory;
            string strArgs = "-Dorg.lwjgl.opengl.Display.allowSoftwareOpenGL=true -Dorg.lwjgl.opengl.Display.noinput=true " + 
                "-jar tectonicus.jar " + 
                "worldDir=\"" + ServerRoot + "\\world\" " + 
                "outputDir=\"" + ServerRoot + "\\renders\\tectonicus\" " +
                "mode=cmd verbose=true numSamples=0";

            //First run the biome extractor tool
            Process prc = new Process();
            prc.StartInfo.UseShellExecute = false;
            prc.StartInfo.FileName = YAMS.Util.JavaPath() + "java.exe";
            prc.StartInfo.Arguments = strArgs;
            prc.StartInfo.CreateNoWindow = true;
            prc.StartInfo.RedirectStandardError = true;
            prc.StartInfo.RedirectStandardInput = true;
            prc.StartInfo.RedirectStandardOutput = true;
            prc.StartInfo.WorkingDirectory = this.FullFolderPath;

            //Set up events
            prc.OutputDataReceived += new DataReceivedEventHandler(ProcessOutput);
            prc.ErrorDataReceived += new DataReceivedEventHandler(ProcessError);
            prc.EnableRaisingEvents = true;

            //Finally start the thing
            prc.Start();
            prc.BeginOutputReadLine();
            prc.BeginErrorReadLine();

            Database.AddLog(this.Name + " Path: " + strArgs, this.BaseName);
            Database.AddLog(this.Name + " Started", this.BaseName);

            while (!prc.WaitForExit(1000)) ;

            if (prc.ExitCode == 0)
            {
                Database.AddLog(this.Name + " Completed", this.BaseName);
                this.Complete = true;
                this.Result = true;
            }
            else
            {
                Database.AddLog(this.Name + " Failed: " + prc.ExitCode, this.BaseName, "error");
                this.Complete = true;
                this.Result = false;
            }
            Thread.Sleep(10000);

            //Re-enable server saving and updating
            this.Server.EnableSaving();

            //Must always call this to let base class know we're done
            this.Finish();
        }

        private void ProcessOutput(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName);
        }
        private void ProcessError(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "error");
        }

    }
}
