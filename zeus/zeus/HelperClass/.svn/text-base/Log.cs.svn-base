using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

namespace zeus.HelperClass
{
    public static class Helper
    {
        public static Color GetColor(string rgbString)
        {
            string[] param = rgbString.Split(';');

            int red = 0, green = 0, blue = 0;

            if (param.Length >= 3)
            {
                int.TryParse(param[0], out red);
                int.TryParse(param[1], out green);
                int.TryParse(param[2], out blue);
            }

            return Color.FromArgb(red, green, blue);
        }
    }
    public class CrashLog
    {
        public static void AddCrash(Exception crash)
        {
            if (Ipaybox.Debug)
                System.Diagnostics.Debug.WriteLine(crash.ToString());
            FileInfo fi = null;
            DirectoryInfo di = new DirectoryInfo(Ipaybox.StartupPath + "\\logs");
            if (!di.Exists)
                di.Create();
           
            
             fi = new FileInfo(Ipaybox.StartupPath + "\\logs\\crash.log"); 
            
            StreamWriter sw = null;
            if (fi.Exists)
                sw = fi.AppendText();
            else
                sw = fi.CreateText();

            sw.WriteLine(DateTime.Now.ToString() + " " + crash.ToString());
            sw.WriteLine("----------------------------");

            sw.Close();
            
            
        
        }
    }

    public class AccountInfo
    {
        public string Name { get; set; }
        public string XmlName { get; set; }
        public string Value { get; set; }
        public string ViewText { get; set; }
    }
}
