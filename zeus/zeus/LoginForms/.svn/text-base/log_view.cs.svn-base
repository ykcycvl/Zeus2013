using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace zeus.LoginForms
{
    public partial class log_view : Form
    {
        int current_position=0;
        //int count_str = 36;
        int count_lenght = 1000;
        public log_view()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadLog(dateTimePicker1.Value);
        }

        private void log_view_Load(object sender, EventArgs e)
        {
           LoadLog(dateTimePicker1.Value);

        }
        private void LoadLog(DateTime dt)
        {
            try
            {
                FileInfo fi = new FileInfo(Ipaybox.StartupPath + "\\logs\\" + dt.ToString("yyyy-MM-dd") + ".log");
                if (fi.Exists)
                {
                    StreamReader sr = fi.OpenText();
                    text.Text = sr.ReadToEnd();
                    sr.Close();
                    current_position = text.Text.Length - 1;
                    Focus_postion();
                }
            }
            catch
            {
                text.Text = "Нет лога.";
            }
        
        }

        private void text_TextChanged(object sender, EventArgs e)
        {
                       
        }
        private void Focus_postion()
        {
            text.Focus();
            text.Select(current_position, 0);
            text.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (current_position - count_lenght > 0)
                current_position = current_position - count_lenght;
            else
                current_position = 0;
            Focus_postion();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (current_position + count_lenght < text.Text.Length - 1)
                current_position = current_position + count_lenght;
            else
                current_position = text.Text.Length - 1;
            Focus_postion();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
             try
            {
                FileInfo fi = new FileInfo(Ipaybox.StartupPath + "\\logs\\" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + ".log");
                if (fi.Exists)
                {
                    StreamReader sr = fi.OpenText();
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (txt.Length != text.Text.Length)
                    {
                        text.Text = txt;
                        current_position = text.Text.Length - 1;
                        Focus_postion();
                    }

                }
            }
            catch
            {
                text.Text = "Нет лога.";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
