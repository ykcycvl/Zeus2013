using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace zeus.API
{
    public class Import
    {
        string _filename;

        XmlDocument _doc;

        public Import()
        {
            _doc = new XmlDocument();
            _filename = "import1.xml";
            Load();
        }
        public Import(string filename)
        {
            _doc = new XmlDocument();
            _filename = filename;
            Load();
        }
        /// <summary>
        /// Весь документ 
        /// </summary>
        public string InnerXml
        { get { return _doc != null? _doc.InnerXml:null; } }
        
        private void Load()
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(_filename);

            if (fi.Exists)
            {
                try
                {
                    _doc.Load(fi.FullName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else { _doc.LoadXml("<import></import>"); }
        }
        private void Save()
        {
            /*
            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(_filename);

                System.IO.StreamWriter sw = fi.CreateText();
                sw.Write(_doc.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно сохранить Файл `" + _filename + "`\r\n" + ex.Message);
            }
             */

            try
            {
                // Open the writer

                XmlTextWriter myWriter = new XmlTextWriter(_filename, Encoding.UTF8);

                // Indented for easy reading

                myWriter.Formatting = Formatting.Indented;

                // Write the file

                _doc.WriteTo(myWriter);

                // Close the writer

                myWriter.Close();

            }
            catch (Exception ex)
            {

            }



        }
        public void Add(string xml)
        {
            try
            {
                XmlDocument n = new XmlDocument();
                n.LoadXml(xml);
                foreach (XmlElement el in n.DocumentElement.ChildNodes)
                { Add(el); Save(); }

            }
            catch (Exception ex)
            {
                throw new Exception("Неправильный XML\r\n" + ex.Message);
            }

        }
        public void Add(XmlElement xml)
        {
            try
            {
                _doc.DocumentElement.AppendChild(_doc.ImportNode(xml, true));
                Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно импортировать XmlNode.\r\n" + ex.Message);
            }
        }
        public string[] GetNodeNameList()
        {
            List<string> output = new List<string>();
            try
            {
                foreach (XmlElement el in _doc.DocumentElement.ChildNodes)
                {
                    if (!output.Contains(el.Name))
                        output.Add(el.Name);
                }
                
                string[] arr = new string[output.Count];
                output.CopyTo(arr);


                return arr;
            }
            catch (Exception ex)
            { HelperClass.CrashLog.AddCrash(new Exception("IMPORT::GetNodeNameList\r\n" + ex.Message)); }
            
            return null;
        }
        public string this[int index]
        {
            get
            {
                if (index < 0 || index > _doc.DocumentElement.ChildNodes.Count)
                    throw new IndexOutOfRangeException();

                return _doc.DocumentElement.ChildNodes[index].OuterXml;
            }
        }
        public int ChildCount
        {
            get { return _doc.DocumentElement.ChildNodes.Count; }
        }
        public int CountNode(string node)
        {
            int c = 0;
            try
            {
                foreach (XmlElement el in _doc.DocumentElement.ChildNodes)
                {
                    if (el.Name == node)
                        c++;
                }
            }
            catch (Exception ex)
            { HelperClass.CrashLog.AddCrash(new Exception("IMPORT::CountNode\r\n" + ex.Message)); }

            return c;
        }
        public void Remove(string nodename, string attribute, string value, bool depth)
        {
            bool anychanges = false;
            foreach (XmlElement el in _doc.DocumentElement.ChildNodes)
            {
                if (el.Name == nodename && el.GetAttribute(attribute) == value)
                {
                    _doc.DocumentElement.RemoveChild(el);
                    anychanges = true;
                }
            }
            if (anychanges)
                Save();
        }
    }
}
