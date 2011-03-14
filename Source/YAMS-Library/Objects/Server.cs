using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Net;
using System.Collections.Generic;
using YAMS;
using YAMS.Objects;

namespace YAMS
{
    public class MCServer
    {
        public bool Running = false;
        private bool bolEnableJavaOptimisations = true;
        private int intAssignedMem = 1024;
        
        private string strWorkingDir = "";
        public string ServerDirectory;

        private Regex regRemoveDateStamp = new Regex(@"^([0-9]+\-[0-9]+\-[0-9]+ [0-9]+:[0-9]+:[0-9]+ ){1}");
        private Regex regErrorLevel = new Regex(@"^\[([A-Z])+\]{1}");
        private Regex regPlayerChat = new Regex(@"^(\<([A-Za-z0-9])+\>){1}");
        private Regex regPlayerPM = new Regex(@"^(\[([\w])+\-\>(\w)+\]){1}");
        private Regex regPlayerLoggedIn = new Regex(@"^([\w]+)(?: \[\/[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+\:[0-9]+\] logged in with entity id)");
        private Regex regPlayerLoggedOut = new Regex(@"^([\w]+) ?(lost connection)");
        private Regex regServerVersion = new Regex(@"^(?:Starting minecraft server version )");

        public string ServerType = "vanilla";

        private AddOns.Overviewer gmap;

        public Process prcMinecraft;

        private int intRestartSeconds = 0;
        private Timer timRestarter;

        public int ServerID;
        public string ServerVersion = "";
        public string ServerTitle = "";
        public string LogonMode = "blacklist";
        public bool HasChanged = false;
        public int PID;

        public bool RestartNeeded = false;

        public bool RestartWhenFree = false;

        public Dictionary<string, Player> Players = new Dictionary<string, Player> { };

        public MCServer(int intServerID)
        {
            this.ServerID = intServerID;
            this.strWorkingDir = Core.StoragePath + this.ServerID.ToString() + "\\config\\";

            this.bolEnableJavaOptimisations = Convert.ToBoolean(Database.GetSetting(this.ServerID, "ServerEnableOptimisations"));
            this.intAssignedMem = Convert.ToInt32(Database.GetSetting(this.ServerID, "ServerAssignedMemory"));
            this.ServerTitle = Convert.ToString(Database.GetSetting(this.ServerID, "ServerTitle"));
            this.ServerType = Convert.ToString(Database.GetSetting(this.ServerID, "ServerType"));
            this.LogonMode = Convert.ToString(Database.GetSetting(this.ServerID, "ServerLogonMode"));

            this.ServerDirectory = Core.StoragePath + this.ServerID.ToString() + @"\";
        }

        public void Start()
        {
            if (this.Running) return;
            //Refresh Variables
            this.bolEnableJavaOptimisations = Convert.ToBoolean(Database.GetSetting(this.ServerID, "ServerEnableOptimisations"));
            this.intAssignedMem = Convert.ToInt32(Database.GetSetting(this.ServerID, "ServerAssignedMemory"));
            this.ServerTitle = Convert.ToString(Database.GetSetting(this.ServerID, "ServerTitle"));
            this.ServerType = Convert.ToString(Database.GetSetting(this.ServerID, "ServerType"));
            this.RestartWhenFree = false;
            this.RestartNeeded = false;

            //What type of server are we running?
            string strFile = "";
            switch (this.ServerType)
            {
                case "vanilla":
                    strFile = "minecraft_server.jar";
                    break;
                case "bukkit":
                    strFile = "craftbukkit.jar";
                    break;
                default:
                    strFile = "minecraft_server.jar";
                    break;
            }

            //First check if an update is waiting to be applied
            if (!Util.ReplaceFile(Core.RootFolder + "\\lib\\" + strFile, Core.RootFolder + "\\lib\\" + strFile + ".UPDATE")) return;

            //Also check if a new properties file is to be applied
            if (!Util.ReplaceFile(this.strWorkingDir + "server.properties", this.strWorkingDir + "server.properties.UPDATE")) return;

            this.prcMinecraft = new Process();

            try
            {
                //Basic arguments in all circumstances
                var strArgs = "-Xmx" + intAssignedMem + "M -Xms" + intAssignedMem + @"M -jar " + "\"" + Core.RootFolder + "\\lib\\";
                strArgs += strFile;
                strArgs += "\" nogui";
                var strFileName = YAMS.Util.JavaPath() + "java.exe";

                //If we have enabled the java optimisations add the additional
                //arguments. See http://www.minecraftforum.net/viewtopic.php?f=1012&t=68128
                if (bolEnableJavaOptimisations && YAMS.Util.HasJDK())
                {
                    var intGCCores = Environment.ProcessorCount - 1;
                    if (intGCCores == 0) intGCCores = 1;
                    strArgs = "-server -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:ParallelGCThreads=" + intGCCores + " -XX:+AggressiveOpts " + strArgs;
                    strFileName = YAMS.Util.JavaPath("jdk") + "java.exe";
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
                this.prcMinecraft.EnableRaisingEvents = true;
                this.prcMinecraft.Exited += new EventHandler(ServerExited);
                
                //Finally start the thing
                this.prcMinecraft.Start();
                this.prcMinecraft.BeginOutputReadLine();
                this.prcMinecraft.BeginErrorReadLine();

                this.Running = true;
                Database.AddLog("Server Started", "server", "info", false, this.ServerID);

                //Save the process ID so we can kill if there is a crash
                this.PID = this.prcMinecraft.Id;
                Util.AddPID(this.prcMinecraft.Id);
            }
            catch (Exception e)
            {
                Database.AddLog("Failed to start Server: " + e.Message, "library", "error", false, this.ServerID);
            }
            
        }

        public void Stop()
        {
            if (!Running) return;
            this.Send("stop");
            this.prcMinecraft.WaitForExit();
            this.prcMinecraft.CancelErrorRead();
            this.prcMinecraft.CancelOutputRead();
            Database.AddLog("Server Stopped", "server", "info", false, this.ServerID);
            this.Running = false;
        }

        public void Restart()
        {
            this.Stop();
            System.Threading.Thread.Sleep(10000);
            this.Start();
        }

        public void RestartIfEmpty()
        {
            if (this.Players.Count == 0)
            {
                this.Restart();
            }
            else
            {
                Database.AddLog("Deferred Restart until empty", "app", "warn", true, this.ServerID);
                this.RestartWhenFree = true;
            }
        }

        //Restart the server after specified number of seconds and warn users it's going to happen
        public void DelayedRestart(int intSeconds)
        {
            this.intRestartSeconds = intSeconds;
            this.timRestarter = new Timer();
            this.timRestarter.Interval = 1000; //Every second as we want to update the players
            this.timRestarter.Elapsed += new ElapsedEventHandler(RestarterTick);
            this.timRestarter.Enabled = true;
            Database.AddLog("AutoRestart initiated with " + intSeconds.ToString() + " second timer", "server", "info", false, this.ServerID);

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
               this.Restart();
            }

            this.intRestartSeconds--;
        }
        public void CancelDelayedRestart()
        {
            this.timRestarter.Enabled = false;
            this.intRestartSeconds = 0;
            Database.AddLog("Delayed restart cancelled", "server", "info", false, this.ServerID);
        }

        //Send command to stdin on the server process
        public void Send(string strMessage)
        {
            if (!this.Running || this.prcMinecraft == null || this.prcMinecraft.HasExited) return;
            this.prcMinecraft.StandardInput.WriteLine(strMessage);
        }

        //Some shortcut commands
        public void Save()
        {
            this.Send("save-all");
            //Generally this needs a long wait
            System.Threading.Thread.Sleep(10000);
        }
        public void EnableSaving()
        {
            this.Send("save-on");
            //Generally this needs a long wait
            System.Threading.Thread.Sleep(10000);
        }
        public void DisableSaving()
        {
            this.Send("save-off");
            //Generally this needs a long wait
            System.Threading.Thread.Sleep(10000);
        }
        public void Whisper(string strUsername, string strMessage)
        {
            this.Send("tell " + strUsername + " " + strMessage);
        }

        //Catch the output from the server process
        private void ServerOutput(object sender, DataReceivedEventArgs e) { if (e.Data != null) YAMS.Database.AddLog(DateTime.Now, e.Data, "server", "out", false, this.ServerID); }
        private void ServerError(object sender, DataReceivedEventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;

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
                        if (regPlayerChat.Match(strMessage).Success || regPlayerPM.Match(strMessage).Success) strLevel = "chat";
                        else strLevel = "info";
                        
                        //See if it's a log in or log out event
                        Match regLogIn = this.regPlayerLoggedIn.Match(strMessage);
                        Match regLogOut = this.regPlayerLoggedOut.Match(strMessage);
                        if (regLogIn.Success) this.PlayerLogin(regLogIn.Groups[1].Value); //is a login event
                        if (regLogOut.Success) this.PlayerLogout(regLogOut.Groups[1].Value); //logout event

                        //See if it's the server version tag
                        Match regVersion = this.regServerVersion.Match(strMessage);
                        if (regVersion.Success) this.ServerVersion = strMessage.Replace("Starting minecraft server version ", "");
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

            Database.AddLog(datTimeStamp, strMessage, "server", strLevel, false, this.ServerID);
        }

        private void ServerExited(object sender, EventArgs e)
        {
            DateTime datTimeStamp = DateTime.Now;
            Database.AddLog(datTimeStamp, "Server Exited", "server", "warn", false, this.ServerID);
            this.Running = false;
            Util.RemovePID(this.PID);
        }

        //Login and out events
        private void PlayerLogin(string strName)
        {
            this.Players.Add(strName, new Objects.Player(strName, this));
            this.HasChanged = true;
        }
        private void PlayerLogout(string strName)
        {
            this.Players.Remove(strName);
            //Check if we should restart the server for an update or a request
            if (this.RestartWhenFree) this.RestartIfEmpty();
        }

        //Returns the amount of RAM being used by this server
        public int GetMemory()
        {
            if (this.Running)
            {
                return Convert.ToInt32(this.prcMinecraft.WorkingSet64 / (1024 * 1024));
            }
            else { return 0; }
        }

        //Returns the amount of Virtual Memory being used by the server
        public int GetVMemory()
        {
            if (this.Running)
            {
                return Convert.ToInt32(this.prcMinecraft.VirtualMemorySize64 / (1024 * 1024));
            } else { return 0; }
        }

        //Call to create a google map of the world using Overviewer
        public void MapWorld()
        {
            this.gmap = new AddOns.Overviewer(this);
            this.gmap.Start();
        }

        //Read contents of a config file into a list
        public List<string> ReadConfig(string strFile)
        {
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader r = new StreamReader(this.strWorkingDir + strFile))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        if (line != "") lines.Add(line);
                    }
                }

                return lines;
            }
            catch (IOException e)
            {
                Database.AddLog("Exception reading config file " + strFile + ": " + e.Message, "web", "warn");
                return new List<string>();
            }
        }

    }
}
