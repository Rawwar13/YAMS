using System;
using System.Diagnostics;
using System.IO;

namespace YAMS
{
    public static class Server
    {
        public static bool bolServerRunning = false;
        public static bool bolEnableJavaOptimisations = false;
        public static int intAssignedMem = 1024;

        public static Process prcMinecraft;

        public static void Start()
        {
            if (bolServerRunning) return;

            prcMinecraft = new Process();

            try
            {
                //Basic arguments in all circumstances
                var strArgs = "-Xmx" + intAssignedMem + "M -Xms" + intAssignedMem + "M -jar ..\\lib\\minecraft_server.jar nogui";

                //If we have enabled the java optimisations add the additional
                //arguments. See http://www.minecraftforum.net/viewtopic.php?f=1012&t=68128
                if (bolEnableJavaOptimisations && YAMS.Util.HasJDK())
                {
                    var intGCCores = Environment.ProcessorCount - 1;
                    if (intGCCores == 0) intGCCores = 1;
                    strArgs = "-server -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:ParallelGCThreads=" + intGCCores + " -XX:+AggressiveOpts " + strArgs;
                }
                prcMinecraft.StartInfo.UseShellExecute = false;
                prcMinecraft.StartInfo.FileName = YAMS.Util.JavaPath() + "javaw.exe";
                prcMinecraft.StartInfo.Arguments = strArgs;
                prcMinecraft.StartInfo.CreateNoWindow = true;
                prcMinecraft.StartInfo.RedirectStandardError = true;
                prcMinecraft.StartInfo.RedirectStandardInput = true;
                prcMinecraft.StartInfo.RedirectStandardOutput = true;
                prcMinecraft.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\server\\";

                //Set up events
                prcMinecraft.OutputDataReceived += new DataReceivedEventHandler(ServerOutput);
                prcMinecraft.ErrorDataReceived += new DataReceivedEventHandler(ServerError);
                
                //Finally start the thing
                prcMinecraft.Start();
                prcMinecraft.BeginOutputReadLine();
                prcMinecraft.BeginErrorReadLine();

                bolServerRunning = true;
                YAMS.Database.AddLog("Server Started", "library");

            }
            catch (Exception e)
            {
                YAMS.Database.AddLog("Failed to start Server: " + e.Message, "library", "error");
            }
            
        }

        public static void Stop()
        {
            if (!bolServerRunning) return;
            Send("stop");
            prcMinecraft.EnableRaisingEvents = false;
            prcMinecraft.CancelErrorRead();
            prcMinecraft.CancelOutputRead();
            //prcMinecraft.Kill();
            prcMinecraft.WaitForExit();
            YAMS.Database.AddLog("Server Stopped", "library");
            bolServerRunning = false;
        }

        public static void Send(string strMessage)
        {
            if (!bolServerRunning || prcMinecraft == null || prcMinecraft.HasExited) return;
            prcMinecraft.StandardInput.WriteLine(strMessage);
        }

        private static void ServerOutput(object sender, DataReceivedEventArgs e) { YAMS.Database.AddLog(e.Data, "server"); }
        private static void ServerError(object sender, DataReceivedEventArgs e) { YAMS.Database.AddLog(e.Data, "server", "error"); }
    }
}
