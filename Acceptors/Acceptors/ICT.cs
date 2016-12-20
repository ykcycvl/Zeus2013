using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Printers
{
    class ICT
    {
        public System.IO.Ports.SerialPort com = new System.IO.Ports.SerialPort();

        private byte[] print = new byte[4] { 0x50, 0xF3, 0x00, 0x00 };
        private byte[] Cut = new byte[2] { 0x1B, 0xF5};
        private byte[] MotorForward = new byte[3] { 0x1B, 0x00, 0x09 };
        private byte[] MotorBacward = new byte[3] { 0x1B, 0x01, 0x09 };

        public bool ok;
         
              
        public ICT(string port)
        {
            com.PortName = port;
            com.BaudRate = 9600;
            com.Parity = System.IO.Ports.Parity.None;
            com.DataBits = 8;
            com.StopBits = System.IO.Ports.StopBits.One;

            // Снятие управления с потоков 
            com.DtrEnable = false;
            com.RtsEnable = false;

            com.ReadTimeout = 100;
            com.WriteTimeout = 10000;

            com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
            com.ReceivedBytesThreshold = 1;
            System.Threading.Thread.Sleep(100);

            try
            {
                com.Open();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public bool IsOpen()
        {
            if(com != null)
                return com.IsOpen;
            else
                return false;
        }
        public bool ReadAns()
        {
            
            if (com.BytesToRead > 0)
            {
                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, ans.Length);

                switch (ans[0])
                {
                    case 0xBB: return true;

                    case 0x45: return false;

                }
                return false;
            }
            else
            {
                return ok;
            }
            


        }
        public bool Print(string str)
        {
            ok = false;
            byte[] toprint = System.Text.Encoding.Convert(Encoding.Default, Encoding.GetEncoding(866), Encoding.Default.GetBytes((str+"\r\n\r\n\r\n\r\n").Replace("\r\n"," \r")));
            byte[] end = new byte[1] {0x0C};

            byte[] towrite = new byte[toprint.Length + print.Length + 1];

            int i = 0;

            for (int j = 0; j < print.Length; j++)
            {
                towrite[i] = print[j];
                i++;
                
            }

            for (int j = 0; j < toprint.Length; j++)
            {
                towrite[i] = toprint[j];
                i++;
            }
            towrite[i] = end[0];

            com.Write(towrite, 0, towrite.Length);
            System.Threading.Thread.Sleep(100);

            int sl = 0;
            while (!ReadAns())
            {
                if (sl > 100000)
                    break;
                System.Threading.Thread.Sleep(1000);
                sl += 1000;
                
            }

            Cuting();
            return ReadAns();
  
        }
        public bool Test()
        {
            ok = false;
            if (com.IsOpen)
            {
                com.Write(MotorForward, 0, MotorForward.Length);
                System.Threading.Thread.Sleep(100);
                if (ReadAns())
                {
                    ok = false;
                    com.Write(MotorBacward, 0, MotorBacward.Length);
                    System.Threading.Thread.Sleep(100);
                }
                return ReadAns();
            }
            else
                return false;
        }
        public bool Cuting()
        {
            ok = false;

            com.Write(Cut, 0, Cut.Length);
            System.Threading.Thread.Sleep(100);
            return ReadAns();
        }
        private void DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (com.BytesToRead != 0)
            {
                byte[] ans = new byte[com.BytesToRead];
                try
                {
                    com.Read(ans, 0, ans.Length);
                }catch
                {ok =  false;}

                switch (ans[0])
                {
                    case 0xBB: ok = true; break;

                    case 0x45: ok = false; break;

                }
                 
            }

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
