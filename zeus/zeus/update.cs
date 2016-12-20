using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;


namespace zeus
{
    class UpdateCore
    {
        public enum FileCheckError { OK, UPDATE, NULL };
        XmlDocument list;
        CRC32 crc32;
        WebClient Client ;
        bool Needtorestart;

        public UpdateCore()
        {
            //list.Load("update.xml");
            crc32 = new CRC32();
            
            Client = new WebClient();

            Client.Headers.Add("user-agent", "IPAYBOX ZEUS UPDATECORE V0.8.12");
            Client.Headers.Add("Cache-Control", "no-cache");
            Client.Headers.Add("Pragma", "no-cache");
        }
        private string TryDownloadUpdateXml()
        { 
            bool t = false;
            int count = 0;
            string xml = "";

            while (!t && count < Ipaybox.UpdateUrl.Length*2)
            {
                try
                {
                    Client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

                    Stream str = new MemoryStream(Client.DownloadData(Ipaybox.UpdateUrl[Ipaybox.UpdateUrlIndex] + "update.xml"));;

                    if (Client.ResponseHeaders[HttpResponseHeader.ContentEncoding] != null)
                        if (Client.ResponseHeaders[HttpResponseHeader.ContentEncoding].ToLower().Contains("gzip"))
                        {
                            str = new GZipStream(str, CompressionMode.Decompress);
                        }
                        else
                        {
                            if (Client.ResponseHeaders[HttpResponseHeader.ContentEncoding].ToLower().Contains("deflate"))
                            {
                                str = new DeflateStream(str, CompressionMode.Decompress);
                            }
                        }

                    StreamReader sr = new StreamReader(str);

                    xml = sr.ReadToEnd();

                    try
                    {
                        list.LoadXml(xml);

                        if(Ipaybox.RequestTimeout!= 0.5)
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
                    Ipaybox.UpdateUrlIndex++;

                    if (Ipaybox.UpdateUrlIndex == Ipaybox.UpdateUrl.Length)
                        Ipaybox.UpdateUrlIndex = 0;
                    count++;
                }
            }

            return xml;
        }
        public void ValidateXmlAndLocal()
        {
            Ipaybox.UpdateState = 1;

            Ipaybox.AddToLog(Ipaybox.Logs.Update, "Запуск обновления. Download core file...");
            Ipaybox.NeedUpdates.Core = false;

            list = new XmlDocument();

            try
            {
                string xml = TryDownloadUpdateXml();

                if (xml != null)
                {
                    if (xml.Length > 0 && xml.IndexOf("result") ==  -1)
                    {
                        Ipaybox.Incass.bytesRead += xml.Length;
                        Ipaybox.FlushStatistic();
                        Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Успешно.");
                    }
                    else
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t  НЕ Успешно.");
                        // Останавливаем обновление  
                        Ipaybox.NeedUpdates.Core = true;
                        Ipaybox.Working = false;
                        return;
                    }
                }
                else
                {
                    //Останавливаем обновление
                    Ipaybox.NeedUpdates.Core = true;
                    Ipaybox.Working = false;
                    return;
                }
            }
            catch
            {
                //Останавливаем обновление
                Ipaybox.NeedUpdates.Core = true;
                Ipaybox.Working = false;
                return;
            }

            if (!(list.DocumentElement != null && list.DocumentElement.Name == "fileinfo"))
            {
                Ipaybox.NeedUpdates.Core = true;
                Ipaybox.Working = false;
                return;
            }

            XmlElement root = list.DocumentElement;

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == "file")
                {
                    // Сравнить данные по файлу
                    CheckDirExist(row.GetAttribute("filepath"));
                    FileCheckError err = ValidateLocalFile(row.GetAttribute("filepath"), row.GetAttribute("length"), row.GetAttribute("crc"));


                    //Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Проверка файла " + row.GetAttribute("filepath") +" - "+ err.ToString());
                    switch (err)
                    {
                       case FileCheckError.UPDATE:
                            bool result = DownloadFileGZ(row.GetAttribute("filepath"), row.GetAttribute("length"), row.GetAttribute("crc"));

                            if (!result)
                            { 
                                //Файл не скачался обновление остановить.

                                Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Файл не скачался обновление остановить.");
                    
                                Ipaybox.NeedUpdates.Core = true;
                                Ipaybox.Working = false;
                                return;
                                
                            }
                            break;
                    }
                }
            }
            if (Needtorestart)
                Ipaybox.NeedToRestart = true;

            Ipaybox.UpdateState = 0;
            Ipaybox.AddToLog(Ipaybox.Logs.Update, "Выход из обновления.");
        }
        public void CheckDirExist(string path)
        {
            string[] dirs = path.Split('\\');
            if (dirs.Length > 1)
            {
                string dr = Ipaybox.StartupPath;
                for (int i = 0; i < dirs.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(dirs[i]))
                    {
                        DirectoryInfo di = new DirectoryInfo(dr + "\\" + dirs[i]);
                        if (!di.Exists)
                            di.Create();
                        dr = dr + "\\" + dirs[i];
                    }
                }
            }
        }
        private bool DownloadFile(string file, string len, string crc)
        {
            bool ok = false;
            int counts=0;
            string ftpfile = "";
            string localfile ="";
            // Скачивать и заменять файло.

            if (file[0] == '\\')
            {
                ftpfile = Ipaybox.UpdateUrl[Ipaybox.UpdateUrlIndex] + file.Replace(@"\", "");
                localfile = Ipaybox.StartupPath + file;
            }
            else
            {
                ftpfile = Ipaybox.UpdateUrl[Ipaybox.UpdateUrlIndex] + file.Replace(@"\", "/");
                localfile = Ipaybox.StartupPath + "\\" + file;
            }
            while (!ok && counts < 3)
            {
                try
                {
                    if (file.Replace("\\","").LastIndexOf(".") > 0 && file.IndexOf("rstrt.exe") == -1)
                    {
                        var _fileName = file.Substring(0, file.LastIndexOf(".")).Replace("\\", "");
                            
                        var _fileExtension = file.Substring(file.LastIndexOf(".") + 1, file.Length - file.LastIndexOf(".") - 1);

                        switch (_fileExtension.ToLower())
                        { 
                            case "dll":
                            case"pdb":
                            case"exe":
                                localfile = Ipaybox.StartupPath + "\\" + _fileName + "." + _fileExtension + ".update";
                                Needtorestart = true;
                                break;
                        }
                    }

                    Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Скачиваем файл " + ftpfile);
                    Ipaybox.AddToLog(Ipaybox.Logs.Update, "Сохраняем как:" + localfile);
                    Client.DownloadFile(ftpfile, localfile);
                    FileInfo f = new FileInfo(localfile);
                    if(f.Exists)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Скачали");
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "\t Загружен." + ftpfile);
                        Ipaybox.Incass.bytesRead += (int)f.Length; 
                        Ipaybox.Incass.bytesSend += ftpfile.Length;
                        Ipaybox.Incassation.Bytessend += (uint)ftpfile.Length;
                        Ipaybox.Incassation.Bytesrecieved += (uint)f.Length; 
                        Ipaybox.FlushStatistic(); }

                }
                catch(Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "\t Не удалось " + ex.ToString());
                    Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Не удалось загрузить." + ftpfile);
                    return false;
                }
                if (ValidateLocalFile(localfile, len, crc) == FileCheckError.OK)
                {
                    ok = true;

                }
                else
                {
                    counts++;
                }
            }

            return ok;
        }
        private bool DownloadFileGZ(string file, string len, string crc)
        {
            bool ok = false;
            int counts = 0;
            string ftpfile = "";
            string localfile = "";
            // Скачивать и заменять файло.

            if (file[0] == '\\')
            {
                ftpfile = Ipaybox.UpdateUrl[Ipaybox.UpdateUrlIndex] + file.Replace(@"\", "");
                localfile = Ipaybox.StartupPath + file;
            }
            else
            {
                ftpfile = Ipaybox.UpdateUrl[Ipaybox.UpdateUrlIndex] + file.Replace(@"\", "/");
                localfile = Ipaybox.StartupPath + "\\" + file;
            }
            while (!ok && counts < 3)
            {
                try
                {
                    if (file.Replace("\\", "").LastIndexOf(".") > 0 && file.IndexOf("rstrt.exe") == -1)
                    {
                        var _fileName = file.Substring(0, file.LastIndexOf(".")).Replace("\\", "");

                        var _fileExtension = file.Substring(file.LastIndexOf(".") + 1, file.Length - file.LastIndexOf(".") - 1);

                        switch (_fileExtension.ToLower())
                        {
                            case "dll":
                            case "pdb":
                            case "exe":
                                localfile = Ipaybox.StartupPath + "\\" + _fileName + "." + _fileExtension + ".update";
                                Needtorestart = true;
                                break;
                        }
                    }

                    Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Скачиваем файл " + ftpfile);
                    Ipaybox.AddToLog(Ipaybox.Logs.Update, "Сохраняем как:" + localfile);

                    HttpWebRequest _req = (HttpWebRequest)HttpWebRequest.Create(ftpfile);
                    _req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                    HttpWebResponse _resp = (HttpWebResponse)_req.GetResponse();
                    Stream str = _resp.GetResponseStream();

                    if (_resp.ContentEncoding.ToLower().Contains("gzip"))
                        str = new GZipStream(str, CompressionMode.Decompress);
                    else
                        if (_resp.ContentEncoding.ToLower().Contains("deflate"))
                            str = new DeflateStream(str, CompressionMode.Decompress);

                    byte[] inBuf = new byte[100000];
                    int bytesReadTotal = 0;

                    FileStream fstr = new FileStream(localfile, FileMode.Create, FileAccess.Write);

                    while (true)
                    {
                        int n = str.Read(inBuf, 0, 100000);
                        if ((n == 0) || (n == -1))
                        {
                            break;
                        }

                        fstr.Write(inBuf, 0, n);

                        bytesReadTotal += n;
                    }

                    str.Close();
                    fstr.Close();

                    FileInfo f = new FileInfo(localfile);
                    if (f.Exists)
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Скачали");
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "\t Загружен." + ftpfile);
                        Ipaybox.Incass.bytesRead += (int)f.Length;
                        Ipaybox.Incass.bytesSend += ftpfile.Length;
                        Ipaybox.Incassation.Bytessend += (uint)ftpfile.Length;
                        Ipaybox.Incassation.Bytesrecieved += (uint)f.Length;
                        Ipaybox.FlushStatistic();
                    }
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "\t Не удалось " + ex.ToString());
                    Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t Не удалось загрузить." + ftpfile);
                    return false;
                }
                if (ValidateLocalFile(localfile, len, crc) == FileCheckError.OK)
                    ok = true;
                else
                    counts++;
            }

            return ok;
        }
        private FileCheckError ValidateLocalFile(string file, string length, string crc)
        {
            FileInfo fi = null;
            try
            {
                if(file.IndexOf(Ipaybox.StartupPath) == -1)
                    fi = new FileInfo(Ipaybox.StartupPath + "\\" + file.TrimStart(new char[] {'\\'}));
                else
                    fi = new FileInfo( file);
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t\t Проверка " + file + " " +ex.ToString());
                return FileCheckError.UPDATE;
            }
            
            if (!fi.Exists)
            {
                return FileCheckError.UPDATE;
            }

            crc32 = new CRC32();
            String crclocal = String.Empty;
            
            using (FileStream fs = fi.OpenRead()) //here you pass the file name
            {
                foreach (byte b in crc32.ComputeHash(fs))
                {
                    crclocal += b.ToString("x2").ToLower();
                }
            }
            if (length == fi.Length.ToString() && crc == crclocal)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t\t Проверка " + file + " (" + length + ":" + crc + ")(" + fi.Length.ToString() + ":" + crclocal + ")= OK");
                return FileCheckError.OK;
            }
            else
            {
                Match m = Regex.Match(file, @"([.]exe$)||([.]dll$)");

                if (m.Success)
                {
                    file += ".update";
                    return ValidateLocalFile(file, length, crc);
                }

                Ipaybox.AddToLog(Ipaybox.Logs.Update, "\t\t Проверка " + file + " (" + length + ":" + crc + ")(" + fi.Length.ToString() + ":" + crclocal + ")=UPDATE");
                return FileCheckError.UPDATE;
            }
        }

        // Настройки для подключения к фискальному серверу
        public void UpdateFRSSettings()
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка настроек удаленного ФР...");

            try
            {
                string data = "<request>";
                data += "<protocol>1.00</protocol>";
                data += "<type>1.00</type>";
                data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
                data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
                data += "<FRSsettings/>";
                data += "</request>";

                string resp = TryPostData(data);

                if (resp != "")
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Получен ответ от сервера...");
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(resp);

                        if (resp.IndexOf("result") == -1)
                        {
                            Ipaybox.FRSSettings.LoadXml(doc.InnerXml);

                            // Проверяем не пришел ли нам мусор...
                            if (Ipaybox.FRSSettings.DocumentElement.ChildNodes.Count > 0)
                                if (Ipaybox.FRSSettings.DocumentElement.ChildNodes[0].Name == "frs")
                                {
                                    if (Ipaybox.FRSSettings.DocumentElement.ChildNodes.Count > 0)
                                    {
                                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Загружены настройки для удаленного ФР...");
                                        Ipaybox.FRSSettings.Save(Ipaybox.StartupPath + "\\config\\frssettings.xml");

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

                                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Настройки удаленного ФР применены");
                                        }
                                        catch
                                        {
                                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось применить настройки фискального сервера");
                                        }
                                    }
                                }
                        }
                    }
                    catch
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Произошла ошибка при обработке ответа. Неправильный XML.");
                    }
                }
                else
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Ответ от сервера не получен.");
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка. " + ex.Message);
            }
        }
        // ПИН-коды
        public void UpdatePIN()
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка списка ПИН-кодов с сервера...");

            try
            {
                string data = "<request>";
                data += "<protocol>1.00</protocol>";
                data += "<type>1.00</type>";
                data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
                data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
                data += "<pincode/>";
                data += "</request>";

                string resp = TryPostData(data);

                if (resp != "")
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Получен ответ от сервера...");
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(resp);

                        if (resp.IndexOf("result") == -1)
                        {
                            Ipaybox.TPIN.LoadXml(doc.InnerXml);

                            // Проверяем не пришел ли нам мусор...
                            if (Ipaybox.TPIN.DocumentElement.ChildNodes.Count > 0)
                                if (Ipaybox.TPIN.DocumentElement.ChildNodes[0].Name == "pins")
                                {
                                    if (Ipaybox.TPIN.DocumentElement.ChildNodes[0].ChildNodes.Count > 0)
                                    {
                                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Загружены ПИН-коды...");
                                        Ipaybox.TPIN.Save(Ipaybox.StartupPath + "\\config\\tpin.xml");
                                    }
                                }
                        }
                    }
                    catch
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Произошла ошибка при обработке ответа. Неправильный XML.");
                    }
                }
                else
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Ответ от сервера не получен.");
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка. " + ex.Message);
            }
        }

        // Провайдеры
        public void UpdateProviderList()
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка списка провайдеров с сервера...");
            string data = "<request>";
            data += "<protocol>1.00</protocol>";
            data += "<type>1.00</type>";
            data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
            data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
            data += "<providerlist/>";
            data += "</request>";

            string resp = TryPostData(data);

            if (resp != "")
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Получен ответ от сервера...");
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resp);
                    if (resp.IndexOf("result") == -1)
                    {
                        Ipaybox.providers.LoadXml(doc.InnerXml);

                        // Проверяем не пришел ли нам мусор...
                        if (Ipaybox.providers.DocumentElement.ChildNodes.Count > 0)
                            if (Ipaybox.providers.DocumentElement.ChildNodes[0].Name == "providers")
                            {
                                Ipaybox.providers.Save(Ipaybox.StartupPath + "\\config\\providers.xml");
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Список провайдеров загружен...");
                                Ipaybox.NeedUpdates.ProviderList = false;
                            }
                    }
                }
                catch
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Произошла ошибка при обработке ответа. Неправильный XML.");
                }
            }
            else
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Ответ от сервера не получен.");
            }            
        }
        public string SendRequestPost(string URL, string subject)
        {
            string strOut = "";
            HttpWebResponse result = null;
            HttpWebRequest req = null;
            Stream newStream = null;
            Stream ReceiveStream = null;
            StreamReader sr = null;

            try
            {
                // Url запрашиваемого методом POST скрипта
                req = (HttpWebRequest)WebRequest.Create(URL);
                req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                req.Method = "POST";
                req.Timeout = 60000;
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
                result = (HttpWebResponse)req.GetResponse();
                ReceiveStream = result.GetResponseStream();

                if (result.ContentEncoding.ToLower().Contains("gzip"))
                    ReceiveStream = new GZipStream(ReceiveStream, CompressionMode.Decompress);
                else
                    if (result.ContentEncoding.ToLower().Contains("deflate"))
                        ReceiveStream = new DeflateStream(ReceiveStream, CompressionMode.Decompress);

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
         
        // Комиссия
        public void UpdateComiss(bool critical)
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка профилей комиссий...");
            XmlDocument comission = new XmlDocument();
            try
            {
                string xml = TryDownloadComiss(ref comission);

                if (xml != null)
                {
                    if (xml.Length > 0)
                    {
                        Ipaybox.Incass.bytesRead += xml.Length;
                        Ipaybox.FlushStatistic();

                        if (xml.IndexOf("result") == -1)
                        {
                            // нормальный
                            Ipaybox.comiss.LoadXml(xml);
                            
                            Ipaybox.comiss.Save(Ipaybox.StartupPath+"\\config\\comiss.xml");
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Профили успешно загружены.");
                            Ipaybox.NeedUpdates.Comission = false;
                        }
                        else
                        { 
                            // ошибка
                        }
                    }
                    else
                    {
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не удалось загрузить профили.");
                        if (critical)
                        {// Останавливаем обновление   
                        }
                        else
                        {// не заменяем  
                        }
                         
                        
                    }
                }
                else
                {
                    //Останавливаем обновление
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не удалось загрузить профили.");
                }
            }
            catch
            {
                //Останавливаем обновление
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Не удалось загрузить профили.");
            }
        }
        public string SendRequestGET(string URL)
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
                req.Method = "GET";
                req.Timeout = 60000;
                req.UserAgent = "Zeus ver. " + Ipaybox.CoreVersion + "; " + Ipaybox.Terminal.terminal_id;
                // эта строка необходима только при защите скрипта на сервере Basic авторизацией
                //req.Credentials = new NetworkCredential("login", "password");
                //req.ContentType = "application/x-www-form-urlencoded";
                //byte[] SomeBytes = null;
                // передаем список пар параметров / значений для запрашиваемого скрипта методом POST
                // в случае нескольких параметров необходимо использовать символ & для разделения параметров
                // в данном случае используется кодировка windows-1251 для Url кодирования спец. символов значения параметров
                //SomeBytes = Encoding.GetEncoding(1251).GetBytes("inputmessage=" + HttpUtility.UrlEncode(subject, Encoding.GetEncoding(1251)));
                //req.ContentLength = SomeBytes.Length;
                //newStream = req.GetRequestStream();
                //newStream.Write(SomeBytes, 0, SomeBytes.Length);
                //newStream.Close();
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

                byte[] cp1251bytes = Encoding.GetEncoding(1251).GetBytes(strOut);
                byte[] utfbytes = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, cp1251bytes);


                return Encoding.UTF8.GetString(utfbytes);
            }
            catch (Exception ex)
            {
                //string filename = "Sender_log_" + System.DateTime.Now.Year + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Day + ".txt";
                //FileInfo file = new FileInfo(filename);
                //StreamWriter sw;
                //if (File.Exists(filename))
                //    sw = file.AppendText();
                //else
                //    sw = file.CreateText();
                //sw.WriteLine("---------------------------------------------");
                //sw.WriteLine("Время по Вл-ку: " + System.DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:ms"));
                //sw.WriteLine("URL: " + URL);
                //sw.WriteLine("---------------------------------------------");
                //sw.WriteLine("------Содержание Запроса--------");
                ////sw.WriteLine(subject);
                //sw.WriteLine("------Конец      Запроса--------");
                //sw.WriteLine("Ответ от сервера");
                //sw.WriteLine("------Содержание Ответа---------");
                //sw.WriteLine(strOut);
                //sw.WriteLine("------Конец      Ответа---------");
                //sw.WriteLine("------Содержание Exception------");
                //sw.WriteLine(ex.Message);
                //sw.WriteLine("------Конец      Exception------");
                //sw.WriteLine("------Конец      Запроса--------");
                //sw.Close();
                //return ex.Message;
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
        private string TryDownloadComiss(ref XmlDocument d)
        {
            bool t = false;
            int count = 0;
            string xml = "";
            while (!t && count < Ipaybox.ServiceUrl.Length * 2)
            {
                try
                {
                    //xml = SendRequestGET(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "xml_comiss.exe?trm_id="+Ipaybox.Terminal.terminal_id+"&trm_p="+Ipaybox.Terminal.terminal_pass);
                    xml = Client.DownloadString(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "xml_comiss.exe?trm_id=" + Ipaybox.Terminal.terminal_id + "&trm_p=" + Ipaybox.Terminal.terminal_pass);
                   

                    try
                    {
                        //string utf8 = Encoding.UTF8.GetString(Encoding.Convert(Encoding.ASCII, Encoding.UTF8, Encoding.ASCII.GetBytes(xml)));
                       
                        d.LoadXml( xml);
                        Ipaybox.Incass.bytesSend += 15;
                        Ipaybox.Incass.bytesRead += xml.Length;
                        Ipaybox.FlushStatistic();
                        break ;
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

        // Терминал - Информация
        private string TryDownloadTerminalInfo()
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Загрузка информации о терминале.");
            XmlDocument d = new XmlDocument();
            bool t = false;
            int count = 0;
            string xml = "";
            while (!t && count < Ipaybox.ServiceUrl.Length * 2)
            {
                try
                {
                    //xml = SendRequestGET(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "xml_comiss.exe?trm_id="+Ipaybox.Terminal.terminal_id+"&trm_p="+Ipaybox.Terminal.terminal_pass);
                    xml = Client.DownloadString(Ipaybox.ServiceUrl[Ipaybox.ServiceUrlIndex] + "xml_terminal.exe?trm_id=" + Ipaybox.Terminal.terminal_id + "&trm_p=" + Ipaybox.Terminal.terminal_pass);

                    try
                    {
                        //string utf8 = Encoding.UTF8.GetString(Encoding.Convert(Encoding.ASCII, Encoding.UTF8, Encoding.ASCII.GetBytes(xml)));

                        d.LoadXml(xml);
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "...Успешно загружена.");
                        Ipaybox.Incass.bytesSend += 10;
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
        public void UpdateTerminalInfo(bool critical)
        {
            XmlDocument old = Ipaybox.terminal_info;
            try
            {
                string xml = TryDownloadTerminalInfo();

                if (xml != null)
                {
                    if (xml.Length > 0)
                    {
                        if (xml.IndexOf("result") == -1)
                        {
                            // нормальный
                           
                            ApplyTerminalInfo(xml);
                            Ipaybox.NeedUpdates.Trm_info = false;

                        }
                        else
                        {
                            // ошибка
                        }
                    }
                    else
                    {
                        if (critical)
                        {// Останавливаем обновление   
                        }
                        else
                        {// не заменяем  
                        }


                    }
                }
                else
                {
                    //Останавливаем обновление
                }
            }
            catch
            {
                //Останавливаем обновление
            }

        }
        public string GetTextFromRecivedXml(string xml, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlElement root = doc.DocumentElement;

            for(int i=0;i<root.ChildNodes.Count;i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];
                if(row.Name == name)
                    return row.InnerText;
            }

            return null;
        }
        //private void ApplyTerminalInfo(string xml)
        //{
        //    XmlElement root = Ipaybox.terminal_info.DocumentElement;

        //    for(int i=0;i<root.ChildNodes.Count;i++)
        //    {
        //        XmlElement row = (XmlElement)root.ChildNodes[i];

        //        string text = GetTextFromRecivedXml(xml, row.Name);

        //        if(text != null)
        //            row.InnerText = text;

        //    }

        //    Ipaybox.terminal_info.Save(Ipaybox.StartupPath + "\\config\\terminal.xml");
            
        //}
        private void SetNew(string name, string value)
        {
            bool exist = false;
            XmlElement root = Ipaybox.terminal_info.DocumentElement;


            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];

                if (row.Name == name )
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
        private void ApplyTerminalInfo (string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlElement root = doc.DocumentElement;

            string config = "<ipaybox>";


            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement row = (XmlElement)root.ChildNodes[i];
                if (row.Name == "top" || row.Name == "groups" || row.Name == "banners")
                {
                    config += "<" + row.Name + ">\r\n" + row.InnerXml + "</" + row.Name + ">\r\n";
                }
                else
                    SetNew(row.Name, row.InnerText);

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
            config += "</ipaybox>";

            XmlDocument d = new XmlDocument();
            d.LoadXml(config);

            Ipaybox.config = d;
            Ipaybox.config.Save(Ipaybox.StartupPath + "\\config\\config.xml");

            Ipaybox.terminal_info.Save(Ipaybox.StartupPath + "\\config\\terminal.xml");
        }
    }
}
