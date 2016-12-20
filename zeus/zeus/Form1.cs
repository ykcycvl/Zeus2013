using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Net.Security;
using System.Threading;
using Printers;
using System.IO;
using System.Xml.Serialization;
using WD;
using System.Runtime.InteropServices;
using KVPair = System.Collections.Generic.KeyValuePair<int, int>;
using zeus.HelperClass;


namespace zeus
{
    public partial class main : Form
    {
       //PictureBox[] pics ;

       // int pb_count = 0;
        flushform flush = new flushform();
        static object locker = new object();
        static object modem = new object();
        static object autozreport = new object();
        
        static int counttreads;
        static int countmodem;
        static int countmonitoring;
        //static bool IsMonitorngSended;
        public main()
        {
            InitializeComponent();
        }
        private void CheckDirExist()
        {
            DirectoryInfo dt = new DirectoryInfo(Application.StartupPath+"\\config");

            if (!dt.Exists)
                dt.Create();

            dt = new DirectoryInfo(Application.StartupPath + "\\img");
            if (!dt.Exists)
                dt.Create();
            dt = new DirectoryInfo(Application.StartupPath + "\\img\\digit");
            if (!dt.Exists)
                dt.Create();

            dt = new DirectoryInfo(Application.StartupPath + "\\img15");
            if (!dt.Exists)
                dt.Create();
            dt = new DirectoryInfo(Application.StartupPath + "\\sounds");
            if (!dt.Exists)
                dt.Create();
            dt = new DirectoryInfo(Application.StartupPath + "\\img15\\digit");
            if (!dt.Exists)
                dt.Create();
            dt = new DirectoryInfo(Application.StartupPath + "\\osmp17");
            if (!dt.Exists)
                dt.Create();

            dt = new DirectoryInfo(Application.StartupPath + "\\osmp17\\digit");
            if (!dt.Exists)
                dt.Create();
        }
        private void InitializeIpaybox()
        {
          /*  Ipaybox.GW_CHILD = 5;
            Ipaybox.SW_HIDE = 0;
            Ipaybox.SW_SHOW = 5;*/
            //CheckDirExist();
            FileInfo fi = new FileInfo(Ipaybox.StartupPath + @"\debug");
            Ipaybox.Debug = fi.Exists;

            Sound.Initialize();

            if (flush != null)
            {
                ((Label)flush.Controls["label1"]).Text = "Инициализация оборудования...";
                Application.DoEvents();
            }
            
            //this.TopMost = false;

            Ipaybox.Working = true;
            //
            Ipaybox.CountForms = 0;
            Ipaybox.cFormIndex = 0;
            
            Ipaybox.StartForm = this;

            //this.Visible = false;

            Ipaybox.providers = new XmlDocument();
            Ipaybox.config = new XmlDocument();
            Ipaybox.comiss = new XmlDocument();
            Ipaybox.pays = new XmlDocument();
            Ipaybox.NetOptions = new XmlDocument();
            Ipaybox.terminal_info  = new XmlDocument();
            Ipaybox.TPIN = new XmlDocument();
            Ipaybox.FRSSettings = new XmlDocument();
            Ipaybox.XML_monitoring = new XmlDocument();
            Ipaybox.XML_SendFile = new XmlDocument();
            Ipaybox.images = new System.Collections.Specialized.NameValueCollection();

            Ipaybox.Option = new zeus.HelperClass.Settings();
            Ipaybox.Option.SaveOnChanges = true;
            // 
            string[] ComPorts = System.IO.Ports.SerialPort.GetPortNames();
            string descr = "";

            foreach (string s in ComPorts)
                descr += s + " ";

            try
            {
                try
                {
                    if (flush != null)
                    {
                        ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Поиск принтера...";
                        Application.DoEvents();
                    }

                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Доступные COM порты:" + descr);

                    if (!Ipaybox.AtolDriver)
                        if (!Ipaybox.WindowsPrinter)
                        {
                            if (Ipaybox.KKMPrim21K)
                            {
                                Ipaybox.prim21k = new _prim21k.Prim21KNF();

                                if (Ipaybox.prim21k.Initialize(ComPorts))
                                {
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, Ipaybox.prim21k.Model.ToString() + " подключен к порту " + Ipaybox.prim21k.ComPort);

                                    if (flush != null)
                                    {
                                        ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Найден ККМ Прим21-К. Порт " + Ipaybox.prim21k.ComPort;
                                        Application.DoEvents();
                                    }
                                }
                                else
                                {
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Принтер не обнаружен");

                                    if (flush != null)
                                    {
                                        ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Принтер не обнаружен";
                                        ((Label)flush.Controls["PrinterInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                                        Application.DoEvents();
                                    }
                                }
                            }
                            else
                                if (Ipaybox.Printer == null)
                                {
                                    Ipaybox.Printer = new Printers.Indepened();
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Принтер " + Ipaybox.Printer.PrnModel + ", подключен к порту " + Ipaybox.Printer.port);

                                    if (Ipaybox.Printer.PrnModel == Model.NULL)
                                    {
                                        if (flush != null)
                                        {
                                            ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Принтер не найден";
                                            ((Label)flush.Controls["PrinterInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                                        }
                                    }
                                    else
                                        if (flush != null)
                                        {
                                            ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Найден принтер " + Ipaybox.Printer.PrnModel + ". Порт " + Ipaybox.Printer.port;
                                        }

                                    Application.DoEvents();
                                }
                        }
                        else
                        {
                            Ipaybox.doc = new System.Drawing.Printing.PrintDocument();
                            Ipaybox.doc.PrintController = new System.Drawing.Printing.StandardPrintController();
                            Ipaybox.doc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Ipaybox.printDoc_PrintPage);
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Принтер WINDOWS");

                            if (flush != null)
                            {
                                ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Принтер WINDOWS";
                                Application.DoEvents();
                            }
                        }
                    else
                    {
                        try
                        {
                            Ipaybox.FiscalRegister = true;
                            Ipaybox.FRegister = new FR.Atol();

                            if (Ipaybox.Option["fiscal-mode"] == null)
                                Ipaybox.Option["fiscal-mode"] = "false";

                            Ipaybox.FRegister.FiscalMode = bool.Parse(Ipaybox.Option["fiscal-mode"]);

                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "ККМ " + Ipaybox.FRegister.Model + " найдена на порту " + Ipaybox.FRegister.ComPort);

                            if (flush != null)
                            {
                                ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Найден ККМ " + Ipaybox.FRegister.Model + ". Порт " + Ipaybox.FRegister.ComPort;
                                Application.DoEvents();
                            }
                        }
                        catch (Exception ex)
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Невозможно инициализировать драйвер АТОЛ.\r\n" + ex.Message);

                            if (flush != null)
                            {
                                ((Label)flush.Controls["PrinterInfoLabel"]).Text = "Невозможно инициализировать драйвер АТОЛ";
                                ((Label)flush.Controls["PrinterInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                                Application.DoEvents();
                            }

                            if (!Ipaybox.Debug)
                                Ipaybox.Working = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }

                if (flush != null)
                {
                    ((Label)flush.Controls["AcceptorInfoLabel"]).Text = "Поиск купюроприемника...";
                    Application.DoEvents();
                }

                if (Ipaybox.Bill == null)
                {
                    Ipaybox.Bill = new Acceptors.Independed();

                    if (Ipaybox.Bill.model == Acceptors.Model.NULL && !Ipaybox.Debug)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "\tНе найден купюрник. Не работаем.");
                        Ipaybox.Working = false;

                        if (flush != null)
                        {
                            ((Label)flush.Controls["AcceptorInfoLabel"]).Text = "Не найден купюроприемник";
                            ((Label)flush.Controls["AcceptorInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                            Application.DoEvents();
                        }
                    }
                    else
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Купюроприемник " + Ipaybox.Bill.model + ", подключен к порту ");

                        if (flush != null)
                        {
                            ((Label)flush.Controls["AcceptorInfoLabel"]).Text = "Купюроприемник " + Ipaybox.Bill.model + ", подключен к порту";
                            Application.DoEvents();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            try
            {
                if (Ipaybox.Wd == null)
                {
                    Ipaybox.Wd = new WD.Independed(ComPorts);

                    if (Ipaybox.Wd.watch != Independed.WD.NULL)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Найден WatchDog " + Ipaybox.Wd.watch.ToString() + " " + Ipaybox.Wd.Version + ", подключен к порту ");

                        if (flush != null)
                        {
                            ((Label)flush.Controls["WDInfoLabel"]).Text = "Найден WatchDog " + Ipaybox.Wd.watch.ToString() + " " + Ipaybox.Wd.Version;
                            Application.DoEvents();
                        }
                    }
                    else
                    {
                        if (flush != null)
                        {
                            ((Label)flush.Controls["WDInfoLabel"]).Text = "WatchDog не найден";
                            ((Label)flush.Controls["WDInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                            Application.DoEvents();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            // Загрузить данные инкасации
            Ipaybox.LoadIncass();

            if (flush != null)
            {
                ((Label)flush.Controls["label1"]).Text = "Загрузка конфигурационных файлов...";
                Application.DoEvents();
            }

            try
            { 
                try
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка информации о терминале.");
                    Ipaybox.terminal_info.Load(Ipaybox.StartupPath + @"\config\terminal.xml");

                    if (File.Exists(Ipaybox.StartupPath + @"\config\frssettings.xml"))
                        Ipaybox.FRSSettings.Load(Ipaybox.StartupPath + @"\config\frssettings.xml");

                    if (File.Exists(Ipaybox.StartupPath + @"\config\tpin.xml"))
                    {
                        try
                        {
                            Ipaybox.TPIN.Load(Ipaybox.StartupPath + @"\config\tpin.xml");

                            XmlElement root = (XmlElement)Ipaybox.TPIN.DocumentElement;
                            XmlElement el = (XmlElement)root.SelectSingleNode("pins");

                            if (el.ChildNodes.Count > 0)
                                Ipaybox.MasterPIN_IsActive = false;
                            else
                                Ipaybox.MasterPIN_IsActive = true;
                        }
                        catch
                        {
                            Ipaybox.MasterPIN_IsActive = true;
                        }
                    }

                    Ipaybox.LoadTerminalData();

                    if (Ipaybox.Terminal.jur_name == "")
                        Ipaybox.NeedUpdates.Trm_info = true;

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text = "Информация о терминале загружена";
                        Application.DoEvents();
                    }
                }
                catch
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Неуспешно...");
                    Form c = new options();
                    c.Show();
                    Ipaybox.Working = false;
                    Ipaybox.ServiceMenu = true;
                    Ipaybox.Internet = true;
                    Ipaybox.NetOption = 0;
                }
                try
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка информации о провайдерах.");
                    Ipaybox.providers.Load(Ipaybox.StartupPath + @"\config\providers.xml");
                    //SortNodes(Ipaybox.providers.DocumentElement.SelectSingleNode("providers"), "sortorder");
                    Ipaybox.Initialize_ProviderNames();

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text += "\r\nИнформация о провайдерах загружена";
                        Application.DoEvents();
                    }
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не успешно. Необходима загрузка данных с сервера.");

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text = "Ошибка! Информация о провайдерах не загружена!";
                        ((Label)flush.Controls["OtherInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                        Application.DoEvents();
                    }

                    Ipaybox.NeedToUpdateConfiguration = true;
                    Ipaybox.NeedUpdates.ProviderList = true;
                    Ipaybox.Working = false;
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }
                try
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка информации о провайдерах на главной странице.");
                    Ipaybox.config.Load(Ipaybox.StartupPath + @"\config\config.xml");
                    Ipaybox.Initialize_GroupNames();

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text += "\r\nТоп провайдеров загружен";
                        Application.DoEvents();
                    }
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не успешно. Необходима загрузка данных с сервера.");

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text = "Ошибка! Топ провайдеров не загружен!";
                        ((Label)flush.Controls["OtherInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                        Application.DoEvents();
                    }

                    Ipaybox.NeedToUpdateConfiguration = true;
                    Ipaybox.NeedUpdates.Trm_info = true;
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }
                
                try
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка информации о номерной ёмкости.");
                    //Ipaybox.XML_PhoneRange = new XmlDocument();
                    string[] src = File.ReadAllLines(Ipaybox.StartupPath + @"\config\phonerange.txt");

                    for (int i = 0; i < src.Length; i++)
                    {
                        string[] s = src[i].Split(',');
                        Ipaybox.PhoneRange r = new Ipaybox.PhoneRange();
                        r.from = ulong.Parse(s[0]);
                        r.to = ulong.Parse(s[1]);
                        r.prv = s[2];
                        Ipaybox.PhoneRanges.Add(r);
                    }

                    //Ipaybox.XML_PhoneRange.Load(Ipaybox.StartupPath + @"\config\phonerange.xml");

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text += "\r\nНомерные емкости загружены";
                        Application.DoEvents();
                    }
                }
                catch(Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не успешно. Необходима загрузка данных с сервера.");

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text = "Ошибка! Номерные емкости не загружены!";
                        ((Label)flush.Controls["OtherInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                        Application.DoEvents();
                    }

                    Ipaybox.NeedToUpdateConfiguration = true;
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }

                try
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка информации о комиссии.");
                    Ipaybox.comiss.Load(Ipaybox.StartupPath + @"\config\comiss.xml");

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text += "\r\nИнформация о комиссии загружена";
                        Application.DoEvents();
                    }
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не успешно. Необходима загрузка данных с сервера.");

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text = "Ошибка! Комиссии не загружены!";
                        ((Label)flush.Controls["OtherInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                        Application.DoEvents();
                    }

                    Ipaybox.NeedToUpdateConfiguration = true;
                    Ipaybox.Working = false; 
                    Ipaybox.NeedUpdates.Comission = true;
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }
                try
                {
                    Ipaybox.NetOptions.Load(Ipaybox.StartupPath + @"\config\net.xml");
                    Ipaybox.LoadNetXML();
                }
                catch (Exception ex)
                {
                    Ipaybox.NetOption = 0;
                    Ipaybox.NetModemName = "";
                    Ipaybox.SaveNetXML();
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }
                try
                {
                    Ipaybox.XML_SendFile.Load(Ipaybox.StartupPath + @"\file.xml");
                }
                catch
                {
                    Ipaybox.XML_SendFile.LoadXml("<files></files>");
                }

                try
                {
                    /* Загрузка интерфейса. Версия 2.0 */
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка информации об интерфейсе.");
                    string interfaceXML = File.ReadAllText(Ipaybox.StartupPath + @"\config\interface.min.xml");
                    var xmlSerializer = new XmlSerializer(typeof(interfaceList));
                    var stringReader = new StringReader(interfaceXML);
                    Ipaybox.ifcList = (interfaceList)xmlSerializer.Deserialize(stringReader);

                    for (int i = 0; i < Ipaybox.ifcList.zeusinterface.Count; i++)
                        if (Ipaybox.ifcList.zeusinterface[i].id.ToString() == Ipaybox.Terminal.Interface)
                        {
                            Ipaybox.ifc = Ipaybox.ifcList.zeusinterface[i];
                            break;
                        }

                    Ipaybox.Terminal.InterfaceVersion = Ipaybox.ifcList.ifc.version;
                    Version MinRequiredCoreVersion1 = new Version(Ipaybox.ifcList.ifc.coreversion);

                    if (MinRequiredCoreVersion1 > Assembly.GetExecutingAssembly().GetName().Version)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Версия ПО ниже, чем требуется для корректной работы интерфейса. Необходимо обновление ПО.");
                        Ipaybox.NeedUpdates.Core = true;
                    }

                    LoadProviderImages();

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text += "\r\nОписание интерфейса загружено";
                        Application.DoEvents();
                    }
                }
                catch(Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не успешно. Необходимо обновить интерфейс.");

                    if (flush != null)
                    {
                        ((Label)flush.Controls["OtherInfoLabel"]).Text = "Ошибка! Информация об интерфейсе не загружена!";
                        ((Label)flush.Controls["OtherInfoLabel"]).ForeColor = System.Drawing.Color.Red;
                        Application.DoEvents();
                    }

                    Ipaybox.NeedToUpdateConfiguration = true;
                    Ipaybox.Working = false;
                    Ipaybox.NeedUpdates.Interface = true;
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }

                if (flush != null)
                {
                    ((Label)flush.Controls["label1"]).Text = "Синхронизация времени...";
                    Application.DoEvents();
                }

                HelperClass.SyncTime sync = new zeus.HelperClass.SyncTime(Ipaybox.ServiceUrl);
                sync.TrySyncTime(1);              

                //core.UpdateTerminalInfo(true);
                // Загрузка платежей
                try
                {
                    Ipaybox.pays.Load(Ipaybox.StartupPath + @"\import.xml");
                }
                catch
                {
                    Ipaybox.pays.LoadXml("<pays></pays>");
                }

                // Инициализация мониторинга.
                Ipaybox.XML_monitoring.LoadXml("<monitoring></monitoring>");

                // Если есть платежи на отправку
                if (Ipaybox.pays.DocumentElement.ChildNodes.Count > 0)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "В файле import.xml содержатся непроведенные платежи " + Ipaybox.pays.DocumentElement.ChildNodes.Count.ToString() + " шт.");
                    Ipaybox.NeedToSendPays = true;
                }

                if(!Ipaybox.NeedToUpdateConfiguration)
                    Ipaybox.NeedToUpdateConfiguration = false;

                if(!Ipaybox.NeedUpdates.Trm_info && !Ipaybox.NeedUpdates.ProviderList && !Ipaybox.NeedUpdates.Comission && !Ipaybox.NeedUpdates.Core && !Ipaybox.NeedUpdates.Config)
                    Ipaybox.NeedToUpdateConfiguration = false;
            }
            catch (Exception ex)
            {
                Ipaybox.Working = false;
                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            Ipaybox.Modem = new modem();
        }

        private void LoadProviderImages()
        {
            Bitmap [] bmp = new Bitmap[400];
            int count = 0;

            // Загрузка изображений провайдеров (кнопок провайдеров)
            for (int i = 0; i < Ipaybox.ifc.prvImages.Count; i++)
            {
                bmp[count] = new Bitmap(Ipaybox.StartupPath + @"\" + Ipaybox.ifc.prvImages[i].img);
                Ipaybox.images.Add(Ipaybox.ifc.prvImages[i].img, count.ToString());
                count++;
            }

            for (int i = 0; i < Ipaybox.ifc.groupImages.Count; i++)
            {
                if (!string.IsNullOrEmpty(Ipaybox.ifc.groupImages[i].img))
                {
                    if (Ipaybox.images[Ipaybox.ifc.groupImages[i].img] == null)
                    {
                        try
                        {
                            bmp[count] = new Bitmap(Ipaybox.StartupPath + @"\" + Ipaybox.ifc.groupImages[i].img);
                            Ipaybox.images.Add(Ipaybox.ifc.groupImages[i].img, count.ToString());
                            count++;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            var xmlSerializer = new XmlSerializer(typeof(zInterface));
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, Ipaybox.ifc, new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty) }));
            string xml = stringWriter.ToString();

            MatchCollection mc = Regex.Matches(xml, "<button .*?>");
            xml = "<buttons>";

            foreach (Match m in mc)
            {
                xml += "\r\n" + m.Groups[0].ToString();
            }

            xml += "</buttons>";

            XmlDocument buttonXml = new XmlDocument();
            buttonXml.InnerXml = xml;
            XmlNodeList buttons = buttonXml.SelectNodes("/*/button");

            foreach (XmlElement el in buttons)
            {
                string name = el.GetAttribute("img");
                if (!string.IsNullOrEmpty(name))
                {
                    if (Ipaybox.images[name] == null)
                    {
                        try
                        {
                            bmp[count] = new Bitmap(Ipaybox.StartupPath + @"\" + name);
                            Ipaybox.images.Add(name, count.ToString());
                            count++;
                        }
                        catch
                        {
                        }
                    }
                    else { }
                }
            }

            if (count > 0)
            { 
                Ipaybox.Pics = new Bitmap[count];

                for (int i = 0; i < count; i++)
                {
                    Ipaybox.Pics[i] = bmp[i];
                }
            }
        }
        private void CheckExistProcess()
        {
            try
            {
                System.Diagnostics.Process[] n = System.Diagnostics.Process.GetProcessesByName("zeus");

                for (int i = 0; i < n.Length -1; i++)
                {
                    n[i].Kill();
                    n[i].WaitForExit();
                }
            }
            catch
            {}
        }
        private void LoadUrl()
        {
            Ipaybox.UpdateUrl = new string[]
            {       
                "http://update1.ipaybox.ru/",
                "http://update2.ipaybox.ru/"
            };

            Ipaybox.ServiceUrl = new string[]
            {
                "https://xml2.ipaybox.ru/xml/",
                "https://xml1.ipaybox.ru/xml/",
                "http://xml2.ipaybox.ru/xml/",
                "http://xml1.ipaybox.ru/xml/"
            };
        }
        public static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch
            {
                //ActivityLog.InsertSyncActivity(ex);
            }
        }

        public static void Remove(string file1, string file2)
        {
            bool removed = false;
            byte trycount = 0;

            if (File.Exists(file1))
            {
                while (!removed && trycount < 3)
                {
                    trycount++;

                    try
                    {
                        //Установка аттрибутов файла если файл существует
                        if (File.Exists(file1))
                        {
                            try
                            {
                                File.SetAttributes(file1, FileAttributes.Normal);
                            }
                            catch
                            {
                                Console.WriteLine("Cannot set file attributes for source file!");
                            }
                        }

                        if (File.Exists(file2))
                        {
                            try
                            {
                                File.SetAttributes(file2, FileAttributes.Normal);
                            }
                            catch
                            {
                                Console.WriteLine("Cannot set file attributes for destination file!");
                            }
                        }

                        Console.Write("Copying " + file1 + " with " + file2 + " ...");
                        File.Copy(file1, file2, true);
                        System.Threading.Thread.Sleep(3000);
                        Console.WriteLine("OK");
                        Console.Write("Deleting " + file1 + " ...");
                        File.Delete(file1);
                        System.Threading.Thread.Sleep(3000);
                        Console.WriteLine("OK");
                        removed = true;
                    }
                    catch (Exception ex) { Console.WriteLine("Error! " + ex.Message); removed = false; System.Threading.Thread.Sleep(3000); }
                }
            }
        }

        private void main_Load(object sender, EventArgs e)
        {
            Ipaybox.StartupPath = Application.StartupPath;

            FileInfo updaterFI = new FileInfo(Ipaybox.StartupPath + "\\updater.exe.update");

            if (updaterFI.Exists)
                try
                {
                    Remove(Ipaybox.StartupPath + "\\updater.exe.update", Ipaybox.StartupPath + "\\updater.exe");
                }
                catch { }

            try
            {
                flush.Show();
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            try
            {                
                InitiateSSLTrust();
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            string[] args = Environment.CommandLine.Split(' ');

            if (args.Length > 0)
            {
                foreach (string row in args)
                {
                    if (row.ToLower() == "prtwin" || row.ToLower() == "prnwin")
                    {
                        Ipaybox.WindowsPrinter = true;
                        Ipaybox.EpsonT400 = false;
                        Ipaybox.Args = "prtwin";
                    }
                    if (row.ToLower() == "atol" || row.ToLower() == "atol")
                    {
                        Ipaybox.AtolDriver = true;
                        Ipaybox.Args = "atol";
                    }
                    if (row.ToLower() == "prim21k")
                    {
                        Ipaybox.KKMPrim21K = true;
                        Ipaybox.Args = "prim21k";
                    }
                    if (row.ToLower() == "t400" || row.ToLower() == "t400")
                    {
                        Ipaybox.WindowsPrinter = true;
                        Ipaybox.EpsonT400 = true;
                        Ipaybox.Args = "t400";
                    }
                }
            }
          //  Ipaybox.Args = "atol";
          //  Ipaybox.AtolDriver = true;
            // СБрасываем индексы URLов
            Ipaybox.UpdateUrlIndex = 0;
            Ipaybox.ServiceUrlIndex = 0;
            
            if (Screen.PrimaryScreen.Bounds.Width == 1280 && Screen.PrimaryScreen.Bounds.Height == 1024)
            {
                Ipaybox.Resolution = new Size(1280, 1024);
                Ipaybox.Inches = 17;
            }
            else
            {
                //if (Screen.PrimaryScreen.Bounds.Width == 1024 && Screen.PrimaryScreen.Bounds.Height == 768)
                //{
                    Ipaybox.Resolution = new Size(1024, 768);
                    Ipaybox.Inches = 15;
               // }
            }

            try
            {
                FileInfo fi = new FileInfo(Ipaybox.StartupPath + @"\debug");
                Ipaybox.Debug = fi.Exists;
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }
            
            try
            {
                _cursor.Hide();

                if (!Ipaybox.Debug)
                    Ipaybox.HideExplorer();

            }
            catch { }
            //Ipaybox.Resolution = new Size(1024, 768);
            //Ipaybox.Inches = 15;

            try
            {
                LoadUrl();
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            try
            {
                CheckDirExist();
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            try
            {
                Ipaybox.RequestTimeout = float.Parse("0,5");
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            // Убиваем все процессы Зевса
            try
            {
                CheckExistProcess();
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }
            // Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            try
            {
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }
            
            //Ipaybox.Debug = true;
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Старт приложения. Инициализация.");
            Ipaybox.CoreVersion = Application.ProductVersion;
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Версия ПО:" + Ipaybox.CoreVersion);

            pays_send_timer.Enabled = false;
            conf_update.Enabled = false;
            link_timer.Enabled = false;
            print_timer.Enabled = false;

            // Инициализируем приложение
            try
            {
                InitializeIpaybox();
            }
            catch
            {
                Ipaybox.Working = false;
                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.NeedUpdates.Core = true;
            }

            pays_send_timer.Enabled = true;
            conf_update.Enabled = true;
            link_timer.Enabled = true;
            print_timer.Enabled = true;

            this.BackColor =  Color.FromArgb(230, 230, 230);

            if (flush != null)
            {
                ((Label)flush.Controls["label1"]).Text = "Проверка модема...";
                Application.DoEvents();
            }

            if (Ipaybox.NetOption == 1)
            {
                try
                {
                    _Modem.Init();
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }
            }

            if (Ipaybox.IsBlocked)
                Ipaybox.Working = false;

            try
            {
                ((Label)flush.Controls["label1"]).Text = "Старт приложения...";
                Application.DoEvents();
                flush.Dispose();
                flush = null;
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
            }

            // Запускаем процесс
            Main_Process();
        }

        public void Main_Process()
        { 
            // Главный процесс вызывается из всех форм
            if(!Ipaybox.ServiceMenu)
                if (!string.IsNullOrEmpty(Ipaybox.PRV_SELECTED_ID))
                {
                    // выбран провайдер, обработать его
                    StartProcessProvider(Ipaybox.PROVIDER_XML);
                    //StartForm(Ipaybox.Forms.Main);
                }
                else
                    StartForm(Ipaybox.Forms.Main);
        }

        private void StartProcessUserForm(zFormLink userForm)
        {
            Ipaybox.current_User_Form = userForm;
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Запуск формы " + userForm.name + "\r\n");

            switch (userForm.type.ToLower())
            { 
                case "info":
                    StartForm(Ipaybox.Forms.Info);
                    break;
                case "edit":
                    StartForm(Ipaybox.Forms.UserForm);
                    break;
            }
        }
        private void StartProcessForm(int index)
        {
            int curForm = 0;
            XmlElement root = Ipaybox.PROVIDER_XML;
          
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];
                if (row.Name == "form")
                {
                    curForm++;
                    // Если текущая форма == та которую нужно обработать, то
                    if (index == curForm)
                    {
                        string type = row.GetAttribute("type");

                        switch (type.ToLower())
                        { 
                            case "info":
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Запуск формы info\r\n"+row.InnerXml+"\r\n");
                                Ipaybox.FORM_XML = row;
                                StartForm(Ipaybox.Forms.Info);
                                break;
                            case "edit":
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Запуск формы edit");
                                Ipaybox.FORM_XML = row;
                                StartForm(Ipaybox.Forms.Account);
                                break;
                        }   
                    }
                }
            }
        }
        private void StartProcessProvider(XmlElement prv)
        {
            // Если ещё не одна форма не обрабатывалась надо
            // получить кол-во форм для обработки и запускать процесс обработки каждой формы.
            if (Ipaybox.CountForms == 0 && Ipaybox.cFormIndex == 0)
            {
                XmlElement root = prv;

                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    XmlElement row = (XmlElement)root.ChildNodes[i];
                    if (row.Name == "form")
                    {
                        Ipaybox.CountForms++;
                    }
                }
            }

            if (Ipaybox.cFormIndex < Ipaybox.CountForms)
            {
                Ipaybox.cFormIndex++;
                StartProcessForm(Ipaybox.cFormIndex);
            }
            else
            { 
                // закончились формы для обработки, нужно проверить номер внести деньги и выдать чек
                if (!Ipaybox.curPay.IsAccountConfirmed)
                { 
                    // Подтверждены ли данные
                    StartForm(Ipaybox.Forms.AcceptAccount);
                }
                else 
                    if (!Ipaybox.curPay.IsMoney)
                    {
                        // Нужно внести денюжку
                        StartForm(Ipaybox.Forms.Money);
                    }
                    else
                        if (!Ipaybox.curPay.IsRecieptPrinted)
                        {
                            if (!Ipaybox.FRS.RemoteFR)
                                // Нужно внести денюжку
                                StartForm(Ipaybox.Forms.Thanks);
                        }
            }
        }
        public void StartForm(Ipaybox.Forms form)
        {
            Form st = new Form();

            if (Ipaybox.Working && !Ipaybox.IsBlocked)
            {                
                switch (form)
                {
                    case Ipaybox.Forms.Main: st = new main_menu();
                        break;
                    case Ipaybox.Forms.Info: st = new info();
                        break;
                    case Ipaybox.Forms.Account: st = new edit();
                        break;
                    case Ipaybox.Forms.AcceptAccount: st = new acceptaccount();
                        break;
                    case Ipaybox.Forms.Money: st = new entermoney();
                        break;
                    case Ipaybox.Forms.Thanks: st = new thanks(Ipaybox.curPay.from_amount.ToString(),Ipaybox.curPay.to_amount.ToString(),"",Ipaybox.curPay.account.ToString());
                        break;
                    case Ipaybox.Forms.Information: st = new LoginForms.information();
                        break;
                }
            }
            else
            {
                st = new info();
            }

            st.Show();
            //if(Ipaybox.CurrentForm != null)
             //   Ipaybox.CurrentForm.Dispose(); 
            //Ipaybox.CurrentForm =  st;
        }

        private void pays_send_timer_Tick(object sender, EventArgs e)
        {
            if (Ipaybox.Import.ChildCount > 0 && counttreads == 0)
            {
                new Thread(SendAllPays).Start();
                counttreads++;
            }

            //label1.Text = Ipaybox.NeedToSendPays.ToString() + " - Threads:" + counttreads ;
            //int req = (int)(Ipaybox.RequestTimeout * 1000);
            //if (req == 0)
            //    req = 200000;
            //pays_send_timer.Interval = req;
            //pays_send_timer.Stop();
            //pays_send_timer.Start();
        }
        private void SendAllPays()
        {
            lock (locker)
            {
                SendPays sp = new SendPays();
                bool sended = sp.SendImport(SendPays.Type.AllInOne);

                while (!sended)
                { 
                    sended = sp.SendImport(SendPays.Type.Iteraction); 
                }

                counttreads--;
            }
        }
        private void UpdateAll()
        {
            lock (locker)
            { 
                UpdateCore core = new UpdateCore();
                Ipaybox.UpdateState = 1;

                // Файлы
                if (Ipaybox.NeedUpdates.Core)
                {
                    System.Diagnostics.Process[] n = System.Diagnostics.Process.GetProcessesByName("updater");

                    if (n.Length == 0)
                    {
                        if (!string.IsNullOrEmpty(Ipaybox.Args))
                        {
                            System.Diagnostics.Process.Start(Ipaybox.StartupPath + @"\updater.exe", Ipaybox.Args);
                        }
                        else
                            System.Diagnostics.Process.Start(Ipaybox.StartupPath + @"\updater.exe");
                    }

                    //core.ValidateXmlAndLocal();
                }
                else
                {
                    //Обновление конфигурации
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Обновление конфигурации...");
                    if (!string.IsNullOrEmpty(Ipaybox.Terminal.terminal_id) && !string.IsNullOrEmpty(Ipaybox.Terminal.terminal_pass))
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "ID/пароль заполнены. Запуск обновления конфигурации");
                        core.UpdateFRSSettings();
                        // ПИН-коды
                        core.UpdatePIN();

                        // провайдеры
                        if (Ipaybox.NeedUpdates.ProviderList)
                            core.UpdateProviderList();

                        // терминал
                        if (Ipaybox.NeedUpdates.Trm_info)
                            core.UpdateTerminalInfo(false);

                        // комиссия
                        if (Ipaybox.NeedUpdates.Comission)
                            core.UpdateComiss(false);

                        if (!Ipaybox.NeedUpdates.ProviderList && !Ipaybox.NeedUpdates.Trm_info && !Ipaybox.NeedUpdates.Comission && !Ipaybox.NeedUpdates.Core)
                            Ipaybox.NeedToUpdateConfiguration = false;

                        InitializeIpaybox();

                        Ipaybox.UpdateState = 0;
                    }
                    else
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Невозможно запустить обновление конфигурации. Не заполнено ID/пароль");
                    }
                }

                counttreads--;
            }
        }
        private void SetModemConnect()
        {
            if(countmodem == 0)
            {
                countmodem++;
                if (Ipaybox.Modem != null)
                {
                    if (!Ipaybox.Internet)
                    {
                        Ipaybox.Modem.Connect(Ipaybox.NetModemName);
                        if (!Ipaybox.Modem.IsConnected(Ipaybox.NetModemName))
                            Ipaybox.Modem = null;
                    }
                }
                else
                {
                    try
                    {
                        Ipaybox.Modem = new modem();
                        Ipaybox.Modem.Connect(Ipaybox.NetModemName);
                        Ipaybox.Modem.IsConnected(Ipaybox.NetModemName);
                    }
                    catch (Exception ex)
                    {
                        HelperClass.CrashLog.AddCrash(ex);
                    }
                }
                countmodem--;
            }
        }
        private void conf_update_Tick(object sender, EventArgs e)
        {
            if (Ipaybox.NeedToUpdateConfiguration && counttreads ==0 && Ipaybox.Internet)
            {
                new Thread(UpdateAll).Start();
                counttreads++;
            }

            if (Ipaybox.NeedToRestart && (Ipaybox.IsMainMenu || !Ipaybox.Working || Ipaybox.ServiceMenu))
            {
                if (!string.IsNullOrEmpty(Ipaybox.Args))
                {
                    System.Diagnostics.Process.Start(Ipaybox.StartupPath + @"\rstrt.exe", Ipaybox.Args);
                }
                else
                    System.Diagnostics.Process.Start(Ipaybox.StartupPath + @"\rstrt.exe");

                Ipaybox.NeedToRestart = false;
            }

            /* Добавлено 07/06/2012. Тен Вадим.
             * Если выставлен флаг о необоходимости перезагрузки и отображается главное меню - перезагрузка терминала
             */
            if (Ipaybox.NeedToReboot && (Ipaybox.IsMainMenu || !Ipaybox.Working))
            {
                Ipaybox.NeedToReboot = false;
                System.Diagnostics.Process.Start("SHUTDOWN", "-r -t 01");
            }

            /* Добавлено 28/06/2012. Тен Вадим.
             * Если выставлен флаг о необоходимости выключения и отображается главное меню - выключения терминала
             */
            if (Ipaybox.NeedShutdown && (Ipaybox.IsMainMenu || !Ipaybox.Working))
            {
                Ipaybox.NeedShutdown = false;
                System.Diagnostics.Process.Start("SHUTDOWN", "-s -t 01");
            }
            //int req = (int)(Ipaybox.RequestTimeout * 1000);
            //if(req == 0)
            //    req = 5000;
            //conf_update.Interval = req ;
            //conf_update.Stop();
            //conf_update.Start();
        }

        private void monitoring_Tick(object sender, EventArgs e)
        {
            if (countmonitoring == 0)
            {
                countmonitoring++;
                new Thread(Monitoring).Start();
            }
            
            if (Ipaybox.IsBlocked)
            {
                Ipaybox.Working = false;
            }
        }
        public void Monitoring()
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Опрос статусов устройств.");

            // Получить статус купюрника
            if (Ipaybox.Bill == null)
            {
                Ipaybox.Bill = new Acceptors.Independed();
                if (Ipaybox.Bill.model == Acceptors.Model.NULL)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Купюроприемник не найден.");
                    Ipaybox.Working = false;
                }
                else
                {
                    if (Ipaybox.Bill.Error)
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Ошибка купюроприемника " + Ipaybox.Bill.ErrorMsg);
                    else
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Купюроприемник работает");

                    Ipaybox.Working = true;
                }
            }
            else
            {
                if (Ipaybox.Bill.model == Acceptors.Model.NULL)
                {
                    Ipaybox.Bill = new Acceptors.Independed();
                    if (Ipaybox.Bill.model == Acceptors.Model.NULL)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Купюроприемник не найден.");
                        Ipaybox.Working = false;
                    }
                    else
                    {
                        if (Ipaybox.Bill.Error)
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Ошибка купюроприемника " + Ipaybox.Bill.ErrorMsg);
                        else
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Купюроприемник работает");
                        Ipaybox.Working = true;
                    }
                }
                else
                {
                    Ipaybox.Bill.Pooling();
                    if (!Ipaybox.Working && Ipaybox.Bill.Error == false)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Купюроприемник работает");
                        Ipaybox.Working = true;
                    }
                    else
                    {
                        if (Ipaybox.Bill.Error)
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Ошибка купюроприемника " + Ipaybox.Bill.ErrorMsg);
                        else
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Купюроприемник работает");
                    }
                }
            }

            monitoring m = new monitoring();

            if (m.Send())
                Ipaybox.IsMonitorngSended = true;
            else
                Ipaybox.IsMonitorngSended = false;

            UpdateCore core = new UpdateCore();

            countmonitoring--;
        }
        private void link_timer_Tick(object sender, EventArgs e)
        {
            if (!link_bw.IsBusy)
                link_bw.RunWorkerAsync();
        }
        private void link_bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Ipaybox.NetOption == 1)
            {
                if (Ipaybox.Modem != null)
                {
                    if (!Ipaybox.Modem.IsConnected(Ipaybox.NetModemName))
                    {
                        new Thread(SetModemConnect).Start();

                        if (Ipaybox.Modem.IsConnected(Ipaybox.NetModemName))
                        {
                            if (Ipaybox.IsMonitorngSended)
                                Ipaybox.WDConnectCount = 0;
                            else
                            {
                                Ipaybox.WDConnectCount++;
                                ResetModemIfNeeded();
                            }
                        }
                        else
                        {
                            Ipaybox.WDConnectCount++;
                            ResetModemIfNeeded();
                        }
                    }
                    else
                    {
                        if (Ipaybox.IsMonitorngSended)
                            Ipaybox.WDConnectCount = 0;
                        else
                        {
                            Ipaybox.WDConnectCount++;
                            ResetModemIfNeeded();
                        }
                    }
                }
                else
                {
                    try
                    {
                        Ipaybox.Modem = new modem();

                        Ipaybox.Modem.Connect(Ipaybox.NetModemName);
                        if (Ipaybox.Modem.IsConnected(Ipaybox.NetModemName))
                        {
                            if (Ipaybox.IsMonitorngSended)
                                Ipaybox.WDConnectCount = 0;
                            else
                            {
                                Ipaybox.WDConnectCount++;
                                ResetModemIfNeeded();
                            }
                        }
                        else
                        {
                            Ipaybox.WDConnectCount++;
                            ResetModemIfNeeded();
                        }
                    }
                    catch (Exception ex)
                    {
                        HelperClass.CrashLog.AddCrash(ex);
                    }
                }
            }
        }
        private void ResetModemIfNeeded()
        {
            if (Ipaybox.WDConnectCount > 39)
            {
                if (Ipaybox.Wd.watch != Independed.WD.NULL)
                {
                    if (Ipaybox.Modem == null)
                        Ipaybox.Modem = new modem();
                    
                    //Перед тем, как передергивать модем по питанию, сначала нужно попытаться разорвать соединение
                    try
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Попытка разрыва интернет-соединения...");
                        Ipaybox.Modem.Disconnect(Ipaybox.NetModemName);

                        if (!Ipaybox.Modem.IsConnected(Ipaybox.NetModemName))
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Соединение разорвано");
                        else
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось разорвать соединение");
                    }
                    catch (Exception ex)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка при попытке разрыва соединения: " + ex.Message);
                    }

                    bool res = Ipaybox.Wd.ResetModem();
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Перезагрузка модема с помошью " + Ipaybox.Wd.watch.ToString() + ": " + res.ToString());
                }

                Ipaybox.WDConnectCount = 0;
                System.Threading.Thread.Sleep(10000);
                Ipaybox.Modem = new modem();
            }
        }
        private void CreateDelayedZReport()
        {
            if (!Ipaybox.FRegister.AddDelayedZReport())
            {
                if (Ipaybox.FRegister.ErrorNumber != "-3828")
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Невозможно снять Z-отчет: #" + Ipaybox.FRegister.ErrorNumber + " (" + Ipaybox.FRegister.ErrorMessage + ")");
                    Ipaybox.Working = false;
                }
                else
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не нуждается в снятие Z-отчета: #" + Ipaybox.FRegister.ErrorNumber + " (" + Ipaybox.FRegister.ErrorMessage + ")");
                    Ipaybox.Option["last-report"] = DateTime.Now.ToString();
                }
            }
            else
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Z-отчет успешно помещен в память ("+DateTime.Now.ToString()+") : #" + Ipaybox.FRegister.ErrorNumber + " (" + Ipaybox.FRegister.ErrorMessage + ")");
                Ipaybox.Option["last-report"] = DateTime.Now.ToString();
            }
        }
        private void print_timer_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Ipaybox.PrintString) && !Ipaybox.IsPrinting)
            {
                new Thread(Ipaybox.Print).Start();
            }

            lock (autozreport)
            {
                // здесь надо бы написать автоснятие
                if (Ipaybox.FiscalRegister && Ipaybox.FRegister != null && Ipaybox.FRegister.FiscalMode)
                {
                    DateTime now = DateTime.Now;
                    DateTime time_report;

                    if (DateTime.TryParse(DateTime.Now.ToShortDateString() + " " + Ipaybox.Option["auto-zreport"], out time_report))
                    {
                        if (now >= time_report)
                        {
                            //поидее надо делать снятие отчета
                            if (Ipaybox.Option["last-report"] == null)
                            {
                                //ОТчет никогда не был снят снят - снимаем
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Дата последнего снятия не установлена. Снимаем Z-отчет");
                                CreateDelayedZReport();
                            }
                            else
                            {
                                DateTime last_report = DateTime.Parse(Ipaybox.Option["last-report"]);
                                // дата последнего снятия меньше чем текущая.
                                if (last_report.Date < now.Date)
                                {
                                    CreateDelayedZReport();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}