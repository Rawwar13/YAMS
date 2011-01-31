namespace YAMS_Updater
{
    partial class frmWorking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWorking));
            this.label2 = new System.Windows.Forms.Label();
            this.lblCurrentItem = new System.Windows.Forms.Label();
            this.prgCurrentTask = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Current Item: ";
            this.label2.UseWaitCursor = true;
            // 
            // lblCurrentItem
            // 
            this.lblCurrentItem.AutoSize = true;
            this.lblCurrentItem.Location = new System.Drawing.Point(102, 9);
            this.lblCurrentItem.Name = "lblCurrentItem";
            this.lblCurrentItem.Size = new System.Drawing.Size(16, 13);
            this.lblCurrentItem.TabIndex = 2;
            this.lblCurrentItem.Text = "...";
            this.lblCurrentItem.UseWaitCursor = true;
            // 
            // prgCurrentTask
            // 
            this.prgCurrentTask.Location = new System.Drawing.Point(15, 25);
            this.prgCurrentTask.Name = "prgCurrentTask";
            this.prgCurrentTask.Size = new System.Drawing.Size(422, 23);
            this.prgCurrentTask.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgCurrentTask.TabIndex = 3;
            this.prgCurrentTask.UseWaitCursor = true;
            // 
            // frmWorking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 65);
            this.ControlBox = false;
            this.Controls.Add(this.prgCurrentTask);
            this.Controls.Add(this.lblCurrentItem);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWorking";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Working...";
            this.UseWaitCursor = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label lblCurrentItem;
        public System.Windows.Forms.ProgressBar prgCurrentTask;
    }
}