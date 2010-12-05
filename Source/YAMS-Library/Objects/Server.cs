using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Net;
using System.Collections.Generic;

namespace YAMS
{
    public class MCServer
    {
        public bool Running = false;
        private bool bolEnableJavaOptimisations = true;
        private int intAssignedMem = 1024;
        
        private string strWorkingDir = "";

        private Regex regRemoveDateStamp = new Regex(@"([0-9]+\-[0-9]+\-[0-9]+ [0-9]+:[0-9]+:[0-9]+ ){1}");
        private Regex regErrorLevel = new Regex(@"\[([A-Z])+\]{1}");
        private Regex regPlayerChat = new Regex(@"(\<([A-Za-z0-9])+\>){1}");
        private Regex regPlayerLoggedIn = new Regex(@"([\w]+)(?: \[\/[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+\:[0-9]+\] logged in with entity id)");
        private Regex regPlayerLoggedOut = new Regex(@"([a-zA-Z0-9]+) ?(lost connection)");

        public Process prcMinecraft;

        private int intRestartSeconds = 0;
        private Timer timRestarter;

        public List<string> Players = new List<string> { };

        public int ServerID;

        public MCServer(int intServerID)
        {
            this.ServerID = intServerID;
            this.strWorkingDir = YAMS.Core.RootFolder + "\\servers\\" + this.ServerID.ToString() + "\\config\\";

            this.bolEnableJavaOptimisations = Convert.ToBoolean(YAMS.Database.GetSetting(this.ServerID, "ServerEnableOptimisations"));
            this.intAssignedMem = Convert.ToInt32(YAMS.Database.GetSetting(this.ServerID, "ServerAssignedMemory"));
        }

        public void Start()
        {
            if (this.Running) return;

            //First check if an update is waiting to be applied
            if (!YAMS.Util.ReplaceFile(YAMS.Core.RootFolder + "\\lib\\minecraft_server.jar", YAMS.Core.RootFolder + "\\lib\\minecraft_server.jar.UPDATE")) return;

            //Also check if a new properties file is to be applied
            if (!YAMS.Util.ReplaceFile(this.strWorkingDir + "server.properties", this.strWorkingDir + "server.properties.UPDATE")) return;

            this.prcMinecraft = new Process();

            try
            {
                //Basic arguments in all circumstances
                var strArgs = "-Xmx" + intAssignedMem + "M -Xms" + intAssignedMem + @"M -jar ..\..\..\lib\minecraft_server.jar nogui";
                var strFileName = YAMS.Util.JavaPath() + "javaw.exe";

                //If we have enabled the java optimisations add the additional
                //arguments. See http://www.minecraftforum.net/viewtopic.php?f=1012&t=68128
                if (bolEnableJavaOptimisations && YAMS.Util.HasJDK())
                {
                    var intGCCores = Environment.ProcessorCount - 1;
                    if (intGCCores == 0) intGCCores = 1;
                    strArgs = "-server -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:ParallelGCThreads=" + intGCCores + " -XX:+AggressiveOpts " + strArgs;
                    strFileName = YAMS.Util.JavaPath("jdk") + "javaw.exe";
                }
                this.prcMinecraft.StartInfo.UseShellExecute = false;
                this.prcMinecraft.StartInfo.FileName = strFileName;
                this.prcMinecraft.StartInfo.Arguments = strArgs;
                this.prcMinecraft.StartInfo.CreateNoWindow = true;
                this.prcMinecraft.StartInfo.RedirectStandardError = true;
                this.prcMinecraft.StartInfo.RedirectStandardInput = true;
                this.prcMinecraft.StartInfo.RedirectStandardOutput = true;
                this.prcMinecraft.StartInfo.WorkingDirectory = this.strWorkingDir;

                //Set up events
                this.prcMinecraft.OutputDataReceived += new DataReceivedEventHandler(ServerOutput);
                this.prcMinecraft.ErrorDataReceived += new DataReceivedEventHandler(ServerError);
                
                //Finally start the thing
                this.prcMinecraft.Start();
                this.prcMinecraft.BeginOutputReadLine();
                this.prcMinecraft.BeginErrorReadLine();

                this.Running = true;
                YAMS.Database.AddLog("Server Started", "server", "info", false, this.ServerID);

                //Start our wrapper
                //YAMS.Wrapper.Listen wrapper = new YAMS.Wrapper.Listen(IPAddress.Parse(YAMS.Database.GetSetting("ListenIP", "YAMS")), Convert.ToInt32(YAMS.Database.GetSetting("ListenPort", "YAMS")));

            }
            catch (Exception e)
            {
                YAMS.Database.AddLog("Failed to start Server: " + e.Message, "library", "error", false, this.ServerID);
            }
            
        }

        public void Stop()
        {
            if (!Running) return;
            this.Send("stop");
            this.prcMinecraft.WaitForExit();
            this.prcMinecraft.CancelErrorRead();
            this.prcMinecraft.CancelOutputRead();
            YAMS.Database.AddLog("Server Stopped", "server", "info", false, this.ServerID);
            this.Running = false;
        }

        public void Restart()
        {
            this.Stop();
            System.Threading.Thread.Sleep(5000);
            this.Start();
        }

        //Restart the server after specified number of seconds and warn users it's going to happen
        public void DelayedRestart(int intSeconds)
        {
            this.intRestartSeconds = intSeconds;
            this.timRestarter = new Timer();
            this.timRestarter.Interval = 1000; //Every second as we want to update the players
            this.timRestarter.Elapsed += new ElapsedEventHandler(RestarterTick);
            this.timRestarter.Enabled = true;
            YAMS.Database.AddLog("AutoRestart initiated with " + intSeconds.ToString() + " second timer", "server", "info", false, this.ServerID);

        }
        private void RestarterTick(object source, ElapsedEventArgs e)
        {
            //How may seconds are left?  Send appropriate message
            if (this.intRestartSeconds > 100)
            {
                if (this.intRestartSeconds % 10 == 0) Send("say Server will restart in " + this.intRestartSeconds.ToString() + " seconds.");
            }
            else if (this.intRestartSeconds <= 100 && this.intRestartSeconds > 10)
            {
                if (this.intRestartSeconds % 5 == 0) Send("say Server will restart in " + this.intRestartSeconds.ToString() + " seconds.");
            }
            else if (this.intRestartSeconds <= 10 && this.intRestartSeconds > 0)
            {
                Send("say Server will restart in " + this.intRestartSeconds.ToString() + " seconds.");
            }
            else if (this.intRestartSeconds <= 0)
            {
               timRestarter.Enabled = false;
               Restart();
            }

            this.intRestartSeconds--;
        }
        public void CancelDelayedRestart()
        {
            this.timRestarter.Enabled = false;
            this.intRestartSeconds = 0;
            YAMS.Database.AddLog("Delayed restart cancelled", "server", "info", false, this.ServerID);
        }

        //Send command to stdin on the server process
        public void Send(string strMessage)
        {
            if (!this.Running || this.prcMinecraft == null || this.prcMinecraft.HasExited) return;
            this.prcMinecraft.StandardInput.WriteLine(strMessage);
        }

        //Catch the output from the server process
        private void ServerOutput(object sender, DataReceivedEventArgs e) { if (e.Data != null) YAMS.Database.AddLog(e.Data, "server", "out", false, this.ServerID); }
        private void ServerError(object sender, DataReceivedEventArgs e)
        {
            //Catch null messages (usually as server is going down)
            if (e.Data == null) return;
           
            //MC's server seems to use stderr for things that aren't really errors, so we need some logic to catch that.
            string strLevel = "info";
            string strMessage = e.Data;
            //Strip out date and time info
            strMessage = this.regRemoveDateStamp.Replace(strMessage, "");

            //Work out the error level then remove it from the string
            Match regMatch = this.regErrorLevel.Match(strMessage);
            strMessage = this.regErrorLevel.Replace(strMessage, "").Trim();

            if (regMatch.Success)
            {
                switch (regMatch.Groups[0].Value)
                {
                    case "[INFO]":
                        //Check if it's player chat
                        Match regChat = this.regPlayerChat.Match(strMessage);
                        if (regChat.Success) strLevel = "chat";
                        else strLevel = "info";
                        
                        //See if it's a log in or log out event
                        Match regLogIn = this.regPlayerLoggedIn.Match(strMessage);
                        Match regLogOut = this.regPlayerLoggedOut.Match(strMessage);
                        if (regLogIn.Success) Players.Add(regLogIn.Groups[1].Value); //is a login event
                        if (regLogOut.Success) Players.Remove(regLogOut.Groups[1].Value); //logout event
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

            YAMS.Database.AddLog(strMessage, "server", strLevel, false, this.ServerID);
        }
    }
}
