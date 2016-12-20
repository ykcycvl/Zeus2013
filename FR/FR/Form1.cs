using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FR
{
    public partial class Form1 : Form
    {  
        FiscalRegisters fr;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // FiscalRegisters fr = new FiscalRegisters();
          
            try
            {
                fr = new Atol(true);//ghbdtn rfhbdfh
               
                /*fr.SetPassword(PasswordMode.Kassir, "1");
                fr.SetPassword(PasswordMode.KKMAdministrator, "29");
                fr.SetPassword(PasswordMode.SystemAdministrator, "30");*/

               
                //fr.ClosePayment();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ClearStatus()
        { toolStripStatusLabel1.Text = "Выполняется..."; }
        private void button1_Click(object sender, EventArgs e)
        {
            ClearStatus();
            if(!fr.Buy(what.Text, 1, Decimal.Parse(amount.Text.Replace(".",",")), text.Text))
                MessageBox.Show(fr.ErrorMessage + "\r\n #"+fr.ErrorNumber);
            ShowErr();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearStatus();
            fr.GetZReport();
            ShowErr();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearStatus();
            fr.GetXReport();
            ShowErr();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearStatus();
            fr.GetDelayedZReports();
            ShowErr();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearStatus();
            fr.AddDelayedZReport();
            ShowErr();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearStatus();
            fr.GetStatus();
            ShowErr();
        }
        private void ShowErr()
        { toolStripStatusLabel1.Text = "Ошибка " + fr.ErrorNumber + " (" + fr.ErrorMessage + ")"; }

        private void button7_Click(object sender, EventArgs e)
        {
            fr.ShowFiscalSettingsForm();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ProgressAnimator a = new ProgressAnimator();
            a.Show();
           // System.Threading.Thread.Sleep(10000);
           // a.Dispose();
        }
    }
}
