using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace zeus.LoginForms
{
    public partial class PrinterSettings : Form
    {
        private string filename = Ipaybox.StartupPath + @"\start_zeus.js";

        public PrinterSettings()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void PrinterSettings_Load(object sender, EventArgs e)
        {
            FileInfo fi = new FileInfo(filename);
            string p = string.Format("WSHShell = WScript.CreateObject(\"WScript.Shell\");\r\nWSHShell.Run(\"{0}\\zeus.exe\",3);", Ipaybox.StartupPath);

            if (fi.Exists)
            {
                p = File.ReadAllText(filename);
            }
            /*else
                File.Create(filename);*/

            Match m = Regex.Match(p, "zeus.exe(?<val>.*?)[\"]");

            string SelectedOption = string.Empty;

            if (m.Success)
            { 
                SelectedOption = m.Groups["val"].Value.ToString().Trim();
            }

            if (SelectedOption.ToLower() == "atol")
            {
                radioButton1.Checked = true;
            }
            else
            {
                if (SelectedOption.ToLower() == "prtwin")
                {
                    radioButton2.Checked = true;
                }
                else
                {
                    radioButton3.Checked = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText(filename, String.Format("WSHShell = WScript.CreateObject(\"WScript.Shell\");\r\nWSHShell.Run(\"{0}\\zeus.exe {1}\",3);", Ipaybox.StartupPath, GetOption()));
        }
        private string GetOption()
        {
            string res = string.Empty;

            if (radioButton1.Checked)
            {
                res = "atol";
            }
            else
            {
                if (radioButton2.Checked)
                {
                    res = "prtwin";
                }
                else
                {
                    if (radioButton3.Checked)
                    {
                        res = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Необходимо выбрать опцию!");
                    }
                }
            }

            return res;
        }
    }
}
