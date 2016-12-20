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
    public partial class FRParametrs : Form
    {
        void time_report_Leave(object sender, System.EventArgs e)
        {
            //Изменяем время автоснятия z-отчетов

            System.DateTime dt;

            if (System.DateTime.TryParse("2001-01-01 " + time_report.Text, out dt))
            {
                Ipaybox.Option["auto-zreport"] = dt.ToShortTimeString();
            }
            else
            {
                MessageBox.Show("Неправильное время `" + time_report + "`","Изменения не сохранены!", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }



        }
        public FRParametrs()
        {
            InitializeComponent();

            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void ClearStatus()
        {
            errorN.Text = "Выполняется...";
            Message.Text = "";
            Application.DoEvents();
        }
        private void ShowResult()
        {
            errorN.Text = Ipaybox.FRegister.ErrorNumber;
            Message.Text = Ipaybox.FRegister.ErrorMessage;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ClearStatus();
            Ipaybox.FRegister.GetStatus();
            ShowResult();
        }

        private void FRParametrs_Load(object sender, EventArgs e)
        {
            if (Ipaybox.FRegister != null)
            {
                label1.Text = "Модель: " + Ipaybox.FRegister.Model;
                label2.Text = "COM-порт:" + Ipaybox.FRegister.ComPort;
                label3.Text = "Протокол:" + Ipaybox.FRegister.Protocol;
                label4.Text = "Версия:" + Ipaybox.FRegister.Version;
                ShowResult();
                checkBox1.Checked = Ipaybox.FRegister.FiscalMode;

                if (Ipaybox.Option["auto-zreport"] != null)
                {
                    checkBox2.Checked = true;
                    time_report.Text = Ipaybox.Option["auto-zreport"];
                }
                else
                {
                    checkBox2.Checked = false;
                    time_report.Enabled = false;
                }
            }
            else
            { 
                
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Ipaybox.Option["fiscal-mode"] = checkBox1.Checked.ToString();
            Ipaybox.FRegister.FiscalMode = checkBox1.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearStatus();
            Ipaybox.FRegister.GetXReport();
            ShowResult();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearStatus();
            Ipaybox.FRegister.GetZReport();
            ShowResult();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Ipaybox.FRegister.ShowFiscalSettingsForm();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                time_report.Enabled = true;
                time_report.Text = "23:59";
            }
            else
            {
                time_report.Enabled = false;
                Ipaybox.Option.Remove("auto-zreport");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearStatus();
            Ipaybox.FRegister.GetDelayedZReports();
            ShowResult();
        }

      
    }
}
