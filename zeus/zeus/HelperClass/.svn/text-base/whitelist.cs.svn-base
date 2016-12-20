using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WhiteList
{
    public class WhiteListItem
    {
        public string phoneNumber = string.Empty;
        public int checkSum = -21;
    }

    public class WhiteList : List<WhiteListItem>
    {
        private string filename = zeus.Ipaybox.StartupPath + @"\config\whitelist.xml";

        public void Load()
        {
            if (File.Exists(filename))
            {
                WhiteList wl = (WhiteList)Noti.Shared.Helper.Serializer.Deserialize(typeof(WhiteList), File.ReadAllText(filename));

                foreach (WhiteListItem wli in wl)
                {
                    this.Add(wli);
                }
            }
        }

        public void Save()
        {
            File.WriteAllText(filename, Noti.Shared.Helper.Serializer.Serialize(typeof(WhiteList), this));
        }
    }

    public static class PhoneNumber
    {
        public static WhiteList GetAll()
        {
            WhiteList wl = new WhiteList();
            wl.Load();
            return wl;
        }

        public static void Delete(string phonenumber)
        {
            WhiteList wl = new WhiteList();
            wl.Load();
            WhiteListItem wli = wl.Find(p => p.phoneNumber == phonenumber);
            wl.Remove(wli);
            wl.Save();
        }

        public static void Add(string phonenumber)
        {
            if (phonenumber.Trim() == string.Empty)
            {
                return;
            }
            
            WhiteList wl = new WhiteList();
            WhiteListItem wli = new WhiteListItem();
            wli.phoneNumber = phonenumber;
            wli.checkSum = CRC32.Compute(phonenumber);
            wl.Load();
            wl.Add(wli);
            wl.Save();
        }

        public static bool Exists(string phonenumber)
        {
            WhiteList wl = new WhiteList();
            wl.Load();

            WhiteListItem wli = wl.Find(p => p.phoneNumber == phonenumber);

            if (wli != null)
            {
                if (wli.checkSum == CRC32.Compute(phonenumber))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
