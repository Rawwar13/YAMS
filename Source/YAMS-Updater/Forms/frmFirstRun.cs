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
            if (!Directory.Exists(Core.RootFolder + @"\servers\")) Directory.CreateDirectory(Core.RootFolder + @"\servers\");
            WorkingForm.prgCurrentTask.Value = 100;

            //Grab latest web app
            WorkingForm.lblCurrentItem.Text = "Downloading web app";
            WorkingForm.Refresh();
            AutoUpdate.UpdateIfNeeded(AutoUpdate.strYAMSWebURL["live"], Core.RootFolder + @"\web.zip");

            //Grab latest server jar
            WorkingForm.lblCurrentItem.Text = "Downloading latest Minecraft Server";
            WorkingForm.Refresh();
            AutoUpdate.UpdateIfNeeded(AutoUpdate.strMCServerURL, Core.RootFolder + @"\lib\minecraft_server.jar.UPDATE");

            //Download our add-ons if we want them
            if (this.chkOverviewer.Checked || this.chkC10t.Checked)
            {
                WorkingForm.lblCurrentItem.Text = "Downloading add-ons";
                WorkingForm.Refresh();
                AutoUpdate.UpdateIfNeeded(AutoUpdate.strYAMSVersionsURL["live"], YAMS.Core.RootFolder + @"\lib\versions.json");
                string json = File.ReadAllText(YAMS.Core.RootFolder + @"\lib\versions.json");
                Dictionary<string, string> dicVers = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                
                //Grab overviewer
                if (this.chkOverviewer.Checked)
                {
                    WorkingForm.lblCurrentItem.Text = "Downloading Overviewer";
                    WorkingForm.Refresh();
                    string strOverviewerVer = dicVers["overviewer"];
                    if (AutoUpdate.UpdateIfNeeded(AutoUpdate.GetExternalURL("overviewer", strOverviewerVer), Core.RootFolder + @"\apps\overviewer.zip"))
                    {
                        WorkingForm.lblCurrentItem.Text = "Extracting Overviewer";
                        WorkingForm.Refresh();
                        AutoUpdate.ExtractZip(Core.RootFolder + @"\apps\overviewer.zip", Core.RootFolder + @"\apps\");
                        File.Delete(Core.RootFolder + @"\apps\overviewer.zip");
                        if (Directory.Exists(Core.RootFolder + @"\apps\overviewer\")) Directory.Delete(Core.RootFolder + @"\apps\overviewer\", true);
                        Directory.Move(Core.RootFolder + @"\apps\Overviewer-" + strOverviewerVer, Core.RootFolder + @"\apps\overviewer");
                        Database.SaveSetting("OverviewerVer", strOverviewerVer);
                    }
                }

                //Grab c10t
                if (this.chkC10t.Checked)
                {
                    WorkingForm.lblCurrentItem.Text = "Downloading c10t";
                    WorkingForm.Refresh();
                    string strC10tVer = dicVers["c10t"];
                    if (AutoUpdate.UpdateIfNeeded(AutoUpdate.GetExternalURL("c10t", strC10tVer), Core.RootFolder + @"\apps\c10t.zip"))
                    {
                        WorkingForm.lblCurrentItem.Text = "Extracting c10t";
                        WorkingForm.Refresh();
                        AutoUpdate.ExtractZip(Core.RootFolder + @"\apps\c10t.zip", Core.RootFolder + @"\apps\");
                        File.Delete(Core.RootFolder + @"\apps\c10t.zip");
                        if (Directory.Exists(Core.RootFolder + @"\apps\c10t\")) Directory.Delete(Core.RootFolder + @"\apps\c10t\", true);
                        Directory.Move(Core.RootFolder + @"\apps\c10t-" + strC10tVer, Core.RootFolder + @"\apps\c10t");
                        Database.SaveSetting("C10TVer", strC10tVer);
                    }
                }
            }

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
            Database.SaveSetting("ListenIP", Util.GetListenIP().ToString());
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
