using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace zeus.API
{
    public enum HTTPMetods { POST, GET };
    /// <summary>
    /// Transport Layer
    /// </summary>
    class HTTP
    {
        int activeurl = 0;
        /// <summary>
        /// Список URL
        /// </summary>
        private List<string> _url;

        public List<string> URL
        {
            get { return _url; }
            set { _url = value; }
        }
        /// <summary>
        /// Connection Timeout
        /// </summary>
        private int _timeout;
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        
        public string ActiveURL
        { get { return _url[activeurl]; } }
        
        private void Init()
        {
            _url = new List<string>();
            _url.AddRange(Ipaybox.ServiceUrl);
        }
        public HTTP()
        {
            _timeout = 60000; // = 60 сек
            Init();
        }
        /// <summary>
        /// Конструктор с указанием Timeout (мс)
        /// </summary>
        /// <param name="Timeout">Интервал ожидания ответа (мс)</param>
        public HTTP(int Timeout)
        {
            _timeout = Timeout; 
            Init();        
        }
        public string Send(string url, string data)
        {
            string strOut = "";
            WebResponse result = null;
            HttpWebRequest req = null;
            System.IO.Stream newStream = null;
            System.IO.Stream ReceiveStream = null;
            System.IO.StreamReader sr = null;
            try
            {
                // Url запрашиваемого методом POST скрипта
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.Timeout = _timeout;
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
                newStream = req.GetRequestStream();
                newStream.Write(SomeBytes, 0, SomeBytes.Length);
                newStream.Close();
                // считываем результат работы
                result = req.GetResponse();
                ReceiveStream = result.GetResponseStream();
                Encoding encode = Encoding.GetEncoding(1251);
                sr = new System.IO.StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);

                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    strOut += str;
                    count = sr.Read(read, 0, 256);
                }

                Ipaybox.Incassation.Bytesrecieved = Ipaybox.Incassation.Bytesrecieved + (uint)strOut.Length;
                Ipaybox.Incassation.Bytessend = Ipaybox.Incassation.Bytessend + (uint)data.Length;

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
        public System.Xml.XmlDocument Send(string data)
        {
            for (int i = 0; i < _url.Count; i++)
            {
                try
                {
                    string resp = Send(_url[i]+"sevice.exe", data);
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(resp);
                    activeurl = i;
                    return doc;
                }
                catch
                {
                    //if (Ipaybox.Debug)
                    //    HelperClass.CrashLog.AddCrash(new Exception("HTTP:SEND(DATA)\r\n" + ex.Message));
                }

            }
            return null;
        }
        public string Send(System.Collections.Specialized.NameValueCollection vars)
        { return ""; }

        
      

    }
}
