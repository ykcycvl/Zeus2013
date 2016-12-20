using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMicronics.StarIO;

namespace Printers
{
    public class Tup900
    {
        //  This is what we will be using to communicate with the printer through StarIO
        private IPort sPort;
        //  Status can be retrived with this variable, goto 
        private StarPrinterStatus sPrinterStatus;
         
        //Create new CodePage class to check for codepages on computer.
        codePage codePageClass = new codePage();

        public System.IO.Ports.SerialPort com = new System.IO.Ports.SerialPort();
        public bool Retract;
        public bool ok;
        public bool ERROR;
        public string ErrMsg;


        public Tup900(string port)
        {
            try
            {
                com.PortName = port;
                this.sPort = Factory.I.GetPort(port, "9600,n,8,1,h", 10000);

                if (!Test())                    
                    throw new Exception("NOT TUP900 ON " + port);
            }
            catch (PortException ex)
            {
                throw ex;
            }
        }
        public bool IsOpen()
        {
            if (sPort!= null)
                return true;
            else
                return false;
        }
        public bool Print(string str)
        {
            ok = Test();

            if (ok)
            {
                Encoding targetCodePage = Encoding.GetEncoding(1251);
                string CMD = codePageClass.getStarIOcodePageCmd("1251");
                byte[] encodedBytes = targetCodePage.GetBytes(CMD + str + "\x0a");
                WritePortHelper(encodedBytes);

                Cuting();
            }

            return ok;
        }
        public bool PrintFromBytes(byte[] str)
        {
            ok = Test();

            if (ok)
            {
                Encoding targetCodePage = Encoding.GetEncoding(866);
                string CMD = codePageClass.getStarIOcodePageCmd("866");

                byte[] cmd = targetCodePage.GetBytes(CMD);
                byte[] font = new byte[]{ 0x1B, 0x30, 0x1B, 0x1E, 0x46, 0x01 };

                WritePortHelper(font);
                WritePortHelper(targetCodePage.GetBytes(CMD));
                WritePortHelper(str);
                WritePortHelper(targetCodePage.GetBytes("\x0a"));

                Cuting();
            }

            return ok;
        }
        public bool Test()
        {
            bool result = false;
            ERROR = false;
            ErrMsg = "";
            //Try and get status, Catch errors if we could not get status.
            try
            {
                  //If during monitoring the printer port goes null, lets reconnect to the printer.
                if (this.sPort == null)
                {
                    try
                    {
                        this.sPort = Factory.I.GetPort(com.PortName, "9600,n,8,1,h", 10000);
                    }
                    catch
                    {
                        this.sPort = null;
                        return false;
                    }
                }
                //Use StarIO to get status
                this.sPrinterStatus = this.sPort.GetParsedStatus();

                //Status = NULL
                if (this.sPrinterStatus == null)
                {
                    Factory.I.ReleasePort(this.sPort);
                    return false;
                }
                else
                    result = true;

                //Status = Offline
                if (this.sPrinterStatus.Offline == true) //offline == true
                {
                    result= !(ERROR = true);
                    ErrMsg = "OFF-LINE|";
                }
                //Status = Cover Open
                if (this.sPrinterStatus.CoverOpen == true)
                {
                    result = !(ERROR = true);
                    ErrMsg += "COVER_OPENED|";
                }
                else
                    ErrMsg += "COVER_CLOSED|";

                //Paper is empty
                if (this.sPrinterStatus.ReceiptPaperEmpty == true)
                {
                    result = !(ERROR = true);
                    ErrMsg += "PAPER_END|";
                }
                else
                    ErrMsg += "PAPER_PRSENT|";

            }
            
            catch (PortException p)//If there was a problem getting status, lets catch the port error
            {
                Factory.I.ReleasePort(this.sPort);
                ErrMsg = p.ToString();
                return false;
            }
            return result;
        }
        public string GetVersion()
        {
            if (Test())
                return "STAR TUP900";

            return "";


        }
        public void Cuting()
        {
            printToPrinter("\x1b\x64\x02");

        }
        private uint WritePortHelper(byte[] writeBuffer)
        {
            uint zeroProgressOccurances = 0;
            uint totalSizeCommunicated = 0;
            while ((totalSizeCommunicated < writeBuffer.Length) && (zeroProgressOccurances < 2))
            {
                uint sizeCommunicated = sPort.WritePort(writeBuffer, totalSizeCommunicated, (uint)writeBuffer.Length - totalSizeCommunicated);
                if (sizeCommunicated == 0)
                {
                    zeroProgressOccurances++;
                }
                else
                {
                    totalSizeCommunicated += sizeCommunicated;
                    zeroProgressOccurances = 0;
                }
            }
            return totalSizeCommunicated;
        }

        private void printToPrinter(string prnData)
        {

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            //     Encoding your string to a byte array and how code pages effect string/char encoding.
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            // You can use the following command to convert your string to a byte array but notice how we can specify a code page.
            // Here is way to set CODE PAGE 852 to match with the printer CODE PAGE 852
            //  
            //     Encoding ec = Encoding.GetEncoding(852); 
            //     byte[] command = ec.GetBytes(prnData);
            //
            // If your software will be using a different code page, this is how you can encode the string correctly to get the desired text to byte encoding.
            //
            // In this example we will be using UTF8 to encode our string to a byte array but you can use ASCII for legacy systems like this.
            //
            //     byte[] command = ASCIIEncoding.ASCII.GetBytes(print);
            //
            // Using System.Text.Encoding, we can call Encoding.UTF8.GetBytes(string); to convert our string hex data to a byte array. (Recommended)
            byte[] dataByteArray = Encoding.UTF8.GetBytes(prnData);

            //Write bytes to printer
            uint amountWritten = 0;
            uint amountWrittenKeep;
            while (dataByteArray.Length > amountWritten)
            {
                //This while loop is very important because in here is where the bytes are being sent out through StarIO to the printer
                amountWrittenKeep = amountWritten;
                amountWritten += sPort.WritePort(dataByteArray, amountWritten, (uint)dataByteArray.Length - amountWritten);

                if (amountWrittenKeep == amountWritten) // no data is sent
                {
                    StarMicronics.StarIO.Factory.I.ReleasePort(sPort);
                    return; //exit this entire function
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
            if(sPort != null)
            {
                Factory.I.ReleasePort(this.sPort);
            }
        }
        
    }
}
