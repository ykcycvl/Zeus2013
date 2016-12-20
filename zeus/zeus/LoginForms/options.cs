using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Web;
using System.Net;
using remoteFR;

namespace zeus
{
    public partial class options : Form
    {
        private Point currentCursorPos = Cursor.Position;
        PrintDocument doc = new PrintDocument();
        static string conText = "";
        static Color conTextColor;

        private void printDoc_PrintPage(Object sender, PrintPageEventArgs e)
        {
            FileInfo fi = new FileInfo(Ipaybox.StartupPath + "\\config\\vkp80.prn");
            StreamReader sr = fi.OpenText();
            string check = sr.ReadToEnd();
            sr.Close();

            check = check.Replace("[agent_jur_name]", Ipaybox.Terminal.jur_name.Trim());
            check = check.Replace("[agent_adress]", Ipaybox.Terminal.jur_adress.Trim());
            check = check.Replace("[agent_inn]", "ИНН " + Ipaybox.Terminal.jur_inn.Trim());
            check = check.Replace("[agent_support_phone]", Ipaybox.Terminal.support_phone.Trim());
            check = check.Replace("[bank]", Ipaybox.Terminal.bank.Trim());
            check = check.Replace("[terms_number]", Ipaybox.Terminal.terms_number.Trim());
            check = check.Replace("[count_bill]", Ipaybox.Incass.countchecks.ToString().Trim());
            check = check.Replace("[terminal_id]", Ipaybox.Terminal.terminal_id.Trim());
            check = check.Replace("[trm_adress]", Ipaybox.Terminal.trm_adress.Trim());
            check = check.Replace("[date]", DateTime.Now.ToString().Trim());
            check = check.Replace("[amount]", Ipaybox.curPay.from_amount.ToString() + " руб.");
            check = check.Replace("[to_amount]", Ipaybox.curPay.to_amount.ToString() + " руб.");

            if (Ipaybox.FRS.RemoteFR)
                try
                {
                    check = remoteFR.RemoteFiscalRegister.tryFormFicsalCheck(Ipaybox.Terminal.jur_name.Trim(), Ipaybox.FRS.headertext, check, "Тест", Ipaybox.Terminal.terminal_id.Trim(), Ipaybox.Terminal.terminal_pass, "0", "0", "1", Ipaybox.FRS.RemoteFiscalRegisterURL, Ipaybox.FRS.checkWidth, Ipaybox.FRS.remoteFRtimeout);
                }
                catch (Exception ex) { Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);};

            if (Ipaybox.EpsonT400)
            {
                Font printFont = new Font("Courier New", 8);
                RectangleF rf = new RectangleF(0, 0, 220, 0);
                e.Graphics.DrawString("ШАБЛОН " + fi.Name + "\r\n" + check, printFont, Brushes.Black, rf);
            }
            else
            {
                Font printFont = new Font("Courier New", 10);
                e.Graphics.DrawString("ШАБЛОН " + fi.Name + "\r\n" + check, printFont, Brushes.Black, 0, 0);
            }
        }
        public bool Changed;
        public options()
        {
            InitializeComponent();
        }
        private void Reset()
        {
            XmlElement root1 = Ipaybox.terminal_info.DocumentElement;
            if (root1 == null)
            {
                Ipaybox.terminal_info = new XmlDocument();
                Ipaybox.terminal_info.LoadXml("<terminal></terminal>");
                root1 = Ipaybox.terminal_info.DocumentElement;
            }

            if (textBox1.Text != Ipaybox.Terminal.terminal_id)
            {
                SetNew("terminal_id", textBox1.Text);

                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.NeedUpdates.Trm_info = true;
            }

            if (textBox2.Text != Ipaybox.Terminal.terminal_pass)
            {
                SetNew("password", Ipaybox.getMd5Hash(textBox2.Text));
                Ipaybox.NeedToUpdateConfiguration = true;
                Ipaybox.NeedUpdates.Trm_info = true;
            }

            if (textBox3.Text != Ipaybox.Terminal.pincode)
                SetNew("pin", Ipaybox.getMd5Hash(textBox3.Text));
            if (textBox4.Text != Ipaybox.Terminal.secret_number)
                SetNew("secret_number", Ipaybox.getMd5Hash(textBox4.Text));

            Ipaybox.terminal_info.Save(Ipaybox.StartupPath + "\\config\\terminal.xml");

            Ipaybox.LoadTerminalData();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
            LoadF()  ;
        }
        private void SetNew(string name, string value)
        {
            bool exist = false;
            XmlElement root = Ipaybox.terminal_info.DocumentElement;


            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == name)
                {
                    row.InnerText = value;
                    exist = true;
                }
            }

            if (!exist)
            {
                XmlElement el = Ipaybox.terminal_info.CreateElement(name);
                el.InnerText = value;

                Ipaybox.terminal_info.DocumentElement.InsertAfter(el, Ipaybox.terminal_info.DocumentElement.LastChild);
            }

        }
        private string GetAcceptorFirmwareVersion()
        {
            string res = string.Empty;

            try
            {
                res = Ipaybox.Bill.Version;
            }
            catch (Exception ex)
            {
                throw ex;
            }
                
            return res;
        }
        private void LoadF()
        {
            Changed = true;
          
            XmlElement root1 = Ipaybox.terminal_info.DocumentElement;

            if (root1 != null)
            {
                for (int i = 0; i < root1.ChildNodes.Count; i++)
                {
                    XmlElement el = (XmlElement)root1.ChildNodes[i];
                    switch (el.Name)
                    {
                        case "terminal_id":
                            textBox1.Text = el.InnerText;
                            break;
                        case "password":
                            textBox2.Text = el.InnerText;
                            break;
                        case "pin":
                            textBox3.Text = el.InnerText;
                            break;
                        case "secret_number":
                            textBox4.Text = el.InnerText;
                            break;
                    }
                }
                Lock(false);
                Changed = false;
            }

            if (Ipaybox.Bill != null)
            {
                if (Ipaybox.Bill.model.ToString() != "NULL")
                {
                    //Версия прошивки купюроприемника
                    string firmwareVersion = string.Empty;

                    try
                    {
                        firmwareVersion = GetAcceptorFirmwareVersion();
                    }
                    catch (Exception ex)
                    {
                        firmwareVersion = string.Empty;
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                    }

                    label6.Text = Ipaybox.Bill.model.ToString() + " " + firmwareVersion;
                    label6.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    label6.Text = "Модель не определена! NULL";
                    label6.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                label6.Text = "Не опознан - Не работает";
                label6.ForeColor = System.Drawing.Color.Red;
            }

            if (Ipaybox.Printer != null)
            {
                label8.Text = Ipaybox.Printer.PrnModel.ToString();
                label8.ForeColor = System.Drawing.Color.Black;

                if (Ipaybox.Printer.PrnModel == Printers.Model.NULL)
                    label8.ForeColor = System.Drawing.Color.Red;
            }
            else
                if (Ipaybox.WindowsPrinter)
                {
                    label8.Text = "WIN PRINTER";
                    label8.ForeColor = System.Drawing.Color.Black;
                }
                else
                    if (Ipaybox.FiscalRegister)
                    {
                        if (Ipaybox.FRegister != null)
                        {
                            label8.Text = Ipaybox.FRegister.Model + "[" + Ipaybox.FRegister.ComPort + "]";
                            label8.ForeColor = System.Drawing.Color.Black;
                            froptions.Visible = true;

                        }
                        else
                        {
                            label8.Text = "ФР не найден!";
                            label8.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        if (Ipaybox.KKMPrim21K)
                        {
                            label8.Text = Ipaybox.prim21k.Model.ToString() + "[" + Ipaybox.prim21k.ComPort + "]";
                            label8.ForeColor = System.Drawing.Color.Black;
                        }
                        else
                        {
                            label8.Text = "Не найден принтер";
                            label8.ForeColor = System.Drawing.Color.Red;
                        }
                    }

            ShowStatistic();
        }
        private void ShowStatistic()
        {
            count_bill.Text = Ipaybox.Incassation.CountBill.ToString();
            amount.Text = Ipaybox.Incassation.Amount.ToString();

            bytes_recieve.Text = (Ipaybox.Incassation.Bytesrecieved/1024).ToString() + " Kb";
            bytes_send.Text = (Ipaybox.Incassation.Bytessend/1024).ToString() + " Kb";
        }
        private void options_Load(object sender, EventArgs e)
        {
            if (!Ipaybox.ServiceMenuActive)
            {
                Ipaybox.ServiceMenuActive = true;
                if (Ipaybox.WindowsPrinter)
                {
                    doc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
                    doc.EndPrint += new PrintEventHandler(EndPrint);
                }

                this.Text = "Настройки терминала - Версия ПО: " + Ipaybox.CoreVersion;
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Пин-код верный - вход в меню настройки терминала.");
                this.TopLevel = true;
                if (Ipaybox.Debug)
                    this.TopMost = false;

                LoadF();
            }
            else
            {
                this.Dispose();
            }
        }
        
        private void PrintCheck()
        {
            string check = "ИНКАССАЦИОННЫЙ ЧЕК\r\n" +
                            "------------------\r\n" +
                            "Терминал: " + Ipaybox.Terminal.terminal_id + "\r\n" +
                            "Адрес: " + Ipaybox.Terminal.trm_adress + "\r\n" +
                            "Дата: " + DateTime.Now.ToString() + "\r\n" +
                            "------------------\r\n" +
                            "ID:  " + Ipaybox.Incassation.Id.ToString() + "\r\n" +
                            "Кол-во купюр: " + Ipaybox.Incassation.CountBill.ToString() + "\r\n" +
                            "Кол-во 10:    " + Ipaybox.Incassation.CountBill10.ToString() + "\r\n" +
                            "Кол-во 50:    " + Ipaybox.Incassation.CountBill50.ToString() + "\r\n" +
                            "Кол-во 100:   " + Ipaybox.Incassation.CountBill100.ToString() + "\r\n" +
                            "Кол-во 500:   " + Ipaybox.Incassation.CountBill500.ToString() + "\r\n" +
                            "Кол-во 1000:  " + Ipaybox.Incassation.CountBill1000.ToString() + "\r\n" +
                            "Кол-во 5000:  " + Ipaybox.Incassation.CountBill5000.ToString() + "\r\n" +
                            "Байт принято: " + Ipaybox.Incassation.Bytesrecieved.ToString() + "\r\n" +
                            "Байт послано: " + Ipaybox.Incassation.Bytessend.ToString() + "\r\n" +
                            "Платежей:     " + Ipaybox.Incassation.CountPay.ToString() + "\r\n" +
                            "------------------\r\n" +
                            "СУММА:        " + Ipaybox.Incassation.Amount.ToString() + "\r\n" +
                            "------------------\r\n";

            Ipaybox.PrintString = check;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Start();

            this.TopMost = false;
            DialogResult dr = MessageBox.Show("Вы действительно хотите забрать деньги?", "Подтверждение инкассации", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                //SendIncass si = new SendIncass();
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Выбран пункт проинкассировать платежи.");

                try
                {
                    Ipaybox.IncassCheck = true;
                    PrintCheck();
                    var inc = new zeus.API.IncassHistoryEntity(Ipaybox.Incassation) { DateStoped = DateTime.Now, TerminalId = Ipaybox.Terminal.terminal_id, userID = Ipaybox.userID };
                    //Ipaybox.IncassHistory.Load();
                    Ipaybox.IncassHistory.Clear60();
                    Ipaybox.IncassHistory.Add(inc);

                    Ipaybox.IncassHistory.Save();
                    Ipaybox.Incassation.UserID = Ipaybox.userID;
                    Ipaybox.Import.Add("<i>"+Ipaybox.Incassation.IncassNow()+"</i>");
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Инкассация добавлена в import.");

                    ShowStatistic();

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Инкассация НЕ ПРОШЛА! \r\n"+ex.Message,"Ошибка",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Инкассация не прошла.");
                }
            }
            this.TopMost = true;
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Ipaybox.NetOption == 1 && !String.IsNullOrEmpty(Ipaybox.OpsosName) && !String.IsNullOrEmpty(Ipaybox.IMSI))
            {
                label13.Text = "Оператор: " + Ipaybox.OpsosName + ". IMSI: " + Ipaybox.IMSI;
            }
            else
            { 
                if (Ipaybox.NetOption == 0)
                    label13.Text = "Локальная сеть";
                else
                    label13.Text = "Оператор: не определен. IMSI: неизвестно";
            }

            if (Ipaybox.IsBlocked)
            {
                label11.ForeColor = Color.Red;
                label11.Text = "Заблокирован!";
            }
            else
            {
                label11.ForeColor = Color.Green;
                label11.Text = "Активен";
            }

            if (!string.IsNullOrEmpty(conText))
                lHasConnection.Text = conText;

            if (conTextColor != null)
                lHasConnection.ForeColor = conTextColor;

            if(Ipaybox.UpdateState == 1)
                update.Text = "Обновление: Выполняется.";
            else
                update.Text = "Обновление: Не нуждается.";

            try {
                pays.Text = "";
                if (Ipaybox.Import.CountNode("pay") > 0)
                    pays.Text = Ipaybox.Import.CountNode("pay").ToString() + " Платежей в очереди!";
                
                if (Ipaybox.Import.CountNode("statistic") > 0)
                    pays.Text = pays.Text + " ИНКАССАЦИЯ НЕ ОТПРАВЛЕНА!";
            }
            catch { pays.Text = "Платежей в очереди: НЕТ "; }

            Server.Text = "Рабочий URL: " + Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex];
            time.Text = "Текущее ВРЕМЯ: " + DateTime.Now.ToString() +" WDC:"+Ipaybox.WDConnectCount.ToString();
            if (Ipaybox.Wd.watch != WD.Independed.WD.NULL)
                watchdog.Text = Ipaybox.Wd.watch.ToString() + " " + Ipaybox.Wd.Version;
            button1.Enabled = Changed;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Ipaybox.NeedToRestart = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Start();
            conTextColor = Color.Black;
            lHasConnection.ForeColor = conTextColor;
            conText = "Проверка...";
            lHasConnection.Text = conText;
            this.TopMost = false;
            System.Threading.Thread tr = new System.Threading.Thread(TestConnection);
            tr.Start();
        }
        private void TestConnection()
        {
            string message = "";
            SendPays sp = new SendPays();
            long starttime = Environment.TickCount;

            switch (sp.TestServer())
            {
                case 0: message = "Связь с сервером есть. Без ошибок."; break;
                case 1: message = "Связь есть. ТЕРМИНАЛ НЕ АВТОРИЗОВАН! "; break;
                case 2: message = "Связь с сервером отсутствует. "; break;
                default:
                    message = sp.XML_Response;
                    break;
            }
            long stoptime = Environment.TickCount;
            long delta = stoptime - starttime;
            if (message != null)
            {
                conText = message + " [" + delta.ToString() + " ms]";
                conTextColor = Color.Green;
                //MessageBox.Show(message + "\r\n\r\n\r\n Время ответа: " + delta.ToString() + " ms");
            }
            else
            {
                conText = message + " [" + delta.ToString() + " ms]";
                conTextColor = Color.Red;
                //MessageBox.Show("Связи нет!!!\r\n\r\n\r\n Время ответа: " + delta.ToString() + " ms");
            }
        }
        private void EndPrint(object sender, PrintEventArgs e)
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "OK!");
        }
        private void button7_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Start();

            if(Ipaybox.Printer != null)
                if (Ipaybox.Printer.Test())
                {
                    // Загружаем шаблон для текущей модели прнтера
                    string find = Ipaybox.Printer.PrnModel.ToString();
                    FileInfo fi = new FileInfo(Ipaybox.StartupPath + "\\config\\" + find + ".prn");

                    try
                    {
                        StreamReader sr = fi.OpenText();
                        string check = sr.ReadToEnd();
                        sr.Close();

                        check = check.Replace("[agent_jur_name]", Ipaybox.Terminal.jur_name.Trim());
                        check = check.Replace("[agent_adress]", Ipaybox.Terminal.jur_adress.Trim());
                        check = check.Replace("[agent_inn]", "ИНН " + Ipaybox.Terminal.jur_inn.Trim());
                        check = check.Replace("[agent_support_phone]", Ipaybox.Terminal.support_phone.Trim());
                        check = check.Replace("[bank]", Ipaybox.Terminal.bank.Trim());
                        check = check.Replace("[terms_number]", Ipaybox.Terminal.terms_number.Trim());
                        check = check.Replace("[count_bill]", Ipaybox.Incass.countchecks.ToString().Trim());
                        check = check.Replace("[terminal_id]", Ipaybox.Terminal.terminal_id.Trim());
                        check = check.Replace("[trm_adress]", Ipaybox.Terminal.trm_adress.Trim());
                        check = check.Replace("[date]", DateTime.Now.ToString().Trim());
                        check = check.Replace("[amount]", Ipaybox.curPay.from_amount.ToString() + " руб.");
                        check = check.Replace("[to_amount]", Ipaybox.curPay.to_amount.ToString() + " руб.");

                        if (Ipaybox.FRS.RemoteFR)
                        {
                            byte[] bytes = remoteFR.RemoteFiscalRegister.convertToByteArray(Ipaybox.Terminal.jur_name.Trim(), Ipaybox.FRS.headertext, check, "Тест", Ipaybox.Terminal.terminal_id.Trim(), Ipaybox.Terminal.terminal_pass, "0", "0", "1", Ipaybox.FRS.RemoteFiscalRegisterURL, Ipaybox.FRS.checkWidth, Ipaybox.FRS.remoteFRtimeout);

                            if (bytes.Length > 0)
                            {
                                Ipaybox.Printer.PrintFromBytes(bytes);

                                if (remoteFR.RemoteFiscalRegister.OK)
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Данные с ФС получены. Печать фискального чека.");
                                else
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Данные с ФС НЕ получены. Печать нефискального чека с заголовком.");
                            }
                            else
                            {
                                Ipaybox.Printer.Print("ШАБЛОН " + fi.Name + "\r\n" + check);
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Данные с ФС НЕ получены. Печать нефискального чека.");
                            }
                        }
                        else
                        {
                            Ipaybox.Printer.Print("ШАБЛОН " + fi.Name + "\r\n" + check);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не могу найти шаблон для принтера. Поставьте терминал на обновление. " + ex.Message + " " + fi.FullName);
                    }
                }
                else
                { MessageBox.Show("Принтер НЕИСПРАВЕН."); }
            else
                if (Ipaybox.WindowsPrinter)
                {
                    try
                    {
                        doc.Print();
                    }
                    catch (Exception ex)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось напечатать чек! " + ex.Message);
                    }
                }
                else
                {
                    if (Ipaybox.KKMPrim21K)
                    {
                        if (Ipaybox.prim21k.Model != _prim21k.KKMModel.NULL)
                        {
                            string find = "prm";
                            FileInfo fi = new FileInfo(Ipaybox.StartupPath + "\\config\\" + find + ".prn");
                            if (!fi.Exists)
                                throw new Exception("Не найден шаблон! Обновите программу!");
                            StreamReader sr = fi.OpenText();
                            string check = sr.ReadToEnd();
                            sr.Close();

                            check = check.Replace("[agent_jur_name]", Ipaybox.Terminal.jur_name.Trim());
                            check = check.Replace("[agent_adress]", Ipaybox.Terminal.jur_adress.Trim());
                            check = check.Replace("[agent_inn]", "ИНН " + Ipaybox.Terminal.jur_inn.Trim());
                            check = check.Replace("[agent_support_phone]", Ipaybox.Terminal.support_phone.Trim());
                            check = check.Replace("[bank]", Ipaybox.Terminal.bank.Trim());
                            check = check.Replace("[terms_number]", Ipaybox.Terminal.terms_number.Trim());
                            check = check.Replace("[count_bill]", Ipaybox.Incass.countchecks.ToString().Trim());
                            check = check.Replace("[terminal_id]", Ipaybox.Terminal.terminal_id.Trim());
                            check = check.Replace("[trm_adress]", Ipaybox.Terminal.trm_adress.Trim());
                            check = check.Replace("[date]", DateTime.Now.ToString().Trim());
                            check = check.Replace("[amount]", Ipaybox.curPay.from_amount.ToString() + " руб.");
                            check = check.Replace("[to_amount]", Ipaybox.curPay.to_amount.ToString() + " руб.");

                            if (Ipaybox.FRS.RemoteFR)
                                try
                                {
                                    check = remoteFR.RemoteFiscalRegister.tryFormFicsalCheck(Ipaybox.Terminal.jur_name.Trim(), Ipaybox.FRS.headertext, check, "Тест", Ipaybox.Terminal.terminal_id.Trim(), Ipaybox.Terminal.terminal_pass, "0", "0", "1", Ipaybox.FRS.RemoteFiscalRegisterURL, Ipaybox.FRS.checkWidth, Ipaybox.FRS.remoteFRtimeout);
                                }
                                catch (Exception ex) { Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message); }

                            int res = Ipaybox.prim21k.PrintPND(check);

                            if (res != 0)
                            {
                                MessageBox.Show("Ошибка при распечатке чека! " + Ipaybox.prim21k.LastAnswer);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка! Принтер не обнаружен!");
                        }
                    }
                    else
                        if(Ipaybox.FiscalRegister)
                            if(Ipaybox.FRegister != null)
                            {
                                string find = "VKP80";
                                FileInfo fi = new FileInfo(Ipaybox.StartupPath + "\\config\\" + find + ".prn");
                                if (!fi.Exists)
                                    throw new Exception("Не найден шаблон! Обновите программу!");
                                StreamReader sr = fi.OpenText();
                                string check = sr.ReadToEnd();
                                sr.Close();

                                check = check.Replace("[agent_jur_name]", Ipaybox.Terminal.jur_name.Trim());
                                check = check.Replace("[agent_adress]", Ipaybox.Terminal.jur_adress.Trim());
                                check = check.Replace("[agent_inn]", "ИНН " + Ipaybox.Terminal.jur_inn.Trim());
                                check = check.Replace("[agent_support_phone]", Ipaybox.Terminal.support_phone.Trim());
                                check = check.Replace("[bank]", Ipaybox.Terminal.bank.Trim());
                                check = check.Replace("[terms_number]", Ipaybox.Terminal.terms_number.Trim());
                                check = check.Replace("[count_bill]", Ipaybox.Incass.countchecks.ToString().Trim());
                                check = check.Replace("[terminal_id]", Ipaybox.Terminal.terminal_id.Trim());
                                check = check.Replace("[trm_adress]", Ipaybox.Terminal.trm_adress.Trim());
                                check = check.Replace("[date]", DateTime.Now.ToString().Trim());
                                check = check.Replace("[amount]", Ipaybox.curPay.from_amount.ToString() + " руб.");
                                check = check.Replace("[to_amount]", Ipaybox.curPay.to_amount.ToString() + " руб.");

                                if (Ipaybox.FRegister.FiscalMode)
                                {
                                    Ipaybox.FRegister.Buy("Сотовая св.", 1, 0, "ШАБЛОН " + fi.Name + "\r\n" + check);
                                }
                                else
                                {
                                    string s = check;

                                    try
                                    {
                                        s = remoteFR.RemoteFiscalRegister.tryFormFicsalCheck(Ipaybox.Terminal.jur_name.Trim(), Ipaybox.FRS.headertext, check, "Тест", Ipaybox.Terminal.terminal_id.Trim(), Ipaybox.Terminal.terminal_pass, "0", "0", "1", Ipaybox.FRS.RemoteFiscalRegisterURL, Ipaybox.FRS.checkWidth, Ipaybox.FRS.remoteFRtimeout);
                                    }
                                    catch (Exception ex)
                                    {
                                        Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                                    }

                                    if (!Ipaybox.FRegister.PrintText("ШАБЛОН " + fi.Name + "\r\n" + s))
                                    {
                                        label8.Text = "ФР " + Ipaybox.FRegister.Model + " неисправен. #" + Ipaybox.FRegister.ErrorNumber + "(" + Ipaybox.FRegister.ErrorMessage + ")";
                                        label8.ForeColor = System.Drawing.Color.Red;
                                    }
                                }
                            }
                            else
                            { MessageBox.Show("ФИСКАЛЬНЫЙ РЕГИСТРАТОР НЕ ОБНАРУЖЕН."); }
                         else
                            { MessageBox.Show("Принтер НЕ ОБНАРУЖЕН."); }       
                }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
            button1.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
            button1.Enabled = true;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
            button1.Enabled = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
            button1.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Lock(true);
        }
        private void Lock(bool state)
        {
            textBox1.Enabled = state;
            textBox2.Enabled = state;
            textBox3.Enabled = state;
            textBox4.Enabled = state;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Start();

            Ipaybox.NeedUpdates.Core = true;
            Ipaybox.NeedToUpdateConfiguration = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Start();

            Ipaybox.NeedUpdates.Comission = true;
            Ipaybox.NeedUpdates.Config = true;
            Ipaybox.NeedUpdates.ProviderList = true;
            Ipaybox.NeedUpdates.Trm_info = true;
            Ipaybox.NeedToUpdateConfiguration = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Точно хотите перезагрузить автомат?", "Подтверждение.",MessageBoxButtons.OKCancel);

            if (dr == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("SHUTDOWN", "-r -t 01");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            _cursor.Hide();

            if (Ipaybox.Bill != null && Ipaybox.Bill.model != Acceptors.Model.NULL)
            {
                Acceptors.Result res = Ipaybox.Bill.SendCommand(Acceptors.Commands.StopTake);

                if (!Ipaybox.Working && !Ipaybox.Bill.Error && !Ipaybox.IsBlocked)
                {
                    Ipaybox.Working = true;
                }
            }

            Ipaybox.ServiceMenu = false;
            Ipaybox.StartForm.Main_Process();
            Ipaybox.ServiceMenuActive = false;
            this.Dispose();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            Form n = new LoginForms.netopt();
            n.ShowDialog();
            timer2.Start();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            Form m = new LoginForms.log_view();
            m.ShowDialog();
            timer2.Start();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            Form p = new LoginForms.security1();
            p.ShowDialog();
            timer2.Start();
        }
        private void OnFocus()
        {
            this.TopMost = true;
        }

        private void froptions_Click(object sender, EventArgs e)
        {
            Form o = new LoginForms.FRParametrs();
            this.TopMost = false; Application.DoEvents();
            o.ShowDialog();
            this.TopMost = true; Application.DoEvents();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            this.TopMost = false;
            LoginForms.PrintIncass frm = new zeus.LoginForms.PrintIncass();
            frm.ShowDialog();
            this.TopMost = true;
            timer2.Start();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            this.TopMost = false;
            WhiteList.WLForm frm = new WhiteList.WLForm();
            frm.ShowDialog();
            this.TopMost = true;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Закрыть форму
            button12_Click(sender, e);
        }

        private void options_Shown(object sender, EventArgs e)
        {
            _cursor.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            this.TopMost = false;
            zeus.LoginForms.AcceptorTestFrm frm = new zeus.LoginForms.AcceptorTestFrm();
            frm.ShowDialog();
            this.TopMost = true;
            timer2.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            this.TopMost = false;
            zeus.LoginForms.PrintOptions frm = new zeus.LoginForms.PrintOptions();
            frm.ShowDialog();
            this.TopMost = true;
            timer2.Start();
        }

        private void options_MouseMove(object sender, MouseEventArgs e)
        {
            timer2.Stop();
            timer2.Start();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                if (Ipaybox.NetOption == 1)
                {
                    _Modem.Init();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить все данные с терминала? После удаления данных программа будет перезапущена.", "Подтверждение удаления данных", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                System.IO.File.Delete("config/comiss.xml");
                System.IO.File.Delete("config/config.xml");
                System.IO.File.Delete("config/fs.xml");
                System.IO.File.Delete("config/incass.xml");
                System.IO.File.Delete("config/net.xml");
                System.IO.File.Delete("config/providers.xml");
                System.IO.File.Delete("config/settings.xml");
                System.IO.File.Delete("config/terminal.xml");
                System.IO.File.Delete("config/tpin.xml");
                System.IO.File.Delete("config/whitelist.xml");
                System.IO.File.Delete("history.xml");
                System.IO.File.Delete("incass.xml");
                MessageBox.Show("Все файлы удалены. Программа будет перезапущена");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Обнуление конфигурации не завершено! Программа будет перезапущена" + ex.Message);
            }
            finally { Ipaybox.NeedToRestart = true; }
        }

        private void button20_Click(object sender, EventArgs e)
        {
        }
    }
}
