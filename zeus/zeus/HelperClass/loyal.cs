using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeus.HelperClass
{
    public class loyal
    {
        string account;
        string pwd;
        int discount;

        public bool isRegistered()
        { 
            if (pwd == null || pwd == "")
                return false;
            else
                return true;
        }
    }
}
