using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;

namespace YAMS
{
    public static class Server
    {
        public static bool bolServerRunning = false;
        public static bool bolEnableJavaOptimisations = true;
        public static int intAssignedMem = 1024;
        
        private static string strRootFolder = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        private static Regex regRemoveDateStamp = new Regex(@"([0-9]+\-[0-9]+\-[0-9]+ [0-9]+:[0-9]+:[0-9]+ ){1}");
        private static Regex regErrorLevel = new Regex(@"\[([A-Z])+\]{1}");
        private static Regex regPlayerChat = new Regex(@"(\<([A-Za-z0-9])+\>){1}");

        public static Process prcMinecraft;

        private static int intRestartSeconds = 0;
        private static Timer timRestarter;

        public static void Start()
        {
            if (bolServerRunning) return;

            //First check if an update is waiting to be applied
            if (!YAMS.Util.ReplaceFile(strRootFolder + "\\lib\\minecraft_server.jar", strRootFolder + "\\lib\\minecraft_server.jar.UPDATE")) return;

            //Also check if a new properties file is to be applied
            if (!YAMS.Util.ReplaceFile(strRootFolder + "\\server\\server.properties", strRootFolder + "\\server\\server.properties.UPDATE")) return;

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
                YAMS.Database.AddLog("Server Started", "server");

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
            prcMinecraft.WaitForExit();
            prcMinecraft.CancelErrorRead();
            prcMinecraft.CancelOutputRead();
            YAMS.Database.AddLog("Server Stopped", "server");
            bolServerRunning = false;
        }

        public static void Restart()
        {
            Stop();
            System.Threading.Thread.Sleep(5000);
            Start();
        }

        //Restart the server after specified number of seconds and warn users it's going to happen
        public static void DelayedRestart(int intSeconds)
        {
            intRestartSeconds = intSeconds;
            timRestarter = new Timer();
            timRestarter.Interval = 1000; //Every second as we want to update the players
            timRestarter.Elapsed += new ElapsedEventHandler(RestarterTick);
            timRestarter.Enabled = true;
            YAMS.Database.AddLog("AutoRestart initiated with " + intSeconds.ToString() + " second timer", "server");

        }
        private static void RestarterTick(object source, ElapsedEventArgs e)
        {
            //How may seconds are left?  Send appropriate message
            if (intRestartSeconds > 100)
            {
                if (intRestartSeconds % 10 == 0) Send("say Server will restart in " + intRestartSeconds.ToString() + " seconds.");
            }
            else if (intRestartSeconds <= 100 && intRestartSeconds > 10)
            {
                if (intRestartSeconds % 5 == 0) Send("say Server will restart in " + intRestartSeconds.ToString() + " seconds.");
            }
            else if (intRestartSeconds <= 10  && intRestartSeconds > 0) {
                Send("say Server will restart in " + intRestartSeconds.ToString() + " seconds.");
            }
            else if (intRestartSeconds <= 0)
            {
               timRestarter.Enabled = false;
               Restart();
            }

            intRestartSeconds--;
        }

        //Send command to stdin on the server process
        public static void Send(string strMessage)
        {
            if (!bolServerRunning || prcMinecraft == null || prcMinecraft.HasExited) return;
            prcMinecraft.StandardInput.WriteLine(strMessage);
        }

        //Catch the output from the server process
        private static void ServerOutput(object sender, DataReceivedEventArgs e) { if (e.Data != null) YAMS.Database.AddLog(e.Data, "server", "info"); }
        private static void ServerError(object sender, DataReceivedEventArgs e)
        {
            //Catch null messages (usually as server is going down)
            if (e.Data == null) return;
           
            //MC's server seems to use stderr for things that aren't really errors, so we need some logic to catch that.
            string strLevel = "info";
            string strMessage = e.Data;
            //Strip out date and time info
            strMessage = regRemoveDateStamp.Replace(strMessage, "");

            //Work out the error level then remove it from the string
            Match regMatch = regErrorLevel.Match(strMessage);
            strMessage = regErrorLevel.Replace(strMessage, "").Trim();

            if (regMatch.Success)
            {
                switch (regMatch.Groups[0].Value)
                {
                    case "[INFO]":
                        //Check if it's player chat
                        Match regChat = regPlayerChat.Match(strMessage);
                        if (regChat.Success) strLevel = "chat";
                        else strLevel = "info";
                        break;
                    case "[WARNING]":
                        strLevel = "warn";
                        break;
                    default:
                        strLevel = "error";
                        break;
                }
            }
            else { strLevel = "error"; }

            YAMS.Database.AddLog(strMessage, "server", strLevel);
        }
    }
}
