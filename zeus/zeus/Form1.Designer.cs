namespace zeus
{
    partial class main
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
            this.pays_send_timer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.conf_update = new System.Windows.Forms.Timer(this.components);
            this.monitoring = new System.Windows.Forms.Timer(this.components);
            this.link_timer = new System.Windows.Forms.Timer(this.components);
            this.print_timer = new System.Windows.Forms.Timer(this.components);
            this.link_bw = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // pays_send_timer
            // 
            this.pays_send_timer.Enabled = true;
            this.pays_send_timer.Interval = 10000;
            this.pays_send_timer.Tick += new System.EventHandler(this.pays_send_timer_Tick);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(186, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // conf_update
            // 
            this.conf_update.Enabled = true;
            this.conf_update.Interval = 10000;
            this.conf_update.Tick += new System.EventHandler(this.conf_update_Tick);
            // 
            // monitoring
            // 
            this.monitoring.Enabled = true;
            this.monitoring.Interval = 300000;
            this.monitoring.Tick += new System.EventHandler(this.monitoring_Tick);
            // 
            // link_timer
            // 
            this.link_timer.Enabled = true;
            this.link_timer.Interval = 10000;
            this.link_timer.Tick += new System.EventHandler(this.link_timer_Tick);
            // 
            // print_timer
            // 
            this.print_timer.Enabled = true;
            this.print_timer.Interval = 1000;
            this.print_timer.Tick += new System.EventHandler(this.print_timer_Tick);
            // 
            // link_bw
            // 
            this.link_bw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.link_bw_DoWork);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 405);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IPAYBOX - Zeus - CORE";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer pays_send_timer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer conf_update;
        private System.Windows.Forms.Timer monitoring;
        private System.Windows.Forms.Timer link_timer;
        private System.Windows.Forms.Timer print_timer;
        private System.ComponentModel.BackgroundWorker link_bw;

    }
}

