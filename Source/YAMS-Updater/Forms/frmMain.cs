using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;

namespace YAMS_Updater
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            timStatus.Tick += new EventHandler(timStatus_Tick);
            timStatus.Start();
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
    }
}
