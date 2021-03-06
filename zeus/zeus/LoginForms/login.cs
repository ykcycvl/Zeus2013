﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace zeus
{
    public partial class login : Form
    {
        private string pin;
        private int EnteringPINcount = 0;

        public login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pin += "1";
            textBox1.Text = pin;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pin += "2";
            textBox1.Text = pin;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pin += "3";
            textBox1.Text = pin;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pin += "4";
            textBox1.Text = pin;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pin += "5";
            textBox1.Text = pin;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pin += "6";
            textBox1.Text = pin;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pin += "7";
            textBox1.Text = pin;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            pin += "8";
            textBox1.Text = pin;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            pin += "9";
            textBox1.Text = pin;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            pin += "A";
            textBox1.Text = pin;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pin += "0";
            textBox1.Text = pin;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            pin += "B";
            textBox1.Text = pin;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            pin = "" ;
            textBox1.Text = pin;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            bool AccessGranted = false;
            string UserName = "ПИН по-умолчанию.";

            if (!Ipaybox.MasterPIN_IsActive)
            {
                for (int i = 0; i < Ipaybox.TPIN.GetElementsByTagName("person").Count; i++)
                {
                    XmlElement el = Ipaybox.TPIN.GetElementsByTagName("person")[i] as XmlElement;
                    string pin_el = el.GetAttribute("pin").ToString();

                    if (pin_el == Ipaybox.getMd5Hash(pin))
                    {
                        AccessGranted = true;
                        UserName = el.GetAttribute("name").ToString();
                        Ipaybox.userID = Convert.ToUInt32(el.GetAttribute("pid"));
                        break;
                    }
                }
            }
            else
            {
                XmlElement el = Ipaybox.terminal_info.GetElementsByTagName("pin")[0] as XmlElement;
                string pin_el = el.InnerText;

                if (pin_el == Ipaybox.getMd5Hash(pin))
                {
                    AccessGranted = true;
                    Ipaybox.userID = 1;
                }
            }

            if (AccessGranted)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Попытка входа в сервисное меню. " + UserName);
                Form i = new options();
                Ipaybox.LoginFormActive = false;
                i.Show();
                this.Dispose();
            }
            else
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Попытка входа в сервисное меню pin-code:" + pin);
                EnteringPINcount++;
                pin = "";

                if (EnteringPINcount > 2)
                {
                    Ipaybox.Working = false;
                    Ipaybox.InvalidPinEntered = true;
                    Ipaybox.ServiceMenu = false;
                    Ipaybox.StartForm.Main_Process();
                    Ipaybox.LoginFormActive = false;
                    this.Dispose();
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "ПИН введен неверно 3 раза");
                }
            }

            textBox1.Text = pin;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Ipaybox.InvalidPinEntered = true;
            Ipaybox.Working = false;
            Ipaybox.ServiceMenu = false;
            Ipaybox.StartForm.Main_Process();
            Ipaybox.LoginFormActive = false;
            this.Dispose();
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Выход в главное меню по тайм-ауту.");
        }

        private void login_Load(object sender, EventArgs e)
        {
            if (!Ipaybox.LoginFormActive)
            {
                Ipaybox.LoginFormActive = true;
                _cursor.Show();

                if (!Ipaybox.Debug)
                    this.TopMost = true;

                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Вход в Сервисное меню - Ввод пин-кода.");
            }
            else
                this.Dispose();
        }

        private void login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ipaybox.ServiceMenu = false;
            Ipaybox.StartForm.Main_Process();
            Ipaybox.LoginFormActive = false;
            this.Dispose();
        }
    }
}
