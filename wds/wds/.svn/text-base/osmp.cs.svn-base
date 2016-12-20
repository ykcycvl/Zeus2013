using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;


namespace WD
{
    public class Osmp  
    {
        public string port;
        SerialPort com = new SerialPort();
        public Osmp(string port)
        {
            try
            {
                com.PortName = port;
                com.BaudRate = 9600;
                com.Parity = Parity.None;
                com.StopBits = StopBits.One;
                com.WriteTimeout = 10000;
                com.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        ~Osmp()
        {
            if (com.IsOpen)
                com.Close();
        }
        public string Identification()
        {
            byte[] ident = new byte[] { 0x4F, 0x53, 0x50, 0x01 };

            com.Write(ident, 0, ident.Length);

            System.Threading.Thread.Sleep(1000);

            if (com.BytesToRead > 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, ans.Length);

                return Encoding.Default.GetString(ans);
            }

            return "";
        }
        public void Close()
        {
            if (com.IsOpen)
                com.Close();
        }
        public bool ResetModem()
        {
            bool ret = false;

            byte[] reset = new byte[] { 0x4F, 0x53, 0x50, 0x02 };

            com.Write(reset, 0, reset.Length);

            System.Threading.Thread.Sleep(250);

            if (com.BytesToRead > 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, ans.Length);

                if (ans[0] == 0x05)
                    return true;
                else
                    return false;
            }

            return ret;
        }
        public bool StartTimer()
        {
            bool ret = false;

            byte[] reset = new byte[] { 0x4F, 0x53, 0x50, 0x03 };

            com.Write(reset, 0, reset.Length);

            System.Threading.Thread.Sleep(250);

            if (com.BytesToRead > 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, ans.Length);

                if (ans[0] == 0x05)
                    return true;
                else
                    return false;
            }

            return ret;
        }
        public bool StopTimer()
        {
            bool ret = false;

            byte[] reset = new byte[] { 0x4F, 0x53, 0x50, 0x04 };

            com.Write(reset, 0, reset.Length);

            System.Threading.Thread.Sleep(250);

            if (com.BytesToRead > 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, ans.Length);

                if (ans[0] == 0x05)
                    return true;
                else
                    return false;
            }

            return ret;
        }
        public bool ResetTimer()
        {
            bool ret = false;

            byte[] reset = new byte[] { 0x4F, 0x53, 0x50, 0x05 };

            com.Write(reset, 0, reset.Length);

            System.Threading.Thread.Sleep(250);

            if (com.BytesToRead > 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, ans.Length);

                if (ans[0] == 0x05)
                    return true;
                else
                    return false;
            }

            return ret;
        }
    }
}
