using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YAMS_Gui
{
    public partial class frmMain : Form
    {
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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            DataSet ds = YAMS.Database.ReturnLogRows();
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            YAMS.Database.AddLog("button press", "tester", "warn");
        }

    }
}
