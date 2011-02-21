using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using YAMS;

namespace YAMS.AddOns
{
    class c10t : App
    {
        
        public c10t(MCServer s, string strParams = "night=false&mode=normal")
            : base(s, "c10t", @"c10t.exe", "c10t", true, strParams) {}

        public override void DoWork()
        {

            string strArgs = "";
            string ServerRoot = this.Server.ServerDirectory;

            //World path
            strArgs += "-w \"" + ServerRoot + "\\world\" ";

            //Decide file name
            string strFileName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;
            //Night mode
            if (this.jobParams.ContainsKey("night"))
            {
                if (jobParams["night"] == "true")
                {
                    strFileName += "-night";
                    strArgs += " -n";
                }
                else
                {
                    strFileName += "-day";
                }
            }
            //Rendering Mode
            if (this.jobParams.ContainsKey("mode"))
            {
                if (jobParams["mode"] == "oblique")
                {
                    strFileName += "-oblique";
                    strArgs += " -q";
                }
                else if (jobParams["mode"] == "isometric")
                {
                    strFileName += "-isometric";
                    strArgs += " -z";
                }
                else if (jobParams["mode"] == "normal")
                {
                    strFileName += "-normal";
                }
            }

            strFileName += ".png";

            //How many threads to use?
            var intThreads = Environment.ProcessorCount;
            strArgs += " -m " + intThreads;

            //Output file
            strArgs += " -o \"" + ServerRoot + "\\renders\\" + strFileName;

            Process prcc10t = new Process();
            prcc10t.StartInfo.UseShellExecute = false;
            prcc10t.StartInfo.FileName = this.FullExePath;
            prcc10t.StartInfo.Arguments = strArgs;
            prcc10t.StartInfo.CreateNoWindow = true;
            prcc10t.StartInfo.RedirectStandardError = true;
            prcc10t.StartInfo.RedirectStandardInput = true;
            prcc10t.StartInfo.RedirectStandardOutput = true;
            prcc10t.StartInfo.WorkingDirectory = this.FullFolderPath;

            //Set up events
            prcc10t.OutputDataReceived += new DataReceivedEventHandler(AppOutput);
            prcc10t.ErrorDataReceived += new DataReceivedEventHandler(AppError);
            prcc10t.EnableRaisingEvents = true;

            //Finally start the thing
            prcc10t.Start();
            prcc10t.BeginOutputReadLine();
            prcc10t.BeginErrorReadLine();

            Database.AddLog(this.Name + " Started", this.BaseName);

            while (!prcc10t.WaitForExit(1000)) ;

            if (prcc10t.ExitCode == 0)
            {
                Database.AddLog(this.Name + " Completed", this.BaseName);
                this.Complete = true;
                this.Result = true;
            }
            else
            {
                Database.AddLog(this.Name + " Failed: " + prcc10t.ExitCode, this.BaseName, "error");
                this.Complete = true;
                this.Result = false;
            }

            //Must always call this to let base class know we're done
            this.Finish();
        }

        //Catch what the process sends back
        private void AppOutput(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName);
        }
        private void AppError(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            if (e.Data != null) Database.AddLog(datTimeStamp, e.Data, this.BaseName, "error");
        }

    }
}
