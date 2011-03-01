using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace YAMS_Updater
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            timStatus.Tick += new EventHandler(timStatus_Tick);
            timStatus.Start();

            //Get the version numbers of our compiled files
            lblDLL.Text = FileVersionInfo.GetVersionInfo(Path.Combine(Program.RootFolder, "YAMS-Library.dll")).FileVersion;
            lblSVC.Text = FileVersionInfo.GetVersionInfo(Path.Combine(Program.RootFolder, "YAMS-Service.exe")).FileVersion;
            lblGUI.Text = FileVersionInfo.GetVersionInfo(Path.Combine(Program.RootFolder, "YAMS-Updater.exe")).FileVersion;
            lblDB.Text = YAMS.Database.GetSetting("DBSchema", "YAMS");

            //Listen ports for the webservers
            lblAdminPort.Text = YAMS.Database.GetSetting("AdminListenPort", "YAMS");
            lblPublicPort.Text = YAMS.Database.GetSetting("PublicListenPort", "YAMS");

            //Storage Path
            lblStoragePath.Text = YAMS.Database.GetSetting("StoragePath", "YAMS");
            txtStoragePath.Text = YAMS.Database.GetSetting("StoragePath", "YAMS");

            //Set current update branch
            switch (YAMS.Database.GetSetting("UpdateBranch", "YAMS")) {
                case "live":
                    selUpdateBranch.SelectedIndex = 0;
                    break;
                case "dev":
                    selUpdateBranch.SelectedIndex = 1;
                    break;
                default:
                    selUpdateBranch.SelectedIndex = 0;
                    break;
            }

        }

        void timStatus_Tick(object sender, EventArgs e)
        {
            ServiceController svcYAMS = new ServiceController("YAMS_Service");

            switch (svcYAMS.Status) {
                case ServiceControllerStatus.Stopped:
                    lblStatus.Text = "Stopped";
                    btnStop.Enabled = false;
                    btnStart.Enabled = true;
                    break;
                case ServiceControllerStatus.Running:
                    lblStatus.Text = "Running";
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    break;
                case ServiceControllerStatus.Paused:
                    lblStatus.Text = "Paused";
                    btnStop.Enabled = true;
                    btnStart.Enabled = true;
                    break;
                case ServiceControllerStatus.StartPending:
                    lblStatus.Text = "Starting";
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    break;
                case ServiceControllerStatus.StopPending:
                    lblStatus.Text = "Stopping";
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    break;
                case ServiceControllerStatus.PausePending:
                    lblStatus.Text = "Pausing";
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    break;
                case ServiceControllerStatus.ContinuePending:
                    lblStatus.Text = "Continuing";
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    break;
                default:
                    lblStatus.Text = "Unknown";
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    break;
            }
            this.Refresh();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Program.StartService();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Program.StopService();
        }

        private void btnConsoleStart_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost:" + YAMS.Database.GetSetting("ListenPort", "YAMS") + "/admin");
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            YAMS.Database.SaveSetting("AdminPassword", txtPassword.Text);
        }

        private void btnSwitchBranch_Click(object sender, EventArgs e)
        {
            switch (selUpdateBranch.SelectedIndex)
            {
                case 0:
                    YAMS.Database.SaveSetting("UpdateBranch", "live");
                    break;
                case 1:
                    YAMS.Database.SaveSetting("UpdateBranch", "dev");
                    break;
                default:
                    YAMS.Database.SaveSetting("UpdateBranch", "live");
                    break;
            }
        }

        private void btnUpdateClient_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please be aware if you have large worlds or mods, this can take a long time\n\nThe app may report \"Not Responding\" but it is still copying.");
            YAMS.Util.CopyMCClient();
        }

        private void btnChangePath_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will stop all servers and move their files to the new location", "Confirm move", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Stop the service, which in turn stops all the servers
                Program.StopService();
                bool bolStopped = false;
                while (!bolStopped)
                {
                    ServiceController svcYAMS = new ServiceController("YAMS_Service");
                    if (svcYAMS.Status.Equals(ServiceControllerStatus.Stopped))
                    {
                        bolStopped = true;
                    }
                    else { System.Threading.Thread.Sleep(1000); }
                }

                //Copy all files to the new location
                YAMS.Util.Copy(YAMS.Database.GetSetting("StoragePath", "YAMS"), txtStoragePath.Text);

                //Set the DB path
                YAMS.Database.SaveSetting("StoragePath", txtStoragePath.Text);

                //Start the service back up
                Program.StartService();
            }
        }

    }
}
