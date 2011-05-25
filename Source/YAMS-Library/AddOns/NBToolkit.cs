using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using YAMS;

namespace YAMS.AddOns
{
    class NBToolkit : App
    {

        public NBToolkit(MCServer s, string strParams = "mode=oregen")
            : base(s, "nbtoolkit", @"NBToolkit.exe", "NBToolkit", true, strParams) {}

        public override void DoWork()
        {
            bool bolStoppedServer = false;
            
            //We shouldn't run this if the server is online, so check
            if (this.Server.Running)
            {
                //It is running, if it's empty we could stop it
                if (this.Server.Players.Count == 0)
                {
                    Database.AddLog("Stopping server for " + this.Name + ".", this.BaseName, "info", false, this.Server.ServerID);
                    this.Server.Stop();
                    bolStoppedServer = true;
                }
                else
                {
                    //There are people on, so warn and dump out of here
                    Database.AddLog("Couldn't run " + this.Name + " as server running and not empty.", this.BaseName, "warn", true, this.Server.ServerID);
                    this.Complete = true;
                    this.Result = false;
                    return;
                }
            }

            //Build arguements
            string strArgs = "";
            
            //First run the biome extractor tool
            Process prc = new Process();
            prc.StartInfo.UseShellExecute = false;
            prc.StartInfo.FileName = this.FullExePath;
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

            Database.AddLog("Biome Extractor Started", this.BaseName);

            while (!prc.WaitForExit(1000)) ;

            if (prc.ExitCode == 0)
            {
                Database.AddLog("Biome Extractor Completed", this.BaseName);
                this.Complete = true;
                this.Result = true;
            }
            else
            {
                Database.AddLog("Biome Extractor Failed: " + prc.ExitCode, this.BaseName, "error");
                this.Complete = true;
                this.Result = false;
            }

            //Did we stop the server earlier?
            if (bolStoppedServer)
            {
                Database.AddLog("Restarting server after Biome Extractor completed.", this.BaseName, "info", false, this.Server.ServerID);
                this.Server.Start();
            }

            //Must always call this to let base class know we're done
            this.Finish();
        }

        //Catch what the process sends back
        private void ProcessOutput(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "info", false, this.Server.ServerID);
        }
        private void ProcessError(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "error", false, this.Server.ServerID);
        }

    }
}
