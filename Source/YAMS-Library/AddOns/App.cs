using System;
using System.IO;
using System.Threading;
using YAMS;

namespace YAMS.AddOns
{
    public abstract class App
    {
        //We're usually going to operate on a server
        public MCServer Server;

        //Where is the main executable?
        protected string MainExe = @"nowhere.exe";
        protected string FullFolderPath = "";
        protected string FullExePath = "";

        //What's it called
        protected string Name = "App";
        protected string BaseName = "app";

        //Do we need the client?
        protected bool RequiresClient = false;

        //Is it even installed?
        public bool IsInstalled = false;

        //How did it all go?
        public bool Complete = false;
        public bool Result = false;

        //Are we doing something?
        public bool Running = false;

        //Set this class's server
        public App(MCServer s, string strBaseName, string strMainExe, string strName, bool bolRequiresClient)
        {
            this.BaseName = strBaseName;
            this.MainExe = strMainExe;
            this.Name = strName;
            this.RequiresClient = bolRequiresClient;
            this.FullFolderPath = Core.RootFolder + @"\apps\" + this.BaseName;
            this.FullExePath = this.FullFolderPath + @"\" + this.MainExe;
            if (File.Exists(this.FullExePath))
            {
                this.IsInstalled = true;
                this.Server = s;
            }
            else
            {
                Database.AddLog(this.Name + " is not installed", "addons", "error");
            }
        }

        //Start doing work, can't think of a situation where we *wouldn't* want this in a new thread.
        public void Start()
        {
            if (this.IsInstalled)
            {
                if ((this.RequiresClient && Util.HasMCClientSystem()) || !this.RequiresClient)
                {
                    this.Running = true;
                    ThreadStart threadDelegate = new ThreadStart(this.DoWork);
                    Thread newThread = new Thread(threadDelegate);
                    newThread.Start();
                }
                else
                {
                    Database.AddLog(this.Name + " requires the MC client installed", "addons", "error");
                }
            }
        }

        //When we're done, make sure it's known.
        public virtual void Finish()
        {
            this.Running = false;
        }

        //The workhorse of any app should always call Finish() when done.
        public abstract void DoWork();

    }
}
