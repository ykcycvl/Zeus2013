using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace zeus.HelperClass
{
    public class Settings
    {
        private string _filename;
        private bool _saveOnChange;

        NameValueCollection nvc;
        /// <summary>
        /// Сохранять ли изменения сразу после изменений.
        /// </summary>
        public int Count
        { 
            get {return nvc.Count;}
        }
        public bool SaveOnChanges
        {
            get { return _saveOnChange; }
            set { _saveOnChange = value; }
        }
        private void LoadSettings()
        {
            if (nvc.Count != 0)
                nvc.Clear();

            FileInfo fi = new FileInfo(_filename);
            if (fi.Exists)
            {

                try
                {
                    StreamReader sr = fi.OpenText();
                    string xml = sr.ReadToEnd();
                    sr.Close();

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(xml);

                    foreach (System.Xml.XmlElement el in doc.DocumentElement)
                    {
                        nvc.Add(el.Name, el.InnerText);
                    }
                }
                catch(Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Невозможно открыть/прочитать файл с настройками.\r\n" + ex.Message);
                }
            }
        }
        public void Remove(string Param)
        {
            if (nvc[Param] != null)
            {
                nvc.Remove(Param);
                SaveSettings();
            }
        }
        public void SaveSettings()
        {
            if (nvc.Count > 0)
            {
                StringBuilder sb = new StringBuilder("<settings>\r\n");
                for (int i = 0; i < nvc.Count; i++)
                {
                    sb.Append("<");
                    sb.Append(nvc.GetKey(i));
                    sb.Append(">");
                    sb.Append(nvc[i]);
                    sb.Append("</");
                    sb.Append(nvc.GetKey(i));
                    sb.Append(">\r\n");
                }
                sb.Append("</settings>");

                FileInfo fi = new FileInfo(_filename);

                try
                {
                    StreamWriter sw = fi.CreateText();
                    sw.WriteLine(sb.ToString());
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Невозможно сохранить файл с настройками.\r\n" + ex.Message);
                }
                
            }
        
        }
        public Settings()
        {
            nvc = new NameValueCollection();
            DirectoryInfo di = new DirectoryInfo("config\\");
            if (!di.Exists)
                di.Create();


            _filename = "config\\settings.xml";

            LoadSettings();
        }
        public Settings(string FullPath)
        {
            nvc = new NameValueCollection();
            _filename = FullPath;
            FileInfo fi = new FileInfo(_filename);
            if (!fi.Exists)
            {
                try { fi.CreateText(); }
                catch (Exception ex) 
                { throw new Exception("Невозможно создать файл:\r\n"+_filename+"\r\n"+ ex.Message); }
            }
        }

        public string this[string Name]
        {
            get{return nvc[Name];}
            set
            { 
                if (nvc[Name] == null)
                    nvc.Add(Name, value);
                else
                    nvc[Name] = value;
                
                if (_saveOnChange)
                    SaveSettings();
                 
            }
        }
    }
}
