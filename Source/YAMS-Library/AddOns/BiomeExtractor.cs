using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using YAMS;

namespace YAMS.AddOns
{
    class BiomeExtractor : App
    {
        
        public BiomeExtractor(MCServer s, string strParams = "")
            : base(s, "biome-extractor", @"MinecraftBiomeExtractor.jar", "Biome Extractor", true, strParams) {}

        public override void DoWork()
        {
            bool bolStoppedServer = false;
            
            //We shouldn't run this if the server is online, so check
            if (this.Server.Running)
            {
                //It is running, if it's empty we could stop it
                if (this.Server.Players.Count == 0)
                {
                    Database.AddLog("Stopping server for Biome Extractor.", this.BaseName, "info", false, this.Server.ServerID);
                    this.Server.Stop();
                    bolStoppedServer = true;
                }
                else
                {
                    //There are people on, so warn and dump out of here
                    Database.AddLog("Couldn't run Biome Extractor as server running and not empty.", this.BaseName, "warn", true, this.Server.ServerID);
                    this.Complete = true;
                    this.Result = false;
                    return;
                }
            }
            
            //First run the biome extractor tool
            Process prcBiomeExtractor = new Process();
            prcBiomeExtractor.StartInfo.UseShellExecute = false;
            prcBiomeExtractor.StartInfo.FileName = YAMS.Util.JavaPath() + "java.exe";
            prcBiomeExtractor.StartInfo.Arguments = "-jar MinecraftBiomeExtractor.jar -nogui \"" + Core.StoragePath + this.Server.ServerID + "\\world\"";
            prcBiomeExtractor.StartInfo.CreateNoWindow = true;
            prcBiomeExtractor.StartInfo.RedirectStandardError = true;
            prcBiomeExtractor.StartInfo.RedirectStandardInput = true;
            prcBiomeExtractor.StartInfo.RedirectStandardOutput = true;
            prcBiomeExtractor.StartInfo.WorkingDirectory = this.FullFolderPath;

            //Set up events
            prcBiomeExtractor.OutputDataReceived += new DataReceivedEventHandler(BiomeExtractorOutput);
            prcBiomeExtractor.ErrorDataReceived += new DataReceivedEventHandler(BiomeExtractorError);
            prcBiomeExtractor.EnableRaisingEvents = true;

            //Finally start the thing
            prcBiomeExtractor.Start();
            prcBiomeExtractor.BeginOutputReadLine();
            prcBiomeExtractor.BeginErrorReadLine();

            Database.AddLog("Biome Extractor Started", this.BaseName);

            while (!prcBiomeExtractor.WaitForExit(1000)) ;

            if (prcBiomeExtractor.ExitCode == 0)
            {
                Database.AddLog("Biome Extractor Completed", this.BaseName);
                this.Complete = true;
                this.Result = true;
            }
            else
            {
                Database.AddLog("Biome Extractor Failed: " + prcBiomeExtractor.ExitCode, this.BaseName, "error");
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
        private void BiomeExtractorOutput(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "info", false, this.Server.ServerID);
        }
        private void BiomeExtractorError(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "error", false, this.Server.ServerID);
        }

    }
}
