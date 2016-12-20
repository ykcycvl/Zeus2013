using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using IpayboxControls;

namespace FR
{
    class FiscalSettings
    {
        public delegate void SettingsChangedEventHandler(object Sender);

        public event SettingsChangedEventHandler SettingsChanged;

        const string _filename = "fs.xml";
        private System.Collections.Specialized.NameValueCollection nvc;
        ScrTextBox t1;
        ScrTextBox t2;
        ScrTextBox t3;
        ScrTextBox t4;
        System.Windows.Forms.Form p;

        public FiscalSettings()
        {
            nvc = new System.Collections.Specialized.NameValueCollection();

            try { LoadSettings(); }
            catch
            { InitializeForm(); }

        
        }
        public void InitializeForm()
        {
            p = new System.Windows.Forms.Form();
            p.TopMost = true;
            p.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            p.Text = "Введите пароли доступа к ККМ.";
            p.Size = new System.Drawing.Size(400, 250);

            System.Windows.Forms.Label l1 = new System.Windows.Forms.Label();
            l1.Text = "Пароль Системного администратора ККМ:";
            l1.Location = new System.Drawing.Point(10,10);
            l1.AutoSize = true;
            System.Windows.Forms.Label l2 = new System.Windows.Forms.Label();
            l2.Text = "Пароль администратора ККМ:";
            l2.Location = new System.Drawing.Point(10, 50);
            l2.AutoSize = true;
            System.Windows.Forms.Label l3 = new System.Windows.Forms.Label();
            l3.Text = "Пароль кассира ККМ:";
            l3.Location = new System.Drawing.Point(10, 90);
            l3.AutoSize = true;
            System.Windows.Forms.Label l4 = new System.Windows.Forms.Label();
            l4.Text = "Пароль техника ККМ:";
            l4.Location = new System.Drawing.Point(10, 130);
            l4.AutoSize = true;

            t1 = new ScrTextBox();
            t1.Location = new System.Drawing.Point(250, 10);
            t1.Size = new System.Drawing.Size(100, 10);
            t2 = new ScrTextBox();
            t2.Location = new System.Drawing.Point(250, 50);
            t2.Size = new System.Drawing.Size(100, 10);
            t3 = new ScrTextBox();
            t3.Location = new System.Drawing.Point(250, 90);
            t3.Size = new System.Drawing.Size(100, 10);
            t4 = new ScrTextBox();
            t4.Location = new System.Drawing.Point(250, 130);
            t4.Size = new System.Drawing.Size(100, 10);

            System.Windows.Forms.Button b = new System.Windows.Forms.Button();
            b.Text = "Применить";
            b.AutoSize = true;
            b.Location = new System.Drawing.Point(150, 180);
            b.Click += new EventHandler(b_Click);

            p.Controls.Add(l1);
            p.Controls.Add(l2);
            p.Controls.Add(l3);
            p.Controls.Add(l4);
            p.Controls.Add(t1);
            p.Controls.Add(t2);
            p.Controls.Add(t3);
            p.Controls.Add(t4);

            p.Controls.Add(b);

            p.Show();
        }
        

        void b_Click(object sender, EventArgs e)
        {
            nvc.Clear();
            nvc.Add("sysadmin", t1.Text);
            nvc.Add("admin", t2.Text);
            nvc.Add("kassa", t3.Text);
            nvc.Add("tech", t4.Text);
            try
            {
                SaveSettings();

                System.Windows.Forms.MessageBox.Show("Данные успешно сохранены!", "Сохранение.", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                if (SettingsChanged != null)
                    SettingsChanged(this);
                
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("При сохранении параметров произошла ошибка.\r\n"+ex.Message, "Сохранение.", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

            }
            p.Dispose();
        }
        public string this[string Name]
        {
            get{ return nvc[Name]; }
        }
        private void SaveSettings()
        { 
            FileInfo fi = new FileInfo("config\\" + _filename);
            DirectoryInfo di = new DirectoryInfo("config\\");
            if (!di.Exists)
                di.Create();

            StreamWriter sw ;
           /* if (!fi.Exists)
                fi.Create();*/

            sw = fi.CreateText();
            

            StringBuilder sb = new StringBuilder("<fiscal-settings>");

            for (int i = 0; i < nvc.Count; i++)
            {
                sb.Append("<" + nvc.GetKey(i) + ">" + nvc[i] + "</" + nvc.GetKey(i) + ">");
            }

            sb.Append("</fiscal-settings>");

            sw.Write(sb.ToString());
            sw.Close();


        }
        public void Event()
        {
            if (SettingsChanged != null)
                SettingsChanged(this);
        }
        private void LoadSettings()
        {
            FileInfo fi = new FileInfo("config\\" + _filename);

            if (fi.Exists)
            {
                StreamReader sr = fi.OpenText();
                string xml = sr.ReadToEnd();
                sr.Close();

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                doc.LoadXml(xml);

                foreach (System.Xml.XmlElement el in doc.DocumentElement)
                {
                    nvc.Add(el.Name, el.InnerText);
                }

                if (SettingsChanged != null)
                    SettingsChanged(this);


            }
            else
            {
                throw new Exception("Не найден файл "+fi.FullName);
            }
        }
    }
}
