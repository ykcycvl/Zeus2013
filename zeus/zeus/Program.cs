using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Drawing.Printing;
using Acceptors;
using Microsoft.Win32;
using WD;
using System.Media;
using _prim21k;
using System.Xml.Serialization;
using zeus.HelperClass;

namespace zeus
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>flushform flush = new flushform();
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
    }

    public static class _cursor
    {
        public static bool Visible = true;

        public static void Hide()
        {
            /*if (Visible && !Ipaybox.Debug)
            {
                Cursor.Hide();
                Visible = false;
            }*/
        }

        public static void Show()
        {
            if (!Visible)
            {
                Cursor.Show();
                Visible = true;
            } 
        }
    }

    public static class Ipaybox
    {
        /*public static loyalList loyalClients = new loyalList();
        public static goodClients gclients = new goodClients();*/
        public static bool LoginFormActive = false;
        public static bool ServiceMenuActive = false;

        //Добавление кнопки на форму
        public static void AddButton(zeus.HelperClass.zButton btn, ref zPictureBox img, System.Windows.Forms.Form f, EventHandler tar)
        {
            string location = btn.location;
            string limg = btn.img;
            string tag = btn.value;
            string key = btn.key;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap tmp = Ipaybox.Pics[int.Parse(Ipaybox.images[limg])];
            img = new zPictureBox();
            img.Location = new Point(X, Y);
            img.Image = tmp;
            img.Size = tmp.Size;
            img.Tag = tag;
            img.key = key;
            // НЕРАБОТАЕТ
            img.Click += tar;

            //f.Controls.Add(img);
        }
        //Добавление баннера на форму
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

                                if (form == "mainmenu" || form == "all")
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
        public static interfaceList ifcList = new interfaceList();
        public static zInterface ifc = new zInterface();
        public static bool IncassCheck = false;
        public static uint userID = 1;
        public static string OpsosName = string.Empty;
        public static string IMSI = string.Empty;
        public static string Args;
        public static NameValueCollection images;
        public enum Logs { Main, Update }
        public enum Forms { Main, Info, Account, AcceptAccount, Money, Thanks, Information, UserForm }

        public struct TERMINAL
        {
            public string jur_name;
            public string jur_inn;
            public string jur_adress;
            public string bank;
            public string trm_adress;
            public string terms_number;
            public string support_phone;
            public string terminal_id;
            public string terminal_pass;
            public string pincode;
            public string secret_number;
            public string configuration_id;
            public string Interface;
            public string InterfaceVersion;
            public bool FiscalMode;
        }
        public struct FRS
        {
            public static string RemoteFiscalRegisterURL = "http://localhost:5881/buy/";
            public static string headertext = "";
            public static bool RemoteFR = false;
            public static int remoteFRtimeout = 9000;
            public static int checkWidth = 38;
        }
        public struct PhoneRange
        {
            public ulong from;
            public ulong to;
            public string prv;
        }

        public static List<PhoneRange> PhoneRanges = new List<PhoneRange>();        
        public struct PAY
        {
            public string prv_id;
            public string prv_name;
            public string prv_img;
            public string account;
            public Decimal from_amount;
            public Decimal to_amount;
            public string extra;
            public bool IsAccountConfirmed; // Подтверждены ли введенные данные
            public bool IsMoney;            // Всунуты ли деньги
            public bool IsRecieptPrinted;   //Напечатан ли чек
            public bool IsOnlyOnline;       // онлайновый ли это провайдер?
            public string Options;
            public DateTime Date;
            public string txn_id;
            public bool IsPaySended;
            public List<HelperClass.AccountInfo> ViewText;
        }
        public struct Configuration
        {
            public bool Comission;
            public bool ProviderList;
            public bool Core;
            public bool Config;
            public bool Trm_info;
            public bool Interface;
        }
        public struct Statistic
        {
            public int count;       // кол-во бумажек всунуто
            public int countchecks; // кол-во чеков распечатано
            public int bytesSend;   // байт послано
                    
            public int bytesRead;   // байт принято
            public int incass_amount;// сумма инкасации
            public DateTime incass_date_start; // Дата начала инкасации
            public int CountR10;
            public int CountR50;
            public int CountR100;
            public int CountR500;
            public int CountR1000;
            public int CountR5000;

        }
        public struct Form
        {
            public enum Window
            {
                Main, Info, Edit
            } 
            public static string name;
            public static XmlElement formxml;
        }
        public enum Ececution { Provider, Group, UserSet }

        public static bool IsPrinting;
        public static string PRV_SELECTED_ID;
        public static string USER_SET_NAME;
        public static zFormLink current_User_Form;
        public struct PRV_SELECTED_GROUP
        {
            public static string id;
            public static string parent;
        }
        //public static string    PRV_SELECTED_GROUP;
        public static bool      IsMainMenu;
        public static bool SoundPlaying;
        public static PrintDocument doc;
        public static bool Internet;
        public static modem Modem;
        public static string modemPort;
        public static int NetOption; // 0 - Локалка; 1- Модем
        public static string NetModemName;
        public static XmlElement showComission;
        public static Configuration NeedUpdates;
        private static bool? _IsBlocked;
        public static bool IsBlocked
        { 
            get
            {
                if (!_IsBlocked.HasValue)
                {
                    _IsBlocked = Boolean.Parse((string.IsNullOrEmpty(Ipaybox.Option["blocked"]) ? "false" : Ipaybox.Option["blocked"]));
                }
                return _IsBlocked.Value;
            }
            set
            {
                if (_IsBlocked != value)
                {
                    Ipaybox.Option["blocked"] = value.ToString();
                    _IsBlocked = value;
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Терминал " + (value ? "заблокирован" : "разблокирован"));
                }
            }
        }
        public static API.Import Import;
        public static bool Working;             // Работать ли автомату
        public static bool InvalidPinEntered = false;

        public static string[] UpdateUrl;
        public static string[] ServiceUrl;
        /// <summary>
        /// Настройки программы типа параметр-значение
        /// </summary>
        public static HelperClass.Settings Option;


        public static Size Resolution;      // Сколько вешать в пискселях.
        public static int Inches;           // Сколько дюймов использовать.
        public static Bitmap[] Pics;    // 

        public static int UpdateUrlIndex;
        public static int ServiceUrlIndex;
        public static string CoreVersion;

        public static float RequestTimeout; // Устанавливаем таймаут обращения к серваку (секунды)

        // Платеж
        public static PAY curPay;
        public static bool WindowsPrinter;
        public static bool EpsonT400 = false;
        public static bool KKMPrim21K = false;
        public static bool AtolDriver;
        public static bool FiscalRegister;
        // Терминал
        public static TERMINAL Terminal;
        public static bool Debug;
        public static Statistic Incass;                 // структура инкасации

        public static API.IncassHistory _incassHistory;

        public static API.IncassHistory IncassHistory
        {
            get 
            {
                if (_incassHistory == null)
                {
                    _incassHistory = new zeus.API.IncassHistory(true);
                }

                return _incassHistory;
            }
        }
        
        public static API.Incass Incassation;           // Объект инкассации
        public static XmlElement FORM_XML;              // XML описание выбранной формы

        public static XmlDocument XML_monitoring;       // мониторинг
        //public static XmlDocument XML_PhoneRange;       // Номерные Ёмкости
        
        public static XmlDocument providers  ;          // Список провайдеров
        public static XmlDocument NetOptions;           // Список провайдеров
        public static XmlDocument config;               // Настройки терминала
        public static XmlDocument comiss;               // профили комиссии
        public static XmlDocument pays;                 // Платежи
        public static XmlDocument terminal_info;        // Информация о терминале
        public static bool MasterPIN_IsActive = true;   // Признак активности мастер-ПИНа
        public static XmlDocument TPIN;                 // ПИН-коды техников
        public static XmlDocument FRSSettings;          // Настройки удаленного ФР
        public static XmlDocument XML_Statistic;        // XML структура статистики
        //public static XmlElement XML_Interface;         // XML документ
        public static XmlDocument XML_SendFile;         // XML документ - описание запросов файлов

        /// <summary>
        /// Названия провайдеров - ключ id
        /// </summary>
        public static System.Collections.Specialized.NameValueCollection ProviderNames;
        /// <summary>
        /// Заполнить массив неазваний провайдеров
        /// </summary>
        public static void Initialize_ProviderNames()
        {
            XmlElement root = (XmlElement)Ipaybox.providers.DocumentElement.SelectSingleNode("providers");

            if (ProviderNames == null)
                ProviderNames = new NameValueCollection();
            else ProviderNames.Clear();

            foreach (XmlElement el in root)
            {
                if(el.Name.ToLower().Trim() == "provider")
                    ProviderNames.Add(el.GetAttribute("id"), el.GetAttribute("name"));
            }
            
        }
        /// <summary>
        /// Названия групп - ключ id
        /// </summary>
        public static System.Collections.Specialized.NameValueCollection GroupNames;
        /// <summary>
        /// Заполнить массив неазваний провайдеров
        /// </summary>
        public static void Initialize_GroupNames()
        {
            XmlElement root = (XmlElement)config.DocumentElement.SelectSingleNode("groups") ;

            if (GroupNames == null)
                GroupNames = new NameValueCollection();
            else GroupNames.Clear();

            foreach (XmlElement el in root)
            {
                GroupNames.Add(el.GetAttribute("id"), el.GetAttribute("name"));
            }

        }
        public static string StartupPath;
        public static int UpdateState; // 0 - IDLING 1 - Updating
        public static string PrintString;
        public static XmlElement PROVIDER_XML;

        public static bool NeedToSendPays;
        public static bool NeedToUpdateConfiguration;
        public static bool NeedToRestart;
        public static bool NeedToReboot;
        public static bool NeedShutdown;

        public static System.Windows.Forms.Form CurrentForm;
        public static System.Windows.Forms.Form MainForm;
        public static main StartForm;
        // Купюрник
        public static Acceptors.Independed Bill;
        // Принтер
        public static Printers.Indepened Printer;
        // ККМ Прим-21К
        public static _prim21k.Prim21KNF prim21k;
        // Фискальный регистратор
        public static FR.FiscalRegisters FRegister;

        //WD
        public static WD.Independed Wd;
        public static int WDConnectCount;

        public static int CountForms;   // Всего форм
        public static int cFormIndex;  // текущая форма обработки
        public static int uscFormIndex;
        public static bool ServiceMenu;
        public static bool IsMonitorngSended;
        [DebuggerStepThrough]
        public static void AddToLog(Logs l, string message)
        {
            if (Ipaybox.Debug)
                System.Diagnostics.Debug.WriteLine(message);
            FileInfo fi = null;
            DirectoryInfo di = new DirectoryInfo(Ipaybox.StartupPath + "\\logs");
            if (!di.Exists)
                di.Create();
            switch (l)
            {
                case Logs.Main: fi = new FileInfo(Ipaybox.StartupPath + "\\logs\\"+DateTime.Now.ToString("yyyy-MM-dd")+".log");break;
                case Logs.Update: fi = new FileInfo(Ipaybox.StartupPath + "\\logs\\update.log"); break;
                default:
                    fi = new FileInfo(Ipaybox.StartupPath + "\\logs\\default.log"); break;
            }
            StreamWriter sw = null;
            try
            {
                if (fi.Exists)
                    sw = fi.AppendText();
                else
                    sw = fi.CreateText();

                string append = DateTime.Now.ToString("HH:mm:ss") + " " + message;

                sw.WriteLine(append);
               
            }
            catch
            { }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
            

        }
        /// <summary>
        /// Запустить Explorer.exe
        /// </summary>
        public static void StartExplorer()
        {

            System.Diagnostics.Process[] pr = System.Diagnostics.Process.GetProcessesByName("explorer");
            if (pr.Length == 0)
            {
                System.Diagnostics.Process.Start("explorer");
            }
        }
        /// <summary>
        /// Убрать Explorer.exe + Установить ветвь реестра AutoRestartShell + 0
        /// </summary>
        public static void HideExplorer()
        {
            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
            OurKey.SetValue("AutoRestartShell", 0);

            System.Diagnostics.Process[] pr = System.Diagnostics.Process.GetProcessesByName("explorer");
            for (int i = 0; i < pr.Length; i++)
            {
                pr[i].Kill();
            }
        }
        public static void SaveNetXML()
        {   
            string xml = "<ipaybox>";
            xml += "<net_opt>"+Ipaybox.NetOption+"</net_opt>";
            xml += "<net_name>"+Ipaybox.NetModemName+"</net_name>";
            xml +="</ipaybox>";

            NetOptions.LoadXml(xml);

            NetOptions.Save(Ipaybox.StartupPath + "\\config\\net.xml");
        
        }
        public static void LoadNetXML()
        { 
            XmlElement root = (XmlElement)Ipaybox.NetOptions.DocumentElement;

            for(int i=0;i<root.ChildNodes.Count;i++)
            {
                XmlElement el = (XmlElement)root.ChildNodes[i];

                switch (el.Name)
                {
                    case "net_opt":
                        try
                        {
                            Ipaybox.NetOption = int.Parse(el.InnerText);

                            if (Ipaybox.NetOption == 0)
                                Ipaybox.Internet = true;
                        }
                        catch
                        { Ipaybox.NetOption = 0; }
                        break;
                    case "net_name":
                        Ipaybox.NetModemName = el.InnerText;
                        break;
                }
                
            }
            
        }
        public static void LoadTerminalData()
        {
            // Устанавливаем параметры d 0
            Ipaybox.FlushToMain();

            Ipaybox.Terminal.bank = "";
            Ipaybox.Terminal.jur_inn = "";
            Ipaybox.Terminal.jur_name = "";
            Ipaybox.Terminal.jur_adress = "";
            Ipaybox.Terminal.support_phone = "";
            Ipaybox.Terminal.terminal_id = "";
            Ipaybox.Terminal.terminal_pass = "";
            Ipaybox.Terminal.terms_number = "";
            Ipaybox.Terminal.trm_adress = "";
            Ipaybox.Terminal.Interface = "";
            Ipaybox.Terminal.FiscalMode = false;
            XmlElement root = Ipaybox.terminal_info.DocumentElement;

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];
                switch (row.Name)
                {
                    case "terminal_id":
                        Ipaybox.Terminal.terminal_id = row.InnerText;
                        break;
                    case "interface":
                        Ipaybox.Terminal.Interface = row.InnerText;
                        break;
                    case "password":
                        Ipaybox.Terminal.terminal_pass = row.InnerText;
                        break;
                    case "agent_jur_name":
                        Ipaybox.Terminal.jur_name = row.InnerText;
                        break;
                    case "agent_inn":
                        Ipaybox.Terminal.jur_inn = row.InnerText;
                        break;
                     case "agent_adress":
                        Ipaybox.Terminal.jur_adress = row.InnerText;
                        break;
                    case "bank":
                        Ipaybox.Terminal.bank = row.InnerText;
                        break;
                    case "terms_number":
                        Ipaybox.Terminal.terms_number = row.InnerText;
                        break;
                    case "terminal_adress":
                        Ipaybox.Terminal.trm_adress = row.InnerText;
                        break;
                    case "pin":
                        Ipaybox.Terminal.pincode = row.InnerText;
                        break;
                    case "secret_number":
                        Ipaybox.Terminal.secret_number = row.InnerText;
                        break;
                    case "agent_support_phone":
                        Ipaybox.Terminal.support_phone = row.InnerText;
                        break;
                    case "configuration-id":
                        Ipaybox.Terminal.configuration_id = row.InnerText;
                        break;
                    case "fiscal-mode":
                        if(!bool.TryParse(row.InnerText, out Ipaybox.Terminal.FiscalMode))
                            Ipaybox.Terminal.FiscalMode = false;                        
                        break;
                }
            }

            try
            {
                Ipaybox.FRS.RemoteFR = false;
                Ipaybox.FRS.headertext = "";
                Ipaybox.FRS.checkWidth = 38;
                Ipaybox.FRS.RemoteFiscalRegisterURL = "";
                Ipaybox.FRS.remoteFRtimeout = 10000;

                XmlNode frsroot = Ipaybox.FRSSettings.DocumentElement.ChildNodes[0];

                for (int i = 0; i < frsroot.ChildNodes.Count; i++)
                {
                    XmlElement row = (XmlElement)frsroot.ChildNodes[i];
                    switch (row.Name)
                    {
                        case "remoteFR":
                            Ipaybox.FRS.RemoteFR = Convert.ToBoolean(row.InnerText);
                            break;
                        case "checkWidth":
                            Ipaybox.FRS.checkWidth = Convert.ToInt32(row.InnerText);
                            break;
                        case "remoteFRtimeout":
                            Ipaybox.FRS.remoteFRtimeout = Convert.ToInt32(row.InnerText);
                            break;
                        case "headertext":
                            Ipaybox.FRS.headertext = row.InnerText;
                            break;
                        case "remoteFRurl":
                            Ipaybox.FRS.RemoteFiscalRegisterURL = row.InnerText;
                            break;
                    }
                }
            }
            catch
            {
                Ipaybox.AddToLog(Logs.Main, "Не удалось применить настройки фискального сервера");
            }
        }

        // Сбросить провайдера
        public static void FlushToMain()
        {
           PRV_SELECTED_ID = null;
           PRV_SELECTED_GROUP.id = "";
           PROVIDER_XML = null;
           FORM_XML = null;
           CountForms = 0;
           cFormIndex = 0;

           curPay.IsAccountConfirmed = false;
           curPay.account = "";
           curPay.from_amount = 0;
           curPay.to_amount = 0;
           curPay.extra = "";
           curPay.IsMoney = false;
           curPay.IsRecieptPrinted = false;
           curPay.Options = "";
           curPay.prv_id = "";
           curPay.prv_img = "";
           curPay.prv_name = "";
           curPay.txn_id = "";
           curPay.ViewText = null;
        }
        public static void Print()
        {
            IsPrinting = true;

            if (!Ipaybox.WindowsPrinter)
            {
                // Печатаем
                if (Ipaybox.Printer != null && Ipaybox.Printer.port != null)
                    if (Ipaybox.Printer.Test())
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Печатаем чек");
                        Ipaybox.Printer.Print(PrintString);
                    }
                    else
                    { 
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Чек не напечатан. Ошибка");
                    }
                else
                {
                    if (Ipaybox.KKMPrim21K)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Печатаем чек");

                        if (Ipaybox.prim21k.Model != _prim21k.KKMModel.NULL)
                        {
                            int res = Ipaybox.prim21k.PrintPND(PrintString);

                            if (res == 0)
                            {
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Чек успешно распечатан");
                            }
                            else
                            {
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка при печати чека. " + Ipaybox.prim21k.LastAnswer);
                            }
                        }
                        else
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Чек не напечатан. Ошибка. Порт не определен.");
                            Ipaybox.Working = false;
                        }
                    }
                    else
                        if (Ipaybox.FiscalRegister)
                        {
                            if (Ipaybox.FRegister != null)
                            {
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Печатаем чек");
                                Ipaybox.FRegister.PrintText(PrintString);
                            }
                        }
                        else
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Чек не напечатан. Ошибка");
                            //new System.Threading.Thread(SendMonitoring).Start();
                        }
                }
            }
            else
            {
                try
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Печатаем чек через WINDOWS PRINTER");
                    doc.Print();
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось распечатать чек \r\n" + ex.Message);
                    //new System.Threading.Thread(SendMonitoring).Start();
                }
            }
            
            PrintString = "";
            IsPrinting = false;
            Ipaybox.IncassCheck = false;
        }
        public static void printDoc_PrintPage(Object sender, PrintPageEventArgs e)
        {
            try
            {
                if (Ipaybox.FRS.RemoteFR && !Ipaybox.IncassCheck)
                    PrintString = remoteFR.RemoteFiscalRegister.tryFormFicsalCheck(Ipaybox.Terminal.jur_name.Trim(), Ipaybox.FRS.headertext, PrintString, "Сотовая св.", Ipaybox.Terminal.terminal_id.Trim(), Ipaybox.Terminal.terminal_pass, Ipaybox.curPay.txn_id, Ipaybox.curPay.from_amount.ToString(), "1", Ipaybox.FRS.RemoteFiscalRegisterURL, Ipaybox.FRS.checkWidth, Ipaybox.FRS.remoteFRtimeout);
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Logs.Main, ex.Message);
            }

            if (Ipaybox.EpsonT400)
            {
                Font printFont = new Font("Courier New", 8);
                RectangleF rf = new RectangleF(0, 0, 220, 0);
                PrintString = PrintString.Replace("\r\n\r\n", "\r\n");
                PrintString = PrintString.Trim();
                e.Graphics.DrawString(PrintString, printFont, Brushes.Black, rf);
            }
            else
            {
                Font printFont = new Font("Courier New", 10);
                e.Graphics.DrawString(PrintString, printFont, Brushes.Black, 0, 0);
            }
        }
        public static void LoadIncass()
        {
            Incassation = new zeus.API.Incass(Ipaybox.StartupPath.TrimEnd('\\') + "\\config\\incass.xml");
            Incassation.AutoCommit = true;

            Ipaybox.Import = new zeus.API.Import();


            if (Ipaybox.XML_Statistic == null)
                Ipaybox.XML_Statistic = new XmlDocument();
            bool isstart = false;
            try {

                Ipaybox.XML_Statistic.Load(Ipaybox.StartupPath.TrimEnd('\\') + "\\config\\incass.xml");
                XmlElement root = (XmlElement)Ipaybox.XML_Statistic.DocumentElement ;

                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    XmlElement row = (XmlElement)root.ChildNodes[i];

                    switch (row.Name)
                    {
                        case "count_bill": Incass.count = int.Parse(row.InnerText); break;
                        case "count10": Incass.CountR10 = int.Parse(row.InnerText); break;
                        case "count50": Incass.CountR50 = int.Parse(row.InnerText); break;
                        case "count100": Incass.CountR100 = int.Parse(row.InnerText); break;
                        case "count500": Incass.CountR500 = int.Parse(row.InnerText); break;
                        case "count1000": Incass.CountR1000 = int.Parse(row.InnerText); break;
                        case "count5000": Incass.CountR5000 = int.Parse(row.InnerText); break;
                        case "count_check": Incass.countchecks = int.Parse(row.InnerText); break;
                        case "bytes_read": Incass.bytesRead= int.Parse(row.InnerText); break;
                        case "bytes_send": Incass.bytesSend = int.Parse(row.InnerText); break;
                        case "amount": Incass.incass_amount = int.Parse(row.InnerText); break;
                        case "incass-start-date":
                            isstart = true;
                            if (!DateTime.TryParse(row.InnerText,out Incass.incass_date_start))
                            {
                                Incass.incass_date_start = DateTime.Now;  
                               
                            }
                            break;

                        //case "count_bill": Incass.count = int.Parse(row.InnerText); break;
                    }
                }
            
            }
            catch { }
            if (!isstart)
            {
                Incass.incass_date_start = DateTime.Now;  
                FlushStatistic();
            }
        }
        [System.Diagnostics.DebuggerStepThrough]
        public static void FlushStatistic()
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml("<statistic></statistic>");

            // кол-во купюр
            XmlElement el = doc.CreateElement("count_bill");
            el.InnerText = Ipaybox.Incass.count.ToString();

            XmlNode root =  doc.DocumentElement;
            root.InsertAfter(el, root.FirstChild);

            // кол-во 10
            el = doc.CreateElement("count10");
            el.InnerText = Ipaybox.Incass.CountR10.ToString();
            root.InsertAfter(el, root.FirstChild);

            // кол-во 50
            el = doc.CreateElement("count50");
            el.InnerText = Ipaybox.Incass.CountR50.ToString();
            root.InsertAfter(el, root.FirstChild);

            // кол-во 100
            el = doc.CreateElement("count100");
            el.InnerText = Ipaybox.Incass.CountR100.ToString();
            root.InsertAfter(el, root.FirstChild);
            // кол-во 500
            el = doc.CreateElement("count500");
            el.InnerText = Ipaybox.Incass.CountR500.ToString();
            root.InsertAfter(el, root.FirstChild);
            // кол-во 1000
            el = doc.CreateElement("count1000");
            el.InnerText = Ipaybox.Incass.CountR1000.ToString();
            root.InsertAfter(el, root.FirstChild);

            // кол-во 5000
            el = doc.CreateElement("count5000");
            el.InnerText = Ipaybox.Incass.CountR5000.ToString();
            root.InsertAfter(el, root.FirstChild);

            // кол-во чеков
            el = doc.CreateElement("count_check");
            el.InnerText = Ipaybox.Incass.countchecks.ToString();
            root.InsertAfter(el, root.FirstChild);
            
            // количество принятых байт
            el = doc.CreateElement("bytes_read");
            el.InnerText = Ipaybox.Incass.bytesRead.ToString();
            root.InsertAfter(el, root.FirstChild);

            // количество переданных байт
            el = doc.CreateElement("bytes_send");
            el.InnerText = Ipaybox.Incass.bytesSend.ToString();
            root.InsertAfter(el, root.FirstChild);
            
            // сумма инкасации
            el = doc.CreateElement("amount");
            el.InnerText = Ipaybox.Incass.incass_amount.ToString();
            root.InsertAfter(el, root.FirstChild);
            // сумма инкасации
            el = doc.CreateElement("incass-start-date");
            el.InnerText = Ipaybox.Incass.incass_date_start.ToString();
            root.InsertAfter(el, root.FirstChild);
            //
            doc.Save(Ipaybox.StartupPath + @"\incass.xml");
        }
        public static void AddImage(zeus.HelperClass.zImage zimg, ref zPictureBox img, System.Windows.Forms.Form f)
        {
            string location = zimg.location;
            string limg = zimg.src;
            string size = zimg.size;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap bmp = new Bitmap(Ipaybox.StartupPath + @"\" + limg);
            bmp.MakeTransparent();
            img = new zPictureBox();
            img.Location = new Point(X, Y);
            img.Image = bmp;
            img.Tag = "";

            if (zimg.type == "solid" || zimg.type == "alert")
                img.BackColor = Color.White;
            else
                if (zimg.type == "error")
                {
                    img.Visible = false;
                    img.BackColor = Color.White;
                }
                else
                    img.BackColor = Color.Transparent;

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

        public static void AddPopup(PictureBox img, System.Windows.Forms.Form f)
        {
            Panel panel = new Panel();
            panel.Location = img.Location;
            panel.Size = img.Size;
            panel.BackgroundImage = img.Image;
            panel.BackColor = Color.Transparent;
            img.Name = "popup";
            f.Controls.Add(img);
            f.Controls["popup"].BringToFront();
        }

        public static void AddPanel(PictureBox img, System.Windows.Forms.Form f)
        {
            Panel panel = new Panel();
            panel.Location = img.Location;
            panel.Size = img.Size;
            panel.BackgroundImage = img.Image;
            panel.BackColor = Color.Transparent;
            panel.Name = "helpPanel";
            f.Controls.Add(panel);
            f.Controls["helpPanel"].BringToFront();
        }

        public static void AddImage(XmlElement el, ref PictureBox img, System.Windows.Forms.Form f)
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

            f.Controls.Add(img);
        }

        /// <summary>
        /// Добавление надписи на форму. ver. 2.0
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="label"></param>
        /// <param name="f"></param>
        public static void AddLabel(zeus.HelperClass.zLabel lbl, ref Label label, System.Windows.Forms.Form f)
        {
            string location = lbl.location;
            string text = lbl.text;
            string size = lbl.size;
            string font = lbl.fontFamily;
            string style = lbl.fontStyle;
            string font_size = lbl.fontSize;
            string color = lbl.color;
            string textalign = lbl.textAlign;
            string textTransform = lbl.textTransform;

            if (lbl.value != null && lbl.value != string.Empty)
                text = lbl.value;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            label = new Label();
            //labels[label_count].Parent = img[1];
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
                FontStyle style1 = FontStyle.Regular;

                if (!string.IsNullOrEmpty(style))
                {
                    style1 = (FontStyle)Enum.Parse(typeof(FontStyle), style, true);
                }

                label.Font = new Font(font, float.Parse(font_size), style1);
            }

            if (!string.IsNullOrEmpty(textalign))
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
            f.Controls.Add(label);
        }

        public static void AddLabel(XmlElement el, ref Label label, System.Windows.Forms.Form f)
        {
            string location = el.GetAttribute("location");
            string text = el.GetAttribute("text");
            string size = el.GetAttribute("size");
            string font = el.GetAttribute("font");
            string style = el.GetAttribute("style");
            string font_size = el.GetAttribute("font-size");
            string color = el.GetAttribute("color");
            string textalign = el.GetAttribute("textalign");

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            label  = new Label();
            //labels[label_count].Parent = img[1];
            label.Text = text;
            label.BackColor = Color.Transparent;
            label.Location = new Point(X, Y);

            if (size.Length != 0)
                label.Size = new Size(int.Parse(size.Split(';')[0]), int.Parse(size.Split(';')[1]));
            else
                label.AutoSize = true;
            // Цвет
            int red =0;
            int green =0;
            int blue =0;

            if(color.Length > 5)
            {
                red     = int.Parse(color.Split(';')[0]);
                green   = int.Parse(color.Split(';')[1]);
                blue    = int.Parse(color.Split(';')[2]);

                label.ForeColor = Color.FromArgb(red, green, blue);
            }
            if (font.Length != 0 && font_size.Length != 0)
            { 
                FontStyle style1 = FontStyle.Regular;

                if (!string.IsNullOrEmpty(style))
                { 
                    style1= (FontStyle)Enum.Parse(typeof( FontStyle),style,true);
                }

                label.Font = new Font(font, float.Parse(font_size),style1);
            }

            if (!string.IsNullOrEmpty(textalign))
            {
                var con = (ContentAlignment)Enum.Parse(typeof(ContentAlignment), textalign, true);
                label.TextAlign = con;
            }

            f.Controls.Add(label);
         }

        /// <summary>
        /// Добавление флеш-баннера на форму
        /// </summary>
        /// <param name="flash"></param>
        /// <param name="bannerList"></param>
        /// <param name="f"></param>
        /// <param name="tar"></param>
        public static void AddBanner(zeus.HelperClass.zBanner flash, ref AxShockwaveFlashObjects.AxShockwaveFlash banner, ref System.Windows.Forms.Form f, PreviewKeyDownEventHandler tar)
        {
            string location = flash.location;
            string size = flash.size;
            string src = flash.src;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);
            int width = int.Parse(size.Split(';')[0]);
            int height = int.Parse(size.Split(';')[1]);

            AxShockwaveFlashObjects.AxShockwaveFlash fl = new AxShockwaveFlashObjects.AxShockwaveFlash();
            fl.Stop();
            fl.Size = new Size(width, height);
            fl.Location = new Point(X, Y);
            fl.Movie = Ipaybox.StartupPath + @"\" + src;

            fl.PreviewKeyDown += tar;
            f.Controls.Add(fl);
        }

        /// <summary>
        /// Добавление кнопки на форму главного меню (внутри группы) ver. 2.0
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="img"></param>
        /// <param name="f"></param>
        /// <param name="tar"></param>
        public static void AddButton(zeus.HelperClass.zButton btn, ref PictureBox img, System.Windows.Forms.Form f, EventHandler tar)
        {
            string location = btn.location;
            string limg = btn.img;
            string tag = btn.value;

            int X = int.Parse(location.Split(';')[0]);
            int Y = int.Parse(location.Split(';')[1]);

            Bitmap tmp = Ipaybox.Pics[int.Parse(Ipaybox.images[limg])];
            img = new PictureBox();
            img.Location = new Point(X, Y);
            img.Image = tmp;
            img.Size = tmp.Size;
            img.BackColor = Color.Transparent;
            img.Tag = tag;
            // НЕРАБОТАЕТ
            img.Click += tar;
            f.Controls.Add(img);
        }
        public static string GetImageFromInterface_Prv(string id)
        {
            try
            {
                zeus.HelperClass.zMenuItemButton prvButton = Ipaybox.ifc.prvImages.Find(p => p.id == Convert.ToInt32(id));

                if (prvButton != null)
                    return prvButton.img;
            }
            catch { }

            return "";
        }
        public static string GetImageFromInterface_Group(string id)
        {
            try
            {
                zeus.HelperClass.zMenuItemButton prvButton = Ipaybox.ifc.groupImages.Find(p => p.id == Convert.ToInt32(id));

                if (prvButton != null)
                    return prvButton.img;
            }
            catch { }

            return "";
        }
        public static void ReInitPrinter()
        {
            if (Ipaybox.Printer.port != null)
            {
                Ipaybox.Printer.Close();
                Ipaybox.Printer = new Printers.Indepened();
            }
            else 
            { 
                Ipaybox.Printer = new Printers.Indepened();

            }
        }
        // закодировать
        public static string getMd5Hash(string input)
        {
            if (input == null)
                return "";
            if (input.Length == 0)
                return "";
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        // Проверить подпись
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class zPictureBox : PictureBox
    {
        public string key = "";
    }
#region _____SOUND
    public static class Sound
    {
        private static SoundPlayer player;
        public static void Initialize()
        {
            player = new SoundPlayer();
        }

        public static void Play(string location)
        {
            try
            {
                player.Stop();
            }
            catch
            {}

            try
            {
                player.SoundLocation = location;
                player.Play();
            }
            catch
            { }
        }
    }
#endregion

}
