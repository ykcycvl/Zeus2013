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
using zeus.HelperClass;
using zeus;

namespace zeus
{
    public partial class main_menu : Form
    {
        private Bitmap _backBuffer;
        private bool IsHiding = false;
        public static List<AxShockwaveFlashObjects.AxShockwaveFlash> lFlash;
        zPictureBox[] pics = new zPictureBox[50];
        List<zeus.HelperClass.zBanner> banners = new List<zeus.HelperClass.zBanner>();
        List<AxShockwaveFlashObjects.AxShockwaveFlash> bannerList = new List<AxShockwaveFlashObjects.AxShockwaveFlash>();
        PictureBox dalee = new PictureBox();
        //PictureBox nazad = new PictureBox();
        //PictureBox v_menu = new PictureBox();

        //XmlElement xml;
        int pb_count = 0;
        //int banner_count = 0;

        bool _timerEnabled;
        public bool TimerEnabled
        { get { return _timerEnabled; }
            set { _timerEnabled = value; flush_timer.Enabled = value; }
        }

        //bool IsGroup;
        //int Group_id;

        //int Start;
        //int Count;
        /// <summary>
        /// Страница которую надо показать.
        /// </summary>
        static int Page=0;
   
        public main_menu()
        {
            InitializeComponent();
            this.KeyPress += new KeyPressEventHandler(main_menu_KeyPress);
            this.KeyDown += new KeyEventHandler(main_menu_KeyDown);
            lFlash = new List<AxShockwaveFlashObjects.AxShockwaveFlash>();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |ControlStyles.UserPaint |  ControlStyles.DoubleBuffer, true);
        }
        void main_menu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F4)
            {
                Ipaybox.StartForm.Dispose();
            }
        }

        void main_menu_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }

            base.OnSizeChanged(e);
        }
        private void Process_PRV(XmlElement prv)
        {
            XmlElement root = prv;

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];
                if (row.Name == "provider")
                {
                    string id_ = row.GetAttribute("id");
                }
            }
        }
        private void Process(Ipaybox.Ececution what, string id)
        {
            // Обработчик 
            if (what == Ipaybox.Ececution.Provider || what == Ipaybox.Ececution.UserSet)
            {
                // Если кликнули по провайдеру
                XmlElement root = (XmlElement)Ipaybox.providers.DocumentElement.SelectSingleNode("providers");

                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    XmlElement row = (XmlElement)root.ChildNodes[i];
                    if (row.Name == "provider")
                    {
                        string id_ = row.GetAttribute("id");
                        if (id_ == id)
                        {
                            Ipaybox.PROVIDER_XML = row;
                            Ipaybox.PRV_SELECTED_ID = id;
                            Ipaybox.PRV_SELECTED_GROUP.id = row.GetAttribute("group-id");
                            break;
                        }
                     }
                }

                if(id != "information")
                    Ipaybox.StartForm.Main_Process();

                pooling.Stop();
                pooling.Dispose();
                this.Dispose();
            }
            else
            {
                Hide_Elements();
                // Если кликнули по иконке группы
                Ipaybox.PRV_SELECTED_GROUP.id = id;
                this.main_menu_Load(this, new EventArgs());  
            }
        }

        // Загрузить интерфейс в соответствии с настройками интерфейса
        private void LoadThisForm()
        {
            try
            {
                for (int i = 0; i < Ipaybox.ifc.systemForms.Count; i++)
                    this.BackgroundImage = new Bitmap(Ipaybox.StartupPath + @"\" + Ipaybox.ifc.systemForms[i].bgimg);
            }
            catch
            {
                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.NeedUpdates.Core = true;
                Ipaybox.Working = false;
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
            for (int i = 0; i < pb_count; i++)
            {
                try
                {
                    PictureBox pb = (PictureBox)this.pics[i];
                    Rectangle dest = new Rectangle(pb.Left, pb.Top, pb.Width, pb.Height);
                    string p = pb.Tag.ToString();

                    if (pb.Visible || (pb.Tag.ToString() != "tomenu" && Ipaybox.PRV_SELECTED_GROUP.id == ""))
                        g.DrawImage(pb.Image, dest);
                }
                catch
                { }
            }

            g.Dispose();
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (!IsHiding)
            {
                //pevent.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
                if (_backBuffer != null)
                    pevent.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
                else
                    DrawScreen();
            }
        }
        void main_menu_Click(object sender, System.EventArgs e)
        {
            MouseEventArgs p = (MouseEventArgs)e;

            for (int i = 0; i < pb_count; i++)
            {
                if ( p.X  > pics[i].Left + 5 && p.X < pics[i].Left + pics[i].Width - 5
                    && p.Y > pics[i].Top + 5 &&  p.Y < pics[i].Top + pics[i].Height - 5)
                { 
                    if(pics[i].Visible)
                        Pic_Click(pics[i],new EventArgs());
                    break;
                }
            }
           // throw new System.NotImplementedException();
        }
        protected override void OnPaint(PaintEventArgs e)
        {           
            //base.OnPaint (e); //optional but not recommended
        }
        void main_menu_Paint(object sender, PaintEventArgs e)
        {
            // PictureBox tmp = new PictureBox();
            // try
            // {
            //   tmp =  (PictureBox)sender;
            // }
            // catch
            // {}
            // base.OnPaint(e);
            //if (tmp != null)
            //e.Graphics.DrawImage(tmp.Image, tmp.Location.X, tmp.Location.Y);
            //throw new NotImplementedException();
        }
        protected override void OnClosed(EventArgs e)
        {
            lFlash.Clear();
            base.OnClosed(e);
        }

        private void main_menu_Load(object sender, EventArgs e)
        {
            pooling.Enabled = CanPolling;
            /*axShockwaveFlash1.Stop();
            axShockwaveFlash1.LoadMovie(0, Ipaybox.StartupPath + @"\flash\mtsklass2605.swf");
            axShockwaveFlash1.PreviewKeyDown += new PreviewKeyDownEventHandler(OnBannerKeyDown);
            axShockwaveFlash1.Size = new Size(968, 151);*/
            //axShockwaveFlash1.Play();

            TransparentPanel panel = new TransparentPanel();
            /*panel.Location = axShockwaveFlash1.Location;
            panel.Size = axShockwaveFlash1.Size;
            panel.Click += new EventHandler(OnPanel_Click);
            this.Controls.Add(panel);
            panel.BringToFront();*/

            pb_count = 0;
            Ipaybox.IsMainMenu = true;
            this.DoubleBuffered = true;
            //support.Text = Ipaybox.Terminal.jur_name+ " ИНН "+Ipaybox.Terminal.jur_inn + " Тех. поддержка:"+ Ipaybox.Terminal.support_phone;

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            this.Size = Ipaybox.Resolution;

            if (Ipaybox.Inches == 17)
                this.Location = new Point(0, 0);

            support.Top = this.Bottom - 15;
            support.Left = (this.Size.Width - support.Size.Width) / 2;
            this.StartPosition = FormStartPosition.CenterScreen;

            if (!Ipaybox.Debug)
                this.TopMost = true;

            try
            {
                _cursor.Hide();
            }
            catch { }

            LoadThisForm();

            if (string.IsNullOrEmpty(Ipaybox.PRV_SELECTED_GROUP.id))
            {
                LoadTop5(Ipaybox.Inches);
            }
            else
            {
                Load_Group(Ipaybox.PRV_SELECTED_GROUP.id);
            }

            DrawScreen();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Ipaybox.IsMainMenu = false;
        }
        private void LoadTop5(int inches)
        {
            TimerEnabled = false;
            LoadGroups();

            try
            {
                zeus.HelperClass.zMainMenuForm frm = Ipaybox.ifc.systemForms.Find(p => p.name == "main_menu").top;
                int count = frm.count;
                int startx = int.Parse(frm.startPos.Split(';')[0]);
                int starty = int.Parse(frm.startPos.Split(';')[1]);
                int inline_count = frm.hcount;
                int dx = frm.dx;
                int dy = frm.dy;

                for (int i = 0; i < frm.buttons.Count; i++)
                {
                    Ipaybox.AddButton(frm.buttons[i], ref pics[pb_count], this, new EventHandler(this.Pic_Click));
                    pb_count++;
                }

                for (int i = 0; i < frm.banners.Count; i++) 
                {
                    Ipaybox.AddBanner(frm.banners[i], this, new EventHandler(this.Pic_Click));
                    //AddFlash(frm.banners[i], this, new EventHandler(this.Pic_Click));
                }

                XmlElement root = (XmlElement)Ipaybox.config.DocumentElement.SelectSingleNode("top");

                if (root == null)
                    root = (XmlElement)Ipaybox.config.DocumentElement.SelectSingleNode("top5");

                int x = startx, y = starty;
                int count1 = 0;

                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    if (i >= count)
                        break;

                    XmlElement row = (XmlElement)root.ChildNodes[i];

                    if (row.Name == "prv")
                    {
                        string id = row.GetAttribute("id");
                        string img = Ipaybox.GetImageFromInterface_Prv(id);

                        if (img == "")
                            break;

                        CreateNewProvider(img, x, y, id, new Size());
                        x += dx;
                        count1++;

                        if (count1 > inline_count - 1)
                        {
                            x = startx;
                            y += dy;
                            starty = y;
                            count1 = 0;
                        }
                    }
                    else
                    {
                        if (row.Name == "group")
                        {
                            string id = "group-" + row.GetAttribute("id");
                            string img = Ipaybox.GetImageFromInterface_Group(row.GetAttribute("id"));

                            if (img == "")
                                break;

                            CreateNewProvider(img, x, y, id, new Size());
                            x += dx;
                            count1++;

                            if (count1 > inline_count - 1)
                            {
                                x = startx;
                                y += dy;
                                starty = y;
                                count1 = 0;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Ipaybox.Working = false;
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "LoadTop5. " + ex.Message);
            }
        }
        public static void AddFlash(zeus.HelperClass.zBanner banner, System.Windows.Forms.Form f, EventHandler tar)
        {
            Point location = new Point();
            Size size = new Size();
            string src = banner.src;
            location.X = int.Parse(banner.location.Split(';')[0]);
            location.Y = int.Parse(banner.location.Split(';')[1]);
            size.Width = int.Parse(banner.size.Split(';')[0]);
            size.Height = int.Parse(banner.size.Split(';')[1]);

            try
            {
                AxShockwaveFlashObjects.AxShockwaveFlash Flash = new AxShockwaveFlashObjects.AxShockwaveFlash();
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main_menu));
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
        public static void AddFlash(XmlElement el, System.Windows.Forms.Form f, EventHandler tar)
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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main_menu));
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

        public bool HasGroupProvider(string groupId)
        {
            try
            {
                XmlElement root = (XmlElement)Ipaybox.providers.DocumentElement.SelectSingleNode("providers");
 
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {

                    XmlElement row = (XmlElement)root.ChildNodes[i];
                    string o = row.GetAttribute("group-id");

                    if (row.GetAttribute("group-id") == groupId)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        private void LoadGroups()
        {
            try
            {
                zeus.HelperClass.zMainMenuForm frm = Ipaybox.ifc.systemForms[0].groupList;
                int count = frm.count;

                int startx = int.Parse(frm.startPos.Split(';')[0]);
                int starty = int.Parse(frm.startPos.Split(';')[1]);
                int inline_count = frm.hcount;
                int dx = frm.dx;
                int dy = frm.dy;

                XmlElement root = (XmlElement)Ipaybox.config.DocumentElement.SelectSingleNode("groups");

                int x = startx, y = starty;
                int count1 = 0;

                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    XmlElement row = (XmlElement)root.ChildNodes[i];

                    if (row.Name == "group" && row.GetAttribute("parent") == "0" && HasGroupProvider(row.GetAttribute("id")))
                    {
                        string id = "group-" + row.GetAttribute("id");
                        string img = Ipaybox.GetImageFromInterface_Group(row.GetAttribute("id"));

                        if (img != "")
                        {
                            CreateNewProvider(img, x, y, id, new Size());
                            x += dx;
                            count1++;

                            if (count1 == inline_count)
                            {
                                x = startx;
                                y += dy;
                                
                                count1 = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
               Ipaybox.Working = false;
            }
        }
        [System.Diagnostics.DebuggerStepThrough]
        private void Hide_Elements()
        {
            this.IsHiding = true;
            //this.Controls.Clear();
             for (int i = 0; i < pb_count; i++)
            {
                pics[i].Visible = false;
            }

            pb_count = 0;
            //v_menu.Hide();
            this.IsHiding = false;
            //this.Validate();
        }
        /// <summary>
        /// Спрятать картинку с определенным тэгом
        /// </summary>
        /// <param name="tag">Тэг картинки которую нужно спарятать</param>
        private void HideElement(string tag)
        {
            foreach (PictureBox p in pics)
            {
                if (p == null )
                    break;
                if(p.Tag.ToString() == tag)
                {
                    p.Visible = false;
                    break;
                }
            }
        }
        private void Load_Group(string group_id)
        {
            TimerEnabled = true;
            pb_count = 0;

            zeus.HelperClass.zMainMenuForm frm = Ipaybox.ifc.systemForms[0].group;
            int count = frm.count;
            //Начальные координаты
            int startx = int.Parse(frm.startPos.Split(';')[0]);
            int starty = int.Parse(frm.startPos.Split(';')[1]);
            int dx = frm.dx;
            int dy = frm.dy;
            int inline_count = frm.hcount;
            int countProviders = 0;

            //Добавление кнопок на форму
            for (int i = 0; i < frm.buttons.Count; i++)
            {
                Ipaybox.AddButton(frm.buttons[i], ref pics[pb_count], this, new EventHandler(this.Pic_Click));
                pb_count++;
            }

            try
            {
                /*
                 * Добавление кнопок групп на форму
                 */
                int x = startx, y = starty;
                //int lastx = x, lasty = y;
                int count1 = 0;
                int count2 = 1;
                int CountAdded = 0;

                if (Page == 0)
                {
                    XmlElement root = (XmlElement)Ipaybox.config.DocumentElement.SelectSingleNode("groups");
                   
                    for (int i = 0; i < root.ChildNodes.Count; i++)
                    {
                        XmlElement row = (XmlElement)root.ChildNodes[i];
                        if (row.Name == "group" && row.GetAttribute("parent") == group_id)
                        {
                            string id = "group-" + row.GetAttribute("id");
                            string img = Ipaybox.GetImageFromInterface_Group(row.GetAttribute("id"));
                            if (img != "")
                            {
                                CreateNewProvider(img, x, y, id, new Size(), true);
                                CountAdded++;
                                x += dx;
                                count1++;

                                if (count1 == inline_count)
                                {
                                    x = startx;
                                    y += dy;
                                    count2++;
                                    count1 = 0;
                                }
                            }
                        }
                    }
                }

                int countGroup = Ipaybox.config.DocumentElement.SelectSingleNode("groups").SelectNodes("//group[@parent='"+group_id +"']").Count;
                //XmlNodeList root1 = Ipaybox.providers.DocumentElement.SelectNodes("//provider[@group-id='"+group_id +"']");
                XmlNodeList root1 = Ipaybox.providers.DocumentElement.SelectNodes("//*[@group-id='" + group_id + "']");
                countProviders = root1.Count;
                // Если нет провайдеров в группе
                if (countProviders == 0)
                {
                    //Ipaybox.PRV_SELECTED_GROUP = "";
                    ToMenu();
                }
                if (root1.Count + countGroup <= Page*count+count)
                {
                    HideElement("page++");
                }

                int startPrv = Page > 0 ? Page*count - countGroup:0;
                int countPrv = Page * count + count;

                if (countPrv >= root1.Count + countGroup)
                    countPrv = root1.Count;
                else
                    countPrv = countPrv - countGroup;

                for (int i  = startPrv ; i < countPrv ; i++)
                {
                    XmlElement row = (XmlElement)root1[i];
                    if (row.Name == "provider" && row.GetAttribute("group-id") == group_id)
                    {
                        if (((count2 - 1) * 2 + count1) < count)
                        {
                            string id = row.GetAttribute("id");
                            string img = Ipaybox.GetImageFromInterface_Prv(id);

                            if (img != "")
                            {
                                CreateNewProvider(img, x, y, id, new Size(), true);
                                x += dx;
                                count1++;

                                if (count1 == inline_count)
                                {
                                    x = startx;
                                    y += dy;
                                    startx = x;
                                    starty = y;
                                    count2++;
                                    count1 = 0;
                                }
                            }
                        }
                        else
                        {
                            dalee.Visible = true;
                        }
                    }
                    else
                    {
                        if (row.Name == "group" && row.GetAttribute("group-id") == group_id)
                        {
                            if (((count2 - 1) * 2 + count1) < count)
                            {
                                string id = "group-" + row.GetAttribute("id");
                                string img = Ipaybox.GetImageFromInterface_Group(row.GetAttribute("id"));

                                if (img != "")
                                {
                                    CreateNewProvider(img, x, y, id, new Size(), true);
                                    x += dx;
                                    count1++;

                                    if (count1 == inline_count)
                                    {
                                        x = startx;
                                        y += dy;
                                        startx = x;
                                        starty = y;
                                        count2++;
                                        count1 = 0;
                                    }
                                }
                            }
                            else
                            {
                                dalee.Visible = true;
                            }
                        }
                    }
                }
            }
            catch
            {
                Ipaybox.Working = false;
            }

            if (countProviders > 0)
            {
                string bgimage = Ipaybox.ifc.systemForms[0].bgimg;

                if (bgimage != "")
                {
                    this.BackgroundImage = new Bitmap(Ipaybox.StartupPath + @"\" + bgimage);
                }
            }
        }
        private void CreateNewProvider(string img, int x, int y, string tag, Size size)
        {
            try
            {
                pics[pb_count] = new zPictureBox();
                pics[pb_count].WaitOnLoad = false;

                Bitmap bmp = Ipaybox.Pics[int.Parse(Ipaybox.images[img])];
                pics[pb_count].Image = bmp;
                pics[pb_count].SizeMode = PictureBoxSizeMode.CenterImage;
                pics[pb_count].BackColor = Color.Transparent;
                if (size.Width != 0 && size.Height != 0)
                    pics[pb_count].Size = size;
                else
                    pics[pb_count].Size = bmp.Size;
                pics[pb_count].Location = new Point(x, y);
                pics[pb_count].Tag = tag;
                pics[pb_count].Click += new System.EventHandler(this.Pic_Click);
               
                pics[pb_count].Paint += new PaintEventHandler(main_menu_Paint);
                //this.Controls.Add(pics[pb_count]);

                pb_count++;
            }
            catch { }
        }
        private void CreateNewProvider(string img, int x, int y, string tag, Size size, bool stretch)
        {
            try
            {
                pics[pb_count] = new zPictureBox();
                pics[pb_count].WaitOnLoad = false;

                Bitmap bmp = Ipaybox.Pics[int.Parse(Ipaybox.images[img])];
                pics[pb_count].Image = bmp;
                if(stretch)
                    pics[pb_count].SizeMode = PictureBoxSizeMode.StretchImage;
                else
                    pics[pb_count].SizeMode = PictureBoxSizeMode.CenterImage;
                pics[pb_count].BackColor = Color.Transparent;
                if (size.Width != 0 && size.Height != 0)
                    pics[pb_count].Size = size;
                else
                    pics[pb_count].Size = bmp.Size;

                pics[pb_count].Location = new Point(x, y);
                pics[pb_count].Click += new System.EventHandler(this.Pic_Click);
                pics[pb_count].Paint += new PaintEventHandler(main_menu_Paint);
                pics[pb_count].Tag = tag;
                //this.Controls.Add(pics[pb_count]);
                pb_count++;
            }
            catch { }
        }
        /// <summary>
        /// Возврат в Главное меню
        /// </summary>
        private void ToMenu()
        {
            Ipaybox.PRV_SELECTED_GROUP.id = "";
            Hide_Elements();
            Page = 0;
            this.main_menu_Load(this, new EventArgs());
        }
        private void Pic_Click(object sender, System.EventArgs e)
        {
            Sound.Play(Ipaybox.StartupPath.TrimEnd('\\') + "\\sounds\\" + "click2.wav");
            System.Diagnostics.Debug.WriteLine("Нажата кнопка:" + DateTime.Now.ToString("HH:mm:ss.") + DateTime.Now.Millisecond.ToString());
            zPictureBox pb = (zPictureBox)sender;
            //MessageBox.Show("CLICK PRV=" + pb.Tag);
            if (pb.Tag.ToString() == "tomenu" || (pb.Tag.ToString() == "page--" && Page==0))
            {
                ToMenu();
            }
            else
            {
                if (pb.Tag.ToString() == "page--")
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Страница назад");
                    Page--;
                    Process(Ipaybox.Ececution.Group, Ipaybox.PRV_SELECTED_GROUP.id);
                }
                else
                if (pb.Tag.ToString() == "page++")
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Страница Вперед");
                    Page++;
                    Process(Ipaybox.Ececution.Group, Ipaybox.PRV_SELECTED_GROUP.id);
                }
                else
                {
                    if (pb.Tag.ToString() == "information")
                    {
                        try
                        {
                            Ipaybox.StartForm.StartForm(Ipaybox.Forms.Information);
                            _backBuffer.Dispose();
                            this.BackgroundImage.Dispose();
                            pooling.Stop();
                            pooling.Dispose();
                            this.Dispose();
                            GC.Collect();
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        string[] group = pb.Tag.ToString().Split('-');
                        string groupid = group[1];
                        //IsGroup = true;
                        Hide_Elements();
                        Process(Ipaybox.Ececution.Group, groupid);
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Выбрана Группа ID#" + groupid + "(" + Ipaybox.GroupNames[groupid] + ")");
                    }
                    catch
                    {
                        Page = 0;
                        Process(Ipaybox.Ececution.Provider, pb.Tag.ToString());
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Выбран провайдер ID#" + pb.Tag.ToString().Trim() + "(" + Ipaybox.ProviderNames[pb.Tag.ToString().Trim()] + ")");
                        _backBuffer.Dispose();
                        this.BackgroundImage.Dispose();
                        pooling.Stop();
                        pooling.Dispose();
                        this.Dispose();
                        GC.Collect();
                    }
                }
            }
        }

        private void pooling_Tick(object sender, EventArgs e)
        {
            //прополоть
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
                            if (!Ipaybox.InvalidPinEntered)
                            {
                                pooling.Stop();
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Снят стекер. Показываем сервисное меню.");
                                this.ShowLoginForm("");
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
                        //Ошибок нет - сброс ошибки ввода неверного ПИН-кода
                        Ipaybox.InvalidPinEntered = false;
                    }
                }
                catch
                {
                }
            }
        }

        delegate void ShowLoginFormHandler(String msg);

        private void ShowLoginForm(String msg)
        {
            if (this.InvokeRequired)
            {
                ShowLoginFormHandler lfh = new ShowLoginFormHandler(this.ShowLoginForm);
                this.Invoke(lfh, new Object[] { msg });
                this.Dispose();
            }
            else
            {
                new System.Threading.Thread(SendMonitoring).Start();
                Form n = new login();
                n.Show();

                //Здесь не нужно останавливать таймер проверки состояния купюрника, 
                // иначе форма ввода пин-кода начинает зависать
                this.Dispose();
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

        private void flush_timer_Tick(object sender, EventArgs e)
        {
            if (TimerEnabled)
            {
                ToMenu();
            }
        }

        private void OnPanel_Click(object sender, EventArgs e)
        {
            Panel panel = new Panel();
            panel.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            panel.Location = new Point(0, 0);
            panel.BackColor = Color.Red;
            panel.Click += new EventHandler(OnPanel_ClickNew);
            panel.Name = "panel123";
            this.Controls.Add(panel);
        }

        private void OnPanel_ClickNew(object sender, EventArgs e)
        {
            string s = ((Panel)sender).Name;
            ((Panel)sender).Dispose();
        }
    }
}