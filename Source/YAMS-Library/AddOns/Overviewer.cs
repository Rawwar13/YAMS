using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using YAMS;

namespace YAMS.AddOns
{
    class Overviewer : App
    {
        public Overviewer(MCServer s, string strParams = "rendermodes=normal,lighting,night")
            : base(s, "overviewer", @"overviewer.exe", "Overviewer", true, strParams) {}

        public override void DoWork()
        {

            //Force a server save and turn off level saving
            this.Server.Save();
            this.Server.DisableSaving();

            //If we have Biome Extractor installed, we should run it
            BiomeExtractor BE = new BiomeExtractor(this.Server);
            if (BE.IsInstalled)
            {
                BE.Start();
                while (!BE.Complete) Thread.Sleep(5000);
            }

            string ServerRoot = this.Server.ServerDirectory;
            //Check the proper folders exist
            if (!Directory.Exists(ServerRoot + @"\renders\overviewer\")) Directory.CreateDirectory(ServerRoot + @"\renders\overviewer\");
            if (!Directory.Exists(ServerRoot + @"\renders\overviewer\output\")) Directory.CreateDirectory(ServerRoot + @"\renders\overviewer\output\");
            string strArgs = "";

            if (this.jobParams.ContainsKey("rendermodes"))
            {
                strArgs += " --rendermodes=" + jobParams["rendermodes"];
            }
            
            strArgs += " \"" + ServerRoot + "\\world\" \"" + ServerRoot + "renders\\overviewer\\output\"";

            Process prcOverviewer = new Process();
            prcOverviewer.StartInfo.UseShellExecute = false;
            prcOverviewer.StartInfo.FileName = this.FullExePath;
            prcOverviewer.StartInfo.Arguments = strArgs;
            prcOverviewer.StartInfo.CreateNoWindow = true;
            prcOverviewer.StartInfo.RedirectStandardError = true;
            prcOverviewer.StartInfo.RedirectStandardInput = true;
            prcOverviewer.StartInfo.RedirectStandardOutput = true;
            prcOverviewer.StartInfo.WorkingDirectory = this.FullFolderPath;

            //Set up events
            prcOverviewer.OutputDataReceived += new DataReceivedEventHandler(OverviewerOutput);
            prcOverviewer.ErrorDataReceived += new DataReceivedEventHandler(OverviewerError);
            prcOverviewer.EnableRaisingEvents = true;

            //Finally start the thing
            prcOverviewer.Start();
            prcOverviewer.BeginOutputReadLine();
            prcOverviewer.BeginErrorReadLine();

            Database.AddLog("Overviewer Path: " + strArgs, this.BaseName);
            Database.AddLog("Overviewer Started", this.BaseName);

            while (!prcOverviewer.WaitForExit(1000)) ;

            if (prcOverviewer.ExitCode == 0)
            {
                Database.AddLog("Overviewer Completed", this.BaseName);
                this.Complete = true;
                this.Result = true;
            }
            else
            {
                Database.AddLog("Overviewer Failed: " + prcOverviewer.ExitCode, this.BaseName, "error");
                this.Complete = true;
                this.Result = false;
            }
            Thread.Sleep(10000);

            //Re-enable server saving and updating
            this.Server.EnableSaving();

            //Must always call this to let base class know we're done
            this.Finish();
        }

        private void OverviewerOutput(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "info", false, this.Server.ServerID);
        }
        private void OverviewerError(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null)
            {
                string strMessage = e.Data;
                string strLevel = "info";

                //strip date/time
                strMessage = new Regex(@"^(([0-9]+)-([0-9]+)-([0-9]+) ([0-9]+):([0-9]+):([0-9]+),([0-9]+) )").Replace(strMessage, "");

                Regex regLevel = new Regex(@"^\[([A-Z]+)\] ");
                if (regLevel.Match(strMessage).Success)
                {
                    switch (regLevel.Match(strMessage).Groups[1].Value)
                    {
                        case "INFO":
                            strLevel = "info";
                            break;
                        default:
                            strLevel = "warn";
                            break;
                    }
                }
                strMessage = regLevel.Replace(strMessage, "");

                Database.AddLog(datTimeStamp, strMessage, this.BaseName, strLevel, false, this.Server.ServerID);
            }
        }

    }
}
