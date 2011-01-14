using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YAMS_Updater
{
    public partial class frmFirstRun : Form
    {
        public frmFirstRun()
        {
            InitializeComponent();

            if (YAMS.Util.HasJRE())
            {
                icoJRE.Image = YAMS_Updater.Properties.Resources.accept;
                lblJRE.Text = "JRE is installed and detected";
            }
            else
            {
                lblJRE.Text = "JRE is a requirement for Minecraft server to run, if you don't have the JRE, downloading the JDK may be better";
            }
            if (YAMS.Util.HasJDK())
            {
                icoJDK.Image = YAMS_Updater.Properties.Resources.accept;
                lblJDK.Text = "JDK is installed, and will provide the best experience";
                chkOptimisations.Enabled = true;
            }
            else
            {
                lblJDK.Text = "JDK is not installed, using the JDK instead of the JRE provides many enhancements but is not required";
            }

            tipYAMSUpdate.SetToolTip(this.chkUpdateYAMS, "Includes all YAMS components, you can specify individual components to update in the web app");
        
        }

        private void btnJREDownload_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://cds.sun.com/is-bin/INTERSHOP.enfinity/WFS/CDS-CDS_Developer-Site/en_US/-/USD/ViewProductDetail-Start?ProductRef=jre-6u23-oth-JPR@CDS-CDS_Developer");
        }

        private void btnJDKDownload_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://cds.sun.com/is-bin/INTERSHOP.enfinity/WFS/CDS-CDS_Developer-Site/en_US/-/USD/ViewProductDetail-Start?ProductRef=jdk-6u23-oth-JPR@CDS-CDS_Developer");
        }
    }
}
