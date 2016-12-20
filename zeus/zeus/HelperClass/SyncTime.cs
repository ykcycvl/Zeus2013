using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace zeus.HelperClass
{
    public class SyncTime
    {
        private string[] _url;
        private string data;
        public SyncTime(string URL)
        {
            _url = new string[1]{URL};
            CreatePacket();
        }
        public SyncTime(string[] URLs)
        {
            _url = URLs;
            CreatePacket();
        }
        private void CreatePacket()
        {
            data = "<request>";
            data += "<protocol>1.00</protocol>";
            data += "<type>1.00</type>";
            data += "<terminal>" + Ipaybox.Terminal.terminal_id + "</terminal>";
            data += "<pass>" + Ipaybox.Terminal.terminal_pass + "</pass>";
            data += "<sync-time/></request>";
        }/*
        public void Sync()
        {
            DateTime old = new DateTime();
           
            TimeSpan ts = new TimeSpan();


            string strOut = "";
            WebResponse result = null;
            WebRequest req = null;
            Stream newStream = null;
            Stream ReceiveStream = null;
            StreamReader sr = null;
            try
            {
                // Url запрашиваемого методом POST скрипта
                req = WebRequest.Create(_url);
                req.Method = "POST";
                req.Timeout = 90000;
                // эта строка необходима только при защите скрипта на сервере Basic авторизацией
                //req.Credentials = new NetworkCredential("login", "password");
                //req.ContentType = "application/x-www-form-urlencoded";
                byte[] SomeBytes = null;
                // передаем список пар параметров / значений для запрашиваемого скрипта методом POST
                // в случае нескольких параметров необходимо использовать символ & для разделения параметров
                // в данном случае используется кодировка windows-1251 для Url кодирования спец. символов значения параметров
                SomeBytes = Encoding.GetEncoding(1251).GetBytes(HttpUtility.UrlEncode(data, Encoding.GetEncoding(1251)));
                req.ContentLength = SomeBytes.Length;

                old = DateTime.Now;

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
                ts = DateTime.Now - old;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strOut);

                XmlElement el = (XmlElement)doc.DocumentElement.SelectSingleNode("servertime");

                string dt = el.GetAttribute("time");
                DateTime serverDate;

                if (!DateTime.TryParse(dt, out serverDate))
                    throw new Exception("Невозможно получить серверное время");

                DateTime newdt = serverDate.AddSeconds( ts.TotalSeconds / 2);

                HelperClass.SystemTime.SetSystemTime(newdt);

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
        */
        public void TrySyncTime(int count)
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Запуск процесса синхронизации времени...");
            bool ok = false;
            int i = 0;
            while( i < count && !ok)
            {
                for (int j = 0; j < _url.Length; j++)
                {
                    try 
                    {
                        if (TryPostData(_url[j]+"sevice.exe"))
                        {
                            ok = true;
                            break;
                        }
                    }
                    catch
                    {
                        //System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
                i++;
            }

            if (!ok)
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Время не синхронизировано.");
        }
        public bool TryPostData(string url)
        {
            DateTime old = new DateTime();
            TimeSpan ts = new TimeSpan();

            string strOut = "";
            WebResponse result = null;
            HttpWebRequest req = null;
            Stream newStream = null;
            Stream ReceiveStream = null;
            StreamReader sr = null;
            try
            {
                // Url запрашиваемого методом POST скрипта
                req = (HttpWebRequest)WebRequest.Create(url);
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
                SomeBytes = Encoding.GetEncoding(1251).GetBytes(HttpUtility.UrlEncode(data, Encoding.GetEncoding(1251)));
                req.ContentLength = SomeBytes.Length;

                old = DateTime.Now;

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
                ts = DateTime.Now - old;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strOut);

                Match m = Regex.Match(strOut, @"<restart>(?<val>.*?)</restart>", RegexOptions.Singleline|RegexOptions.IgnoreCase);

                if (m.Success)
                {
                    if (m.Groups["val"].ToString().Trim() == "1")
                    {
                        Ipaybox.NeedToRestart = true;
                    }
                    else
                    {
                        if (m.Groups["val"].ToString().Trim() == "2")
                        {
                            Ipaybox.NeedToReboot = true;
                        }
                        else
                            if (m.Groups["val"].ToString().Trim() == "3")
                            {
                                Ipaybox.NeedShutdown = true;
                            }
                    }
                }

                XmlElement el = (XmlElement)doc.DocumentElement.SelectSingleNode("servertime");

                string dt = el.GetAttribute("time");
                DateTime serverDate;

                if (!DateTime.TryParse(dt, out serverDate))
                    throw new Exception("Невозможно получить серверное время");

                DateTime newdt = serverDate.AddSeconds(ts.TotalSeconds / 2);

                HelperClass.SystemTime.SetSystemTime(newdt);
                return true;
            }
            catch
            {
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
            return false;
        }
    }
}
