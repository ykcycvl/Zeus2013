using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace zeus
{
    class monitoring
    {
        public monitoring()
        { }

        public bool PackFile(string filename, string packed_name)
        {
            bool res = false;

            try
            {
                using (ZipFile zip1 = new ZipFile())
                {
                    zip1.AddFile(filename);
                    zip1.Save(packed_name);
                }
                res = true;
            }
            catch
            {
                res = false;
            }

            return res;

        }
        public void SendFiles()
        {
            if (Ipaybox.XML_SendFile.DocumentElement.ChildNodes.Count > 0)
            {
                try
                {
                    foreach (XmlElement el in Ipaybox.XML_SendFile.DocumentElement.ChildNodes)
                    {
                        if (el.Name == "filerequest")
                        {
                            string id = el.GetAttribute("id");
                            string name = el.GetAttribute("filename");
                            string path = el.GetAttribute("path");
                            string type = el.GetAttribute("type");// тип отправки SERVER/EMAIL
                            string email = el.GetAttribute("email");// тип отправки SERVER/EMAIL
                            switch (path)
                            {
                                case "relative":
                                    name = Ipaybox.StartupPath.TrimEnd('\\') + '\\' + name;
                                    break;
                            }
                            string zipname = id + ".zip";

                            // Удалось создать архив
                            if (PackFile(name, zipname))
                            {
                                System.Threading.Thread.Sleep(30);

                                FileInfo fi = new FileInfo(zipname);
                                if (fi.Exists)
                                {
                                    switch (type.ToLower())
                                    {
                                        case "email":
                                            if (SMTP("Терминал " + Ipaybox.Terminal.terminal_id, "Файл " + name + " с терминала №" + Ipaybox.Terminal.terminal_id + " " + Ipaybox.Terminal.trm_adress, fi.FullName, email))
                                            {
                                                Ipaybox.XML_SendFile.DocumentElement.RemoveChild(el);
                                                Ipaybox.XML_SendFile.Save("file.xml");
                                            }
                                            break;
                                    }
                                    // Отпарвляем по смтп файл

                                    fi.Delete();
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        public bool SMTP(string subject, string body, string file, string email)
        {
            System.Net.Mail.SmtpClient Smtp = new SmtpClient("smtp.z8.ru");
            try
            {
                bool IsFileAttached = false;

                System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();

                Message.To.Add(new MailAddress(email));
                Message.Subject = subject;
                Message.Body = body;

                FileInfo fi = new FileInfo(file);
                if (fi.Exists)
                {
                    // 1 mb = 
                    int MaxFileSize = 1024 * 1024; //
                    if (fi.Length > MaxFileSize)
                    {
                        body += "Запрошенный Вами файл <" + fi.FullName + "> имеет размер больше 1Мб. Передача файла прервана. \r\n";
                    }
                    else
                    {
                        Message.Attachments.Add(new System.Net.Mail.Attachment(file));
                        IsFileAttached = true;
                    }
                }
                else
                {
                    body += "Запрошенный Вами файл не существует. \r\n";
                }

                body += "\r\n-----\r\n";
                body += "Ipaybox.ru\r\n";
                body += "Дата: " + DateTime.Now.ToString();

                Message.BodyEncoding = Encoding.GetEncoding("utf-8"); // Turkish Character Encoding// кодировка эсли нужно
                Message.From = new System.Net.Mail.MailAddress("Ipaybox.ru <noreply@ipaybox.ru>");

                //Smtp.Host = "smtp.z8.ru"; //например для gmail (smtp.gmail.com), mail.ru(smtp.mail.ru)
                Smtp.EnableSsl = false; // включение SSL для gmail нужно, для mail.ru не нада
                Smtp.Credentials = new System.Net.NetworkCredential("ipaybox_9", "hbl6FijITwZ");

                Smtp.Send(Message);//отправка 
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Файл <" + file + "> успешно отправлен на <" + email + ">");
                if (IsFileAttached)
                    Ipaybox.Incass.bytesSend += (int)(fi.Length + fi.Length / 3);
                return true;
            }
            catch (SmtpFailedRecipientsException ex1)
            {
                /*
                 Сообщение не удалось доставить одному или нескольким получателям из свойств To, CC или Bcc.
                 */
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Файл <" + file + "> не доставлен для одного или нескольких адресатов на <" + email + "> StatusCode:" + ex1.StatusCode);
                return true;
            }
            catch (SmtpException ex2)
            {
                HelperClass.CrashLog.AddCrash(ex2);
                /*
                 Сбой подключения к серверу SMTP. или-
                Сбой проверки подлинности. или-
                Bремя операции истекло.
                 */
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Файл <" + file + "> не доставлен. Сбой подключения к SMTP-серверу. StatusCode:" + ex2.StatusCode);
            }
            catch (Exception ex)
            {

                HelperClass.CrashLog.AddCrash(ex);
            }

            return false;
        }

        public void CreateMonitoringInfo()
        {
            Random n = new Random();
            XmlElement el = Ipaybox.XML_monitoring.CreateElement("monitor");

            el.SetAttribute("monitor_id", n.Next(10, 16777215).ToString());
            el.SetAttribute("amount", Ipaybox.Incassation.Amount.ToString());
            el.SetAttribute("count_bill", Ipaybox.Incassation.CountBill.ToString());
            el.SetAttribute("bytes_send", Ipaybox.Incassation.Bytessend.ToString());
            el.SetAttribute("bytes_recieve", Ipaybox.Incassation.Bytesrecieved.ToString());

            if (Ipaybox.NetOption == 1 && Ipaybox.IMSI != string.Empty)
                el.SetAttribute("imsi", Ipaybox.IMSI);

            string accm = "", prnm = "";

            try { accm = Ipaybox.Bill.model.ToString(); }
            catch { accm = "NULL"; }

            try {
                if (Ipaybox.Printer != null)
                    prnm = Ipaybox.Printer.PrnModel.ToString();
                else
                    if (Ipaybox.WindowsPrinter)
                        prnm = "WIN";
                    else
                        if (Ipaybox.FiscalRegister)
                        {
                            if (Ipaybox.FRegister != null)
                            {
                                prnm = Ipaybox.FRegister.Model + "V" + Ipaybox.FRegister.Version;

                                try
                                {
                                    string kkmNumber = Ipaybox.FRegister.getKKMnumber();
                                    string eklzNumber = Ipaybox.FRegister.getEKLZnumber();
                                    prnm += "(" + kkmNumber + ", " + eklzNumber + ")";
                                }
                                catch (Exception ex)
                                {
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось определить номер ККМ и ЭКЛЗ. " + ex.Message);
                                }
                            }
                            else
                            { prnm = "ФР НЕ НАЙДЕН"; }
                        }
                        else
                        {
                            if (Ipaybox.KKMPrim21K)
                            {
                                prnm = "ККМ Прим21К";
                            }
                        }
            }
            catch{ prnm = "NULL";}

            string billver = Ipaybox.Bill.Version;

            string inf = "Купюрник:" + accm + " " + billver + ";Принтер:" + prnm + ";Версия ПО:" + Ipaybox.CoreVersion + " (" + Ipaybox.Terminal.InterfaceVersion + ")";

            if (Ipaybox.Wd.watch != WD.Independed.WD.NULL)
                inf = inf + ";WD:" + Ipaybox.Wd.watch.ToString() + " " + Ipaybox.Wd.Version;
            if (Ipaybox.UpdateState == 1)
                inf = inf + ";Обновляется.";

            el.SetAttribute("info",  inf);
            string acc = "";

            if (Ipaybox.Bill != null)
                if (Ipaybox.Bill.model != Acceptors.Model.NULL)
                    if (!Ipaybox.Bill.Error)
                        acc = "OK";
                    else
                        acc = Ipaybox.Bill.ErrorMsg;
                else
                    acc = "ОШИБКА! Не определена модель.";
            else
                acc = "ОШИБКА! Не инициализирован класс.";

            el.SetAttribute("acceptor", acc);
            string prn = "";
            if (!Ipaybox.WindowsPrinter)
                if (Ipaybox.Printer != null)
                    if (Ipaybox.Printer.PrnModel != Printers.Model.NULL)
                        if (Ipaybox.Printer.Test())
                            prn = "OK";
                        else
                        {
                            if (Ipaybox.Printer.ErrorMessage != null)
                                prn = Ipaybox.Printer.ErrorMessage;
                            else
                            {
                                prn = "FAILED";
                                //Ipaybox.Working = false;
                            }
                        }
                    else
                        prn = "ОШИБКА! Не определена модель.";
                else
                    if (Ipaybox.KKMPrim21K)
                    {
                        if (Ipaybox.prim21k.Model != _prim21k.KKMModel.NULL)
                        {
                            if (Ipaybox.prim21k.LastErrorCode == 0)
                                prn = "OK";
                            else
                            {
                                prn = "ОШИБКА №" + Ipaybox.prim21k.LastErrorCode + " - " + Ipaybox.prim21k.LastAnswer;
                                //Ipaybox.Working = false;
                            }
                        }
                    }
                    else
                        if(Ipaybox.FiscalRegister)
                        {
                            if (Ipaybox.FRegister != null)
                            {
                                Ipaybox.FRegister.GetStatus();

                                if (Ipaybox.FRegister.ErrorNumber == "0")
                                    prn = "OK";
                                else
                                {
                                    prn = "ОШИБКА №"+Ipaybox.FRegister.ErrorNumber + " - " + Ipaybox.FRegister.ErrorMessage;
                                    //Ipaybox.Working = false;
                                }
                                if (!Ipaybox.FRegister.PaperPresent)
                                {
                                    prn = "ЗАКОНЧИЛАСЬ БУМАГА";

                                    if (Ipaybox.FRegister.FiscalMode)
                                    {
                                        Ipaybox.Working = false;
                                    }
                                }
                            }
                            else
                            {
                                prn = "ОШИБКА! Не инициализирован класс ФР!";
                                Ipaybox.Working = false;
                            }
                        }
                        else
                            prn = "ОШИБКА! Не инициализирован класс.";
            else
                prn = "OK";

            el.SetAttribute("printer", prn);

            if (Ipaybox.Working)
                el.SetAttribute("comment", "В работе.");
            else
                el.SetAttribute("comment", "Не работает.");

            el.SetAttribute("state", "0");

            Ipaybox.XML_monitoring.DocumentElement.InsertAfter(el, Ipaybox.XML_monitoring.DocumentElement.LastChild);
        }
        public bool Send()
        {
            if (Ipaybox.XML_monitoring.DocumentElement == null)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ipaybox.XML_monitoring нулевой. Мониторинг не отправлен");
                return false;
            }

            SendFiles();

            if (Ipaybox.XML_monitoring.DocumentElement.ChildNodes.Count == 0)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Сбор информации для мониторинга");
                CreateMonitoringInfo();
            }

            if (Ipaybox.XML_monitoring.DocumentElement.ChildNodes.Count > 0)
            {
                string data = "<request>";
                data += "<protocol>1.00</protocol>";
                data += "<type>1.00</type>";
                data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
                data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
                data += Ipaybox.XML_monitoring.DocumentElement.OuterXml;
                data += "</request>";

                string cpdata = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(data)));
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Информация собрана, сформирован пакет, попытка отправки данных на сервер...");

                string response = TryPostData(cpdata.Normalize());

                if (!string.IsNullOrEmpty(response))
                {
                    XmlDocument resp = new XmlDocument();

                    try
                    {
                        resp.LoadXml(response);

                        for (int i = 0; i < resp.DocumentElement.ChildNodes.Count; i++)
                        {
                            XmlElement row = (XmlElement)resp.DocumentElement.ChildNodes[i];

                            if (row.Name == "sendSMS")
                            {
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Необходима отправка СМС-сообщения");
                                
                                int j = 0;

                                while (!_Modem.SendSMS(row.InnerText) && j < 3)
                                {
                                    j++;
                                }
                            }

                            if (row.Name == "restart")
                            {
                                if (row.InnerText == "1")
                                {
                                    Ipaybox.NeedToRestart = true;
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Получена команда на перезапуск");
                                }
                                else
                                {
                                    if (row.InnerText == "2")
                                    {
                                        Ipaybox.NeedToReboot = true;
                                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Получена команда на перезагрузку");
                                    }
                                    else
                                        if (row.InnerText == "3")
                                        {
                                            Ipaybox.NeedShutdown = true;
                                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Получена команда на выключение");
                                        }
                                }
                            }
                            if (row.Name == "blocked")
                            {
                                if (row.InnerText == "1")
                                {
                                    Ipaybox.IsBlocked = true;
                                    Ipaybox.Working = false;
                                }
                                else
                                {
                                    if (row.InnerText == "0")
                                    {
                                        Ipaybox.IsBlocked = false;
                                    }
                                }
                            }
                            if (row.Name == "monitor")
                            {
                                if (row.GetAttribute("result") == "0")
                                {
                                    Ipaybox.XML_monitoring.LoadXml("<monitoring></monitoring>");
                                }
                            }
                            if (row.Name == "filerequest")
                            {
                                /*
                                 <filerequest id=\"[0]\" filename=\"[1]\" path=\"[2]\" type=\"[3]\" email=\"[4]\" />
                                 */

                                XmlElement element = Ipaybox.XML_SendFile.CreateElement("filerequest");
                                element.SetAttribute("id", row.GetAttribute("id"));
                                element.SetAttribute("filename", row.GetAttribute("filename"));
                                element.SetAttribute("path", row.GetAttribute("path"));
                                element.SetAttribute("type", row.GetAttribute("type"));
                                element.SetAttribute("email", row.GetAttribute("email"));
                                Ipaybox.XML_SendFile.DocumentElement.InsertAfter(element, Ipaybox.XML_SendFile.DocumentElement.LastChild);
                                Ipaybox.XML_SendFile.Save("file.xml");
                                new System.Threading.Thread(SendFiles).Start();

                            }
                            if (row.Name == "configuration-id")
                            {
                                string txt = row.InnerText;
                                if (!String.IsNullOrEmpty(txt))
                                {
                                    if (txt[txt.Length - 1] == 'U')
                                    {
                                        Ipaybox.NeedToUpdateConfiguration = true;
                                        Ipaybox.NeedUpdates.Core = true;
                                        txt = txt.Remove(txt.Length - 1);
                                    }
                                    
                                    // Если с сервера пришел ответ с литерой "I" в configuration-id
                                    // необходимо обновить интерфейс приложения
                                    if (txt[txt.Length - 1] == 'I')
                                    {
                                        Ipaybox.NeedToUpdateConfiguration = true;
                                        Ipaybox.NeedUpdates.Interface = true;
                                        txt = txt.Remove(txt.Length - 1);
                                    }

                                    if (Ipaybox.Terminal.configuration_id != txt)
                                    {
                                        Ipaybox.NeedToUpdateConfiguration = true;
                                        Ipaybox.NeedUpdates.Comission = true;
                                        Ipaybox.NeedUpdates.Config = true;

                                        Ipaybox.NeedUpdates.ProviderList = true;
                                        Ipaybox.NeedUpdates.Trm_info = true;

                                        Ipaybox.Terminal.configuration_id = txt;
                                    }
                                }
                            }
                        }

                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Мониторинг отправлен");

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка: " + ex.Message);
                        return false; 
                    }
                }
            }
            else
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Сбор информации не произведен! Мониторинг не отправлен!");
                return false;
            }

            return false;
        }
        private string SendRequestPost(string URL, string subject)
        {
            string strOut = "";
            WebResponse result = null;
            HttpWebRequest req = null;
            Stream newStream = null;
            Stream ReceiveStream = null;
            StreamReader sr = null;
            try
            {
                // Url запрашиваемого методом POST скрипта
                req = (HttpWebRequest)WebRequest.Create(URL);
                req.Method = "POST";
                req.Timeout = 90000;
                req.UserAgent = "Zeus ver. " + Ipaybox.CoreVersion + "; " + Ipaybox.Terminal.terminal_id;
                // эта строка необходима только при защите скрипта на сервере Basic авторизацией
                //req.Credentials = new NetworkCredential("login", "password");
                //req.ContentType = "application/x-www-form-urlencoded";
                byte[] SomeBytes = null;
                // передаем список пар параметров / значений для запрашиваемого скрипта методом POST
                // в случае нескольких параметров необходимо использовать символ & для разделения параметров
                // в данном случае используется кодировка windows-1251 для Url кодирования спец. символов значения параметров
                SomeBytes = Encoding.GetEncoding(1251).GetBytes(HttpUtility.UrlEncode(subject, Encoding.GetEncoding(1251)));
                req.ContentLength = SomeBytes.Length;
                newStream = req.GetRequestStream();
                newStream.Write(SomeBytes, 0, SomeBytes.Length);
                newStream.Close();
                // считываем результат работы
                result = req.GetResponse();
                ReceiveStream = result.GetResponseStream();
                Encoding encode = Encoding.GetEncoding(1251);
                sr = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);

                while (count > 0)
                {
                    String str = new String(read, 0, count);

                    strOut += str;
                    count = sr.Read(read, 0, 256);
                }

                return strOut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (newStream != null)
                    newStream.Close();
                if (ReceiveStream != null)
                    ReceiveStream.Close();
                if (sr != null)
                    sr.Close();
                if (result != null)
                    result.Close();
            }
        }
        private string TryPostData(string data)
        {
            XmlDocument d = new XmlDocument();
            bool t = false;
            int count = 0;
            string xml = "";
            while (!t && count < Ipaybox.ServiceUrl.Length * 2)
            {
                try
                {
                    xml = SendRequestPost(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "sevice.exe", data);

                    try
                    {
                        if (xml != "")
                        {
                            Ipaybox.Incass.bytesSend += data.Length;
                            Ipaybox.Incass.bytesRead += xml.Length;
                            Ipaybox.FlushStatistic();
                        }

                        d.LoadXml(xml);

                        break;
                    }
                    catch
                    {
                        count++;
                    }
                }
                catch
                {
                    Ipaybox.ServiceUrlIndex++;
                    if (Ipaybox.ServiceUrlIndex == Ipaybox.ServiceUrl.Length)
                        Ipaybox.ServiceUrlIndex = 0;
                    count++;
                }
            }
            return xml;
        }
    }
}
