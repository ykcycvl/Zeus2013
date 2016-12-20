using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Printers
{
    class AV286
    {
        public System.IO.Ports.SerialPort com = new System.IO.Ports.SerialPort();
        public string Version;
        public bool ok;
        public bool ERROR;
        public string ErrMsg;


        public AV286(string port)
        {
            com.PortName = port;
            com.BaudRate = 115200;
            com.Parity = System.IO.Ports.Parity.None;
            com.DataBits = 8;
            com.StopBits = System.IO.Ports.StopBits.One;

            // Снятие управления с потоков 
            com.DtrEnable = false;
            com.RtsEnable = false;

            com.ReadTimeout = 100;
            com.WriteTimeout = 10000;

            com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
            //com.ReceivedBytesThreshold = 1;
            //System.Threading.Thread.Sleep(100);

            try
            {
                com.Open();

                // INIT PRINTER

                byte[] init = new byte[2] { 0x1B, 0x40 };

                com.Write(init, 0, init.Length);
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool IsOpen()
        {
            if (com != null)
                return com.IsOpen;
            else
                return false;
        }
        public bool Print(string str)
        {
            ok = false;
            if (com.IsOpen)
            {
                ok = true;

                //byte[] font = new byte[3] { 0x1B, 0x21, 0x01 };
                byte[] font = new byte[3] { 0x1B, 0x21, 0x00 };
                com.Write(font, 0, font.Length);
                string toprn = "";
                int n = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    toprn += str[i];
                    if (n == 40 && str[i] != '\r')
                    {
                        toprn += "\r\n";
                        n = 0;
                    }
                    if (str[i] == '\n')
                    {
                        n = 0;
                    }
                    n++;

                }
                byte[] toprint = System.Text.Encoding.Convert(Encoding.Default, Encoding.GetEncoding(866), Encoding.Default.GetBytes(toprn + "\r\n"));

                com.Write(toprint, 0, toprint.Length);


                Cuting();

                return ok;
            }

            return ok;
        }
        public bool PrintFromBytes(byte[] str)
        {
            ok = false;
            if (com.IsOpen)
            {
                ok = true;

                byte[] font = new byte[3] { 0x1B, 0x21, 0x00 };
                com.Write(font, 0, font.Length);
                com.Write(str, 0, str.Length);

                Cuting();

                return ok;
            }

            return ok;
        }
        public void DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (com.BytesToRead != 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, ans.Length);

                if (ans.Length > 1)
                {
                    if (ans[0] == 1 && ans[1] == 17)
                        Version = "AV268";

                }
                else
                    if (ans[0] != 0)
                    {
                        ErrMsg = "";
                        string binary = Convert.ToString(ans[0], 2);
                        if (binary.Length < 8)
                        {
                            for (int i = binary.Length; i < 8; i++)
                                binary = "0" + binary;
                        }

                        if (binary[5] == '1')
                        {
                            ErrMsg = "PAPER_END|";
                        }

                        if (binary[4] == '1')
                        {
                            ErrMsg += "COVER_OPEN|";
                        }

                        ErrMsg += "ERROR";
                        ERROR = true;

                    }
                    else
                    {
                        ERROR = false;
                        ErrMsg = "";
                    }
            }
        }
        public bool Test()
        {
            if (ERROR == false)
            {
                return true;
            }
            return false;
        }

        public void Cuting()
        {
            byte[] produce = new byte[2] { 0x1B, 0x69 };

            System.Threading.Thread.Sleep(100);
            com.Write(produce, 0, produce.Length);

        }
        public void Close()
        {
            if (com != null)
            {
                if (com.IsOpen)
                    com.Close();
            }
        }
    }
}
