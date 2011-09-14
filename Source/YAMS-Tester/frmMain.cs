using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using YAMS;

namespace YAMS_Gui
{
    public partial class frmMain : Form
    {
        public delegate void OnChangeCallback(object sender, FileSystemEventArgs e);
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

            dataGridView1.DataSource = null;
            DataSet ds = YAMS.Database.ReturnLogRows();
            dataGridView1.DataSource = ds.Tables[0];

            this.checkBox1.Checked = YAMS.Util.HasJRE();
            this.checkBox2.Checked = YAMS.Util.HasJDK();

            this.textBox1.Text = YAMS.Util.JavaPath();
            this.textBox2.Text = YAMS.Util.JavaPath("jdk");

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            watcher.Filter = "*.sdf";
            //watcher.NotifyFilter = NotifyFilters.LastWrite;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            IniParser parser = new IniParser(@"server.properties");
            txtIniCheck.Text = parser.GetSetting("ROOT", "level-name");

            parser.AddSetting("ROOT", "level-name", "world");
            parser.SaveSettings();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            DataSet ds = YAMS.Database.ReturnLogRows();
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (this.InvokeRequired) {
                this.Invoke(new OnChangeCallback(OnChanged), new object[]{source, e});
            } else {
                dataGridView1.DataSource = null;
                DataSet ds = YAMS.Database.ReturnLogRows();
                dataGridView1.DataSource = ds.Tables[0];
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //YAMS.Core.Servers.ForEach(delegate(MCServer myServer)
            //{
            //    myServer.Stop();
            //});
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //YAMS.Core.Servers.ForEach(delegate(MCServer myServer)
            //{
            //    myServer.Start();
            //});
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(YAMS.AutoUpdate.UpdateIfNeeded(YAMS.AutoUpdate.strMCServerURL, new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\lib\\minecraft_server.jar.UPDATE").ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //YAMS.Core.Servers.ForEach(delegate(MCServer myServer)
            //{
            //    myServer.Restart();
            //});
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //YAMS.Core.Servers.ForEach(delegate(MCServer myServer)
            //{
            //    myServer.DelayedRestart(Convert.ToInt32(textBox3.Text));
            //});
        }

        private void button8_Click(object sender, EventArgs e)
        {
            YAMS.Database.BuildServerProperties(1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            YAMS.Util.FirstRun();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            YAMS.WebServer.StartAdmin();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //YAMS.Core.Servers.ForEach(delegate(MCServer myServer)
            //{
            //    myServer.Players.ForEach(delegate(String name)
            //    {
            //        MessageBox.Show(name);
            //    });
            //});
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Networking.OpenFirewallPort(25565, "YAMS TEST");
        }
    }
}
