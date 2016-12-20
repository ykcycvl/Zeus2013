﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using zeus.HelperClass;

namespace zeus.LoginForms
{
    public partial class information : Form
    {
        public XmlElement Xml_Information;
        private Bitmap _backBuffer;
        private zPictureBox[] img = new zPictureBox[50];
        private Label[]      labels = new Label[20];
        private int label_count = 0;
        private int img_count = 0;
        private zInformation zinfo;

      
        private string CurrentForm="";
        public information( )
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (_backBuffer == null)
            {
                _backBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                Graphics g = Graphics.FromImage(_backBuffer);

                if (this.BackgroundImage != null)
                    g.DrawImageUnscaled(this.BackgroundImage, 0, 0);

                for (int i = 0; i < img_count; i++)
                {
                    if (img[i].Visible)
                    {
                        try
                        {
                            PictureBox pb = (PictureBox)this.img[i];
                            Rectangle dest = new Rectangle(pb.Left, pb.Top, pb.Width, pb.Height);
                            string p = pb.Tag.ToString();
                            g.DrawImage(pb.Image, dest);
                        }
                        catch
                        { }
                    }
                }
            }

            pevent.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);

            //Paint your graphics on g here
            // Нужно прорисовать все картинки тут.

            //g.Dispose();
            //Copy the back buffer to the screen
            //_backBuffer.Dispose();
        }
        void information_Click(object sender, System.EventArgs e)
        {
            MouseEventArgs p = (MouseEventArgs)e;

            for (int i = 0; i < img_count; i++)
            {
                if (p.X >= img[i].Left && p.X <= img[i].Left + img[i].Width
                    && p.Y >= img[i].Top && p.Y <= img[i].Top + img[i].Height)
                {
                    Button_Click(img[i], new EventArgs());
                    break;

                }
            }
        }

        public static string Transformtext(string text)
        {
            text = text.Replace("[terminal.id]", Ipaybox.Terminal.terminal_id);
            text = text.Replace("[terminal.adress]", Ipaybox.Terminal.trm_adress);
            text = text.Replace("[agent.name]", Ipaybox.Terminal.jur_name);
            text = text.Replace("[agent.bank]", Ipaybox.Terminal.bank);
            text = text.Replace("[agent.supportphone]", Ipaybox.Terminal.support_phone);
            text = text.Replace("[agent.INN]", Ipaybox.Terminal.jur_inn);
            text = text.Replace("[agent.jur_adress]", Ipaybox.Terminal.jur_adress);
            return text;
        }
        private void information_Load(object sender, EventArgs e)
        {
            if(!Ipaybox.Debug)
                this.TopMost = true;

            for (int i = 0; i < img_count; i++)
            {    
                img[i].Hide();
            }
            img_count = 0;

            for (int i = 0; i < label_count; i++)
            {
                labels[i].Hide();
            }

            label_count = 0;

            if (Ipaybox.Inches == 17)
            {
                this.Location = new Point(0, 0);
                this.Size = new Size(1280, 1024);
            }
            else
                this.Size = new Size(1024, 768);

            try 
            {
                zinfo = Ipaybox.ifc.information;
                /*Xml_Information = (XmlElement) Ipaybox.XML_Interface.SelectSingleNode("information");
                if (Xml_Information == null)
                    this.Dispose();*/
            }
            catch
            {
                this.Dispose();
            }

            LoadInformation();
        }
        private void LoadInformation()
        {
            if (zinfo != null)
            {
                zForm infoFormToShow = zinfo.forms.Find(p => p.id == CurrentForm);
                Process_Form(infoFormToShow);
            }

            /*if (Xml_Information != null)
            {
                foreach (XmlElement el in Xml_Information)
                {
                    if (el.Name == "form" && el.GetAttribute("id") == CurrentForm)
                    {
                        DateTime start = DateTime.Now;
                        ProcessForm(el);
                        DateTime end = DateTime.Now;
                        MessageBox.Show((end - start).Ticks.ToString());
                        break;
                    }
                }
            }*/
        }

        private void Process_Form(zeus.HelperClass.zForm frm)
        {
            // Установка таймаута бездействия в мс
            if (frm.timeout == 0)
                flush_timer.Interval = 30000;
            else
                flush_timer.Interval = frm.timeout * 1000;

            //Установка "бэкграунда"
            try
            {
                this.BackgroundImage = new Bitmap(Ipaybox.StartupPath + @"\" + frm.bgimg);
            }
            catch
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось загрузить фоновое изображение");
                Dispose();
            }

            //Добавляем элементы на форму
            //Надписи, кнопки, изображения, флэшки и т.п.
            for (int i = 0; i < frm.images.Count; i++)
            {
                Ipaybox.AddImage(frm.images[i], ref img[img_count], this);
                img_count++;
            }

            for (int i = 0; i < frm.buttons.Count; i++)
            {
                Ipaybox.AddButton(frm.buttons[i], ref img[img_count], this, new EventHandler(this.Button_Click));
                img_count++;
            }

            /*if (frm.label != null)
            {
                
            }*/

            for (int i = 0; i < frm.labels.Count; i++)
            {
                frm.labels[i].value = Transformtext(frm.labels[i].value);
                Ipaybox.AddLabel(frm.labels[i], ref labels[label_count], this);
                label_count++;
            }
        }

        private void ProcessForm(XmlElement frm)
        {
            XmlElement root = frm;

            string timeout = root.GetAttribute("timeout");
            if (timeout == "")
            {
                flush_timer.Interval = 15000;
            }
            else
            {
                flush_timer.Interval = int.Parse(timeout) * 1000;
            }
            flush_timer.Stop();
            flush_timer.Start();
            try
            {
                string p = root.GetAttribute("backgroundimage");
                this.BackgroundImage = new Bitmap(Ipaybox.StartupPath + @"\" + p);
            }
            catch
            {
                Dispose(); 
            }

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == "img")
                {
                    AddImage(row, ref img[img_count], this);
                    img_count++;
                }
                if (row.Name == "button")
                {
                    //AddButton(row);
                    AddButton(row, ref img[img_count], this, new EventHandler(this.Button_Click));
                    img_count++;
                }
                if (row.Name == "label")
                {
                    AddLabel(row, ref labels[label_count], this);
                    label_count++;
                }
            }
        }
        public static void AddLabel(XmlElement el, ref Label label, System.Windows.Forms.Form f)
        {
            string location = el.GetAttribute("location");
            string text = el.GetAttribute("text");
            string size = el.GetAttribute("size");
            string font = el.GetAttribute("font");
            string font_size = el.GetAttribute("font-size");
            string color     = el.GetAttribute("color");
            string textalign = el.GetAttribute("textalign");
            string tag = el.GetAttribute("tag");
            



            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            label = new Label();
            //labels[label_count].Parent = img[1];
            label.Text = text;
            label.BackColor = Color.Transparent;
            
            label.Location = new Point(X, Y);

            if (size.Length != 0)
                label.Size = new Size(int.Parse(size.Split(';')[0]), int.Parse(size.Split(';')[1]));
            else
                label.AutoSize = true;
            // Цвет
            int red = 0;
            int green = 0;
            int blue = 0;

            if (color.Length > 5)
            {
                red = int.Parse(color.Split(';')[0]);
                green = int.Parse(color.Split(';')[1]);
                blue = int.Parse(color.Split(';')[2]);

                label.ForeColor = Color.FromArgb(red, green, blue);
            }


            if (font.Length != 0 && font_size.Length != 0)
            {
                label.Font = new Font(font, float.Parse(font_size));

            }

            if (textalign.Length != 0)
            {
                //label.TextAlign = ContentAlignment.BottomCenter
            }
            string t = el.InnerText.ToString();
            label.Text = Transformtext(t);
            f.Controls.Add(label);

        }
        public void AddImage(XmlElement el, ref zPictureBox img, System.Windows.Forms.Form f)
        {
            string location = el.GetAttribute("location");
            string limg = el.GetAttribute("img");
            string size = el.GetAttribute("size");

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img = new zPictureBox();
            img.Location = new Point(X, Y);
            img.Image = bmp;

            if (size != "")
            {
                X = int.Parse(size.Split(';')[0]);
                Y = int.Parse(size.Split(';')[1]);
                img.SizeMode = PictureBoxSizeMode.StretchImage;
                img.Size = new Size(X, Y);
            }
            else
            {
                img.Size = bmp.Size;
            }

            f.Controls.Add(img);


        }
        public void AddButton(XmlElement el, ref zPictureBox img, System.Windows.Forms.Form f, EventHandler tar)
        {
            string location = el.GetAttribute("location");
            string limg = el.GetAttribute("img");
            string tag = el.GetAttribute("value");


            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap tmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);//Ipaybox.Pics[int.Parse(Ipaybox.images[limg])];
            img = new zPictureBox();
            img.Location = new Point(X, Y);
            
            img.Image = tmp;
            img.Size = tmp.Size;
            img.Tag = tag;
            // НЕРАБОТАЕТ
            img.Click += tar;


            //f.Controls.Add(img);



        }
        private void Button_Click(object sender, System.EventArgs e)
        {
            flush_timer.Stop();
            flush_timer.Start();

            Sound.Play(Ipaybox.StartupPath.TrimEnd('\\') + "\\sounds\\" + "click1.wav");
            PictureBox pb = (PictureBox)sender;

            string tag = pb.Tag.ToString();
            System.Diagnostics.Debug.Write("Нажата: "+tag);
             switch (tag)
            { 
                case "vmenu":
                    Ipaybox.StartForm.Main_Process();
                    this.Dispose();
                    break;
                case "back":
                    CurrentForm = "";
                    information_Load(this, new EventArgs());
                    break;
                default:
                    string[] spl = tag.Split(':');
                    if (spl.Length > 1)
                    {
                        switch (spl[0])
                        {
                            case "goform":
                                CurrentForm = spl[1];
                                information_Load(this, new EventArgs());
                                _backBuffer = null;
                                break;
                        }
                    }
                    break;
            }
        }
        private void flush_timer_Tick(object sender, EventArgs e)
        {
            Ipaybox.StartForm.Main_Process();
            this.Dispose();
        }

    }
}
