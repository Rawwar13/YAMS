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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtMemory = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtAdminPassword = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkRestartYAMS = new System.Windows.Forms.CheckBox();
            this.chkRestartMC = new System.Windows.Forms.CheckBox();
            this.chkUpdateAddons = new System.Windows.Forms.CheckBox();
            this.chkUpdateYAMS = new System.Windows.Forms.CheckBox();
            this.chkUpdateJar = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tipYAMSUpdate = new System.Windows.Forms.ToolTip(this.components);
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.btnComplete = new System.Windows.Forms.Button();
            this.timCheckUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtMemory);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.chkAutoStart);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtServerName);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(13, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(678, 162);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Your First Server";
            // 
            // txtMemory
            // 
            this.txtMemory.Location = new System.Drawing.Point(156, 81);
            this.txtMemory.Name = "txtMemory";
            this.txtMemory.Size = new System.Drawing.Size(144, 20);
            this.txtMemory.TabIndex = 6;
            this.txtMemory.Text = "1024";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 84);
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
            this.chkAutoStart.Location = new System.Drawing.Point(156, 112);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(15, 14);
            this.chkAutoStart.TabIndex = 4;
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Start Server immediately?";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(156, 55);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(144, 20);
            this.txtServerName.TabIndex = 2;
            this.txtServerName.Text = "My First YAMS Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(518, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Enter the name of your first Minecraft server, more detailed configuration option" +
    "s are available in the web app";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtAdminPassword);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.chkRestartYAMS);
            this.groupBox3.Controls.Add(this.chkRestartMC);
            this.groupBox3.Controls.Add(this.chkUpdateAddons);
            this.groupBox3.Controls.Add(this.chkUpdateYAMS);
            this.groupBox3.Controls.Add(this.chkUpdateJar);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(12, 185);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(679, 203);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "YAMS Settings";
            // 
            // txtAdminPassword
            // 
            this.txtAdminPassword.Location = new System.Drawing.Point(108, 169);
            this.txtAdminPassword.Name = "txtAdminPassword";
            this.txtAdminPassword.Size = new System.Drawing.Size(193, 20);
            this.txtAdminPassword.TabIndex = 12;
            this.txtAdminPassword.Text = "password";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 172);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Admin Password:";
            // 
            // chkRestartYAMS
            // 
            this.chkRestartYAMS.AutoSize = true;
            this.chkRestartYAMS.Checked = true;
            this.chkRestartYAMS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRestartYAMS.Location = new System.Drawing.Point(17, 139);
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
            this.chkRestartMC.Location = new System.Drawing.Point(17, 116);
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
            this.chkUpdateAddons.Location = new System.Drawing.Point(17, 93);
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
            this.chkUpdateYAMS.Location = new System.Drawing.Point(17, 70);
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
            this.chkUpdateJar.Location = new System.Drawing.Point(17, 47);
            this.chkUpdateJar.Name = "chkUpdateJar";
            this.chkUpdateJar.Size = new System.Drawing.Size(155, 17);
            this.chkUpdateJar.TabIndex = 6;
            this.chkUpdateJar.Text = "Keep Minecraft up-to-date?";
            this.chkUpdateJar.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 19);
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
            // btnComplete
            // 
            this.btnComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnComplete.Location = new System.Drawing.Point(572, 394);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(119, 31);
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
            this.ClientSize = new System.Drawing.Size(703, 437);
            this.ControlBox = false;
            this.Controls.Add(this.btnComplete);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmFirstRun";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YAMS First Run Configuration";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

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
        private System.Windows.Forms.TextBox txtMemory;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkRestartYAMS;
        private System.Windows.Forms.CheckBox chkRestartMC;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.Timer timCheckUpdate;
        private System.Windows.Forms.TextBox txtAdminPassword;
        private System.Windows.Forms.Label label10;

    }
}