using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acceptors
{
    public class EBA03
    {
        System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();
        // Команды
        public enum EBA03_Commands { StatusRequest, Ack, Reset, Stack1, Stack2, Return, Hold, Wait, EnableMoney, DisableMoney, VersionRequest }
        // Возвращает
        public enum EBA03_Returns
        {
            Enable, Accepting, Escrow, Stacking, VendValid, Stacked, Rejecting, Returning, Holding,
            Disable, Initialize, PowerUp, PowerUpWithBilliAcceptor, PowerUpWithBillInStacker, Ack, InvalidCommand, ERROR, NULL, Inhibit1, Inhibit0
        }
        public enum EBA03_Rejecting { InsertionError, MagError, RejectingBiils, CompensatioError, ConveyingError, DenominatioError, PhotoPatternError, PhotoLevelError, NotTransmitted, Null, OperationError }
        public enum EBA03_Failure { StockMotor, TransportMotorSpeed, TransportMotor, CashBoxNotReady, ValidationHeadRemove, BootRomFailure, ExternalRomFailure, RomFailure, Extr }
        //Деньги
        public enum Money { RUB0, RUB10, RUB50, RUB100, RUB500, RUB1000, RUB5000 }
        // ОШИБКИ
        public enum EBA03_Error_Status { StackerFull, StackerOpen, JamInAcceptor, JamInStacker, Pause, Cheated, Failure, CommunicationError }

        public Money EnteredMoney;
        public byte[] LastRead;
        public EBA03_Error_Status Error;
        public EBA03_Returns LastReturn;

        public string Version;

        public bool Rub10;
        public bool Rub50;
        public bool Rub100;
        public bool Rub500;
        public bool Rub1000;
        public bool Rub5000;



        #region CRC16
        // CRC
        const ushort polynomial = 0x8408;
        ushort[] table = new ushort[256];

        private ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }
        private byte[] GetCRC16(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            byte[] ret = new byte[bytes.Length + 2];
            int i = 0;
            while (i < bytes.Length)
            {
                ret[i] = bytes[i];
                i++;
            }
            ret[i] = (byte)(crc & 0x00ff);
            ret[i + 1] = (byte)(crc >> 8);
            //return new byte[] { bytes[0], bytes[1], bytes[2],(byte)(crc & 0x00ff); , (byte)(crc >> 8) };
            return ret;
        }
        private void Crc16Init()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
        #endregion
        private void DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
           

        }
        public EBA03(string PortNumber)
        {
            try
            {
                port.PortName = PortNumber;
                port.BaudRate = 9600;
                port.Parity = System.IO.Ports.Parity.None;
                port.DataBits = 8;
                port.StopBits = System.IO.Ports.StopBits.One;

                port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);

                // Снятие управления с потоков 
                port.DtrEnable = false;
                port.RtsEnable = false;

                Crc16Init();
                // Время ожидания ответа
                port.ReadTimeout = 50;
                port.WriteTimeout = 10000;
                port.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsOpen()
        { return port.IsOpen; }

        public bool SendCommand(EBA03_Commands cmd)
        {
            byte[] Command = new byte[3];         // комавда без CRC
            byte[] SigningCommand;  // Команда с CRC

            switch (cmd)
            {
                case EBA03_Commands.StatusRequest:
                    Command = new byte[3] { 0xFC, 0x05, 0x11 };
                    break;
                case EBA03_Commands.Ack:
                    Command = new byte[3] { 0xFC, 0x05, 0x50 };
                    break;
                case EBA03_Commands.Reset:
                    Command = new byte[3] { 0xFC, 0x05, 0x40 };
                    break;
                case EBA03_Commands.Stack1:
                    Command = new byte[3] { 0xFC, 0x05, 0x41 };
                    break;
                case EBA03_Commands.Stack2:
                    Command = new byte[3] { 0xFC, 0x05, 0x42 };
                    break;
                case EBA03_Commands.Return:
                    Command = new byte[3] { 0xFC, 0x05, 0x43 };
                    break;
                case EBA03_Commands.Hold:
                    Command = new byte[3] { 0xFC, 0x05, 0x44 };
                    break;
                case EBA03_Commands.Wait:
                    Command = new byte[3] { 0xFC, 0x05, 0x45 };
                    break;
                case EBA03_Commands.EnableMoney:
                    Command = new byte[4] { 0xFC, 0x06, 0xC3, 0x00 };
                    break;
                case EBA03_Commands.DisableMoney:
                    Command = new byte[4] { 0xFC, 0x06, 0xC3, 0x01 };
                    break;
                case EBA03_Commands.VersionRequest:
                    Command = new byte[3] { 0xFC, 0x05, 0x88};
                    break;
            }

            SigningCommand = GetCRC16(Command);

            port.Write(SigningCommand, 0, SigningCommand.Length);

            System.Threading.Thread.Sleep(150);

            return true;

        }
        public bool ValidateResponse(byte[] response)
        {

            byte[] forcheck = new byte[response.Length - 2];

            int last1 = response.Length - 2;
            int last2 = response.Length - 1;

            int i = 0;
            while (i < response.Length - 2)
            {
                forcheck[i] = response[i];
                i++;
            }
            byte[] signed = GetCRC16(forcheck);

            if (response[last1] == signed[last1] && response[last2] == signed[last2])
            {
                return true;
            }
            else
                return false;

        }
        public EBA03_Returns ReadAnswer()
        {
            EBA03_Returns ret = EBA03_Returns.NULL;
            int btoread = port.BytesToRead;
             
            if (btoread != 0)
            {
                byte[] buf = new byte[btoread];
                int res = port.Read(buf, 0, btoread);

                if (ValidateResponse(buf))
                {
                    switch (buf[2])
                    {
                        //Enable Idle
                        case 0x11: ret = EBA03_Returns.Enable; break;
                        case 0x12: ret = EBA03_Returns.Accepting; break;
                        case 0x13: ret = EBA03_Returns.Escrow; EnteredMoney = GetMoney(buf[3]); break; //+DATA
                        case 0x14: ret = EBA03_Returns.Stacking; break;
                        case 0x15: ret = EBA03_Returns.VendValid; break;
                        case 0x16: ret = EBA03_Returns.Stacked; break;
                        case 0x17: ret = EBA03_Returns.Rejecting; break; // +DATA
                        case 0x18: ret = EBA03_Returns.Returning; break;
                        case 0x19: ret = EBA03_Returns.Holding; break;
                        case 0x1A: ret = EBA03_Returns.Disable; break;
                        case 0x1B: ret = EBA03_Returns.Initialize; break;
                        case 0x40: ret = EBA03_Returns.PowerUp; break;
                        case 0x41: ret = EBA03_Returns.PowerUpWithBilliAcceptor; break;
                        case 0x42: ret = EBA03_Returns.PowerUpWithBillInStacker; break;
                        case 0x43: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.StackerFull; break;
                        case 0x44: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.StackerOpen; break;
                        case 0x45: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.JamInAcceptor; break;
                        case 0x46: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.JamInStacker; break;
                        case 0x47: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.Pause; break;
                        case 0x48: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.Cheated; break;
                        case 0x49: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.Failure; break; // + DATA
                        case 0x4A: ret = EBA03_Returns.ERROR; Error = EBA03_Error_Status.CommunicationError; break;
                        case 0x50: ret = EBA03_Returns.Ack; break;
                        case 0x4B: ret = EBA03_Returns.InvalidCommand; break;
                        case 0xC3:
                            if (buf[3] == 1)
                                ret = EBA03_Returns.Inhibit0;
                            else
                                ret = EBA03_Returns.Inhibit1;
                            break;
                        case 0x88:
                            Version = Encoding.ASCII.GetString(buf);
                            break;
                            


                    }
                }

            }

            return ret;
        }
        public void Close()
        {
            if (port != null)
                if (port.IsOpen)
                    port.Close();
        }
        
        public Money GetMoney(byte b)
        {
            switch (b)
            {
                case 0x63:
                    return Money.RUB10;

                case 0x64:
                    return Money.RUB50;

                case 0x65:
                    return Money.RUB100;
                case 0x66:
                    return Money.RUB500;

                case 0x67:
                    return Money.RUB1000;
                case 0x68:
                    return Money.RUB5000;

            }
            return Money.RUB0;
        }


    }
}
