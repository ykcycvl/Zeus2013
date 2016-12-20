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
    public partial class info : Form
    {
        Label[] labels = new Label[10];
        PictureBox[] img = new PictureBox[10];
        private int label_count=0;
        private int img_count   = 0;
        public static List<AxShockwaveFlashObjects.AxShockwaveFlash> lFlash;



        public info()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(info_KeyDown);
        }

        void info_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F4)
            {
                Ipaybox.StartForm.Dispose();
            }
            if ( e.KeyCode == Keys.F9)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Вход в сервисное меню по нажатию клавиши.");
                Form n = new login();
                n.Show();
                Ipaybox.ServiceMenu = true;
                ExitForm();
            }
        }

        private void info_Load(object sender, EventArgs e)
        {
            pooling.Enabled = CanPolling;
            lFlash = new List<AxShockwaveFlashObjects.AxShockwaveFlash>();
            sorry.Visible = false;
            this.Size = Ipaybox.Resolution;

            //Отправка мониторинга
            new System.Threading.Thread(SendMonitoring).Start();

            if (Ipaybox.Inches == 17)
                this.Location = new Point(0, 0);

            try
            {
                _cursor.Hide();
            }
            catch { }

            if (Ipaybox.Working && Ipaybox.UpdateState == 0)
            {
                Process_Form(Ipaybox.FORM_XML);
            }
            else
            {
                try
                {
                    Point location = new Point
                        (
                            (int)(1280/2 - (sorry.Width/2)),
                            (int)(1024/2 - (sorry.Height/2))
                        );
                    //this.BackgroundImage = new Bitmap("img15\\not_working.gif");
                    sorry.Location = location;
                    sorry.Visible = true;
                }
                catch { this.Dispose(); }
            }
        }
        private void AddLabel(XmlElement el)
        {
            string location = el.GetAttribute("location");
            string text = el.GetAttribute("text");
            string size = el.GetAttribute("size");
            string font = el.GetAttribute("font");
            string font_size = el.GetAttribute("font-size");
            
            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);
            
            labels[label_count] = new Label();
            //labels[label_count].Parent = img[1];
            labels[label_count].BackColor = Color.Transparent;
            labels[label_count].Location = new Point(X, Y);
            labels[label_count].Size = new Size(int.Parse(size.Split(';')[0]), int.Parse(size.Split(';')[1]));
            labels[label_count].Text = text;
            //labels[label_count].BackColor = Color.Transparent;

            if (font.Length != 0 && font_size.Length != 0)
            {
                labels[label_count].Font = new Font(font,float.Parse(font_size));
            }
               
            this.Controls.Add(labels[label_count]);

            label_count++;
        }
        private void AddButton(XmlElement el)
        {
            string location = el.GetAttribute("location");
            string limg = el.GetAttribute("img");

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img[img_count] = new PictureBox();
            img[img_count].Location = new Point(X, Y);
            img[img_count].Image = bmp;
            img[img_count].Size = bmp.Size;
            img[img_count].Tag = el.GetAttribute("value");

            img[img_count].Click += new System.EventHandler(this.Pic_Click);

            this.Controls.Add(img[img_count]);

            img_count++;
        
        }
        private void AddImage(XmlElement el)
        {
            string location = el.GetAttribute("location");
            string limg = el.GetAttribute("img");
          

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img[img_count] = new PictureBox();
            img[img_count].Location = new Point(X, Y);
            img[img_count].Image = bmp;
            img[img_count].Size = bmp.Size;

            this.Controls.Add(img[img_count]);

            img_count++;
        }
        private void Process_Form(XmlElement frm)
        {
            XmlElement root = frm;

            try
            {
                this.BackgroundImage = new Bitmap(Ipaybox.StartupPath + @"\" + root.GetAttribute("backgroundimage"));
            }
            catch { }


            string bgcolor = string.Empty;

            try
            {
                root.GetAttribute("bgcolor");
            }
            catch
            { 
            }

            if (!string.IsNullOrEmpty(bgcolor))
            {
                string[] rgb = bgcolor.Split(';');

                if (rgb.Length == 3)
                {
                    try
                    {
                        this.BackColor = Color.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                    }
                    catch
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Неверное представление цвета в тэге bgcolor");
                    }
                }
            }

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == "img")
                {
                    Ipaybox.AddImage(row, ref img[img_count], this);
                    img_count++;
                }
                if (row.Name == "button")
                {
                    AddButton(row);
                }
                if (row.Name == "label")
                {
                    Ipaybox.AddLabel(row, ref labels[label_count], this);
                    label_count++;
                }
                if (row.Name == "flash")
                {
                    AddFlash(row, this);
                }
            }

        }
        public static void AddFlash(XmlElement el, System.Windows.Forms.Form f)
        {
            Point location = new Point();
            Size size = new Size();
            string src;
            try
            {
                if (!string.IsNullOrEmpty(el.GetAttribute("location")))
                {
                    location.X = int.Parse(el.GetAttribute("location").Split(';')[0]);
                    location.Y = int.Parse(el.GetAttribute("location").Split(';')[1]);
                }

                if (!string.IsNullOrEmpty(el.GetAttribute("size")))
                {
                    size.Width = int.Parse(el.GetAttribute("size").Split(';')[0]);
                    size.Height = int.Parse(el.GetAttribute("size").Split(';')[1]);
                }

                src = el.GetAttribute("src");
            }
            catch (Exception ex)
            {
                HelperClass.CrashLog.AddCrash(new Exception("AddFlash::ERROR IN ARGUMENTS", ex));
                return;
            }

            try
            {
                AxShockwaveFlashObjects.AxShockwaveFlash Flash = new AxShockwaveFlashObjects.AxShockwaveFlash();
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(info));
                ((System.ComponentModel.ISupportInitialize)(Flash)).BeginInit();
                f.SuspendLayout();
                // 
                // Flash
                // 
                Flash.Enabled = true;
                Flash.Location = location;
                Flash.Name = "Flash";
                Flash.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Flash.OcxState")));
                Flash.Size = new Size(size.Width, size.Height);
                //Flash.SetZoomRect(0, 0, 1280, 184);
                Flash.TabIndex = 0;
                f.Controls.Add(Flash);
                ((System.ComponentModel.ISupportInitialize)(Flash)).EndInit();
                f.ResumeLayout(false);

                Flash.LoadMovie(0, System.IO.Directory.GetCurrentDirectory() +
       System.IO.Path.DirectorySeparatorChar + src);
                Flash.Play();
                //Flash.ClientRectangle = new Rectangle(0, 0, 1280, 184);
                //Flash.ClientSize = size;
                //Flash.Dock = DockStyle.None;
                Flash.ScaleMode = 1;

                Flash.Size = new Size(size.Width, size.Height);
                //Flash.Pan(100, 100, 0);
                //Flash.Zoom(50);
                lFlash.Add(Flash);
                Ipaybox.Option["can-flash-initialize"] = "true";
            }
            catch
            {
                Ipaybox.Option["can-flash-initialize"] = "false";
            }
        }

        private void flush_timer_Tick(object sender, EventArgs e)
        {
            ExitForm();
        }
        private void Pic_Click(object sender, System.EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            //MessageBox.Show("CLICK PRV=" + pb.Tag);

            switch (pb.Tag.ToString().ToLower())
            { 
                case "ok":
                    Ipaybox.StartForm.Main_Process();
                    this.Dispose();break;
                case "cancel":
                    ExitForm();
                    break;
                case "vmenu":
                    ExitForm();
                    break;
            }
        }
        private void ExitForm()
        {
            Ipaybox.FlushToMain();
            try
            {
                Ipaybox.StartForm.Main_Process();
            }
            catch { }
            this.Dispose();
          
        }

        private void pooling_Tick(object sender, EventArgs e)
        {
           
            Pooling();
        }
        /// <summary>
        /// Возможно ли полоть вообще
        /// </summary>
        private bool CanPolling
        {
            get { return Ipaybox.Bill != null; }
        }
        /// <summary>
        /// Полим купюрник
        /// </summary>
        private void Pooling()
        {
            if (CanPolling)
            {
                try
                {
                    Ipaybox.Bill.AllowMoneyEnterOnPooling = false;
                    Ipaybox.Bill.Pooling();

                    // Обработка ошибок
                    if (Ipaybox.Bill.Error == true)
                    {
                        if (Ipaybox.Bill.ErrorMsg.IndexOf("DROP_CASSETTE_REMOVED") >= 0 && Ipaybox.ServiceMenu == false)
                        {
                            //Если не был введен неправильный пин - показать форму входа в сервисное меню
                            if (!Ipaybox.InvalidPinEntered)
                            {
                                if (!Ipaybox.LoginFormActive)
                                {
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Снят стекер. Показываем сервисное меню.");
                                    Form login = new login();
                                    login.Show();
                                    this.Dispose();
                                }
                            }
                        }
                        else
                        {
                            //Стекер не снят - сброс ошибки ввода неверного ПИН-кода
                            Ipaybox.InvalidPinEntered = false;
                        }
                    }
                    else
                    {
                        //Ошибок купюроприемника нет - сброс ошибки ввода неверного ПИН-кода
                        Ipaybox.InvalidPinEntered = false;
                    }
                }
                catch
                {   
                }
            }
        }

        private void SendMonitoring()
        {
            /* Добавлено 18/06/2012. Тен Вадим.
            * Вообще, по-идее, эта хрень должна отправлять инф. по терминалу на сервак.
            * Это по-идее...
            * По-факту, хуй знает, что тут получится, поэтому нужно тестировать и ждать Димона.
            */
            monitoring mon = new monitoring();

            if (mon.Send())
                Ipaybox.IsMonitorngSended = true;
            else
                Ipaybox.IsMonitorngSended = false;
        }
    }
}
