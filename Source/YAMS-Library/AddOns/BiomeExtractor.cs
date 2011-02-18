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
        
        public BiomeExtractor(MCServer s)
            : base(s, "biome-extractor", @"MinecraftBiomeExtractor.jar", "Biome Extractor", true) {}

        public override void DoWork()
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
            
            //Must always call this to let base class know we're done
            this.Finish();
        }

        //Catch what the process sends back
        private void BiomeExtractorOutput(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName);
        }
        private void BiomeExtractorError(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "error");
        }

    }
}
