﻿namespace zeus
{
    partial class main_menu
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
            this.support = new System.Windows.Forms.Label();
            this.pooling = new System.Timers.Timer();
            this.flush_timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // support
            // 
            this.support.AutoSize = true;
            this.support.BackColor = System.Drawing.Color.Transparent;
            this.support.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.support.Location = new System.Drawing.Point(197, 652);
            this.support.Name = "support";
            this.support.Size = new System.Drawing.Size(52, 16);
            this.support.TabIndex = 0;
            this.support.Text = "label1";
            this.support.Visible = false;
            // 
            // pooling
            // 
            this.pooling.Elapsed += new System.Timers.ElapsedEventHandler(this.pooling_Tick);
            // 
            // flush_timer
            // 
            this.flush_timer.Enabled = true;
            this.flush_timer.Interval = 60000;
            this.flush_timer.Tick += new System.EventHandler(this.flush_timer_Tick);
            // 
            // main_menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.support);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "main_menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IPayBox - Zeus";
            this.Load += new System.EventHandler(this.main_menu_Load);
            this.DoubleClick += new System.EventHandler(this.main_menu_Click);
            this.Click += new System.EventHandler(this.main_menu_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label support;
        private System.Timers.Timer pooling;
        private System.Windows.Forms.Timer flush_timer;

    }
}