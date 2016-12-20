using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FR
{
    public partial class Form2 : Form
    {
        public static int Max = 100;
        public static int Step = 5;
        public static bool Forward;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            Forward = true;
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            
            int x = Screen.PrimaryScreen.Bounds.Width/2 - pg.Size.Width/2;
            int y = Screen.PrimaryScreen.Bounds.Height / 2 - pg.Size.Height / 2;
            pg.Location = new Point(x, y);
            pg.Maximum = Max;
            pg.Minimum = 0;
            
            this.Opacity = 0.80;
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Forward)
            {
                if (pg.Value + Step < Max)
                {
                    pg.Value += Step;
                }
                else
                    Forward = false;
            }
            else 
            {
                if (pg.Value - Step > 0)
                {
                    pg.Value -= Step;
                }
                else
                    Forward = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Interval = int.Parse(textBox1.Text);
            pg.Maximum = int.Parse(textBox2.Text);
            pg.Step = int.Parse(textBox3.Text);
            Step = int.Parse(textBox3.Text);
            pg.MarqueeAnimationSpeed = int.Parse(textBox4.Text);

            int w = int.Parse(textBox5.Text);

            switch (w)
            {
                case 1: pg.Style = ProgressBarStyle.Blocks; break;
                case 2: pg.Style = ProgressBarStyle.Continuous; break;
                case 3: pg.Style = ProgressBarStyle.Marquee; break;
                default: pg.Style = ProgressBarStyle.Blocks; break;
            }
            timer1.Start();

        }
    }
}
