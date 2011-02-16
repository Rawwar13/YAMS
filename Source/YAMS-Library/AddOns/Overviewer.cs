using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using YAMS;

namespace YAMS.Apps
{
    class Overviewer
    {

        private MCServer Server;

        private bool BiomeExtractorComplete = false;
        private bool BiomeExtractorResult = false;

        public Overviewer(MCServer Server)
        {
            this.Server = Server;
        }

        public void Start()
        {
            this.Server.Send("save-all");
            Thread.Sleep(10000);
            this.Server.Send("save-off");
            Thread.Sleep(10000);
            this.ExtractBiomes();
            while (!this.BiomeExtractorComplete)
            {
                Thread.Sleep(5000);
            }
            if (this.BiomeExtractorResult)
            {
                RunOverviewer();
            }
            else
            {
                Database.AddLog("Extractor failed", "overviewer");
            }
            this.Server.Send("save-on");
        }

        public void ExtractBiomes()
        {
            //First run the biome extractor tool
            Process prcBiomeExtractor = new Process();
            prcBiomeExtractor.StartInfo.UseShellExecute = false;
            prcBiomeExtractor.StartInfo.FileName = YAMS.Util.JavaPath() + "java.exe";
            prcBiomeExtractor.StartInfo.Arguments = "-jar MinecraftBiomeExtractor.jar -nogui \"" + Core.RootFolder + "\\servers\\" + this.Server.ServerID + "\\world\"";
            prcBiomeExtractor.StartInfo.CreateNoWindow = true;
            prcBiomeExtractor.StartInfo.RedirectStandardError = true;
            prcBiomeExtractor.StartInfo.RedirectStandardInput = true;
            prcBiomeExtractor.StartInfo.RedirectStandardOutput = true;
            prcBiomeExtractor.StartInfo.WorkingDirectory = YAMS.Core.RootFolder + @"\apps\biome-extractor\";

            //Set up events
            prcBiomeExtractor.OutputDataReceived += new DataReceivedEventHandler(BiomeExtractorOutput);
            prcBiomeExtractor.ErrorDataReceived += new DataReceivedEventHandler(BiomeExtractorError);
            prcBiomeExtractor.EnableRaisingEvents = true;

            //Finally start the thing
            prcBiomeExtractor.Start();
            prcBiomeExtractor.BeginOutputReadLine();
            prcBiomeExtractor.BeginErrorReadLine();

            Database.AddLog("Biome Extractor Started", "overviewer");

            while (!prcBiomeExtractor.WaitForExit(1000)) ;

            if (prcBiomeExtractor.ExitCode == 0) {
                Database.AddLog("Biome Extractor Completed", "overviewer");
                this.BiomeExtractorComplete = true;
                this.BiomeExtractorResult = true;
            }
            else {
                Database.AddLog("Biome Extractor Failed: " + prcBiomeExtractor.ExitCode, "overviewer", "error");
                this.BiomeExtractorComplete = true;
                this.BiomeExtractorResult = false;
            }
        }

        private void BiomeExtractorOutput(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, "overviewer");
        }
        private void BiomeExtractorError(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, "overviewer", "error");
        }


        public void RunOverviewer()
        {
            string ServerRoot = Core.RootFolder + @"\servers\" + this.Server.ServerID;

            string strArgs = "--lighting --cachedir=\"" + ServerRoot + "\\renders\\gmap\\cache\" \"" + ServerRoot + "\\world\" \"" + ServerRoot + "\\renders\\gmap\\output\"";

            //First run the biome extractor tool
            Process prcBiomeExtractor = new Process();
            prcBiomeExtractor.StartInfo.UseShellExecute = false;
            prcBiomeExtractor.StartInfo.FileName = Core.RootFolder + @"\apps\overviewer\gmap.exe";
            prcBiomeExtractor.StartInfo.Arguments = strArgs;
            prcBiomeExtractor.StartInfo.CreateNoWindow = true;
            prcBiomeExtractor.StartInfo.RedirectStandardError = true;
            prcBiomeExtractor.StartInfo.RedirectStandardInput = true;
            prcBiomeExtractor.StartInfo.RedirectStandardOutput = true;
            prcBiomeExtractor.StartInfo.WorkingDirectory = YAMS.Core.RootFolder + @"\apps\overviewer\";

            //Set up events
            prcBiomeExtractor.OutputDataReceived += new DataReceivedEventHandler(OverviewerOutput);
            prcBiomeExtractor.ErrorDataReceived += new DataReceivedEventHandler(OverviewerError);
            prcBiomeExtractor.EnableRaisingEvents = true;

            //Finally start the thing
            prcBiomeExtractor.Start();
            prcBiomeExtractor.BeginOutputReadLine();
            prcBiomeExtractor.BeginErrorReadLine();

            Database.AddLog("Overviewer Path: " + strArgs, "overviewer");
            Database.AddLog("Overviewer Started", "overviewer");

            while (!prcBiomeExtractor.WaitForExit(1000)) ;

            if (prcBiomeExtractor.ExitCode == 0)
            {
                Database.AddLog("Overviewer Completed", "overviewer");
                this.BiomeExtractorResult = true;
            }
            else
            {
                Database.AddLog("Overviewer Failed: " + prcBiomeExtractor.ExitCode, "overviewer", "error");
                this.BiomeExtractorResult = false;
            }
            Thread.Sleep(10000);
        }

        private void OverviewerOutput(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, "overviewer");
        }
        private void OverviewerError(object sender, DataReceivedEventArgs e) {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, "overviewer", "error");
        }

    }
}
