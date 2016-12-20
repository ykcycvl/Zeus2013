using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace WD
{
    public class Independed 
    {
        public enum WD {NULL,OSMP,SOBAKA};

        public WD watch;
        Osmp osmp ;
        Sobaka sobaka;
        public string Version = "";
        public Independed(string[] ports)
        {
            watch = WD.NULL;

            List<string> cmports = new List<string>();

            try
            {
                RegistryKey comdevices = Registry.LocalMachine.OpenSubKey("HARDWARE").OpenSubKey("DEVICEMAP").OpenSubKey("SERIALCOMM");
                string[] devices = comdevices.GetValueNames();

                foreach (string s in devices)
                {
                    Match m = Regex.Match(s, @"QCUSB_COM", RegexOptions.IgnoreCase);

                    if (m.Success)
                    {
                        Object o1 = comdevices.GetValue(s);
                        cmports.Add(o1.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
            }

            for (int i = 0; i < ports.Length; i++)
            {
                if (cmports.Count > 0)
                {
                    string s = cmports.Find(p => p == ports[i]);

                    if (!String.IsNullOrEmpty(s))
                        continue;
                }

                try
                {
                    osmp = new Osmp(ports[i]);
                    string ident = osmp.Identification();

                    if (ident.IndexOf("WDT") > -1)
                    {
                        Version = ident;
                        watch = WD.OSMP;
                        break;
                    }
                    else { osmp.Close(); }
                }
                catch (Exception ex)
                { }
            }

            if (watch == WD.NULL)
            {
                try
                {
                    sobaka = new Sobaka();

                    byte WDTType = sobaka.DrvDevType();

                    if (WDTType != 0)
                    {
                        Version = "Type " + WDTType.ToString();
                        watch = WD.SOBAKA;
                    }
                }
                catch (Exception ex)
                { 
                }
            }
        }
        ~Independed()
        {
            switch (watch)
            {
                case WD.OSMP: osmp.Close(); break;
            }
        }
        public bool ResetModem()
        {
            switch (watch)
            {
                case WD.OSMP: return osmp.ResetModem(); break;
                case WD.SOBAKA: return sobaka.ResetModem(); break;
            }

            return false;
        }
        public bool StartTimer()
        {
            switch (watch)
            {
                case WD.OSMP: return osmp.StartTimer(); break;
            }
            return false;
        }
        public bool StopTimer()
        {
            switch (watch)
            {
                case WD.OSMP: return osmp.StopTimer(); break;
                case WD.SOBAKA: return sobaka.StopTimer(); break;
            }
            return false;
        }
        public bool ResetTimer()
        {
            switch (watch)
            {
                case WD.OSMP: return osmp.ResetTimer(); break;
                case WD.SOBAKA: return sobaka.StopTimer(); break;
            }
            return false;
        }
    }
}
