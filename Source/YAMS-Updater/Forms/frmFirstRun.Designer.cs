namespace YAMS_Updater
{
    partial class frmFirstRun
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFirstRun));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnJDKDownload = new System.Windows.Forms.Button();
            this.btnJREDownload = new System.Windows.Forms.Button();
            this.lblJDK = new System.Windows.Forms.Label();
            this.lblJRE = new System.Windows.Forms.Label();
            this.icoJDK = new System.Windows.Forms.PictureBox();
            this.icoJRE = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkOptimisations = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtMemory = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkRestartYAMS = new System.Windows.Forms.CheckBox();
            this.chkRestartMC = new System.Windows.Forms.CheckBox();
            this.chkUpdateAddons = new System.Windows.Forms.CheckBox();
            this.chkUpdateYAMS = new System.Windows.Forms.CheckBox();
            this.chkUpdateJar = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tipYAMSUpdate = new System.Windows.Forms.ToolTip(this.components);
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.chkC10t = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkOverviewer = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnComplete = new System.Windows.Forms.Button();
            this.timCheckUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icoJDK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icoJRE)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnJDKDownload);
            this.groupBox1.Controls.Add(this.btnJREDownload);
            this.groupBox1.Controls.Add(this.lblJDK);
            this.groupBox1.Controls.Add(this.lblJRE);
            this.groupBox1.Controls.Add(this.icoJDK);
            this.groupBox1.Controls.Add(this.icoJRE);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(679, 75);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Prerequisites";
            // 
            // btnJDKDownload
            // 
            this.btnJDKDownload.Location = new System.Drawing.Point(578, 41);
            this.btnJDKDownload.Name = "btnJDKDownload";
            this.btnJDKDownload.Size = new System.Drawing.Size(95, 23);
            this.btnJDKDownload.TabIndex = 7;
            this.btnJDKDownload.Text = "Download JDK";
            this.btnJDKDownload.UseVisualStyleBackColor = true;
            this.btnJDKDownload.Visible = false;
            this.btnJDKDownload.Click += new System.EventHandler(this.btnJDKDownload_Click);
            // 
            // btnJREDownload
            // 
            this.btnJREDownload.Location = new System.Drawing.Point(578, 15);
            this.btnJREDownload.Name = "btnJREDownload";
            this.btnJREDownload.Size = new System.Drawing.Size(95, 23);
            this.btnJREDownload.TabIndex = 6;
            this.btnJREDownload.Text = "Download JRE";
            this.btnJREDownload.UseMnemonic = false;
            this.btnJREDownload.UseVisualStyleBackColor = true;
            this.btnJREDownload.Visible = false;
            this.btnJREDownload.Click += new System.EventHandler(this.btnJREDownload_Click);
            // 
            // lblJDK
            // 
            this.lblJDK.AutoSize = true;
            this.lblJDK.Location = new System.Drawing.Point(199, 46);
            this.lblJDK.Name = "lblJDK";
            this.lblJDK.Size = new System.Drawing.Size(0, 13);
            this.lblJDK.TabIndex = 5;
            // 
            // lblJRE
            // 
            this.lblJRE.AutoSize = true;
            this.lblJRE.Location = new System.Drawing.Point(199, 20);
            this.lblJRE.Name = "lblJRE";
            this.lblJRE.Size = new System.Drawing.Size(0, 13);
            this.lblJRE.TabIndex = 4;
            // 
            // icoJDK
            // 
            this.icoJDK.Image = global::YAMS_Updater.Properties.Resources.cancel;
            this.icoJDK.InitialImage = global::YAMS_Updater.Properties.Resources.cancel;
            this.icoJDK.Location = new System.Drawing.Point(176, 46);
            this.icoJDK.Name = "icoJDK";
            this.icoJDK.Size = new System.Drawing.Size(16, 16);
            this.icoJDK.TabIndex = 3;
            this.icoJDK.TabStop = false;
            // 
            // icoJRE
            // 
            this.icoJRE.Image = global::YAMS_Updater.Properties.Resources.cancel;
            this.icoJRE.InitialImage = global::YAMS_Updater.Properties.Resources.cancel;
            this.icoJRE.Location = new System.Drawing.Point(176, 19);
            this.icoJRE.Name = "icoJRE";
            this.icoJRE.Size = new System.Drawing.Size(16, 16);
            this.icoJRE.TabIndex = 2;
            this.icoJRE.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Java Development Kit (JDK)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Java Runtime Environment (JRE)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkOptimisations);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtMemory);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.chkAutoStart);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtServerName);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(13, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(678, 167);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Your First Server";
            // 
            // chkOptimisations
            // 
            this.chkOptimisations.AutoSize = true;
            this.chkOptimisations.Enabled = false;
            this.chkOptimisations.Location = new System.Drawing.Point(156, 136);
            this.chkOptimisations.Name = "chkOptimisations";
            this.chkOptimisations.Size = new System.Drawing.Size(15, 14);
            this.chkOptimisations.TabIndex = 8;
            this.chkOptimisations.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(137, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Enable Java Optimisations?";
            // 
            // txtMemory
            // 
            this.txtMemory.Location = new System.Drawing.Point(156, 78);
            this.txtMemory.Name = "txtMemory";
            this.txtMemory.Size = new System.Drawing.Size(144, 20);
            this.txtMemory.TabIndex = 6;
            this.txtMemory.Text = "1024";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Assigned Memory (MB)";
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Checked = true;
            this.chkAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoStart.Location = new System.Drawing.Point(156, 109);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(15, 14);
            this.chkAutoStart.TabIndex = 4;
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Start Server immediately?";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(156, 52);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(144, 20);
            this.txtServerName.TabIndex = 2;
            this.txtServerName.Text = "My First YAMS Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(518, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Enter the name of your first Minecraft server, more detailed configuration option" +
                "s are available in the web app";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkRestartYAMS);
            this.groupBox3.Controls.Add(this.chkRestartMC);
            this.groupBox3.Controls.Add(this.chkUpdateAddons);
            this.groupBox3.Controls.Add(this.chkUpdateYAMS);
            this.groupBox3.Controls.Add(this.chkUpdateJar);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(12, 267);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(679, 161);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "YAMS Settings";
            // 
            // chkRestartYAMS
            // 
            this.chkRestartYAMS.AutoSize = true;
            this.chkRestartYAMS.Checked = true;
            this.chkRestartYAMS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRestartYAMS.Location = new System.Drawing.Point(17, 136);
            this.chkRestartYAMS.Name = "chkRestartYAMS";
            this.chkRestartYAMS.Size = new System.Drawing.Size(406, 17);
            this.chkRestartYAMS.TabIndex = 10;
            this.chkRestartYAMS.Text = "Restart YAMS automatically when update is available? (only when no players on)";
            this.chkRestartYAMS.UseVisualStyleBackColor = true;
            // 
            // chkRestartMC
            // 
            this.chkRestartMC.AutoSize = true;
            this.chkRestartMC.Checked = true;
            this.chkRestartMC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRestartMC.Location = new System.Drawing.Point(17, 113);
            this.chkRestartMC.Name = "chkRestartMC";
            this.chkRestartMC.Size = new System.Drawing.Size(420, 17);
            this.chkRestartMC.TabIndex = 9;
            this.chkRestartMC.Text = "Restart Minecraft automatically when update is available? (only when no players o" +
                "n)";
            this.chkRestartMC.UseVisualStyleBackColor = true;
            // 
            // chkUpdateAddons
            // 
            this.chkUpdateAddons.AutoSize = true;
            this.chkUpdateAddons.Checked = true;
            this.chkUpdateAddons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateAddons.Location = new System.Drawing.Point(17, 90);
            this.chkUpdateAddons.Name = "chkUpdateAddons";
            this.chkUpdateAddons.Size = new System.Drawing.Size(149, 17);
            this.chkUpdateAddons.TabIndex = 8;
            this.chkUpdateAddons.Text = "Keep add-ons up-to-date?";
            this.chkUpdateAddons.UseVisualStyleBackColor = true;
            // 
            // chkUpdateYAMS
            // 
            this.chkUpdateYAMS.AutoSize = true;
            this.chkUpdateYAMS.Checked = true;
            this.chkUpdateYAMS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateYAMS.Location = new System.Drawing.Point(17, 67);
            this.chkUpdateYAMS.Name = "chkUpdateYAMS";
            this.chkUpdateYAMS.Size = new System.Drawing.Size(141, 17);
            this.chkUpdateYAMS.TabIndex = 7;
            this.chkUpdateYAMS.Text = "Keep YAMS up-to-date?";
            this.chkUpdateYAMS.UseVisualStyleBackColor = true;
            // 
            // chkUpdateJar
            // 
            this.chkUpdateJar.AutoSize = true;
            this.chkUpdateJar.Checked = true;
            this.chkUpdateJar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateJar.Location = new System.Drawing.Point(17, 44);
            this.chkUpdateJar.Name = "chkUpdateJar";
            this.chkUpdateJar.Size = new System.Drawing.Size(155, 17);
            this.chkUpdateJar.TabIndex = 6;
            this.chkUpdateJar.Text = "Keep Minecraft up-to-date?";
            this.chkUpdateJar.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(388, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "These settings control how YAMS works and may affect any or all of your servers";
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.linkLabel2);
            this.groupBox4.Controls.Add(this.chkC10t);
            this.groupBox4.Controls.Add(this.linkLabel1);
            this.groupBox4.Controls.Add(this.chkOverviewer);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(13, 434);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(678, 94);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Third-party Add-Ons";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(279, 65);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(175, 13);
            this.linkLabel2.TabIndex = 14;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "http://toolchain.eu/minecraft/c10t/";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // chkC10t
            // 
            this.chkC10t.AutoSize = true;
            this.chkC10t.Checked = true;
            this.chkC10t.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkC10t.Location = new System.Drawing.Point(16, 64);
            this.chkC10t.Name = "chkC10t";
            this.chkC10t.Size = new System.Drawing.Size(257, 17);
            this.chkC10t.TabIndex = 13;
            this.chkC10t.Text = "c10t: Minecraft (C)artography (T)ool (by Udoprog)";
            this.chkC10t.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(214, 42);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(247, 13);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://github.com/brownan/Minecraft-Overviewer";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // chkOverviewer
            // 
            this.chkOverviewer.AutoSize = true;
            this.chkOverviewer.Checked = true;
            this.chkOverviewer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOverviewer.Location = new System.Drawing.Point(16, 41);
            this.chkOverviewer.Name = "chkOverviewer";
            this.chkOverviewer.Size = new System.Drawing.Size(192, 17);
            this.chkOverviewer.TabIndex = 11;
            this.chkOverviewer.Text = "Minecraft Overviewer (by Brownan)";
            this.chkOverviewer.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Install these additional apps?";
            // 
            // btnComplete
            // 
            this.btnComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnComplete.Enabled = false;
            this.btnComplete.Location = new System.Drawing.Point(572, 534);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(119, 23);
            this.btnComplete.TabIndex = 4;
            this.btnComplete.Text = "Complete Setup";
            this.btnComplete.UseVisualStyleBackColor = true;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // timCheckUpdate
            // 
            this.timCheckUpdate.Enabled = true;
            this.timCheckUpdate.Interval = 10000;
            // 
            // frmFirstRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 568);
            this.Controls.Add(this.btnComplete);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmFirstRun";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YAMS First Run Configuration";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icoJDK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icoJRE)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox icoJDK;
        private System.Windows.Forms.PictureBox icoJRE;
        private System.Windows.Forms.Label lblJDK;
        private System.Windows.Forms.Label lblJRE;
        private System.Windows.Forms.Button btnJDKDownload;
        private System.Windows.Forms.Button btnJREDownload;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkUpdateAddons;
        private System.Windows.Forms.CheckBox chkUpdateYAMS;
        private System.Windows.Forms.CheckBox chkUpdateJar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip tipYAMSUpdate;
        private System.Windows.Forms.CheckBox chkOptimisations;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtMemory;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkRestartYAMS;
        private System.Windows.Forms.CheckBox chkRestartMC;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox chkOverviewer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.CheckBox chkC10t;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.Timer timCheckUpdate;

    }
}