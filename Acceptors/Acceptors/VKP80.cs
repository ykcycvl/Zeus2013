﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Printers
{
    class VKP80
    {
        public System.IO.Ports.SerialPort com = new System.IO.Ports.SerialPort();
        public bool Retract;
        public bool ok;
        public bool ERROR;
        public string ErrMsg;


        public VKP80(string port)
        {
            // По умолчанию выбрасывать чек
            Retract = false;

            com.PortName = port;
            com.BaudRate = 19200;
            com.Parity = System.IO.Ports.Parity.None;
            com.DataBits = 8;
            com.StopBits = System.IO.Ports.StopBits.One;

            // Снятие управления с потоков 
            com.DtrEnable = false;
            com.RtsEnable = false;

            com.ReadTimeout = 100;
            com.WriteTimeout = 10000;

            //com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
            //com.ReceivedBytesThreshold = 1;
            //System.Threading.Thread.Sleep(100);

            try
            {
                com.Open();

                // INIT PRINTER

                byte[] init = new byte[2] { 0x1B, 0x40 };

                com.Write(init, 0, init.Length);
                System.Threading.Thread.Sleep(100);

                byte[] fonsize = new byte[3] { 0x1B, 0x4D, 0x49 };
                com.Write(fonsize, 0, fonsize.Length);
                System.Threading.Thread.Sleep(100);
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
                byte[] Retr = new byte[3] { 0x1D, 0x65, 0x02 };
                byte[] WORetr = new byte[3] { 0x1D, 0x65, 0x05 };
                if (Retract)
                    com.Write(Retr, 0, Retr.Length);
                else
                    com.Write(WORetr, 0, WORetr.Length);


                byte[] toprint = System.Text.Encoding.Convert(Encoding.Default, Encoding.GetEncoding(866), Encoding.Default.GetBytes(str + "\r\n"));

                com.Write(toprint, 0, toprint.Length);


                Cuting();

                return ok;
            }

            return ok;
        }
        public bool Test()
        {
            bool result = false;
            string ver = GetVersion();
            if (ver == "VKP80" && com.IsOpen)
            {
                byte[] prn_status = new byte[3] { 0x10, 0x04, 0x01 };
                byte[] off_status = new byte[3] { 0x10, 0x04, 0x02 };


                com.Write(prn_status, 0, prn_status.Length);
                System.Threading.Thread.Sleep(100);

                if (com.BytesToRead != 0)
                {
                    byte[] ans = new byte[com.BytesToRead];

                    com.Read(ans, 0, com.BytesToRead);

                    string binary = Convert.ToString(ans[0], 2);

                    if (binary[binary.Length - 4] != '0')
                    {
                        ERROR = true;
                        ErrMsg = "OFF-LINE|";

                        com.Write(off_status, 0, off_status.Length);
                        System.Threading.Thread.Sleep(100);

                        if (com.BytesToRead != 0)
                        {
                            ans = new byte[com.BytesToRead];

                            com.Read(ans, 0, com.BytesToRead);

                            binary = Convert.ToString(ans[0], 2);
                            if (binary.Length < 8)
                            {
                                for (int i = binary.Length; i < 8; i++)
                                    binary = "0" + binary;
                            }
                            if (binary[binary.Length - 3] == '0')
                                ErrMsg += "COVER_CLOSED|";
                            else
                                ErrMsg += "COVER_OPENED|";

                            if (binary[binary.Length - 6] == '0')
                                ErrMsg += "PAPER_PRSENT|";
                            else
                                ErrMsg += "PAPER_END|";

                            if (binary[binary.Length - 7] == '0')
                                ErrMsg += "NO_ERROR|";
                            else
                                ErrMsg += "ERROR|";

                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            else { ERROR = true; ErrMsg = "!VKP80|COM_CLOSED"; }

            return result;
        }
        public string GetVersion()
        {
            string model;
            string version;
            byte[] ident = new byte[3] { 0x1D, 0x49, 0x01 };

            if (com.IsOpen)
            {
                com.Write(ident, 0, ident.Length);
                System.Threading.Thread.Sleep(550);
            }
            else
                return "";
            //     - 5D||5E

          

            if (com.BytesToRead != 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, com.BytesToRead);

                if (ans[0] == 0x5E || ans[0] == 0x5D)
                    return "VKP80";
            }

            return "";


        }
        public void Cuting()
        {
            byte[] produce = new byte[4] { 0x1D, 0x65, 0x03, 0x10 };

            System.Threading.Thread.Sleep(50);
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
