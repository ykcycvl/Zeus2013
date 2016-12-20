using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WhiteList
{
    public partial class WLForm : Form
    {
        public WLForm()
        {
            InitializeComponent();
        }

        private void FillList()
        {
            listBox1.Items.Clear();

            WhiteList wl = new WhiteList();
            wl = PhoneNumber.GetAll();

            for (int i = 0; i < wl.Count; i++)
            {
                listBox1.Items.Add(wl[i].phoneNumber);
            }
        }

        private void WLForm_Load(object sender, EventArgs e)
        {
            FillList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                PhoneNumber.Delete(listBox1.SelectedItem.ToString());
                FillList();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PhoneNumber.Add(textBox1.Text.Trim());
            FillList();
            textBox1.Clear();
        }
    }
}
