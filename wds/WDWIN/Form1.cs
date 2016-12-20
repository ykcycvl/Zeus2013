using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WD;

namespace WDWIN
{
    public partial class Form1 : Form
    {
        Independed wd;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            wd = new Independed(System.IO.Ports.SerialPort.GetPortNames());
            if (wd.watch != Independed.WD.NULL)
            {
                MessageBox.Show("Обнаружен WD: " + wd.Version + "(" + wd.watch.ToString() + ")");
                label1.Text = wd.Version;
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }
         
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            MessageBox.Show(wd.StartTimer().ToString());
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(wd.StopTimer().ToString());
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(wd.ResetTimer().ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(wd.ResetModem().ToString());

        }
    }
}
