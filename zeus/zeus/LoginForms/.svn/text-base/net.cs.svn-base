using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeus.LoginForms
{
    public partial class netopt : Form
    {
        public netopt()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void net_Load(object sender, EventArgs e)
        {
             listBox1.Items.Clear();

             if (Ipaybox.Modem == null)
                 Ipaybox.Modem = new modem();

             if (Ipaybox.Modem != null)
             {
                 foreach (string a in Ipaybox.Modem.Entry)
                 {
                     listBox1.Items.Add(a);
                 }
             }
            
             if (Ipaybox.NetOption == 0)
             {
                 radioButton1.Checked = true;
                 radioButton2.Checked = false;
             }
             else 
             {
                 radioButton2.Checked = true;
                 radioButton1.Checked = false;

                 for (int i = 0; i < listBox1.Items.Count; i++)
                 {
                     if (listBox1.Items[i].ToString() == Ipaybox.NetModemName)
                     {
                         listBox1.SelectedIndex = i;
                     }
                 }
             }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                listBox1.Enabled = false;
            }
            else
            {
                listBox1.Enabled = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                listBox1.Enabled = false;
            }
            else
            {
                listBox1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Ipaybox.NetOption = 0;
                Ipaybox.NetModemName = "";
                Ipaybox.SaveNetXML();
                Ipaybox.Internet = true;
            }
            else
            {
                Ipaybox.NetOption = 1;
                if (listBox1.SelectedItem != null)
                {
                    Ipaybox.NetModemName = listBox1.SelectedItem.ToString();
                    Ipaybox.Internet = false;
                    Ipaybox.SaveNetXML();
                }
                else
                {
                    Ipaybox.NetOption = 0;
                    Ipaybox.NetModemName = "";
                    Ipaybox.SaveNetXML();
                    Ipaybox.Internet = true;
                }
            }
        }
    }
}
