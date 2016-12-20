using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Acceptors;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.Cryptography;
using zeus.HelperClass;

namespace zeus
{
    public partial class entermoney : Form
    {
        public static Form cms;
        public Ipaybox.PAY[] pays = new Ipaybox.PAY[2];
        public bool Is2PaysInstedOne = false;
        public static List<AxShockwaveFlashObjects.AxShockwaveFlash> lFlash;
        zPictureBox[] img = new zPictureBox[10];
        int img_count;
        Label[] labels = new Label[10];
        int labels_count;
        //Acceptors.Independed bill;
        int OkButtonIndex;
        //Начать опрос купюрника
        bool StartPooling;
        //bool ComissRecalculated = false;
        // Внесенная сумма
        Decimal m; 
        //Комиссия
        Decimal comi;
        // Минимальная сумма платежа - глобально
        Decimal Min_pay;
        Decimal Max_pay;
        // кол-во купюр
        int BillCount;
        bool IsPaySended;
        bool FormClose;
        XmlElement xml_thanks;
        string addprn = "";

        public entermoney()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(entermoney_KeyDown);
        }

        void entermoney_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F4)
                Ipaybox.StartForm.Dispose();
        }
        private void LoadThisForm()
        {
            //zeus.HelperClass.loyalRegInfo.passes = 0;

            labels = new Label[10];
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
                    formLink = group.forms.Find(p => p.name.ToLower() == "entermoney");
                else
                    if (provider != null)
                        formLink = provider.forms.Find(p => p.name.ToLower() == "entermoney");

                if (formLink == null)
                {
                    formLink = new zeus.HelperClass.zFormLink();
                    formLink.name = "entermoney";
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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(entermoney));
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
        private void entermoney_Load(object sender, EventArgs e)
        {
            lFlash = new List<AxShockwaveFlashObjects.AxShockwaveFlash>();
            Is2PaysInstedOne = false;
            cms = null;
            Ipaybox.showComission = null;
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Вход в меню внесения денег.");

            if (Ipaybox.FiscalRegister)
            {
                if (Ipaybox.FRegister != null)
                {
                    if (Ipaybox.FRegister.PaperPresent)
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "ФР:Проверяем наличие бумаги - OK");
                    else
                    {
                        if (Ipaybox.FRegister.FiscalMode)
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "ФР:Проверяем наличие бумаги - НЕТ бумаги. Останов приема денег.");
                            Ipaybox.Working = false;
                            ExitForm();
                            return;
                        }
                        else
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "ФР:Проверяем наличие бумаги - НЕТ бумаги. Прием денег разрешен.");
                    }
                }
                else
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "ФР не найден! Не работаем.");
                    Ipaybox.Working = false;
                    ExitForm();
                    return;
                }
            }

            if(Ipaybox.Bill != null)
                Ipaybox.Bill.Flush();

            this.Size = Ipaybox.Resolution;

            if (Ipaybox.Inches == 17)
                this.Location = new Point(0, 0);
            try
            {
                _cursor.Hide();
            }
            catch { }
            
            button1.Visible = Ipaybox.Debug;

            LoadThisForm();

            try
            {
                string min = Ipaybox.PROVIDER_XML.GetAttribute("minpay");
                string max = Ipaybox.PROVIDER_XML.GetAttribute("maxpay");
                if (min == "")
                    minsumm.Text = "10";
                else
                    minsumm.Text = min;
                if (max == "")
                    maxsumm.Text = "не ограничена";
                else
                    maxsumm.Text = max;

            }
            catch { }

            //bill = Ipaybox.Bill;

            try
            {
                Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StartTake);
                if (res == Acceptors.Result.Error || res == Result.Null && !Ipaybox.Debug)
                {
                    Ipaybox.Bill.Error = true;
                    Ipaybox.Bill.ErrorMsg += " START TAKE FAILURE";
                    Ipaybox.Working = false;
                    ExitForm();
                }
                int min = 0;
                int max = 0;
                
                int.TryParse(Ipaybox.PROVIDER_XML.GetAttribute("minpay"),out min);
                int.TryParse(Ipaybox.PROVIDER_XML.GetAttribute("maxpay"),out max);

                Decimal.TryParse(Ipaybox.PROVIDER_XML.GetAttribute("minpay"), out Min_pay);
                Decimal.TryParse(Ipaybox.PROVIDER_XML.GetAttribute("maxpay"), out Max_pay);

                Ipaybox.Bill.accmoney.R10 = true;
                Ipaybox.Bill.accmoney.R50 = true;
                Ipaybox.Bill.accmoney.R100 = true;
                Ipaybox.Bill.accmoney.R500 = true;
                Ipaybox.Bill.accmoney.R1000 = true;

                /*
                 * Закоменчено потому что надо
                if (min >= 50)
                    Ipaybox.Bill.accmoney.R10 = false;
                if (min >= 100)
                    Ipaybox.Bill.accmoney.R50 = false;
                if (min >= 500)
                    Ipaybox.Bill.accmoney.R100 = false;
                if (min >= 1000)
                    Ipaybox.Bill.accmoney.R500 = false;
                */

                StartPooling = true;
            }
            catch
            { 
                // Если что то сломалось.
                if (!Ipaybox.Debug)
                {
                    Ipaybox.Working = false;
                    ExitForm();
                }
            }

            GetComission();
        }

        private void Hide_Elements()
        {
            for (int i = 0; i < labels_count; i++)
            {
                labels[i].Visible = false;
            }

            for (int i = 0; i < img_count; i++)
            {
                img[i].Visible = false;
            }

            money.Visible = false;
            comiss.Visible = false;
            button1.Visible = false;
            account.Visible = false;
            to_money.Visible = false;
            minsumm.Visible = false;
            maxsumm.Visible = false;
        }

        private void Process_Form(zeus.HelperClass.zForm frm)
        {
            labels_count = 0;
            img_count = 0;

            try
            {
                this.BackgroundImage = new Bitmap(Ipaybox.StartupPath + @"\" + frm.bgimg);
            }
            catch
            {
                this.BackColor = Color.FromArgb(230, 230, 230);
            }

            for (int i = 0; i < frm.buttons.Count; i++)
            {
                AddButton(frm.buttons[i]);
            }

            for (int i = 0; i < frm.labels.Count; i++)
            {
                switch (frm.labels[i].name)
                {
                    case "minsumm":
                        AddLabel(frm.labels[i], ref minsumm, this);
                        break;

                    case "maxsumm": AddLabel(frm.labels[i], ref maxsumm, this); break;
                    case "amount": AddLabel(frm.labels[i], ref money, this); break;
                    case "comiss": AddLabel(frm.labels[i], ref comiss, this); break;
                    case "to_amount": AddLabel(frm.labels[i], ref to_money, this); break;
                    case "achtungcomiss":
                        frm.labels[i].text = string.Format("Внимание!\r\nМинимальная сумма платежа {0} руб. Максимальная сумма платежа {1} руб.", Ipaybox.PROVIDER_XML.GetAttribute("minpay"), Ipaybox.PROVIDER_XML.GetAttribute("maxpay"));
                        Ipaybox.AddLabel(frm.labels[i], ref labels[labels_count], this);
                        labels_count++;
                        break;
                    case "textcomiss":
                        frm.labels[i].text = GetTextFromComiss();
                        Ipaybox.AddLabel(frm.labels[i], ref labels[labels_count], this);
                        labels_count++;
                        break;
                    default:
                        Ipaybox.AddLabel(frm.labels[i], ref labels[labels_count], this);
                        labels_count++;
                        break; 
                }
            }

            AddProviderImage(frm.prvImage);

            for (int i = 0; i < frm.images.Count; i++)
            {
                Ipaybox.AddImage(frm.images[i], ref img[img_count], this);

                img_count++;
            }
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
                img[img_count].BackColor = Color.Transparent;
                this.Controls.Add(img[img_count]);
                img_count++;
            }
        }
        
        public bool ShowFullTextMoney { get; set; }
        public void AddLabel(zeus.HelperClass.zLabel lbl, ref Label label, System.Windows.Forms.Form f)
        {
            string location = lbl.location;
            string text = lbl.text;
            string size = lbl.size;
            string font = lbl.fontFamily;
            string font_size = lbl.fontSize;
            string color = lbl.color;
            string textalign = lbl.textAlign;
            string style = lbl.fontStyle;

            label.Visible = lbl.visible;

            if (lbl.showCurrency)
            {
                ShowFullTextMoney = true;
                money.Text = GetFullTextMoney(0);
                comiss.Text = GetFullTextMoney(0);
                to_money.Text = GetFullTextMoney(0);
            }

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            if (text.Length != 0)
                label.Text = text;

            if (lbl.bgcolor.ToLower() == "transparent")
                label.BackColor = Color.Transparent;
            else
                label.BackColor = Color.FromName(lbl.bgcolor);

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
            img[img_count].Tag = btn.value.ToLower();
            img[img_count].Click += new System.EventHandler(this.Pic_Click);
            img[img_count].BackColor = Color.Transparent;
            img[img_count].key = key;

            if (img[img_count].Tag.ToString() == "ok")
            {
                OkButtonIndex = img_count;
                img[img_count].Visible = false;
            }

            this.Controls.Add(img[img_count]);

            img_count++;
        }

        private void AddButton(XmlElement el)
        {

            string location = el.GetAttribute("location");
            string limg = el.GetAttribute("img");

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            img[img_count] = new zPictureBox();
            img[img_count].Location = new Point(X, Y);
            img[img_count].Image = bmp;
            img[img_count].Size = bmp.Size;
            img[img_count].Tag = el.GetAttribute("value").ToLower();
            img[img_count].Click += new System.EventHandler(this.Pic_Click);

            if (img[img_count].Tag.ToString() == "ok")
            {
                OkButtonIndex = img_count;
                img[img_count].Visible = false;
            }

            this.Controls.Add(img[img_count]);

            img_count++;
        }
        string GetTextFromComiss()
        {
            StringBuilder sb = new StringBuilder();

            bool Is=false;
            XmlElement showComission = XmlComissionFind();
            if (showComission != null)
            {
                foreach (XmlElement el in showComission)
                {
                    if (el.Name == "c")
                    {
                        string from = el.GetAttribute("from");
                        string to = el.GetAttribute("to");
                        string fix = el.GetAttribute("fix");

                        string persent = el.GetAttribute("persent");

                        string min = el.GetAttribute("min");
                        string max = el.GetAttribute("max");
                        string summa = "";
                        if (from == "0" && to == "0")
                            summa = "Для любой суммы";
                        else
                            summa = "От " + from + " руб. до " + to + " руб.";
                        string razmer = "";

                        if (!string.IsNullOrEmpty(fix) && fix != "0")
                            razmer = fix + " руб.";
                        else
                            razmer = persent + " %";

                        if (!string.IsNullOrEmpty(min) && min != "0")
                            razmer += ", минимально " + min + " руб.";
                        if (!string.IsNullOrEmpty(max) && max != "0")
                            razmer += ", макимально " + min + " руб.";

                        sb.AppendLine(summa+" "+ razmer );
                        Is = true;
                    }
                }
                if (Is == false)
                    sb.AppendLine( "Для любой суммы Комиссия 0 руб." );
            }
            else
            {
                sb.AppendLine("Для любой суммы Комиссия 0 руб.");
            }

            return sb.ToString();
        }
        private void GetComission(XmlElement el)
        {
            Ipaybox.showComission = el;
            XmlElement root =el;

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == "c" )
                {
                    string from =  row.GetAttribute("from");
                    string to = row.GetAttribute("to");
                    string fix = row.GetAttribute("fix");
                    string persent = row.GetAttribute("persent");

                    string minimum = row.GetAttribute("min");
                    string maximum = row.GetAttribute("max");
                    // разница не установлена
                    if (from.Length == 0 && to.Length == 0 || from == "0" && to == "0")
                    {
                        if (fix.Length != 0 && fix != "0")
                        {
                            comi = Decimal.Parse(fix);
                        }
                        else
                        {
                            comi = m * Decimal.Parse(persent) / 100;
                            
                            if (minimum.Length != 0 && minimum != "0")
                            {
                                if (comi < Decimal.Parse(minimum))
                                {
                                    comi = Decimal.Parse(minimum);
                                }
                            }
                            if (maximum.Length != 0 && maximum != "0")
                            {
                                if (comi > Decimal.Parse(maximum))
                                {
                                    comi = Decimal.Parse(maximum);
                                }
                            }
                        }

                        break;
                    }
                    else
                    {
                        Decimal from1 = Decimal.Parse(from);
                        Decimal to1 = Decimal.Parse(to);
                        if (m >= from1 && m <= to1 || i + 1 == root.ChildNodes.Count)
                        {
                            if (fix.Length != 0 && fix != "0")
                            {
                                comi = Decimal.Parse(fix);
                            }
                            else
                            {
                                comi = m * Decimal.Parse(persent) / 100;

                                if (minimum.Length != 0 && minimum != "0")
                                {
                                    if (comi < Decimal.Parse(minimum))
                                    {
                                        comi = Decimal.Parse(minimum);
                                    }
                                }
                                if (maximum.Length != 0 && maximum != "0")
                                {
                                    if (comi > Decimal.Parse(maximum))
                                    {
                                        comi = Decimal.Parse(maximum);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        XmlElement XmlComissionFind()
        {
            try
            {
                XmlElement root = Ipaybox.comiss.GetElementsByTagName("comission")[0] as XmlElement;

                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    XmlElement row = (XmlElement)root.ChildNodes[i];

                    if (Ipaybox.PROVIDER_XML != null)
                    {
                        if (row.Name == "comiss" && row.GetAttribute("id") == Ipaybox.PROVIDER_XML.GetAttribute("profile"))
                        {
                            return row;
                        }
                    }
                }
            }
            catch (Exception ex){
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось загрузить комиссию: " + ex.Message);
                Ipaybox.NeedToUpdateConfiguration = true;
                return null;
            }
            
            return null;
        }
        private void GetComission()
        {
            /*
             * Если комиссия не нул, 
             * И!
             * (
             * номер находится в белом списке
             * И
             * провайдер не принадлежит группе 1 (сотовые операторы) 
             * И
             * провайдер не принадлежит группе 5 (мегафон)
             * И
             * провайдер не принадлежит группе 10
             * И
             * провайдер не принадлежит группе 11
             * )
             * 
             * ТО берем комиссию, иначе - не берем
             */
            if (XmlComissionFind() != null && !WhiteList.PhoneNumber.Exists(GetAccountValue()))
                GetComission(XmlComissionFind());
        }
        private void Pic_Click(object sender, System.EventArgs e)
        {
            Sound.Play(Ipaybox.StartupPath.TrimEnd('\\') + "\\sounds\\" + "click1.wav");
            zPictureBox pb = (zPictureBox)sender;
            //MessageBox.Show("CLICK PRV=" + pb.Tag);
            string tag = pb.Tag.ToString().ToLower();

            // Нажата не кнопка на клаве
            switch (tag)
            {
                case "ok":
                    try
                    {
                        if (Ipaybox.Bill != null && (Ipaybox.Bill.BillAcceptorActivity || lockButton))
                            return;

                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Нажата кнопка оплатить.");
                        ExitForm();
                    }
                    catch
                    {
                    }
                    break;
                case "cancel":
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Нажата кнопка в меню.");
                    ExitForm();
                    break;
                case "showkomiss":
                    if (cms == null || cms.IsDisposed)
                    {
                        cms = new comiss();
                        cms.Show();
                    }
                    else
                    {
                        cms.Dispose();
                        cms = null;
                    }
                    break;
                case "goprev":
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Нажата кнопка назад."); 
                    Ipaybox.cFormIndex = Ipaybox.cFormIndex - 1;
                    ExitForm();
                    //Ipaybox.StartForm.Main_Process();
                    //this.Dispose();
                    //
                    break;
            }
        }
        /// <summary>
        /// Загрузить шаблон чека из файла.
        /// </summary>
        /// <param name="FileName">Полный путь</param>
        /// <returns>Шаблон / null</returns>
        private string LoadPrinterTemplateFile(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);
            if (fi.Exists)
            {
                StreamReader sr = fi.OpenText();
                string check = sr.ReadToEnd();
                sr.Close();

                return check;
            }
            else
            { return null; }
        }

        /// <summary>
        /// Загрузить шаблон чека
        /// </summary>
        /// <returns>шаблон</returns>
        private string LoadPrinterTemplate()
        {
            string defaulttemplate = "default.prn";
            string DefaultTemplatePath = "";
            string find = "VKP80";
            if (Ipaybox.Printer != null)
            {
                find = Ipaybox.Printer.PrnModel.ToString();
                if (string.IsNullOrEmpty(find) || find == "NULL")
                    find="VKP80";
            }
            else
                if (Ipaybox.KKMPrim21K)
                    find = "prm";            
          
            DirectoryInfo dir = new DirectoryInfo(Ipaybox.StartupPath.TrimEnd('\\') + "\\config");
            
            if (dir.Exists)
            {
                foreach (FileInfo fi in dir.GetFiles("*.prn"))
                {
                    if (fi.Name.ToLower() == defaulttemplate.ToLower())
                    {
                        DefaultTemplatePath = fi.FullName;
                    }
                    if (fi.Name.ToLower() == find.ToLower() + ".prn")
                    {
                        return LoadPrinterTemplateFile(fi.FullName);
                    }
                }
                if (!string.IsNullOrEmpty(DefaultTemplatePath))
                {
                    return DefaultTemplatePath;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Создать чек
        /// </summary>
        /// <returns>Чек для печати</returns>
        private string CreateRecept()
        {
            PrepareCheck prepareCheckForm = new PrepareCheck();
            prepareCheckForm.Show();
            Application.DoEvents();

            string check = LoadPrinterTemplate();

            // заполняем введенные параметры
            string params_c = "";
            string[] opt = Ipaybox.curPay.Options.Split('|');

            for (int i = 0; i < opt.Length; i++)
            {
                if (opt[i] != "")
                {
                    string[] param = opt[i].Split('%');
                    params_c += param[0] + ": " + param[2];
                    if (i + 1 < opt.Length)
                        params_c += "\r\n";
                }
            }

            if (addprn.Length > 5)
            {
                params_c += "\r\n--- ONLINE покупка\r\n";
                string[] lines = addprn.Split('|');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] != null)
                        if (lines[i] != "")
                        {
                            string[] param = lines[i].Split('=');
                            if (param.Length == 2)
                            {
                                params_c += param[0] + ": " + param[1];
                                if (i + 1 < opt.Length)
                                    params_c += "\r\n";
                            }
                            else
                            {
                                params_c = lines[i];
                            }

                        }
                }
                params_c += "\r\n---";
            }

            check = check.Replace("[agent_jur_name]", Ipaybox.Terminal.jur_name.Trim());
            check = check.Replace("[agent_adress]", Ipaybox.Terminal.jur_adress.Trim());
            check = check.Replace("[agent_inn]", "ИНН " + Ipaybox.Terminal.jur_inn.Trim());
            check = check.Replace("[provider_support_phone]", Ipaybox.PROVIDER_XML.GetAttribute("support_phone"));
            check = check.Replace("[agent_support_phone]", Ipaybox.Terminal.support_phone.Trim());
            check = check.Replace("[bank]", Ipaybox.Terminal.bank.Trim());
            check = check.Replace("[terms_number]", Ipaybox.Terminal.terms_number.Trim());
            check = check.Replace("[count_bill]", Ipaybox.Incass.countchecks.ToString().Trim());
            check = check.Replace("[terminal_id]", Ipaybox.Terminal.terminal_id.Trim());
            check = check.Replace("[trm_adress]", Ipaybox.Terminal.trm_adress.Trim());
            check = check.Replace("[date]", Ipaybox.curPay.Date.ToString().Trim());
            check = check.Replace("[prv_name]", Ipaybox.PROVIDER_XML.GetAttribute("name"));
            check = check.Replace("[amount]", Ipaybox.curPay.from_amount.ToString() + " руб.");
            check = check.Replace("[to_amount]", Ipaybox.curPay.to_amount< Max_pay ? Ipaybox.curPay.to_amount.ToString() + " руб." :Max_pay.ToString() + " руб.");
            check = check.Replace("[comiss]", (Ipaybox.curPay.from_amount - Ipaybox.curPay.to_amount).ToString() + " руб.");
            check = check.Replace("[params]", params_c);
            check = check.Replace("[txn_id]", Ipaybox.curPay.txn_id.Trim());

            if (Ipaybox.FRS.RemoteFR && !Ipaybox.IncassCheck)
            {
                check = remoteFR.RemoteFiscalRegister.tryFormFicsalCheck(Ipaybox.Terminal.jur_name.Trim(), Ipaybox.FRS.headertext, check, "Сотовая св.", Ipaybox.Terminal.terminal_id.Trim(), Ipaybox.Terminal.terminal_pass, Ipaybox.curPay.txn_id, Ipaybox.curPay.from_amount.ToString(), "1", Ipaybox.FRS.RemoteFiscalRegisterURL, Ipaybox.FRS.checkWidth, Ipaybox.FRS.remoteFRtimeout);

                if (remoteFR.RemoteFiscalRegister.OK)
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Данные с ФС получены. Печать фискального чека.");
                else
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Данные с ФС НЕ получены. Печать нефискального чека с заголовком.");
            }

            prepareCheckForm.Close();
            return check;
        }
        private string ReceiptToPrint { get; set; }
        private void Print()
        {
            try
            {
                if (Ipaybox.Printer != null || Ipaybox.WindowsPrinter || Ipaybox.FiscalRegister || Ipaybox.KKMPrim21K)
                {
                    // Прячем все что есть на форме
                    //new System.Threading.Thread(SetThanksForm).Start();
                   
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "\t Формирование чека");
                 
                    //Ipaybox.Incass.countchecks++;
                    //Ipaybox.FlushStatistic();
                   
                    // Загружаем шаблон для текущей модели прнтера
                    ReceiptToPrint = CreateRecept();
                   
                    if (!Ipaybox.FiscalRegister)
                    {
                        // Если не используем ФР
                        // формируем строку для печати через головной такймер.
                        Ipaybox.PrintString = ReceiptToPrint;
                    }
                    else
                    {
                        new System.Threading.Thread(RegisterPayFiscal).Start();
                    }                  
                }
                else
                { 
                    // Принтер не подключен.
                }
            }
            catch (Exception ex)
            {
                if (Ipaybox.Debug)
                    MessageBox.Show(ex.ToString());
            }
        }
        private void RegisterPayFiscal()
        {
            if (!Is2PaysInstedOne)
            {
                // Если пользуем ФР то пытаемся сформировать покупку.
                if (!Ipaybox.FRegister.Buy("Сотовая св.", 1, Ipaybox.curPay.from_amount, ReceiptToPrint))
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main,
                        string.Format("Покупка не прошла: Ошибка:{0} Описание:{1}",
                        Ipaybox.FRegister.ErrorNumber,
                        Ipaybox.FRegister.ErrorMessage));
                    if (Ipaybox.FRegister.ErrorNumber != "-1")
                        Ipaybox.Working = false;
                }
            }
            else
            {
                FR.Buy[] b = new FR.Buy[2];

                b[0].Name = Ipaybox.GroupNames[Ipaybox.PROVIDER_XML.GetAttribute("group-id")];
                b[0].Quanity = "1";
                b[0].Amount = pays[0].to_amount;

                b[1].Name = "Неизвестный провайдер";
                b[1].Quanity = "1";
                b[1].Amount = pays[1].to_amount;
                b[1].AdditionalText = ReceiptToPrint;

                if (!Ipaybox.FRegister.Buy(b))
                    Ipaybox.Working = false;
            }
        }

        private void ExitForm()
        {
            if (!FormClose)
            {
                try
                {
                    Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StopTake);

                    if (res == Acceptors.Result.Error || res == Result.Null)
                    {
                        Ipaybox.Bill.Error = true;
                        Ipaybox.Bill.ErrorMsg = "";
                        Ipaybox.Working = false;
                    }
                }
                catch { }

                if (Ipaybox.Bill.Amount > 0)
                {
                    CreatePay();
                }
                else
                    Ipaybox.FlushToMain();

                Ipaybox.Bill.Amount = 0;
                Ipaybox.Bill.BillCount = 0;
                Ipaybox.StartForm.Main_Process();
                FormClose = true;
            }

            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Выход из меню внесения денег.");

            if(cms != null)
                cms.Dispose();

            this.Dispose();
            GC.Collect();
        }
        private void Reset_Timer() 
        {
            flush_timer.Stop();
            flush_timer.Start();
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
                    {
                        Ipaybox.curPay.account = param[2];
                        return param[2];
                    }
                }
            }

            return "";
        }
        /// <summary>
        /// Обновляем информацию по статистике
        /// </summary>
        private void ApplyStatistic()
        {
            // Кол-во чеков
            Ipaybox.Incass.countchecks++;
            Ipaybox.Incass.incass_amount += Convert.ToInt32(m);
            Ipaybox.FlushStatistic();

            //Новая инкассация
            Ipaybox.Incassation.StartTransaction();
            Ipaybox.Incassation.CountPay = Ipaybox.Incassation.CountPay + 1;
            Ipaybox.Incassation.Amount =  Ipaybox.Incassation.Amount + m;
            Ipaybox.Incassation.Commit();

            // Платеж
            Ipaybox.curPay.from_amount = m;
            Ipaybox.curPay.to_amount = m - comi;
            Ipaybox.curPay.IsMoney = true;
            Ipaybox.curPay.Date = DateTime.Now;
            //Ipaybox.curPay.txn_id = DateTime.Now.ToString("yyMMddHHmmss");

            Random p = new Random();

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

            //Ipaybox.curPay.txn_id = DateTime.Now.ToString("yyMMddHHmmss0") + Ipaybox.PRV_SELECTED_ID.Trim() + sHash.Substring(0, 5);
            Ipaybox.curPay.txn_id = DateTime.Now.ToString("yyMMddHHmmss") + Ipaybox.PRV_SELECTED_ID.Trim() + sHash.Substring(0, 5);

            while (Ipaybox.curPay.txn_id.Length > 19)
                Ipaybox.curPay.txn_id = Ipaybox.curPay.txn_id.Substring(0, Ipaybox.curPay.txn_id.Length - 1);

            if (Ipaybox.curPay.to_amount > Max_pay && comi == 0)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Разделяем платеж на 2");

                Ipaybox.Incass.countchecks++;
                Ipaybox.FlushStatistic();
                
                Ipaybox.Incassation.CountPay++;

                pays[0] = new Ipaybox.PAY();
                pays[0].from_amount = Max_pay;
                pays[0].to_amount = Max_pay;
                pays[0].IsMoney = true;
                pays[0].Date = Ipaybox.curPay.Date;
                pays[0].txn_id = Ipaybox.curPay.txn_id;
                pays[0].account = Ipaybox.curPay.account;
                pays[0].extra = Ipaybox.curPay.extra;
                
                pays[1] = new Ipaybox.PAY();
                pays[1].from_amount = m - Max_pay;
                pays[1].to_amount = m - Max_pay;
                pays[1].IsMoney = true;
                pays[1].Date = Ipaybox.curPay.Date;
                pays[1].txn_id = (Convert.ToUInt64(Ipaybox.curPay.txn_id) + 1).ToString();
                pays[1].account = Ipaybox.curPay.account;
                pays[1].extra = Ipaybox.curPay.extra;

                Is2PaysInstedOne = true;
            }
        }
        private void SetPay()
        {
            if (m <= 0)
                return;

            ApplyStatistic();
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Формируем платеж. " + Ipaybox.curPay.Options.Split('%')[0] + ": " + Ipaybox.curPay.Options.Split('%')[2].TrimEnd('|') + " сумма:" + Ipaybox.curPay.from_amount.ToString() + " Транзакция №:" + Ipaybox.curPay.txn_id);
            PutPayintoImport();

            m = 0;
            comi = 0;
            Print();
        }
        private void SetOnlinePay()
        {
            if (m <= 0)
                return;
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Формируем ON-LINE платеж." + Ipaybox.curPay.Options.Split('%')[0] + ": " + Ipaybox.curPay.Options.Split('%')[2].TrimEnd('|') + " сумма:" + Ipaybox.curPay.from_amount.ToString() + " Транзакция №:" + Ipaybox.curPay.txn_id);

            // Сброс статистики на диск.
            ApplyStatistic();

            if (!Is2PaysInstedOne)
            {
                SendPays sp = new SendPays();

                sp.SendPay("pay", Ipaybox.PRV_SELECTED_ID, Ipaybox.Terminal.terminal_id, Ipaybox.Terminal.terminal_pass, GetAccountValue(), Ipaybox.curPay.txn_id, Ipaybox.curPay.from_amount.ToString(), Ipaybox.curPay.to_amount.ToString(), Ipaybox.curPay.Date.ToString("yyyy-MM-dd HH:mm:ss"));

                XmlDocument doc = new XmlDocument();
                string result = "";

                try
                {
                    doc.LoadXml(sp.XML_Response);

                    XmlElement root = doc.DocumentElement;

                    for (int i = 0; i < root.ChildNodes.Count; i++)
                    {
                        XmlElement row = (XmlElement)root.ChildNodes[i];

                        if (row.Name == "result")
                            result = row.InnerText;

                        if (row.Name == "out" && row.GetAttribute("type") == "prn")
                        {
                            addprn += row.GetAttribute("name") + "=" + row.GetAttribute("value") + "|";
                        }
                    }
                    m = 0;
                    comi = 0;
                    Print();
                }
                catch
                {
                    addprn += "Извините, но в данный момент Он-Лайн Сервис не доступен.\r\n Обратитесь в службу тех поддержки.|";
                    PutPayintoImport();
                    Print();

                }
            }
            else
            {
                SendPays sp = new SendPays();

                sp.SendPay("pay", Ipaybox.PRV_SELECTED_ID, Ipaybox.Terminal.terminal_id, Ipaybox.Terminal.terminal_pass, GetAccountValue(), pays[0].txn_id, pays[0].from_amount.ToString(), pays[0].to_amount.ToString(), pays[0].Date.ToString("yyyy-MM-dd HH:mm:ss"));

                XmlDocument doc = new XmlDocument();
                string result = "";

                try
                {
                    doc.LoadXml(sp.XML_Response);
                    pays[0].IsPaySended = true;

                    XmlElement root = doc.DocumentElement;

                    for (int i = 0; i < root.ChildNodes.Count; i++)
                    {
                        XmlElement row = (XmlElement)root.ChildNodes[i];

                        if (row.Name == "result")
                            result = row.InnerText;

                        if (row.Name == "out" && row.GetAttribute("type") == "prn")
                        {
                            addprn += row.GetAttribute("name") + "=" + row.GetAttribute("value") + "|";
                        }
                    }
                    m = 0;
                    comi = 0;
                    PutPayintoImport();
                    Print();
                }
                catch
                {
                    addprn += "Извините, но в данный момент Он-Лайн Сервис не доступен.\r\n Обратитесь в службу тех поддержки.|";
                    PutPayintoImport();
                    Print();
                }
            
            }
        }
        private void PutPayintoImport()
        {
            if (!Is2PaysInstedOne)
            {
                //Если платеж не разделялся
                XmlElement pay = Ipaybox.pays.CreateElement("pay");
                pay.SetAttribute("prv_id", Ipaybox.curPay.prv_id);
                pay.SetAttribute("amount", Ipaybox.curPay.from_amount.ToString());
                pay.SetAttribute("to-amount", Ipaybox.curPay.to_amount.ToString());
                pay.SetAttribute("txn_id", Ipaybox.curPay.txn_id);
                pay.SetAttribute("txn_date", Ipaybox.curPay.Date.ToString("yyyy.MM.dd HH:mm:ss"));
                pay.SetAttribute("receipt", Ipaybox.Incassation.CountPay.ToString());

                //string[] line =  Ipaybox.curPay.Options.Split('
                foreach (string line in Ipaybox.curPay.Options.Split('|'))
                {
                    if (line != "")
                    {
                        string[] param = line.Split('%');
                        if (param[0] != null)
                        {
                            XmlElement extra = Ipaybox.pays.CreateElement("extra");
                            extra.SetAttribute("name", param[1]);
                            extra.SetAttribute("value", param[2]);
                            extra.SetAttribute("CRC", Ipaybox.getMd5Hash("n" + param[1] + "v" + param[2]));
                            pay.AppendChild(extra);
                        }
                    }
                }

                XmlNode root = Ipaybox.pays.DocumentElement;
                root.InsertAfter(pay, root.FirstChild);
                Ipaybox.pays.Save("import.xml");

                Ipaybox.Import.Add(pay);

                Ipaybox.NeedToSendPays = true;
            }
            else
            {
                int i = 0;
                // Если платеж был поделен
                foreach (Ipaybox.PAY p in pays)
                {
                    if (p.IsPaySended == false)
                    {
                        //Если платеж не разделялся
                        XmlElement pay = Ipaybox.pays.CreateElement("pay");
                        pay.SetAttribute("prv_id", Ipaybox.curPay.prv_id);
                        pay.SetAttribute("amount", p.from_amount.ToString());
                        pay.SetAttribute("to-amount", p.to_amount.ToString());
                        pay.SetAttribute("txn_id", p.txn_id);
                        pay.SetAttribute("txn_date", p.Date.ToString("yyyy.MM.dd HH:mm:ss"));
                        pay.SetAttribute("receipt", (Ipaybox.Incassation.CountPay- i).ToString());
                        i++;

                        //string[] line =  Ipaybox.curPay.Options.Split('
                        foreach (string line in Ipaybox.curPay.Options.Split('|'))
                        {
                            if (line != "")
                            {
                                string[] param = line.Split('%');
                                if (param[0] != null)
                                {
                                    XmlElement extra = Ipaybox.pays.CreateElement("extra");
                                    extra.SetAttribute("name", param[1]);
                                    extra.SetAttribute("value", param[2]);
                                    extra.SetAttribute("CRC", Ipaybox.getMd5Hash("n" + param[1] + "v" + param[2]));
                                    pay.AppendChild(extra);

                                }
                            }
                        }

                        XmlNode root = Ipaybox.pays.DocumentElement;
                        root.InsertAfter(pay, root.FirstChild);
                        Ipaybox.pays.Save("import.xml");

                        Ipaybox.Import.Add(pay);

                        Ipaybox.NeedToSendPays = true;
                    }
                }
            }
        
        }
        private void DisableCancel()
        {
            GetAccountValue();

            for (int i = 0; i < img_count; i++)
            {
                if ((img[i] != null) && (img[i].Tag.ToString() == "cancel" || img[i].Tag.ToString() == "goprev"))
                {       
                    img[i].Visible = false;
                    break;
                }
            }
        }
        private void CreatePay()
        {
            if (!IsPaySended)
            {
                if (!Ipaybox.Bill.BillAcceptorActivity)
                {
                    //Pooling();
                    pooling.Stop();
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Запуск процесса формирования платежа."); 
                    
                    IsPaySended = true;
                    
                    Ipaybox.Incass.count += Ipaybox.Bill.BillCount;
                    Ipaybox.Incass.CountR10 += Ipaybox.Bill.CountR10;
                    Ipaybox.Incass.CountR100 += Ipaybox.Bill.CountR100;
                    Ipaybox.Incass.CountR1000 += Ipaybox.Bill.CountR1000;
                    Ipaybox.Incass.CountR50 += Ipaybox.Bill.CountR50;
                    Ipaybox.Incass.CountR500 += Ipaybox.Bill.CountR500;
                    Ipaybox.Incass.CountR5000 += Ipaybox.Bill.CountR5000;

                    Ipaybox.Incassation.StartTransaction();
                    Ipaybox.Incassation.CountBill = Ipaybox.Incassation.CountBill + (uint)Ipaybox.Bill.BillCount;
                    Ipaybox.Incassation.CountBill10 = Ipaybox.Incassation.CountBill10 + (uint)Ipaybox.Bill.CountR10;
                    Ipaybox.Incassation.CountBill50 = Ipaybox.Incassation.CountBill50 +(uint)Ipaybox.Bill.CountR50;
                    Ipaybox.Incassation.CountBill100 = Ipaybox.Incassation.CountBill100 + (uint)Ipaybox.Bill.CountR100;
                    Ipaybox.Incassation.CountBill500 = Ipaybox.Incassation.CountBill500 + (uint)Ipaybox.Bill.CountR500;
                    Ipaybox.Incassation.CountBill1000 = Ipaybox.Incassation.CountBill1000 + (uint)Ipaybox.Bill.CountR1000;
                    Ipaybox.Incassation.CountBill5000 = Ipaybox.Incassation.CountBill5000 +(uint)Ipaybox.Bill.CountR5000;
                    Ipaybox.Incassation.Commit();

                    Ipaybox.Bill.Flush();

                    if (IsSelectedProviderOnline())
                        SetOnlinePay();
                    else
                        SetPay();
                    Ipaybox.Bill.BillAcceptorActivity = false;
                }
            }
        }
       
        public bool IsSelectedProviderOnline()
        {
            if (Ipaybox.PROVIDER_XML != null)
            {
                string root = Ipaybox.PROVIDER_XML.GetAttribute("type");

                if (root.ToLower() == "online")
                    return true;
            }

            return false;
        }
        private void flush_timer_Tick(object sender, EventArgs e)
        {
            if (!Ipaybox.Bill.BillAcceptorActivity)
            {
                try
                {
                    Ipaybox.Bill.SendCommand(Commands.StopTake);

                    if (Ipaybox.Bill.Amount > 0)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Истекло время ожидания купюры. Отправляем платеж.");

                        // 
                        flush_timer.Stop();
                        CreatePay();
                    }
                    else
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Истекло время ожидания купюры."); 
                }
                catch
                {
                    // обращение к купюрнику вызвало прерывание
                }
                //Ipaybox.AddToLog(Ipaybox.Logs.Main, "Выход из формы в главное меню.");

                ExitForm();
            }
        }
        private bool lockButton;
        string GetFullTextMoney(decimal money)
        {
            int ppp = (int)Math.Abs(money);
            int dd = (int)((money - ppp)  * 100) ;
            string cops = dd.ToString();

            if (dd < 10)
                cops = "0" + dd.ToString();

            return string.Format("{0},{1} руб.",ppp,cops);
        }
        private void Pooling()
        {
            if (StartPooling)
            {
                try
                {
                    lockButton = true;
                    Ipaybox.Bill.Pooling();

                    if (m != Ipaybox.Bill.Amount)
                    {
                        DisableCancel();
                        // Обработка денег
                        Decimal kupura = Ipaybox.Bill.Amount - m;
                        m = Ipaybox.Bill.Amount;
                        BillCount = Ipaybox.Bill.BillCount;
                        GetComission();
                        money.Text = ShowFullTextMoney == false ? Ipaybox.Bill.Amount.ToString() : GetFullTextMoney(Ipaybox.Bill.Amount);
                        comiss.Text = ShowFullTextMoney == false ? comi.ToString() : GetFullTextMoney(comi);
                        to_money.Text = ShowFullTextMoney == false ? (Ipaybox.Bill.Amount - comi).ToString() : GetFullTextMoney(Ipaybox.Bill.Amount - comi);
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Внесена купюра:" + kupura.ToString() + " Общая комиссия:" + comiss.Text);

                        if (Ipaybox.Bill.Amount >= Max_pay && Max_pay > 0)
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Достигнута максимальная сумма платежа:" + maxsumm.ToString() + " Прекращаем прием денег.");
                            pooling.Stop();
                            Ipaybox.Bill.BillAcceptorActivity = false;
                            ExitForm();
                        }

                        Reset_Timer();
                    }

                    if (!Ipaybox.Bill.BillAcceptorActivity)
                        lockButton = false;

                    if (m > 0 && img[OkButtonIndex].Visible == false && !IsPaySended)
                        img[OkButtonIndex].Visible = true;

                    // Обработка ошибок
                    if (Ipaybox.Bill.Error == true)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка купюроприемника. " + Ipaybox.Bill.ErrorMsg);

                        if (Ipaybox.Bill.ErrorMsg == "Pause")
                        {
                            Ipaybox.Bill.Error = false;
                            Ipaybox.Bill.ErrorMsg = "";
                        }
                        else
                        {
                            Ipaybox.Working = false;
                            pooling.Enabled = false;
                            pooling.Stop();
                            Ipaybox.Bill.BillAcceptorActivity = false;
                            ExitForm();
                        }
 
                        if (Ipaybox.Debug)
                            MessageBox.Show(Ipaybox.Bill.ErrorMsg);
                    }
                }
                catch (Exception ex)
                { 
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Нет купюрника - Выход из формы. Не работает.");
                    if (!Ipaybox.Debug)
                    {
                        HelperClass.CrashLog.AddCrash(ex);
                        // Если купюрник нулл
                        Ipaybox.Working = false;
                        pooling.Enabled = false;
                        ExitForm();
                    }
                }
            }
        }
        private void pooling_Tick(object sender, EventArgs e)
        {
            //if(!Ipaybox.Debug) // только для совсем дебага
            Pooling();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DisableCancel();
            m += 10;
            string f = GetFullTextMoney(Convert.ToDecimal("100.53"));
            GetComission();
            //m = bill.Amount;
            //BillCount = bill.BillCount;
            //GetComission();
            comiss.Text = comi.ToString();
            
            Reset_Timer();
            money.Text = ShowFullTextMoney == false ? Ipaybox.Bill.Amount.ToString() : GetFullTextMoney(m); ;
            comiss.Text = ShowFullTextMoney == false ? comi.ToString() : GetFullTextMoney(comi);
            to_money.Text = ShowFullTextMoney == false ? (m - comi).ToString() : GetFullTextMoney(m - comi);
         
            img[OkButtonIndex].Visible = true;
        }
    }
 }
