using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Drawing;

namespace zeus.HelperClass
{
    public static class Helper
    {
        public static Color GetColor(string rgbString)
        {
            string[] param = rgbString.Split(';');

            int red=0,green=0,blue=0;

            if (param.Length >= 3)
            {
                int.TryParse(param[0],out red);           
                int.TryParse(param[1],out green);
                int.TryParse(param[2],out blue);
            }

            return Color.FromArgb(red, green, blue);
        }
    }
}
