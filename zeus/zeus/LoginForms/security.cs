using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;



namespace zeus.LoginForms
{
    public partial class security1 : Form
    {
        public security1()
        {
            InitializeComponent();
        }

        private void security_Load(object sender, EventArgs e)
        {
            string shellApplic;
            shellApplic = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "Shell", "notexists").ToString();

            if (shellApplic.ToLower() == "explorer.exe")
                shell.Checked = false;
            else
                shell.Checked = true;

        }

        private void shell_CheckedChanged(object sender, EventArgs e)
        {
            /*if (shell.Checked)
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "Shell", Ipaybox.StartupPath.TrimEnd('\\') + "\\zeus.exe");
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Подменяем shell на zues");
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "Shell", "Explorer.exe");
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Подменяем shell на explorer.exe");

            }*/

            if (shell.Checked)
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "Shell", Ipaybox.StartupPath + "\\start_zeus.bat");
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Подменяем shell на zeus");
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "Shell", "Explorer.exe");
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Подменяем shell на explorer.exe");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Ipaybox.StartExplorer();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Ipaybox.HideExplorer();
        }

    }
}
