using System;
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
    public partial class thanks : Form
    {
        zeus.HelperClass.zForm default_form__;
        zPictureBox[] img = new zPictureBox[10];
        int img_count;
        Label[] labels = new Label[10];
        int labels_count;
        //XmlElement xml_thanks;
        string amount;
        string to_amount;
        string comission;
        string account;
        public thanks(string amount1, string to_amount1, string comission1, string account1)
        {
            amount = amount1;
            to_amount = to_amount1;
            comission = comission1;
            account = account1;
            InitializeComponent();
        }

        private void thanks_Load(object sender, EventArgs e)
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Показываем форму `Спасибо`.");
            this.Size = Ipaybox.Resolution;

            if (Ipaybox.Inches == 17)
                this.Location = new Point(0, 0);
            try
            {
                _cursor.Hide();

                if (!Ipaybox.Debug)
                    this.TopMost = false;
            }
            catch { }
            LoadThisForm();
        }
        private void LoadThisForm()
        {
            try
            {
                zeus.HelperClass.zForm zform = Ipaybox.ifc.forms.Find(p => p.name == "thanks");
                Process_Form(zform);
            }
            catch
            {
                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.NeedUpdates.Config = true;
                Ipaybox.Working = false;
            }
        }

        private void Process_Form(zeus.HelperClass.zForm frm)
        {
            // Установка таймаута бездействия в мс
            if (frm.timeout != 0)
                flush_timer.Interval = frm.timeout * 1000;

            //Установка "бэкграунда"
            try
            {
                this.BackgroundImage = new Bitmap(Ipaybox.StartupPath + @"\" + frm.bgimg);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось загрузить фоновое изображение");
                ExitForm();
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
                Ipaybox.AddButton(frm.buttons[i], ref img[img_count], this, new EventHandler(this.Pic_Click));
                img_count++;
            }

            for (int i = 0; i < frm.labels.Count; i++)
            {
                Ipaybox.AddLabel(frm.labels[i], ref labels[labels_count], this);
                labels_count++;
            }
        }
        public void AddLabel(XmlElement el, ref Label label, System.Windows.Forms.Form f)
        {
            if (label == null)
                label = new Label();
            string location = el.GetAttribute("location");
            string text = el.GetAttribute("text");
            string size = el.GetAttribute("size");
            string font = el.GetAttribute("font");
            string font_size = el.GetAttribute("font-size");
            string color = el.GetAttribute("color");
            string textalign = el.GetAttribute("textalign");
            string style = el.GetAttribute("style");
            string name = el.GetAttribute("name");
            string BackColor = el.GetAttribute("backcolor");

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            //label = new Label();
            //labels[label_count].Parent = img[1];
            if (text.Length != 0)
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

            if (BackColor.Length > 5)
            {
                red = int.Parse(BackColor.Split(';')[0]);
                green = int.Parse(BackColor.Split(';')[1]);
                blue = int.Parse(BackColor.Split(';')[2]);

                label.BackColor = Color.FromArgb(red, green, blue);
            }

            if (font.Length != 0 && font_size.Length != 0)
            {
                switch (style)
                {
                    case "bold": label.Font = new Font(font, float.Parse(font_size), FontStyle.Bold); break;
                    case "italic": label.Font = new Font(font, float.Parse(font_size), FontStyle.Italic); break;
                    case "strikeout": label.Font = new Font(font, float.Parse(font_size), FontStyle.Strikeout); break;
                    case "underline": label.Font = new Font(font, float.Parse(font_size), FontStyle.Underline); break;
                    default:
                        label.Font = new Font(font, float.Parse(font_size), FontStyle.Regular);
                        break;
                }



            }

            if (textalign.Length != 0)
            {
                switch (textalign.ToLower())
                {
                    case "middleright": label.TextAlign = ContentAlignment.MiddleRight; break;
                    case "middleleft": label.TextAlign = ContentAlignment.MiddleLeft; break;
                    case "middlecenter": label.TextAlign = ContentAlignment.MiddleCenter; break;
                    case "topleft": label.TextAlign = ContentAlignment.TopLeft; break;
                    case "topright": label.TextAlign = ContentAlignment.TopRight; break;
                    default:
                        label.TextAlign = ContentAlignment.TopLeft; break;
                }

            }

            switch (name)
            { 
                case "amount":
                    label.Text = amount;
                    break;
                case "to_amount":
                    label.Text = to_amount;
                    break;
                case "account":
                    label.Text = account;
                    break;
                case "comission":
                    label.Text = comission;
                    break;
            }
            f.Controls.Add(label);

        }
        private void Pic_Click(object sender, System.EventArgs e)
        {
            Sound.Play(Ipaybox.StartupPath.TrimEnd('\\') + "\\sounds\\" + "click1.wav");
            PictureBox pb = (PictureBox)sender;
            //MessageBox.Show("CLICK PRV=" + pb.Tag);
            string tag = pb.Tag.ToString().ToLower();

            // Нажата не кнопка на клаве
            switch (tag)
            {
                case "ok":
                   ExitForm();
                    break;
                case "cancel":
                    ExitForm();
                    break;
                case "goprev":
                  ExitForm();
                    break;
            }
        }
        private void ExitForm()
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Выход формы `Спасибо`.");
            Ipaybox.FlushToMain();
            Ipaybox.StartForm.Main_Process();
            this.Dispose();
        }

        private void flush_timer_Tick(object sender, EventArgs e)
        {
            ExitForm();
        }
    }
}
