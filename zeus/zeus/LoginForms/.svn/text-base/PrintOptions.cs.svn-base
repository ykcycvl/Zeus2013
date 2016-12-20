using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace zeus.LoginForms
{
    public partial class PrintOptions : Form
    {
        public PrintOptions()
        {
            InitializeComponent();
        }

        private void PrintOptions_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = Ipaybox.FRS.RemoteFR;
            textBox1.Text = Ipaybox.FRS.checkWidth.ToString();
            textBox2.Text = Ipaybox.FRS.remoteFRtimeout.ToString();
            textBox3.Text = Ipaybox.FRS.RemoteFiscalRegisterURL;

            textBox1.Enabled = checkBox1.Checked;
            textBox2.Enabled = checkBox1.Checked;
            textBox3.Enabled = checkBox1.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Ipaybox.FRS.RemoteFR = checkBox1.Checked;
            textBox1.Enabled = checkBox1.Checked;
            textBox2.Enabled = checkBox1.Checked;
            textBox3.Enabled = checkBox1.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Ipaybox.FRS.RemoteFR = checkBox1.Checked;
            Ipaybox.FRS.checkWidth = Convert.ToInt32(textBox1.Text);
            Ipaybox.FRS.remoteFRtimeout = Convert.ToInt32(textBox2.Text);
            Ipaybox.FRS.RemoteFiscalRegisterURL = textBox3.Text.Trim();
            Reset();
            Close();
        }

        private void Reset()
        {
            XmlElement root1 = Ipaybox.FRSSettings.DocumentElement;

            if (root1 == null)
            {
                Ipaybox.FRSSettings = new XmlDocument();
                Ipaybox.FRSSettings.LoadXml("<response><frs></frs></response>");
                root1 = Ipaybox.FRSSettings.DocumentElement;
            }

            SetNew("remoteFR", Ipaybox.FRS.RemoteFR.ToString());
            SetNew("remoteFRurl", Ipaybox.FRS.RemoteFiscalRegisterURL);
            SetNew("checkWidth", Ipaybox.FRS.checkWidth.ToString());
            SetNew("remoteFRtimeout", Ipaybox.FRS.remoteFRtimeout.ToString());

            Ipaybox.FRSSettings.Save(Ipaybox.StartupPath + "\\config\\frssettings.xml");
        }

        private void SetNew(string name, string value)
        {
            bool exist = false;

            XmlNode root = Ipaybox.FRSSettings.DocumentElement.ChildNodes[0];

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == name)
                {
                    row.InnerText = value;
                    exist = true;
                }
            }

            if (!exist)
            {
                XmlElement el = Ipaybox.FRSSettings.CreateElement(name);
                el.InnerText = value;

                Ipaybox.FRSSettings.DocumentElement.InsertAfter(el, Ipaybox.FRSSettings.DocumentElement.LastChild);
            }

        }
    }
}
