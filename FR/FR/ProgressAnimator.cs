using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


    public class ProgressAnimator
    {
        private ProgressBar pg;
        private Form myAnim;
        public ProgressAnimator()
        {
            CreateForm();
        }

        private void CreateForm()
        {
            myAnim = new Form();

            this.pg = new System.Windows.Forms.ProgressBar();
            this.pg.Location = new System.Drawing.Point(56, 112);
            this.pg.MarqueeAnimationSpeed = 35;
            this.pg.Name = "pg";
            this.pg.Size = new System.Drawing.Size(400, 50);
            this.pg.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pg.TabIndex = 0;
            this.pg.Value = 50;

            this.myAnim.Location = new Point(0, 0);
            this.myAnim.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.myAnim.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            int x = Screen.PrimaryScreen.Bounds.Width / 2 - pg.Size.Width / 2;
            int y = Screen.PrimaryScreen.Bounds.Height / 2 - pg.Size.Height / 2;
            this.pg.Location = new Point(x, y);
            this.myAnim.Opacity = 0.80;
            this.myAnim.Controls.Add(pg);
        }

        public void Show()
        {
            myAnim.Show();
        }
        public void Hide()
        {
            myAnim.Hide();
        }
        public void Dispose()
        {
            myAnim.Dispose();
        }


    }

