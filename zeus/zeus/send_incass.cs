using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;
using System.Drawing.Printing;
using System.Drawing;


namespace zeus
{
    class SendIncass
    {
        PrintDocument doc = new PrintDocument();
        string PrintCheck;
        public SendIncass()
        {
           if (Ipaybox.WindowsPrinter)
                doc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage); 
        }
        private void printDoc_PrintPage(Object sender, PrintPageEventArgs e)
        {

            Font printFont = new Font("Courier New", 10);
            e.Graphics.DrawString(PrintCheck, printFont, Brushes.Black, 0, 0);
        }
        public string SendRequestPost(string URL, string subject)
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
        public string TryPostData(string data)
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

                        d.LoadXml(xml);
                        Ipaybox.Incass.bytesSend += data.Length;
                        Ipaybox.Incass.bytesRead += xml.Length;
                        Ipaybox.FlushStatistic();
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
        public bool SendStatistic()
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Формирование инкассации.");
            if (Ipaybox.Incass.incass_amount > 0)
            {
                string data = "<request>";
                data += "<protocol>1.00</protocol>";
                data += "<type>1.00</type>";
                data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
                data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";

                XmlDocument inc = new XmlDocument();
                inc.Load(Ipaybox.StartupPath + @"\incass.xml");

                XmlElement el = inc.CreateElement("incass-stop-date");
                el.InnerText = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                inc.DocumentElement.InsertAfter(el, inc.DocumentElement.LastChild);

                data += "<statistic> "+ inc.DocumentElement.InnerXml  + "</statistic>";
                data += "</request>";
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Посылаем инкассацию на сервер...");
                string response = TryPostData(data);

                if (response != "")
                {
                    try
                    {
                        XmlDocument resp = new XmlDocument();

                        resp.LoadXml(response);

                        XmlElement root = resp.DocumentElement;


                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            XmlElement row = (XmlElement)root.ChildNodes[i];

                            if (row.Name == "incass")
                            {
                                string res = row.GetAttribute("result");
                                string comment = row.GetAttribute("comment");

                                if (res != "0")
                                {
                                    // все плохо
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Сервер не принял инкассацию.");
                                    return false;
                                }
                                else
                                {
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Успешно. Формируем инкассационный чек.");
                                    string check = "IPAYBOX ZEUS CORE V" + Ipaybox.CoreVersion + "\r\n" +
                                        Ipaybox.Terminal.jur_name + "\r\nТерминал: " + Ipaybox.Terminal.terminal_id + "\r\n";
                                    check += "ИНКАССАЦИОННЫЙ ЧЕК\r\n_____________________\r\n";
                                    check += "СУММА: " + Ipaybox.Incass.incass_amount.ToString() + "\r\n";
                                    check += "Кол-во купюр: " + Ipaybox.Incass.count + "\r\n";
                                    check += "Кол-во купюр 10 руб: " + Ipaybox.Incass.CountR10 + "\r\n";
                                    check += "Кол-во купюр 50 руб: " + Ipaybox.Incass.CountR50 + "\r\n";
                                    check += "Кол-во купюр 100 руб: " + Ipaybox.Incass.CountR100 + "\r\n";
                                    check += "Кол-во купюр 500 руб: " + Ipaybox.Incass.CountR500 + "\r\n";
                                    check += "Кол-во купюр 1000 руб: " + Ipaybox.Incass.CountR1000 + "\r\n";
                                    check += "Кол-во купюр 5000 руб: " + Ipaybox.Incass.CountR5000 + "\r\n";
                                    check += "Кол-во чеков: " + Ipaybox.Incass.countchecks + "\r\n";
                                    check += "Дата начала: " + Ipaybox.Incass.incass_date_start.ToString() + "\r\n";
                                    check += "Дата инкасс: " + DateTime.Now.ToString() + "\r\n";
                                    check += "____________________\r\n";
                                    check += "Сервер инкасации: " + res + "\\" + comment + "\r\n";
                                    check += "Bytes send: " + Ipaybox.Incass.bytesSend + "\r\n";
                                    check += "Bytes recieve: " + Ipaybox.Incass.bytesRead + "\r\n";
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Печатаем чек.");
                                    PrintCheck = check;
                                    if (!Ipaybox.WindowsPrinter)
                                        Ipaybox.Printer.Print(check);
                                    else
                                    {

                                        doc.Print();
                                    }
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Чек распечатан. Очистка статистики.");
                                    Ipaybox.Incass.bytesRead = 0;
                                    Ipaybox.Incass.bytesSend = 0;
                                    Ipaybox.Incass.count = 0;
                                    Ipaybox.Incass.CountR10 = 0;
                                    Ipaybox.Incass.CountR50 = 0;
                                    Ipaybox.Incass.CountR100 = 0;
                                    Ipaybox.Incass.CountR500 = 0;
                                    Ipaybox.Incass.CountR1000 = 0;
                                    Ipaybox.Incass.CountR5000 = 0;
                                    Ipaybox.Incass.countchecks = 0;
                                    Ipaybox.Incass.incass_amount = 0;
                                    Ipaybox.Incass.incass_date_start = DateTime.Now;
                                    Ipaybox.FlushStatistic();
                                    Ipaybox.LoadIncass();
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Инкассация успешно проведена.");
                                    return true;


                                }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        HelperClass.CrashLog.AddCrash(ex);
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка при инкассации нераспознанный ответ сервера.\r\n-----\r\n"+response+"\r\n-----");
                        return false;
                    }


                }
                else
                {
                    return false;
                }
            }
            else
            { return true; }




            return false;
        }

        
    }
}
