using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using zeus.HelperClass;

namespace zeus
{
    public partial class acceptaccount : Form
    {
        private Bitmap _backBuffer;
        bool AccauntValidated;
        bool IsAccountValid ;
        bool IsServiceAvailable;
        zPictureBox[] img = new zPictureBox[10];
        PictureBox prov = new PictureBox();
        int img_count;
        Label[] labels = new Label[10];
        int labels_count;
        int glResult;
        public static List<AxShockwaveFlashObjects.AxShockwaveFlash> lFlash;

        int indexDalee;

        public acceptaccount()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(acceptaccount_KeyDown);
        }
        void acceptaccount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F4)
            {
                Ipaybox.StartForm.Dispose();
            }
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
                            //PictureBox pb = (PictureBox)this.img[i];
                            Rectangle dest = new Rectangle(this.img[i].Left, this.img[i].Top, this.img[i].Width, this.img[i].Height);

                            g.DrawImage(this.img[i].Image, dest);
                        }
                        catch
                        { }
                    }
                }

                /*foreach (Label label in this.Controls.OfType<Label>())
                {
                    label.Visible = false;

                    float X = label.Location.X;
                    float Y = label.Location.Y;
                    StringFormat sf = new StringFormat();

                    if (label.TextAlign == ContentAlignment.TopCenter)
                    {
                        //sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        label.BackColor = Color.Red;
                    }

                    g.DrawString(label.Text, label.Font, new SolidBrush(label.ForeColor), X, Y, sf);
                }*/

                g.Dispose();
            }

            pevent.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
        }

        public static void AddImage(zeus.HelperClass.zImage image, ref zPictureBox img)
        {
            string location = image.location;
            string limg = image.src;
            string size = image.size;

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
        }

        public static void AddImage(XmlElement el, ref PictureBox img)
        {
            string location = el.GetAttribute("location");
            string limg = el.GetAttribute("img");
            string size = el.GetAttribute("size");

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img = new PictureBox();
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

            //f.Controls.Add(img);
        }

        void acceptaccount_Click(object sender, System.EventArgs e)
        {
            MouseEventArgs p = (MouseEventArgs)e;

            for (int i = 0; i < img_count; i++)
            {
                if (img[i].Tag != null && !string.IsNullOrEmpty(img[i].Tag.ToString()) && (p.X >= img[i].Left && p.X <= img[i].Left + img[i].Width
                    && p.Y >= img[i].Top && p.Y <= img[i].Top + img[i].Height))
                {
                    if (img[i].Visible)
                    {
                        Pic_Click(img[i], new EventArgs());
                        break;
                    }
                }
            }
        }

        private void LoadThisForm()
        {
            /* Загрузка формы */
            try
            {
                zeus.HelperClass.zProvider provider = Ipaybox.ifc.customSets.Find(p => p.id.ToString() == Ipaybox.PRV_SELECTED_ID);
                zeus.HelperClass.zGroup group = Ipaybox.ifc.groups.Find(p => p.id.ToString() == Ipaybox.PRV_SELECTED_GROUP.id);
                zeus.HelperClass.zFormLink formLink = new zeus.HelperClass.zFormLink();

                //Если не найдено собственного набора форм для выбранного провайдера
                //и найден набор форм по умолчанию для группы, которой принадлежит провайдер
                //начинаем обработку набора форм по-умолчанию для группы
                if (provider == null && group != null)
                    formLink = group.forms.Find(p => p.name.ToLower() == "acceptaccount");
                else
                    if (provider != null)
                        formLink = provider.forms.Find(p => p.name.ToLower() == "acceptaccount");

                if (formLink == null)
                {
                    formLink = new zeus.HelperClass.zFormLink();
                    formLink.name = "acceptaccount";
                }
                zeus.HelperClass.zForm zform = Ipaybox.ifc.forms.Find(p => p.name.ToLower() == formLink.name.ToLower());
                Process_Form(zform);
            }
            catch
            {
                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.NeedUpdates.Config = true;
                Ipaybox.Working = false;
            }
        }
        public static bool withProviderName = false;

        public static void AddLabel(zeus.HelperClass.zLabel lbl, ref Label label, System.Windows.Forms.Form f)
        {
            string location = lbl.location;
            string text = lbl.text;
            string size = lbl.size;
            string font = lbl.fontFamily;
            string font_size = lbl.fontSize;
            string color = lbl.color;
            string textalign = lbl.textAlign;
            string style = lbl.fontStyle;

            withProviderName = lbl.withProviderName;

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
            }

            label.ForeColor = Color.FromArgb(red, green, blue);

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
                    case "topcenter": label.TextAlign = ContentAlignment.TopCenter; break;
                    default:
                        label.TextAlign = ContentAlignment.TopLeft; break;
                }
            }

            //f.Controls.Add(label);
        }

        public static void AddLabel(XmlElement el, ref Label label, System.Windows.Forms.Form f)
        {
            string location = el.GetAttribute("location");
            string text = el.GetAttribute("text");
            string size = el.GetAttribute("size");
            string font = el.GetAttribute("font");
            string font_size = el.GetAttribute("font-size");
            string color = el.GetAttribute("font-color");
            string textalign = el.GetAttribute("textalign");
            string style = el.GetAttribute("style");

            bool c = false;
            
            bool.TryParse(el.GetAttribute("with-provider-name"), out c);

            withProviderName = c;

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
            }

            label.ForeColor = Color.FromArgb(red, green, blue);

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
                    case "topcenter": label.TextAlign = ContentAlignment.TopCenter; break;
                    default:
                        label.TextAlign = ContentAlignment.TopLeft; break;
                }
            }
            //f.Controls.Add(label);
        }
        private void acceptaccount_Load(object sender, EventArgs e)
        {
            lFlash = new List<AxShockwaveFlashObjects.AxShockwaveFlash>();
            AccauntValidated = false;
            IsAccountValid = false;
            IsServiceAvailable = false;
            this.Size = Ipaybox.Resolution;

            if (Ipaybox.Inches == 17)
                this.Location = new Point(0, 0);

            label1.ForeColor = Color.FromArgb(169, 22, 6);
            //label1.Visible = false;
            
            try
            {
                _cursor.Hide();
            }
            catch { }

            LoadThisForm();
        }
        private void ValidateAccount()
        {
            AccauntValidated = false;
            SendPays sp = new SendPays();

            Random p = new Random();
            int rnd = p.Next(1, 100);
            string str = rnd.ToString();
            if (str.Length < 3)
            {
                for (int i = 0; i < 3 - str.Length; i++)
                {
                    rnd = rnd * 10;
                }
            }

            string account = GetAccountValue();

            MD5 md5hasher = MD5.Create();
            byte[] hash = md5hasher.ComputeHash(Encoding.Default.GetBytes(account));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i]);
            }

            string sHash = sb.ToString();

            while (sHash.Length < 5)
            {
                int r = p.Next(0, 9);
                sHash += r.ToString();
            }

            Ipaybox.curPay.txn_id = DateTime.Now.ToString("yyMMddHHmmss") + Ipaybox.PRV_SELECTED_ID.Trim() + sHash.Substring(0, 5);

            while (Ipaybox.curPay.txn_id.Length > 19)
                Ipaybox.curPay.txn_id = Ipaybox.curPay.txn_id.Substring(0, Ipaybox.curPay.txn_id.Length - 1);

            Ipaybox.curPay.Date = DateTime.Now;
            sp.SendPay("check", Ipaybox.PRV_SELECTED_ID, Ipaybox.Terminal.terminal_id, Ipaybox.Terminal.terminal_pass, GetAccountValue(), Ipaybox.curPay.txn_id, "10", "10", Ipaybox.curPay.Date.ToString("yyyy.MM.dd HH:mm:ss"));

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(sp.XML_Response);

                string result = doc.DocumentElement.SelectSingleNode("result").InnerText;
                string comment = doc.DocumentElement.SelectSingleNode("comment").InnerText;

                int.TryParse(result, out glResult);

                if (result == "0")
                {
                    IsAccountValid = true;
                    IsServiceAvailable = true;
                }
                else
                {
                    IsAccountValid = false;
                    IsServiceAvailable = true;
                }
            }
            catch
            {
                // Не получили ответа или ответ неверный
                IsAccountValid = false;
                IsServiceAvailable = false;
            }
            AccauntValidated = true;
        }

        private void Process_Form(zeus.HelperClass.zForm frm)
        {
            if (IsSelectedProviderOnline())
            {
                ProgressAnimator a = new ProgressAnimator();
                a.Show();
                new System.Threading.Thread(ValidateAccount).Start();

                int zz = 0;

                while (zz < 5000000 || !AccauntValidated)
                {
                    Application.DoEvents();
                    zz++;
                }

                a.Hide();
                a.Dispose();
            }
            else
            {
                IsAccountValid = true;
                IsServiceAvailable = true;
            }

            // XmlElement root = frm;

            try
            {
                this.BackgroundImage = new Bitmap(frm.bgimg);
            }
            catch 
            { 
                this.BackColor = Color.FromArgb(230, 230, 230);
            }

            for (int i = 0; i < frm.images.Count; i++)
            {
                AddImage(frm.images[i], ref img[img_count]);
                img_count++;
            }
            for (int i = 0; i < frm.buttons.Count; i++)
            {
                AddButton(frm.buttons[i]);
            }
            for (int i = 0; i < frm.banners.Count; i++)
            {
                AddBanner(frm.banners[i], this, new EventHandler(this.Pic_Click));
            }
            for (int i = 0; i < frm.labels.Count; i++)
            {
                if (frm.labels[i].name == "account")
                {
                    AddLabel(frm.labels[i], ref label1, this);
                    //labels_count++;
                }
                else
                {
                    Ipaybox.AddLabel(frm.labels[i], ref labels[labels_count], this);
                    labels_count++;
                }
            }

            AddProviderImage(frm.prvImage);

            string accname = "";
            string acctext = "";
            string accvalue = "";
            string[] opt = Ipaybox.curPay.Options.Split('|');

            for (int i = 0; i < opt.Length; i++)
            {
                if (opt[i] != "")
                {
                    string ViewText = string.Empty;

                    if (Ipaybox.curPay.ViewText != null && Ipaybox.curPay.ViewText[i] != null && !string.IsNullOrEmpty(Ipaybox.curPay.ViewText[i].ViewText))
                    {
                        ViewText = Ipaybox.curPay.ViewText[i].ViewText;
                    }

                    string[] param = opt[i].Split('%');
                    accname = param[0].ToUpper();
                    acctext += accname + ": " + (string.IsNullOrEmpty(ViewText) ? param[2] : ViewText) + "\r\n";
                    //AddLabel(param[0] + ": " + param[2], new Point(100, 200 + y), "Times New Roman", "18","255;255;255");
                    if (param[1] == "account")
                        accvalue = param[2];
                }
            }
            //AddLabel(acctext, new Point(100, 200), "Times New Roman", "18", "255;255;255", "");

            label1.Text = acctext;

            if (withProviderName)
            {
                //label1.TextAlign = ContentAlignment.MiddleCenter;
                label1.Text = "ПОЛУЧАТЕЛЬ: " + Ipaybox.PROVIDER_XML.GetAttribute("name") + "\r\n" + label1.Text;
            }

            if (!IsServiceAvailable)
            {
                ShowWarning(188, 500);
                AddLabel("Отсутствует связь.\r\nНевозможно провести платеж на этого провайдера.", new Point(350, 530), "Times New Roman", "20", "169;22;6");
            }
            else
                if (!IsAccountValid)
                {
                    ShowWarning(188, 500);
                    AddLabel(acctext + "\r\nне прошел проверку или сервис провайдера не работает.", new Point(350, 530), "Times New Roman", "20", "169;22;6");
                }
                else
                {
                    // enable
                    img[indexDalee].Visible = true;
                    this.Invalidate();
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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(acceptaccount));
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
            catch (Exception ex)
            {
                Ipaybox.Option["can-flash-initialize"] = "false";
                HelperClass.CrashLog.AddCrash(new Exception("Невозможно инициализировать флэш ролик", ex));
            }
        }
        private void ShowWarning(int x, int y)
        {
            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + "\\osmp17\\warning.gif");
            img[img_count] = new zPictureBox();
            img[img_count].Location = new Point(x, y);
            img[img_count].Image = bmp;
            img[img_count].Size = bmp.Size;
            img[img_count].Visible = true;
            this.Controls.Add(img[img_count]);
            img_count++;
        }
        private void AddProviderImage(zeus.HelperClass.zPrvImage prvImage)
        {
            if (prvImage != null)
            {
                string location = prvImage.location;
                string limg = Ipaybox.GetImageFromInterface_Prv(Ipaybox.PRV_SELECTED_ID);
                string size = prvImage.size;

                int X = int.Parse(location.Split(';')[0]);
                int Y = int.Parse(location.Split(';')[1]);

                Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
                img[img_count] = new zPictureBox();
                img[img_count].Location = new Point(X, Y);
                img[img_count].Image = bmp;

                if (size != "")
                {
                    X = int.Parse(size.Split(';')[0]);
                    Y = int.Parse(size.Split(';')[1]);
                    img[img_count].SizeMode = PictureBoxSizeMode.StretchImage;
                    img[img_count].Size = new Size(X, Y);
                }
                else
                {
                    img[img_count].Size = bmp.Size;
                }

                img[img_count].Visible = true;
                img_count++;
            }
        }

        private void AddProviderImage(XmlElement el)
        {
            string location = el.GetAttribute("location");
            string limg = Ipaybox.GetImageFromInterface_Prv(Ipaybox.PRV_SELECTED_ID);
            string size = el.GetAttribute("size");

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img[img_count] = new zPictureBox();
            img[img_count].Location = new Point(X, Y);
            img[img_count].Image = bmp;

            if (size != "")
            {
                X = int.Parse(size.Split(';')[0]);
                Y = int.Parse(size.Split(';')[1]);
                img[img_count].SizeMode = PictureBoxSizeMode.StretchImage;
                img[img_count].Size = new Size(X, Y);

            }
            else
            {
                img[img_count].Size = bmp.Size;
            }
            img[img_count].Visible = true;
            //this.Controls.Add(img[img_count]);
            img_count++;
        }
        public void AddLabel(string ltext, Point lpoint, string font, string size, string color)
        {
            labels[labels_count] = new Label();
            //labels[label_count].Parent = img[1];
            labels[labels_count].Text = ltext;
            labels[labels_count].BackColor = Color.Transparent;
            labels[labels_count].Location = lpoint;
            labels[labels_count].TextAlign = ContentAlignment.BottomCenter;

            labels[labels_count].AutoSize = true;
            // Цвет
         
            int red = 0;
            int green = 0;
            int blue = 0;

            if (color.Length > 5)
            {
                red = int.Parse(color.Split(';')[0]);
                green = int.Parse(color.Split(';')[1]);
                blue = int.Parse(color.Split(';')[2]);

                labels[labels_count].ForeColor = Color.FromArgb(red, green, blue);
            }

            if (font.Length != 0 && size.Length != 0)
            {
                labels[labels_count].Font = new Font(font, float.Parse(size));
            }

            this.Controls.Add(labels[labels_count]);
            labels_count++;
        }
        private void AddButton(zeus.HelperClass.zButton btn)
        {
            string location = btn.location;
            string limg = btn.img;
            string key = btn.key;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img[img_count] = new zPictureBox();
            img[img_count].Location = new Point(X, Y);
            img[img_count].Image = bmp;
            img[img_count].Size = bmp.Size;
            img[img_count].Tag = btn.value;
            img[img_count].key = key;

            if ((string)img[img_count].Tag == "ok")
            {
                img[img_count].Visible = false;
                indexDalee = img_count;
            }

            if (!string.IsNullOrEmpty(btn.value))
                img[img_count].Click += new System.EventHandler(this.Pic_Click);

            img_count++;
        }
        public static void AddBanner(zeus.HelperClass.zBanner btn, System.Windows.Forms.Form f, EventHandler tar)
        {
            try
            {
                string location = btn.location;
                string limg = btn.src;
                string size = btn.size;

                int X = int.Parse(location.Split(';')[0]);
                int Y = int.Parse(location.Split(';')[1]);
                int sizeX = int.Parse(size.Split(';')[0]);
                int sizeY = int.Parse(size.Split(';')[1]);

                XmlElement root = (XmlElement)Ipaybox.config.DocumentElement.SelectSingleNode("banners");

                Image _gif = null;

                try
                {
                    if (root == null)
                        _gif = Image.FromFile(limg);
                    else
                    {
                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            XmlElement row = (XmlElement)root.ChildNodes[i];

                            if (row.Name == "banner")
                            {
                                string form = row.GetAttribute("form").ToLower();

                                if (form == "acceptaccount" || form == "all")
                                {
                                    string src = row.GetAttribute("src");
                                    _gif = Image.FromFile(src);
                                    break;
                                }
                            }
                        }

                        if (_gif == null)
                        {
                            _gif = Image.FromFile(limg);
                        }
                    }
                }
                catch
                {
                    _gif = Image.FromFile(limg);
                }

                PictureBox pb = new PictureBox();
                pb.Size = new Size(sizeX, sizeY);
                pb.Location = new Point(X, Y);
                pb.BackColor = Color.Transparent;
                pb.Image = _gif;
                f.Controls.Add(pb);
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось загрузить баннер: " + ex.Message);
            }
        }
        private string GetAccountValue()
        {
            string[] opt = Ipaybox.curPay.Options.Split('|');
            
            for (int i = 0; i < opt.Length; i++)
            {
                if (opt[i] != "")
                {
                    string[] param = opt[i].Split('%');
                    if (param[1].ToLower() == "account")
                        return param[2];
                }
            }

            return "";
        }
        private void Pic_Click(object sender, System.EventArgs e)
        {
            //System.GC.Collect();
            Sound.Play(Ipaybox.StartupPath.TrimEnd('\\') + "\\sounds\\" + "click1.wav");
            zPictureBox pb = (zPictureBox)sender;
            //MessageBox.Show("CLICK PRV=" + pb.Tag);
            string tag = pb.Tag.ToString().ToLower();

            // Нажата не кнопка на клаве
            switch (tag)
            {
                case "ok":
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Номер подтвержден");
                        Ipaybox.curPay.IsAccountConfirmed = true;
                        Ipaybox.StartForm.Main_Process();
                        this.Dispose();
                    break;
                case "vmenu":
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Возврат в главное меню.");
                    ExitForm();
                    break;
                case "goprev":
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Возврат в предыдущее меню.");
                    Ipaybox.cFormIndex = Ipaybox.cFormIndex - 1;
                    Ipaybox.StartForm.Main_Process();
                    this.Dispose();
                    //ExitForm();
                    break;
            }
        }
        public bool IsSelectedProviderOnline()
        {
            string root = Ipaybox.PROVIDER_XML.GetAttribute("type");

            if (root.ToLower() == "online")
                return true;

            return false;
        }
        private void ExitForm()
        {
            Ipaybox.FlushToMain();
            Ipaybox.StartForm.Main_Process();

            for (int i = 0; i < this.Controls.Count; i++)
            {
                this.Controls[i].Dispose();
            }

            this.Dispose();
        }
        private void Reset_Timer()
        {
            flush_timer.Stop();
            flush_timer.Start();
        }

        private void flush_timer_Tick_1(object sender, EventArgs e)
        {
            ExitForm();
        }

        private void acceptaccount_Activated(object sender, EventArgs e)
        {
        }      
    }
}
