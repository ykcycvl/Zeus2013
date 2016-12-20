using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Web;
using Ionic.Zip;
using System.Net.Mail;

namespace zeus
{
    

    class SendPays
    {


        /// <summary>
        /// Как будем стучаться на сервер
        /// </summary>
        public enum Type
        { Iteraction, AllInOne }

        XmlDocument req;
        public string XML_Response;
     

        public SendPays()
        {
          
            req = new XmlDocument();
            req.LoadXml("<request></request>");
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
        public string SendRequestPost(string URL, string subject, int RequestTimeOut)
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
                //req.Headers.Add("Accept-Encoding", "gzip");
                req.Method = "POST";
                req.Timeout = RequestTimeOut;
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

                string p = result.Headers.Get("Content-Encoding");
                if (!string.IsNullOrEmpty(p) && p.Contains("gzip"))
                {
                    

                    strOut = API.GZipDecompress.DecompressString(strOut);
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

                            if (!File.Exists(name))
                            {
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Запрошенный файл `"+name+"` не найден!");
                                break;
                            }

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
                                            if (SMTP("Терминал " + Ipaybox.Terminal.terminal_id, "Файл " + name + " с терминала №" + Ipaybox.Terminal.terminal_id+ " "+Ipaybox.Terminal.trm_adress, fi.FullName, email))
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
                    int MaxFileSize = 1024*1024; //
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
                if(IsFileAttached)
                    Ipaybox.Incass.bytesSend += (int)(fi.Length + fi.Length / 3);
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Файл <" + file + "> успешно отправлен на <" + email + ">");
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
        
        public string XmlSendPayCount(int c_pays)
        {
            string pays="";
            for (int i = 0; i < c_pays; i++)
            {
                XmlElement el = (XmlElement)Ipaybox.pays.DocumentElement.ChildNodes[i];
                pays += "<pay ";
                for(int j=0;j<el.Attributes.Count;j++)
                {
                     pays += el.Attributes[j].Name + "=\""+el.Attributes[j].Value+"\" ";
                     
                }
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Готовим платеж к отправке №" + el.GetAttribute("txn_id"));

                pays += ">";  
                pays += el.InnerXml;
                pays += "</pay> ";
            }
            return pays;
        }

        private bool SendImportAll()
        {
            API.HTTP http1 = new zeus.API.HTTP(90000);

            XmlDocument request = new XmlDocument();
            request.LoadXml("<request></request>");
            /*
                         <protocol>1.00</protocol>";
                data += "<type>1.00</type>";
                data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
                data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
             
             */

            XmlElement p = request.CreateElement("protocol");
            p.InnerText = "1.00";
                request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            p = request.CreateElement("type");
            p.InnerText = "1.00";
                request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            p = request.CreateElement("terminal");
            p.InnerText = Ipaybox.Terminal.terminal_id;
                request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            p = request.CreateElement("pass");
            p.InnerText = Ipaybox.Terminal.terminal_pass;
            request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            XmlDocument import = new XmlDocument();
           
            XmlDocument pays = new XmlDocument();
            pays.LoadXml("<p></p>");
            int count = 0;

            import.LoadXml(Ipaybox.Import.InnerXml);

            foreach (XmlElement import_child in import.DocumentElement)
            {
                if (import_child.Name == "pay")
                {
                    count++;
                    pays.DocumentElement.AppendChild(pays.ImportNode(import_child,true));
                }
                else
                    request.DocumentElement.AppendChild(request.ImportNode(import_child, true));
            }

            if(count >0)
            {
                string xml = "<a><pays count=\"" + count.ToString() + "\">" + pays.DocumentElement.InnerXml + "</pays></a>";
               XmlDocument newdoc = new XmlDocument();
                newdoc.LoadXml(xml);
                request.DocumentElement.AppendChild(request.ImportNode(newdoc.DocumentElement.ChildNodes[0], true));
            }

            XmlDocument doc = http1.Send(request.OuterXml);

            if (doc != null)
            {
                XmlElement el = doc.DocumentElement;
                ParseResponse(el);

                return true;
            }
            else
                return false;

        }
        private XmlDocument XmlHeader()
        {
            XmlDocument request = new XmlDocument();
            request.LoadXml("<request></request>");
            /*
                         <protocol>1.00</protocol>";
                data += "<type>1.00</type>";
                data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
                data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
             
             */

            XmlElement p = request.CreateElement("protocol");
            p.InnerText = "1.00";
            request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            p = request.CreateElement("type");
            p.InnerText = "1.00";
            request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            p = request.CreateElement("terminal");
            p.InnerText = Ipaybox.Terminal.terminal_id;
            request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            p = request.CreateElement("pass");
            p.InnerText = Ipaybox.Terminal.terminal_pass;
            request.DocumentElement.InsertAfter(p, request.DocumentElement.FirstChild);

            return request;
        }
        private bool SendImportIteraction()
        {
            API.HTTP http1 = new zeus.API.HTTP(90000);
            // Формируем щапку запроса
            XmlDocument request = XmlHeader();
           
            XmlDocument import = new XmlDocument();

            import.LoadXml(Ipaybox.Import.InnerXml);

            if(import.DocumentElement.ChildNodes.Count == 0)
                return true;

            XmlElement import_child = (XmlElement) import.DocumentElement.ChildNodes[0];

            if(import_child.Name == "pay")
            {
                string xml = "<a><pays count=\"1\">"+import_child.OuterXml+"</pays></a>";
                XmlDocument pays = new XmlDocument();
                pays.LoadXml(xml);
                import_child = (XmlElement)pays.DocumentElement.ChildNodes[0];
            }

            if (import_child != null)
            {
                request.DocumentElement.AppendChild(request.ImportNode(import_child, true));

                XmlDocument doc = http1.Send(request.OuterXml);

                if (doc != null)
                {
                    XmlElement el = doc.DocumentElement;
                    ParseResponse(el);
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        public bool SendImport(Type type)
        {
            if (type == Type.AllInOne)
            {
                return SendImportAll();
            }
            else
            {
                return SendImportIteraction();
            }
        }

        public bool SendAllPays()
        {
            if (Ipaybox.pays.DocumentElement.ChildNodes.Count > 0)
            {
                string data = "<request>";
                data += "<protocol>1.00</protocol>";
                data += "<type>1.00</type>";
                data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
                data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
                if (Ipaybox.pays.DocumentElement.ChildNodes.Count > 5)
                {

                    data += "<pays count=\"1\">" + XmlSendPayCount(1) + "</pays>\r\n";
                }
                else
                {
                    data += "<pays count=\"" + Ipaybox.pays.DocumentElement.ChildNodes.Count + "\">" + Ipaybox.pays.DocumentElement.InnerXml + "</pays>\r\n";

                    foreach (XmlElement row in Ipaybox.pays.DocumentElement)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Готовим платеж к отправке №"+ row.GetAttribute("txn_id"));
                    }
                }
                

                //data += "<pays count=\"" + Ipaybox.pays.DocumentElement.ChildNodes.Count + "\">" + Ipaybox.pays.DocumentElement.InnerXml + "</pays>\r\n";
                //data += GetFileToSend();
                data += "</request>";

                // отправляем файл если надо    
                SendFiles();

                string response = TryPostData(data);

                if (response != "")
                {
                    XML_Response = response;

                    XmlDocument resp = new XmlDocument();
                    try
                    {
                        resp.LoadXml(response);
                    }
                    catch { return false; }

                    return ParseResponse(resp.DocumentElement);
                }
                else
                {
                    return false;
                }
            }
            else
            { 
                return true; 
            }
        }
        private bool ParseResponse(XmlElement el)
        {
            bool res = true;
            for (int i = 0; i < el.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)el.ChildNodes[i];

                if (row.Name == "pay")
                {
                    string txn_id = row.GetAttribute("txn_id");
                    string state = row.GetAttribute("state");
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Получен ответ txn_id="+txn_id+" state="+state);
                    if (state == "0")
                    {
                        try
                        {
                            Ipaybox.Import.Remove("pay", "txn_id", txn_id, true);
                            XmlNode del = FindPayNode(txn_id);

                            if (del != null)
                            {
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Удаляем платеж из import.xml");
                                Ipaybox.pays.DocumentElement.RemoveChild(del);
                            }
                        }
                        catch(Exception ex)
                        {
                            if (Ipaybox.Debug)
                                HelperClass.CrashLog.AddCrash(new Exception("PARSE:PAY\r\n" + ex.Message));
                        }
                    }

                }
                if (row.Name == "incass")
                {
                    string id = row.GetAttribute("id");
                    string state = row.GetAttribute("result");
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Инкассация id=" + id + " state=" + state);
                    if (state == "0")
                    {
                        try
                        {
                            Ipaybox.Import.Remove("statistic", "id", id, true);
                            
                        }
                        catch(Exception ex)
                        {
                            if (Ipaybox.Debug)
                                HelperClass.CrashLog.AddCrash(new Exception("PARSE:INCASS\r\n"+ex.Message));
                        }
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

                /*if (row.Name == "configuration-id")
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
                }*/
            }
            Ipaybox.pays.Save("import.xml");
            if (Ipaybox.pays.DocumentElement.ChildNodes.Count == 0)
                return res;

            return !res;


        }
        private XmlNode FindPayNode(string txn_id)
        {
            XmlElement root = Ipaybox.pays.DocumentElement;

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == "pay" && row.GetAttribute("txn_id") == txn_id)
                    return root.ChildNodes[i];
            }

            return null;
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
                        if (Ipaybox.RequestTimeout != 0.5)
                            Ipaybox.RequestTimeout = float.Parse("0,5");

                        break;
                    }
                    catch
                    {
                        count++;
                        Ipaybox.RequestTimeout += 3;
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
        private string TryPostData(string data, int RequestTimeOut)
        {
            XmlDocument d = new XmlDocument();
            bool t = false;
            int count = 0;
            string xml = "";
            while (!t && count < Ipaybox.ServiceUrl.Length * 2)
            {
                try
                {
                    xml = SendRequestPost(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "sevice.exe", data, RequestTimeOut);

                    if (xml != "")
                    {
                        Ipaybox.Incass.bytesSend += data.Length;
                        Ipaybox.Incass.bytesRead += xml.Length;
                        Ipaybox.FlushStatistic();
                    }

                    d.LoadXml(xml);

                    if (Ipaybox.RequestTimeout != 0.5)
                        Ipaybox.RequestTimeout = float.Parse("0,5");

                    break;
                }
                catch(Exception ex)
                {
                    Ipaybox.ServiceUrlIndex++;

                    if (Ipaybox.ServiceUrlIndex == Ipaybox.ServiceUrl.Length)
                        Ipaybox.ServiceUrlIndex = 0;

                    count++;

                    Ipaybox.RequestTimeout += 3;
                }
            }

            return xml;
        }
        public int TestServer()
        {
            string data = "<request>";
            data += "<protocol>1.00</protocol>";
            data += "<type>1.00</type>";
            data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
            data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
           data += "</request>";

            string response = TryPostData(data,5000);

            if (response != "")
            {


                if (response.IndexOf("result") == -1)
                    return 0;
                else
                    return 1;
            }
            else
            {
                return 3;
            }
        }
        public bool SendPay(string cmd, string prv_id, string trm_id, string trm_p, string account, string txn_id, string summ, string to_summ, string txn_date)
        {
            string param = string.Format("cmd={0}&prv_id={1}&trm_id={2}&trm_p={3}&account={4}&txn_id={5}&amount={6}&to_amount={7}&txn_date={8}",
                cmd, prv_id, trm_id, trm_p, account, txn_id, summ, to_summ, txn_date);
            try
            {
                string xml = TryDownloadOnLine(param);

                if (xml != null)
                {
                    if (xml.Length > 0)
                    {
                        return true;

                    }
                }
            }
            catch
            {
                //Останавливаем обновление               
            }

            return false;
        }
        private string TryDownloadOnLine(string param)
        {
            XmlDocument d = new XmlDocument();
            bool t = false;
            int count = 0;
            string xml = "";
            while (!t && count < Ipaybox.ServiceUrl.Length * 2)
            {
                try
                {
                    WebClient Client = new WebClient();
                    
                    //xml = SendRequestGET(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "xml_comiss.exe?trm_id="+Ipaybox.Terminal.terminal_id+"&trm_p="+Ipaybox.Terminal.terminal_pass);
                    xml = Client.DownloadString(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "xml_online.exe?" + param);
                    try
                    {
                        //string utf8 = Encoding.UTF8.GetString(Encoding.Convert(Encoding.ASCII, Encoding.UTF8, Encoding.ASCII.GetBytes(xml)));

                        d.LoadXml(xml);
                        Ipaybox.Incass.bytesSend += 10;
                        Ipaybox.Incass.bytesRead += xml.Length;
                        Ipaybox.FlushStatistic();

                        XML_Response = xml;
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
