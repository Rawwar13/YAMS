using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using YAMS;

namespace YAMS_Updater
{
    public partial class frmFirstRun : Form
    {
        public frmFirstRun()
        {
            InitializeComponent();
            tipYAMSUpdate.SetToolTip(this.chkUpdateYAMS, "Includes all YAMS components, you can specify individual components to update in the web app");
        
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/brownan/Minecraft-Overviewer");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://toolchain.eu/minecraft/c10t/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.minecraftforum.net/viewtopic.php?f=1022&t=80902");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.minecraftwiki.net/wiki/Tectonicus");
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            frmWorking WorkingForm = new frmWorking();
            WorkingForm.Show();

            //Directory Structure
            WorkingForm.lblCurrentItem.Text = "Creating directory structure";
            WorkingForm.prgCurrentTask.Value = 0;
            WorkingForm.Refresh();
            if (!Directory.Exists(Core.RootFolder + @"\apps\")) Directory.CreateDirectory(Core.RootFolder + @"\apps\");
            WorkingForm.prgCurrentTask.Value = 33;
            if (!Directory.Exists(Core.RootFolder + @"\lib\")) Directory.CreateDirectory(Core.RootFolder + @"\lib\");
            WorkingForm.prgCurrentTask.Value = 66;
            if (!Directory.Exists(Core.StoragePath)) Directory.CreateDirectory(Core.StoragePath);
            WorkingForm.prgCurrentTask.Value = 100;

            //Grab latest web app
            WorkingForm.lblCurrentItem.Text = "Downloading web app";
            WorkingForm.Refresh();
            AutoUpdate.UpdateIfNeeded(AutoUpdate.strYAMSUpdatePath["live"] + @"\web.zip", Core.RootFolder + @"\web.zip");

            //Grab latest server jar
            WorkingForm.lblCurrentItem.Text = "Downloading latest Minecraft Server";
            WorkingForm.Refresh();
            AutoUpdate.UpdateIfNeeded(AutoUpdate.strMCServerURL, Core.RootFolder + @"\lib\minecraft_server.jar.UPDATE");

            //Set our MC Defaults in the DB
            WorkingForm.lblCurrentItem.Text = "Creating your server";
            WorkingForm.Refresh();
            var NewServer = new List<KeyValuePair<string, string>>();
            NewServer.Add(new KeyValuePair<string, string>("admin-slot", "true"));
            NewServer.Add(new KeyValuePair<string, string>("enable-health", "true"));
            NewServer.Add(new KeyValuePair<string, string>("hellworld", "false"));
            NewServer.Add(new KeyValuePair<string, string>("level-name", @"..\\world"));
            NewServer.Add(new KeyValuePair<string, string>("max-connections", "1"));
            NewServer.Add(new KeyValuePair<string, string>("max-players", "20"));
            NewServer.Add(new KeyValuePair<string, string>("motd", "Welcome to a YAMS server!"));
            NewServer.Add(new KeyValuePair<string, string>("online-mode", "true"));
            NewServer.Add(new KeyValuePair<string, string>("public", "false"));
            NewServer.Add(new KeyValuePair<string, string>("pvp", "true"));
            NewServer.Add(new KeyValuePair<string, string>("server-ip", ""));
            NewServer.Add(new KeyValuePair<string, string>("server-name", this.txtServerName.Text));
            NewServer.Add(new KeyValuePair<string, string>("server-port", "25565"));
            NewServer.Add(new KeyValuePair<string, string>("spawn-animals", "true"));
            NewServer.Add(new KeyValuePair<string, string>("spawn-monsters", "true"));
            NewServer.Add(new KeyValuePair<string, string>("verify-names", "true"));
            NewServer.Add(new KeyValuePair<string, string>("white-list", "false"));
            Database.NewServer(NewServer, this.txtServerName.Text, Convert.ToInt32(this.txtMemory.Text));
            
            //Set our YAMS Defaults
            WorkingForm.lblCurrentItem.Text = "Configuring YAMS";
            WorkingForm.Refresh();
            Database.SaveSetting("UpdateJAR", Util.BooleanToString(this.chkUpdateJar.Checked));
            Database.SaveSetting("UpdateSVC", Util.BooleanToString(this.chkUpdateYAMS.Checked));
            Database.SaveSetting("UpdateGUI", Util.BooleanToString(this.chkUpdateYAMS.Checked));
            Database.SaveSetting("UpdateWeb", Util.BooleanToString(this.chkUpdateYAMS.Checked));
            Database.SaveSetting("UpdateAddons", Util.BooleanToString(this.chkUpdateAddons.Checked));
            Database.SaveSetting("RestartOnJarUpdate", Util.BooleanToString(this.chkRestartMC.Checked));
            Database.SaveSetting("RestartOnSVCUpdate", Util.BooleanToString(this.chkRestartYAMS.Checked));
            Database.SaveSetting("EnableJavaOptimisations", "true");
            Database.SaveSetting("AdminListenPort", "56552"); //Use an IANA legal internal port 49152 - 65535
            Database.SaveSetting("PublicListenPort", Convert.ToString(Networking.TcpPort.FindNextAvailablePort(80))); //Find nearest open port to 80 for public site
            Database.SaveSetting("ExternalIP", Util.GetExternalIP().ToString());
            Database.SaveSetting("ListenIP", Networking.GetListenIP().ToString());
            Database.SaveSetting("UpdateBranch", "live");
            Database.SaveSetting("AdminPassword", txtAdminPassword.Text);
            
            //Tell the DB that we've run this
            Database.SaveSetting("FirstRun", "true");

            //Now start the service and exit first run dialog
            WorkingForm.lblCurrentItem.Text = "Starting YAMS service";
            WorkingForm.Refresh();
            Program.StartService();

            WorkingForm.Close();
            this.Close();
        }


    }
}
