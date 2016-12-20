using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;

namespace zeus
{
    public partial class edit : Form
    {
        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        private static extern bool HideCaret(IntPtr hWnd);

        private Bitmap _backBuffer;
        private PictureBox[] img = new PictureBox[100];
        private List<PictureBox> popupImg = new List<PictureBox>();
        private int img_count;
        private Label[] labels = new Label[20];
        private int label_count;
        private Label account;
        private Label accountInputField;
        private static string newkeyboard;
        private string acc_mask;
        private string entered_text;
        private string name;
        private string xmlname;
        private int indexAccountImage;
        private static int oldCountChars;
        private int min;
        private int max;
        public static List<AxShockwaveFlashObjects.AxShockwaveFlash> lFlash;
        public string ViewAccount = string.Empty;
        zeus.HelperClass.zForm default_form__;

        public edit()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(edit_KeyDown);
        }
        /*protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                //parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                parms.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return parms;
            }
        }*/
        void edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F4)
            {
                //Ipaybox.AddToLog(Ipaybox.Logs.Main, "");
                Ipaybox.StartForm.Dispose();
            }
        }
        void edit_Click(object sender, System.EventArgs e)
        {
            if(popupImg.Count > 0)
                if (popupImg[0].Visible)
                {
                    popupImg[0].Visible = false;
                    this.Controls.RemoveByKey("popup");
                    return;
                }

            MouseEventArgs p = (MouseEventArgs)e;

            for (int i = 0; i < img_count; i++)
            {
                if(img[i] != null && img[i].Tag != null && !string.IsNullOrEmpty(img[i].Tag.ToString()) && img[i].Tag.ToString().ToLower() != "skip")
                if (
                    (  p.X >= img[i].Left 
                    && p.X <= img[i].Left + img[i].Width
                    && p.Y >= img[i].Top 
                    && p.Y <= img[i].Top + img[i].Height) && img[i].Visible)
                {
                    Pic_Click(img[i], new EventArgs());
                    break;
                }
            }
        }
        protected void DrawScreen()
        {
            if (_backBuffer != null)
                _backBuffer.Dispose();

            _backBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            Graphics g = Graphics.FromImage(_backBuffer);

            if (this.BackgroundImage != null)
            {
                g.DrawImageUnscaled(this.BackgroundImage, 0, 0);
            }
            //Paint your graphics on g here
            // Нужно прорисовать все картинки тут.
            for (int i = 0; i < img_count; i++)
            {
                try
                {
                    PictureBox pb = (PictureBox)this.img[i];
                    Rectangle dest = new Rectangle(pb.Left, pb.Top, pb.Width, pb.Height);
                    string p = pb.Tag.ToString();

                    if (pb.Visible || (pb.Tag.ToString() != "tomenu" && Ipaybox.PRV_SELECTED_GROUP.id == ""))
                        g.DrawImage(pb.Image, dest);
                }
                catch
                { }
            }

            for (int i = 0; i < label_count; i++ )
            {
                try
                {
                    labels[i].Visible = false;
                    g.FillRectangle(new SolidBrush(labels[i].BackColor), labels[i].Location.X, labels[i].Location.Y, labels[i].Location.X + labels[i].Width, labels[i].Location.Y + labels[i].Height);
                    g.DrawString(labels[i].Text, labels[i].Font, new SolidBrush(labels[i].ForeColor), labels[i].Location.X, labels[i].Location.Y);
                }
                catch { }
            }

            try
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                RectangleF rf = new RectangleF(accountInputField.Location.X, accountInputField.Location.Y, accountInputField.Width, accountInputField.Height);
                g.FillRectangle(new SolidBrush(accountInputField.BackColor), accountInputField.Location.X, accountInputField.Location.Y, accountInputField.Width, accountInputField.Height);
                g.DrawString(accountInputField.Text, accountInputField.Font, new SolidBrush(accountInputField.ForeColor), rf, sf);
            }
            catch { }

            g.Dispose();
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (_backBuffer != null)
            {
                try
                {
                    pevent.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
                }
                catch { }
            }
            else
                DrawScreen();
        }
        private void LoadThisForm()
        {
            /* Загрузка формы */
            try
            {
                zeus.HelperClass.zProvider provider = Ipaybox.ifc.customSets.Find(p => p.id.ToString() == Ipaybox.PRV_SELECTED_ID);
                zeus.HelperClass.zGroup group = Ipaybox.ifc.groups.Find(p => p.id.ToString() == Ipaybox.PRV_SELECTED_GROUP.id);

                //Если не найдено собственного набора форм для выбранного провайдера
                //и найден набор форм по умолчанию для группы, которой принадлежит провайдер
                //начинаем обработку набора форм по-умолчанию для группы
                if (provider == null && group != null)
                {
                    zeus.HelperClass.zForm zform = Ipaybox.ifc.forms.Find(p => p.name == group.forms[Ipaybox.cFormIndex - 1].name);
                    default_form__ = zform;
                    Process_Form(zform);
                }
                else
                {
                    if (provider != null)
                    {
                        zeus.HelperClass.zForm zform = Ipaybox.ifc.forms.Find(p => p.name == provider.forms[Ipaybox.cFormIndex - 1].name);
                        default_form__ = zform;
                        Process_Form(zform);
                    }
                }
            }
            catch
            {
                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.NeedUpdates.Config = true;
                Ipaybox.Working = false;
            }
        }
        private void edit_Load(object sender, EventArgs e)
        {
            //lFlash = new List<AxShockwaveFlashObjects.AxShockwaveFlash>();
            newkeyboard = "";
            oldCountChars = -1;
            account = new Label();
            this.Size = Ipaybox.Resolution;
            if (Ipaybox.Inches == 17)
                this.Location = new Point(0, 0);
            //XmlElement formmm = Ipaybox.FORM_XML;
            // Грузим интерфейс
            LoadThisForm();
            entered_text = "";
            //Ipaybox.FORM_XML = formmm;
            if (Ipaybox.FORM_XML != null)
            {
                Process_Provider(Ipaybox.FORM_XML);
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Вход в раздел ввода номера.");
            }
            else
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось загрузить интерфейс.");
            }
            DrawScreen();
        }
        private void Process_Provider(XmlElement frm)
        {
            for (int i = 0; i < frm.ChildNodes.Count; i++)
            {
                XmlElement el = (XmlElement)frm.ChildNodes[i];
                if (el.Name == "param")
                {
                    if (el.GetAttribute("name") != "static")
                        AddParamPrv(el);
                }
            }
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
                ExitForm();
            }

            //Добавляем элементы на форму
            //Надписи, кнопки, изображения, флэшки и т.п.
            for (int i = 0; i < frm.images.Count; i++)
            {
                if (frm.images[i].type == "popup")
                {
                    popupImg.Add(AddPopup(frm.images[i], this));
                    popupImg[popupImg.Count - 1].Click += new System.EventHandler(edit_Click);
                }
                else
                {
                    AddImage(frm.images[i], ref img[img_count], this);
                    img_count++;
                }
            }

            for (int i = 0; i < frm.buttons.Count; i++)
            {
                AddButton(frm.buttons[i], ref img[img_count], this, new EventHandler(this.Pic_Click));

                if (frm.buttons[i].value == "ok")
                    indexAccountImage = img_count;

                img_count++;
            }

            for (int i = 0; i < frm.banners.Count; i++)
            {
                AddBanner(frm.banners[i], this, new EventHandler(this.Pic_Click));
            }

            for (int i = 0; i < frm.labels.Count; i++)
            {
                AddLabel(frm.labels[i]);

                if (frm.labels[i].withEditName)
                    withEditName = label_count - 1;
            }

            for (int i = 0; i < frm.inputParams.Count; i++)
                AddParam(frm.inputParams[i]);

            //Добавление картинки провайдера
            AddProviderImage(frm.prvImage);
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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit));
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
                HelperClass.CrashLog.AddCrash(new Exception("Form:Edit - Невозможно инициализировать флэш ролик", ex));
            }
        }

        private void AccountChange()
        {
            // нажали на кнопочку, ресетим таймер сброса
            Reset_Timer();

            //Если выбран оператор из группы "Сотовая связь", "Мегафон", "Скайлинк" или "Смартс", 
            //то производится удаление лидирующей восьмерки
            if (Ipaybox.PRV_SELECTED_GROUP.id.Trim() == "1" || Ipaybox.PRV_SELECTED_GROUP.id.Trim() == "5" ||
                Ipaybox.PRV_SELECTED_GROUP.id.Trim() == "10" || Ipaybox.PRV_SELECTED_GROUP.id.Trim() == "11")
                if (entered_text.Trim() == "8")
                    entered_text = entered_text.Replace("8", string.Empty);

            // Показываем либо скрываем кнопку в зависимости от того сколько символов указано в аккаунте.
            if (entered_text.Length >= min)
            {
                img[indexAccountImage].Visible = true;
            }
            else
            {
                img[indexAccountImage].Visible = false;
            }

            string account1 = "";
            bool sys = false;
            //string what = "";
            int size = 0;
            bool collectsize = false;
            string siz = "";
            int entered = 0;

            for(int i = 0;i < acc_mask.Length; i++)
            {
                if(acc_mask[i] == "\\".ToCharArray()[0] )
                {
                    sys = true;
                }
                if (acc_mask[i] == "]".ToCharArray()[0] || acc_mask[i] == "+".ToCharArray()[0])
                {
                    sys = false;
                    collectsize = false;
                }

                if (!collectsize && siz != "")
                {
                    size += int.Parse(siz);
                    int lsize  = int.Parse(siz);
                    siz = "";
                    for (int j = 0; j < lsize; j++)
                    {
                        if (entered_text.Length > entered)
                        {
                            account1  += entered_text[entered];
                            entered++;
                           
                        }
                        else
                        {
                            siz = "";
                           
                            break;
                        }
                    }
                    siz = "";
                    
                }
                if (!sys)
                {
                    if (acc_mask[i] != ']') 
                        account1 += acc_mask[i];
                }
                else
                {
                    if (collectsize)
                    {
                        siz += acc_mask[i];
                    }
                    if (acc_mask[i] == 'd')
                    {
                        // след текст должен быть цифра
                        //what = "d";
                    }

                    if (acc_mask[i] == '[')
                    {
                        collectsize = true;
                    }
                }
                if (entered_text.Length < size)
                    break;
            }

            account.Text = account1;
            ViewAccount = account1.Trim();

            if (accControl != null)
            {
                if (accControl.GetType().Name == "MaskedTextBox")
                {
                    if (((MaskedTextBox)accControl).Mask.Length > 0 && ((MaskedTextBox)accControl).Mask[0] == '8')
                    {
                        ((MaskedTextBox)accControl).Text = "8" + account1.Replace(" ", "");
                    }
                    else
                        ((MaskedTextBox)accControl).Text = account1.Replace(" ", "");

                    ((MaskedTextBox)accControl).Invalidate();
                }
                else
                {
                    ((TextBox)accControl).Clear();
                    ((TextBox)accControl).AppendText(account1);
                    ((TextBox)accControl).Invalidate();
                }

                accountInputField.Text = accControl.Text;

                Size textSize = TextRenderer.MeasureText(accountInputField.Text, accountInputField.Font);

                while (textSize.Width > accountInputField.Width - 50 && accountInputField.Font.Size > 14)
                {
                    Font accFont = new Font(accountInputField.Font.FontFamily, accountInputField.Font.Size - 1, accountInputField.Font.Style);
                    accountInputField.Font = accFont;
                    textSize = TextRenderer.MeasureText(accountInputField.Text, accountInputField.Font);
                }
                while (textSize.Width <= accountInputField.Width - 50 && accountInputField.Font.Size <= 42)
                {
                    Font accFont = new Font(accountInputField.Font.FontFamily, accountInputField.Font.Size + 1, accountInputField.Font.Style);
                    accountInputField.Font = accFont;
                    textSize = TextRenderer.MeasureText(accountInputField.Text, accountInputField.Font);
                }
            }

            //accountInputField.Invalidate();
            this.Invalidate();
        }

        private void ShowKeybord(zeus.HelperClass.zKeyboard keyboard)
        {
            string location = keyboard.location;
            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);
            Point n = new Point(X, Y);

            string limg = keyboard.img;

            if (!string.IsNullOrEmpty(limg))
            {
                // Загрузка картинки клавиатуры
                Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
                img[img_count] = new PictureBox();
                img[img_count].Location = new Point(X, Y);
                img[img_count].Image = bmp;
                img[img_count].Size = bmp.Size;
                img[img_count].Tag = "skip";
                img[img_count].Click += new System.EventHandler(this.Pic_Click);
                img_count++;
            }

            for (int i = 0; i < keyboard.keys.Count; i++)
                AddButtonKey(n, keyboard.keys[i]);
        }
        private void ShowKeybords(string keyboard)
        {
            zeus.HelperClass.zKeyboard kbd = Ipaybox.ifc.keyboards.Find(p => p.name == keyboard);

            if (kbd != null)
            {
                ShowKeybord(kbd);
            }
                /*XmlElement root = Ipaybox.XML_Interface.SelectSingleNode("keyboards") as XmlElement;

                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    XmlElement row = (XmlElement)root.ChildNodes[i];

                    if (row.Name == "keyboard" && row.GetAttribute("name") == keyboard)
                    {
                        ShowKeybord(row);
                        break;
                    }
                }*/
        }
        private void AddParamPrv(XmlElement el)
        {
            // хаваем с провайдера            
            name = el.GetAttribute("name");
            xmlname = el.GetAttribute("xml-name");

            if (withEditName > -1)
                labels[withEditName].Text = labels[withEditName].Text + " " + name ;

            if (Ipaybox.curPay.Options != null)
            {
                string[] lines = Ipaybox.curPay.Options.Split('|');
                Ipaybox.curPay.Options = "";

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] != "")
                    {
                        if (lines[i].IndexOf(xmlname) == -1)
                        {
                            // это не такой 
                            Ipaybox.curPay.Options += lines[i] + "|";
                        }
                        else
                        {
                            entered_text = lines[i].Split('%')[2];
                        }
                    }
                }
            }

            string min1 = el.GetAttribute("min");
            string max1 = el.GetAttribute("max");
            min = int.Parse(min1);
            max = int.Parse(max1);
            string keyboard = el.GetAttribute("keyboard");
            if(string.IsNullOrEmpty(newkeyboard))
                ShowKeybords(keyboard);
            else
                ShowKeybords(newkeyboard);
            string mask = el.GetAttribute("mask");
            acc_mask = mask;
            account.Tag = xmlname;
            this.Controls.Add(account);
            AccountChange();
        }
        
        int withEditName=-1;

        private void CheckPhoneRange()
        {
            try
            {
                if (Ipaybox.PhoneRanges != null)
                {
                    ulong acc = 0;

                    if (ulong.TryParse(entered_text, out acc))
                    {
                        for (int j = 0; j < Ipaybox.PhoneRanges.Count; j++)
                        {
                            if (Ipaybox.PhoneRanges[j].from <= acc && Ipaybox.PhoneRanges[j].to >= acc)
                                if (Ipaybox.PhoneRanges[j].prv != Ipaybox.PRV_SELECTED_ID)
                                {
                                    Ipaybox.PRV_SELECTED_ID = Ipaybox.PhoneRanges[j].prv;

                                    foreach (XmlElement n in Ipaybox.providers.DocumentElement.SelectSingleNode("providers").ChildNodes)
                                    {
                                        if (n.Name.ToLower().Trim() == "provider" && n.GetAttribute("id") == Ipaybox.PRV_SELECTED_ID)
                                        {
                                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Провайдер изменен автоматически");
                                            Ipaybox.PROVIDER_XML = n;
                                            break;
                                        }
                                    }

                                    for (int i = 0; i < img_count; i++)
                                    {
                                        if (img[i].Tag.ToString() == "PRVIMG")
                                        {
                                            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + Ipaybox.GetImageFromInterface_Prv(Ipaybox.PRV_SELECTED_ID));
                                            img[i].Image = bmp;
                                            break;
                                        }
                                    }

                                    break;
                                }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка при проверке номера на корректность. " + ex.Message);
            }
        }
        Control accControl;

        void edit_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void edit_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            throw new NotImplementedException();
        }

        void ctl_GotFocus(object sender, EventArgs e)
        {
            HideCaret(((Control)sender).Handle);
        }

        private void AddButtonKey(Point p, zeus.HelperClass.zKeybButton key)
        {
            // LOCATION
            string location = key.location;
            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            string limg = key.img;

            // SIZE
            string size = key.size;
            int width = int.Parse(size.Split(';')[0]);
            int height = int.Parse(size.Split(';')[1]);

            Bitmap bmp = new Bitmap(width, height);
            img[img_count] = new PictureBox();

            if (!string.IsNullOrEmpty(limg))
                bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);

            img[img_count].Image = bmp;
            img[img_count].Size = bmp.Size;
            img[img_count].Location = new Point(p.X + X, p.Y + Y);
            img[img_count].Tag = "key:" + key.value;
            img[img_count].Click += new System.EventHandler(this.Pic_Click);

            img_count++;
        }

        /// <summary>
        /// Добавление всплывающего изображения на форму. ver 2
        /// </summary>
        private PictureBox AddPopup(zeus.HelperClass.zImage zimage, System.Windows.Forms.Form f)
        {
            string location = zimage.location;
            string limg = zimage.src;
            string size = zimage.size;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            PictureBox img = new PictureBox();
            img.Visible = zimage.visible;
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

            return img;
        }
        /// <summary>
        /// Добавление изображения на форму. ver 2
        /// </summary>
        private void AddImage(zeus.HelperClass.zImage zimage, ref PictureBox img, System.Windows.Forms.Form f)
        {
            try
            {
                string location = zimage.location;
                string limg = zimage.src;
                string size = zimage.size;

                int X = int.Parse(location.Split(';')[0]);
                int Y = int.Parse(location.Split(';')[1]);

                Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
                img = new PictureBox();
                img.Visible = zimage.visible;
                img.Location = new Point(X, Y);
                img.Image = bmp;
                img.Tag = "";

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
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось загрузить изображение " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        /// <summary>
        /// Добавление кнопки на форму. ver 2
        /// </summary>
        public void AddButton(zeus.HelperClass.zButton zbutton, ref PictureBox img, System.Windows.Forms.Form f, EventHandler tar)
        {
            string location = zbutton.location;
            string limg = zbutton.img;
            string tag = zbutton.value;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap tmp = Ipaybox.Pics[int.Parse(Ipaybox.images[limg])];
            img = new PictureBox();
            img.Location = new Point(X, Y);
            img.Image = tmp;
            img.Size = tmp.Size;
            img.Tag = tag;
            // НЕРАБОТАЕТ
            img.Click += tar;
            //f.Controls.Add(img);
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

                                if (form == "edit" || form == "all")
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
                pb.Image = _gif;
                pb.Size = new Size(sizeX, sizeY);
                pb.Location = new Point(X, Y);
                pb.BackColor = Color.Transparent;
                f.Controls.Add(pb);
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось загрузить баннер: " + ex.Message);
            }
        }
        /// <summary>
        /// Добавление надписи на форму. ver 2
        /// </summary>
        private void AddLabel(zeus.HelperClass.zLabel zlabel)
        {
            string location = zlabel.location;
            string text = (string.IsNullOrEmpty(zlabel.text) == true ? zlabel.text : zlabel.text).TrimStart().TrimEnd();
            string size = zlabel.size;
            string font = zlabel.fontFamily;
            string font_size = zlabel.fontSize;
            string textalign = zlabel.textAlign;
            string texttransform = zlabel.textTransform;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            labels[label_count] = new Label();
            labels[label_count].BackColor = Color.Transparent;
            labels[label_count].Location = new Point(X, Y);

            if (size.Split(';').Length >= 2)
                labels[label_count].Size = new Size(int.Parse(size.Split(';')[0]), int.Parse(size.Split(';')[1]));
            else
                labels[label_count].AutoSize = true;

            labels[label_count].Text = text;

            if (zlabel.color.Split(';').Length >= 3)
                labels[label_count].ForeColor = HelperClass.Helper.GetColor(zlabel.color);

            if (font.Length != 0 && font_size.Length != 0)
            {
                labels[label_count].Font = new Font(font, float.Parse(font_size));
            }

            if (textalign.Length != 0)
            {
                switch (textalign.ToLower())
                {
                    case "middleright": labels[label_count].TextAlign = ContentAlignment.MiddleRight; break;
                    case "middleleft": labels[label_count].TextAlign = ContentAlignment.MiddleLeft; break;
                    case "middlecenter": labels[label_count].TextAlign = ContentAlignment.MiddleCenter; break;
                    case "topleft": labels[label_count].TextAlign = ContentAlignment.TopLeft; break;
                    case "topright": labels[label_count].TextAlign = ContentAlignment.TopRight; break;
                    default:
                        labels[label_count].TextAlign = ContentAlignment.TopLeft; break;
                }
            }

            if (texttransform.Length != 0)
            {
                switch (textalign.ToLower())
                {
                    case "uppercase": labels[label_count].Text = labels[label_count].Text.ToUpper(); break;
                    case "lowercase": labels[label_count].Text = labels[label_count].Text.ToLower(); break;
                    default:
                        labels[label_count].TextAlign = ContentAlignment.TopLeft; break;
                }
            }

            if (zlabel.withProviderName == true)
                labels[label_count].Text += Ipaybox.PROVIDER_XML.GetAttribute("name");

            this.Controls.Add(labels[label_count]);
            label_count++;
        }
        /// <summary>
        /// Добавление параметров формы. ver 2
        /// </summary>
        private void AddParam(zeus.HelperClass.zInput zinput)
        {
            Control ctl;

            if (accountInputField != null)
                return;

            //То, что нужно показывать (поле ввода)
            accountInputField = new Label();
            accountInputField.Visible = true;
            accountInputField.AutoSize = false;

            if (!string.IsNullOrEmpty(zinput.type) && zinput.type.ToLower() == "masktextbox")
            {
                string mask = zinput.mask;
                ctl = new MaskedTextBox();
                ((MaskedTextBox)ctl).Mask = mask;
                ((MaskedTextBox)ctl).PromptChar = '*';

                ctl.GotFocus += new EventHandler(ctl_GotFocus);
                ctl.Visible = false;
                this.Controls.Add(ctl);

                account.Visible = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(zinput.type) && zinput.type.ToLower() == "simpletextbox")
                {
                    ctl = new TextBox();
                    ctl.GotFocus += new EventHandler(ctl_GotFocus);
                    ctl.Visible = false;
                    this.Controls.Add(ctl);
                    account.Visible = false;
                }
                else
                    ctl = account;
            }

            string font = zinput.fontFamily;
            string font_size = zinput.fontSize;
            string color = zinput.color;
            string font_style = zinput.fontStyle;
            string textalign = zinput.textAlign;

            // Цвет
            int red = 0;
            int green = 0;
            int blue = 0;

            if (color.Length >= 5)
            {
                red = int.Parse(color.Split(';')[0]);
                green = int.Parse(color.Split(';')[1]);
                blue = int.Parse(color.Split(';')[2]);

                accountInputField.ForeColor = Color.FromArgb(red, green, blue);
            }
            string location = zinput.location;
            string size = zinput.size;

            if (font.Length != 0 && font_size.Length != 0)
            {
                FontStyle fs = FontStyle.Regular;
                switch (font_style.ToLower())
                {
                    case "bold": fs = FontStyle.Bold; break;
                    case "italic": fs = FontStyle.Italic; break;
                }

                accountInputField.Font = new Font(font, float.Parse(font_size), fs);
            }

            if (!string.IsNullOrEmpty(textalign))
            {
                switch (textalign.ToLower())
                {
                    case "center": accountInputField.TextAlign = ContentAlignment.MiddleCenter; break;
                    case "right": accountInputField.TextAlign = ContentAlignment.MiddleRight; break;
                    case "left": accountInputField.TextAlign = ContentAlignment.MiddleLeft; break;
                    default: accountInputField.TextAlign = ContentAlignment.MiddleCenter; break;
                }
                
                /*if (ctl is MaskedTextBox)
                {
                    switch (textalign.ToLower())
                    {
                        case "center": ((MaskedTextBox)ctl).TextAlign = HorizontalAlignment.Center; break;
                        case "right": ((MaskedTextBox)ctl).TextAlign = HorizontalAlignment.Right; break;
                        case "left": ((MaskedTextBox)ctl).TextAlign = HorizontalAlignment.Left; break;
                        default: ((MaskedTextBox)ctl).TextAlign = HorizontalAlignment.Center; break;
                    }
                }*/
            }

            //account.Text = "8(   )-   -    ";
            try
            {
                accountInputField.BackColor = Color.White;
            }
            catch
            {
            }

            accountInputField.Tag = xmlname;
            accountInputField.Location = new Point(int.Parse(location.Split(';')[0]), int.Parse(location.Split(';')[1]));
            accountInputField.Visible = zinput.visible;
            accountInputField.Visible = false;

            if (size.Length != 0)
            {
                accountInputField.MinimumSize = new Size(int.Parse(size.Split(';')[0]), int.Parse(size.Split(';')[1]));
                accountInputField.MaximumSize = new Size(int.Parse(size.Split(';')[0]), int.Parse(size.Split(';')[1]));
            }
            else
                accountInputField.AutoSize = true;
            
            this.Controls.Add(accountInputField);

            if (!string.IsNullOrEmpty(zinput.type) && (zinput.type.ToLower() == "masktextbox" || zinput.type.ToLower() == "simpletextbox"))
            {
                accControl = ctl;
            }
        }
        /// <summary>
        /// Добавление изображения провайдера на форму. ver 2
        /// </summary>
        private void AddProviderImage(zeus.HelperClass.zPrvImage prvimg)
        {
            string location = prvimg.location;
            string limg = Ipaybox.GetImageFromInterface_Prv(Ipaybox.PRV_SELECTED_ID);
            string size = prvimg.size;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img[img_count] = new PictureBox();
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
            img[img_count].Tag = "PRVIMG";
            //this.Controls.Add(img[img_count]);
            img_count++;
        }

        private void AddOptions(string str)
        {
            string[] param = str.Split('%');

            if (Ipaybox.curPay.Options != null)
            {
                string[] j = Ipaybox.curPay.Options.Split('|');
                Ipaybox.curPay.Options = "";
                for (int i = 0; i < j.Length; i++)
                {
                    if (j[i] != "")
                    {
                        if (j[i].IndexOf(param[2]) != -1)
                        {
                            Ipaybox.curPay.Options += j[i] + "|";
                        }
                        else 
                            Ipaybox.curPay.Options += str;
                    }   
                }
            }
            else
                Ipaybox.curPay.Options += str;
            //Ipaybox.curPay.Options += name + "-" + xmlname + "-" + entered_text + "|";
                        
        }
        private void ChangeKeyBoard(string keyboard)
        {
            this.Controls.Remove(accControl);
            //this.Controls.Remove(accountInputField);
            newkeyboard = keyboard;
            img_count = 0;
            label_count = 0;
            Process_Form(default_form__);
            Process_Provider(Ipaybox.FORM_XML);
            DrawScreen();
            this.Invalidate();
        }
        private void clickOk()
        {
            try
            {
                if (Ipaybox.PROVIDER_XML.GetAttribute("ignore_prange").ToLower() == "false" || Ipaybox.PROVIDER_XML.GetAttribute("ignore_prange") == "")
                    CheckPhoneRange();
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Невозможно определить необходимость проверки номера: " + ex.Message);
            }

            Ipaybox.curPay.prv_id = Ipaybox.PRV_SELECTED_ID;

            zeus.HelperClass.AccountInfo info = new zeus.HelperClass.AccountInfo();

            if (Ipaybox.curPay.ViewText == null)
                Ipaybox.curPay.ViewText = new List<zeus.HelperClass.AccountInfo>();

            if (accControl is MaskedTextBox)
            {
                info.ViewText = ((MaskedTextBox)accControl).Text;
            }
            else
            {
                info.ViewText = Regex.Replace(ViewAccount, @"\s{2,}", " ");
            }

            Ipaybox.curPay.Options += name + "%" + xmlname + "%" + entered_text + "|";
            info.Name = name;
            info.XmlName = xmlname;
            info.Value = entered_text;

            try
            {
                if (Ipaybox.curPay.ViewText.Count > (Ipaybox.cFormIndex - 1))
                    Ipaybox.curPay.ViewText.Remove(Ipaybox.curPay.ViewText[Ipaybox.cFormIndex - 1]);
            }
            catch 
            {
                Ipaybox.Working = false;
                Ipaybox.NeedToRestart = true;
            }

            Ipaybox.curPay.ViewText.Add(info);
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Введен номер " + entered_text);
            Ipaybox.StartForm.Main_Process();
            this.Dispose();
        }

        private void Pic_Click(object sender, System.EventArgs e)
        {
            Reset_Timer();
            Sound.Play(Ipaybox.StartupPath.TrimEnd('\\') + "\\sounds\\" + "click1.wav");
            PictureBox pb = (PictureBox)sender;
            //MessageBox.Show("CLICK PRV=" + pb.Tag);
            string tag = pb.Tag.ToString();

            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Нажата кнопка " + tag);

            if (tag.IndexOf("key") == -1)
            {
                // Нажата не кнопка на клаве
                switch (tag)
                {
                    case "ok":
                        if (entered_text == Ipaybox.Terminal.secret_number || Ipaybox.getMd5Hash(entered_text) == Ipaybox.Terminal.secret_number)
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Вход в сервисное меню.");
                            Form n = new login();
                            n.Show();
                            Ipaybox.ServiceMenu = true;
                            ExitForm();
                        }
                        else
                        {
                            clickOk();
                        } 
                        break;
                    case "help":
                        if (popupImg.Count > 0)
                        {
                            popupImg[0].Visible = !popupImg[0].Visible;
                            popupImg[0].BackColor = Color.Transparent;
                            Ipaybox.AddPopup(popupImg[0], this);
                            this.Invalidate();
                        }
                        
                        break;
                    case "cancel":

                        //Ipaybox.PRV_SELECTED_GROUP = "";
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Возврат по кнопке Назад");
                        Ipaybox.PRV_SELECTED_ID = "";
                        ExitToGroup();
                        //ExitForm();
                        break;
                    case "goprev":
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Возврат по кнопке Назад");
                        Ipaybox.cFormIndex = Ipaybox.cFormIndex - 2;

                        if (Ipaybox.cFormIndex < 0)
                        {
                            Ipaybox.PRV_SELECTED_GROUP.id = "";
                            Ipaybox.PRV_SELECTED_ID = "";
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Возврат по кнопке В МЕНЮ");
                            ExitForm();
                            break;
                        }
                        else
                        {
                            if (Ipaybox.curPay.ViewText != null)
                                if (Ipaybox.curPay.ViewText.Count > 0)
                                    Ipaybox.curPay.ViewText.RemoveAt(Ipaybox.curPay.ViewText.Count - 1);

                            Ipaybox.StartForm.Main_Process();
                            this.Dispose();
                            //ExitForm();
                            break;
                        }
                    case "vmenu":
                        Ipaybox.PRV_SELECTED_GROUP.id = "";
                        Ipaybox.PRV_SELECTED_ID = "";
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Возврат по кнопке В МЕНЮ");
                        ExitForm();
                        break;
                }
            }
            else
            {
                string[] s = tag.Split(':');
                if (s[1] == "space")
                {
                    //Пробел, блеать
                    entered_text += " ";
                }
                else
                {
                    if (s[1] == "clear")
                    {
                        // нажата кнопка очистить
                        if (entered_text.Length > 0)
                        {
                            entered_text = entered_text.Remove(entered_text.Length - 1);                        
                        }
                    }
                    else 
                        if (s[1] == "clearall")
                        {
                           entered_text = "";
                           oldCountChars = -1;
                           this.Invalidate();
                        }
                        else
                            if (s.Length == 3)
                            {
                                if (s[1] == "keyboard")
                                {
                                    ChangeKeyBoard(s[2]);
                                }
                                else
                                    if (s[1] == "\\")
                                    {
                                        if (entered_text.Length < max)
                                            entered_text += ":";
                                    }
                                    else
                                    {
                                        if (s[2].ToLower() == "ok")
                                        {
                                            if ((oldCountChars < min && entered_text.Length == min) || (oldCountChars == min && entered_text.Length < min))
                                            {
                                                this.Invalidate();
                                            }

                                            entered_text += s[1];
                                            AccountChange();
                                            oldCountChars = entered_text.Length;
                                            clickOk();
                                            return;
                                        }
                                    }
                            }
                            else
                            {
                                // Если нажата кнопка со значением && максимальное кол-во символов не достигнуто
                                if (entered_text.Length < max)
                                    entered_text += s[1];
                            }
                }

                if((oldCountChars < min && entered_text.Length == min ) || (oldCountChars == min && entered_text.Length < min ))
                {
                    this.Invalidate();
                }
                AccountChange();
                oldCountChars = entered_text.Length;
            }
            DrawScreen();
        }
        private void ExitToGroup()
        {
            Ipaybox.PRV_SELECTED_ID = null;
            //PRV_SELECTED_GROUP = "";
            Ipaybox.PROVIDER_XML = null;
            Ipaybox.FORM_XML = null;
            Ipaybox.CountForms = 0;
            Ipaybox.cFormIndex = 0;

            Ipaybox.curPay.IsAccountConfirmed = false;
            Ipaybox.curPay.account = "";
            Ipaybox.curPay.from_amount = 0;
            Ipaybox.curPay.to_amount = 0;
            Ipaybox.curPay.extra = "";
            Ipaybox.curPay.IsMoney = false;
            Ipaybox.curPay.IsRecieptPrinted = false;
            Ipaybox.curPay.Options = "";
            Ipaybox.curPay.prv_id = "";
            Ipaybox.curPay.prv_img = "";
            Ipaybox.curPay.prv_name = "";
            Ipaybox.curPay.txn_id = "";
            Ipaybox.curPay.ViewText = null;
            Ipaybox.StartForm.Main_Process();

            for (int i = 0; i < this.Controls.Count; i++)
            {
                this.Controls[i].Dispose();
            }

            this.Dispose();
        }
        private void ExitForm()
        {
            try
            {
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    this.Controls[i].Dispose();
                }
                _backBuffer.Dispose();
                GC.Collect();
            }
            catch { }

            Ipaybox.FlushToMain();
            Ipaybox.StartForm.Main_Process();
            GC.Collect();
            this.Dispose();
        }
        private void Reset_Timer()
        {
            flush_timer.Stop(); 
            flush_timer.Start();
        }

        private void flush_timer_Tick(object sender, EventArgs e)
        {
            Ipaybox.PRV_SELECTED_GROUP.id = "";
            Ipaybox.PRV_SELECTED_ID = "";
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Возврат в гл. меню по таймеру.");
            ExitForm();
        }
    }
}
