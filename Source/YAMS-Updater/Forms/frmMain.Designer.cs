namespace YAMS_Updater
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.timStatus = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnUpdateClient = new System.Windows.Forms.Button();
            this.btnConsoleStart = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSwitchBranch = new System.Windows.Forms.Button();
            this.selUpdateBranch = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblStoragePath = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblPublicPort = new System.Windows.Forms.Label();
            this.lblAdminPort = new System.Windows.Forms.Label();
            this.lblGUI = new System.Windows.Forms.Label();
            this.lblSVC = new System.Windows.Forms.Label();
            this.lblDLL = new System.Windows.Forms.Label();
            this.lblDB = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtStoragePath = new System.Windows.Forms.TextBox();
            this.btnChangePath = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tblPortForwards = new System.Windows.Forms.DataGridView();
            this.PFName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PFPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PFStatus = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.lblAdminURL = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblPublicURL = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblExternalIP = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progToolStrip = new System.Windows.Forms.ToolStripProgressBar();
            this.lblPortStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblListenIP = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tblPortForwards)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 65);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service Control";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(88, 32);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(7, 32);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblStatus.Location = new System.Drawing.Point(75, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(91, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "checking status...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnUpdateClient);
            this.groupBox2.Controls.Add(this.btnConsoleStart);
            this.groupBox2.Location = new System.Drawing.Point(187, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(118, 65);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Admin";
            // 
            // btnUpdateClient
            // 
            this.btnUpdateClient.Location = new System.Drawing.Point(6, 39);
            this.btnUpdateClient.Name = "btnUpdateClient";
            this.btnUpdateClient.Size = new System.Drawing.Size(104, 23);
            this.btnUpdateClient.TabIndex = 1;
            this.btnUpdateClient.Text = "Update Client";
            this.btnUpdateClient.UseVisualStyleBackColor = true;
            this.btnUpdateClient.Click += new System.EventHandler(this.btnUpdateClient_Click);
            // 
            // btnConsoleStart
            // 
            this.btnConsoleStart.Location = new System.Drawing.Point(6, 15);
            this.btnConsoleStart.Name = "btnConsoleStart";
            this.btnConsoleStart.Size = new System.Drawing.Size(104, 23);
            this.btnConsoleStart.TabIndex = 0;
            this.btnConsoleStart.Text = "Launch Console";
            this.btnConsoleStart.UseVisualStyleBackColor = true;
            this.btnConsoleStart.Click += new System.EventHandler(this.btnConsoleStart_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnResetPassword);
            this.groupBox3.Controls.Add(this.txtPassword);
            this.groupBox3.Location = new System.Drawing.Point(12, 83);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(293, 80);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Reset Password";
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Location = new System.Drawing.Point(147, 45);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(138, 23);
            this.btnResetPassword.TabIndex = 1;
            this.btnResetPassword.Text = "Set Admin Password";
            this.btnResetPassword.UseVisualStyleBackColor = true;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(7, 19);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(278, 20);
            this.txtPassword.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSwitchBranch);
            this.groupBox4.Controls.Add(this.selUpdateBranch);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(13, 170);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(292, 86);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Update Branch";
            // 
            // btnSwitchBranch
            // 
            this.btnSwitchBranch.Location = new System.Drawing.Point(186, 19);
            this.btnSwitchBranch.Name = "btnSwitchBranch";
            this.btnSwitchBranch.Size = new System.Drawing.Size(98, 31);
            this.btnSwitchBranch.TabIndex = 3;
            this.btnSwitchBranch.Text = "Set";
            this.btnSwitchBranch.UseVisualStyleBackColor = true;
            this.btnSwitchBranch.Click += new System.EventHandler(this.btnSwitchBranch_Click);
            // 
            // selUpdateBranch
            // 
            this.selUpdateBranch.FormattingEnabled = true;
            this.selUpdateBranch.Items.AddRange(new object[] {
            "Live",
            "Development"});
            this.selUpdateBranch.Location = new System.Drawing.Point(7, 20);
            this.selUpdateBranch.Name = "selUpdateBranch";
            this.selUpdateBranch.Size = new System.Drawing.Size(173, 30);
            this.selUpdateBranch.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "check and will require a restart.";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(266, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Changing update branch will take effect at next update";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblStoragePath);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.lblPublicPort);
            this.groupBox5.Controls.Add(this.lblAdminPort);
            this.groupBox5.Controls.Add(this.lblGUI);
            this.groupBox5.Controls.Add(this.lblSVC);
            this.groupBox5.Controls.Add(this.lblDLL);
            this.groupBox5.Controls.Add(this.lblDB);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Location = new System.Drawing.Point(12, 331);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(294, 157);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Information";
            // 
            // lblStoragePath
            // 
            this.lblStoragePath.AutoSize = true;
            this.lblStoragePath.Location = new System.Drawing.Point(99, 126);
            this.lblStoragePath.Name = "lblStoragePath";
            this.lblStoragePath.Size = new System.Drawing.Size(13, 13);
            this.lblStoragePath.TabIndex = 13;
            this.lblStoragePath.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 126);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Storage Path:";
            // 
            // lblPublicPort
            // 
            this.lblPublicPort.AutoSize = true;
            this.lblPublicPort.Location = new System.Drawing.Point(99, 98);
            this.lblPublicPort.Name = "lblPublicPort";
            this.lblPublicPort.Size = new System.Drawing.Size(13, 13);
            this.lblPublicPort.TabIndex = 11;
            this.lblPublicPort.Text = "0";
            // 
            // lblAdminPort
            // 
            this.lblAdminPort.AutoSize = true;
            this.lblAdminPort.Location = new System.Drawing.Point(99, 85);
            this.lblAdminPort.Name = "lblAdminPort";
            this.lblAdminPort.Size = new System.Drawing.Size(13, 13);
            this.lblAdminPort.TabIndex = 10;
            this.lblAdminPort.Text = "0";
            // 
            // lblGUI
            // 
            this.lblGUI.AutoSize = true;
            this.lblGUI.Location = new System.Drawing.Point(99, 55);
            this.lblGUI.Name = "lblGUI";
            this.lblGUI.Size = new System.Drawing.Size(13, 13);
            this.lblGUI.TabIndex = 9;
            this.lblGUI.Text = "0";
            // 
            // lblSVC
            // 
            this.lblSVC.AutoSize = true;
            this.lblSVC.Location = new System.Drawing.Point(99, 42);
            this.lblSVC.Name = "lblSVC";
            this.lblSVC.Size = new System.Drawing.Size(13, 13);
            this.lblSVC.TabIndex = 8;
            this.lblSVC.Text = "0";
            // 
            // lblDLL
            // 
            this.lblDLL.AutoSize = true;
            this.lblDLL.Location = new System.Drawing.Point(99, 29);
            this.lblDLL.Name = "lblDLL";
            this.lblDLL.Size = new System.Drawing.Size(13, 13);
            this.lblDLL.TabIndex = 7;
            this.lblDLL.Text = "0";
            // 
            // lblDB
            // 
            this.lblDB.AutoSize = true;
            this.lblDB.Location = new System.Drawing.Point(99, 16);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(13, 13);
            this.lblDB.TabIndex = 6;
            this.lblDB.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Public Port:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(31, 85);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Admin Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Updater Version:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Service Version:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "DLL Version:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "DB Schema:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtStoragePath);
            this.groupBox6.Controls.Add(this.btnChangePath);
            this.groupBox6.Location = new System.Drawing.Point(13, 259);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(292, 68);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Change Storage Path";
            // 
            // txtStoragePath
            // 
            this.txtStoragePath.Location = new System.Drawing.Point(10, 39);
            this.txtStoragePath.Name = "txtStoragePath";
            this.txtStoragePath.Size = new System.Drawing.Size(193, 20);
            this.txtStoragePath.TabIndex = 1;
            // 
            // btnChangePath
            // 
            this.btnChangePath.Location = new System.Drawing.Point(209, 39);
            this.btnChangePath.Name = "btnChangePath";
            this.btnChangePath.Size = new System.Drawing.Size(75, 23);
            this.btnChangePath.TabIndex = 0;
            this.btnChangePath.Text = "Move Path";
            this.btnChangePath.UseVisualStyleBackColor = true;
            this.btnChangePath.Click += new System.EventHandler(this.btnChangePath_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.tblPortForwards);
            this.groupBox7.Location = new System.Drawing.Point(312, 13);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(351, 314);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Port Forwarding";
            // 
            // tblPortForwards
            // 
            this.tblPortForwards.AllowUserToAddRows = false;
            this.tblPortForwards.AllowUserToDeleteRows = false;
            this.tblPortForwards.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.tblPortForwards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblPortForwards.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PFName,
            this.PFPort,
            this.PFStatus});
            this.tblPortForwards.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tblPortForwards.Location = new System.Drawing.Point(10, 19);
            this.tblPortForwards.MultiSelect = false;
            this.tblPortForwards.Name = "tblPortForwards";
            this.tblPortForwards.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.tblPortForwards.RowHeadersVisible = false;
            this.tblPortForwards.Size = new System.Drawing.Size(335, 286);
            this.tblPortForwards.TabIndex = 13;
            // 
            // PFName
            // 
            this.PFName.HeaderText = "Name";
            this.PFName.Name = "PFName";
            this.PFName.ReadOnly = true;
            // 
            // PFPort
            // 
            this.PFPort.HeaderText = "Port";
            this.PFPort.Name = "PFPort";
            this.PFPort.ReadOnly = true;
            // 
            // PFStatus
            // 
            this.PFStatus.HeaderText = "Status";
            this.PFStatus.Name = "PFStatus";
            this.PFStatus.ReadOnly = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.lblListenIP);
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Controls.Add(this.lblAdminURL);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.lblPublicURL);
            this.groupBox8.Controls.Add(this.label12);
            this.groupBox8.Controls.Add(this.lblExternalIP);
            this.groupBox8.Controls.Add(this.label11);
            this.groupBox8.Location = new System.Drawing.Point(312, 331);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(351, 157);
            this.groupBox8.TabIndex = 9;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Addresses";
            // 
            // lblAdminURL
            // 
            this.lblAdminURL.AutoSize = true;
            this.lblAdminURL.Location = new System.Drawing.Point(81, 45);
            this.lblAdminURL.Name = "lblAdminURL";
            this.lblAdminURL.Size = new System.Drawing.Size(16, 13);
            this.lblAdminURL.TabIndex = 12;
            this.lblAdminURL.Text = "...";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 45);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(60, 13);
            this.label14.TabIndex = 11;
            this.label14.Text = "Admin Site:";
            // 
            // lblPublicURL
            // 
            this.lblPublicURL.AutoSize = true;
            this.lblPublicURL.Location = new System.Drawing.Point(80, 31);
            this.lblPublicURL.Name = "lblPublicURL";
            this.lblPublicURL.Size = new System.Drawing.Size(16, 13);
            this.lblPublicURL.TabIndex = 10;
            this.lblPublicURL.Text = "...";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Public Site:";
            // 
            // lblExternalIP
            // 
            this.lblExternalIP.AutoSize = true;
            this.lblExternalIP.Location = new System.Drawing.Point(80, 16);
            this.lblExternalIP.Name = "lblExternalIP";
            this.lblExternalIP.Size = new System.Drawing.Size(16, 13);
            this.lblExternalIP.TabIndex = 8;
            this.lblExternalIP.Text = "...";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "External IP:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progToolStrip,
            this.lblPortStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 494);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(675, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progToolStrip
            // 
            this.progToolStrip.Name = "progToolStrip";
            this.progToolStrip.Size = new System.Drawing.Size(100, 16);
            this.progToolStrip.Step = 1;
            // 
            // lblPortStatus
            // 
            this.lblPortStatus.Name = "lblPortStatus";
            this.lblPortStatus.Size = new System.Drawing.Size(140, 17);
            this.lblPortStatus.Text = "Checking port forwards...";
            // 
            // lblListenIP
            // 
            this.lblListenIP.AutoSize = true;
            this.lblListenIP.Location = new System.Drawing.Point(81, 58);
            this.lblListenIP.Name = "lblListenIP";
            this.lblListenIP.Size = new System.Drawing.Size(16, 13);
            this.lblListenIP.TabIndex = 14;
            this.lblListenIP.Text = "...";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 58);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(51, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Listen IP:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 516);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(335, 430);
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YAMS Control";
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tblPortForwards)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer timStatus;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnConsoleStart;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnResetPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox selUpdateBranch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblPublicPort;
        private System.Windows.Forms.Label lblAdminPort;
        private System.Windows.Forms.Label lblGUI;
        private System.Windows.Forms.Label lblSVC;
        private System.Windows.Forms.Label lblDLL;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSwitchBranch;
        private System.Windows.Forms.Button btnUpdateClient;
        private System.Windows.Forms.Label lblStoragePath;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnChangePath;
        private System.Windows.Forms.TextBox txtStoragePath;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridView tblPortForwards;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label lblAdminURL;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblPublicURL;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblExternalIP;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridViewTextBoxColumn PFName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PFPort;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PFStatus;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblPortStatus;
        private System.Windows.Forms.ToolStripProgressBar progToolStrip;
        private System.Windows.Forms.Label lblListenIP;
        private System.Windows.Forms.Label label13;

    }
}