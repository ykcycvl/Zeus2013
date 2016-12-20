using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Acceptors
{
    class CashCode_CCNET
    {
        System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();

        public enum CCNET_Commands { ACK, NACS, RESET, POOL, STACK, IDENT, ENABLE_MONEY_WO_ESCROW, ENABLE_MONEY_WITH_ESCROW, DISABLE_MONEY, RETURN, GET_STATUS };
        public enum CCNET_STATUS { NULL, ILLEGAL_COMMAND, POWER_UP, INITIALIZE, IDLING, DISABLED, ACCEPTING, STACKING, REJECTING, STKMISSING, ERROR, ACK, NACK, Version, ESCROW, RETURN, PAUSE };
        public enum CCNET_ERRORS { NULL, JAM_IN_ACCEPTOR, JAM_IN_STACKER, DROP_CASSETTE_FULL, DROP_CASSETTE_REMOVED, CHEATED };
        public enum Money { RUB0, RUB10, RUB50, RUB100, RUB500, RUB1000, RUB5000 };
        public Money EnteredMoney;
        public CCNET_ERRORS LastError;
        public CCNET_Commands LastCmd;
        public string LastErrorMessage;
        public string Version;
        public string Port;
        #region CRC16
        // CRC
        const ushort polynomial = 0x8408;
        ushort[] table = new ushort[256];
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
        public CashCode_CCNET(string PortNumber)
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
                port.ReadTimeout = 100;
                port.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SendCommand(CCNET_Commands cmd)
        {
            byte[] Command = new byte[4];         // комавда без CRC
            byte[] SigningCommand;  // Команда с CRC

            switch (cmd)
            {
                case CCNET_Commands.IDENT:
                    Command = new byte[4] { 0x02, 0x03, 0x06, 0x37 };
                    break;
                case CCNET_Commands.ACK:
                    Command = new byte[4] { 0x02, 0x03, 0x06, 0x00 };
                    break;
                case CCNET_Commands.NACS:
                    Command = new byte[4] { 0x02, 0x03, 0x06, 0xFF };
                    break;
                case CCNET_Commands.POOL:
                    Command = new byte[4] { 0x02, 0x03, 0x06, 0x33 };
                    break;
                case CCNET_Commands.RESET:
                    Command = new byte[4] { 0x02, 0x03, 0x06, 0x30 };
                    break;
                case CCNET_Commands.STACK:
                    Command = new byte[4] { 0x02, 0x03, 0x06, 0x35 };
                    break;
                case CCNET_Commands.ENABLE_MONEY_WO_ESCROW:
                    Command = new byte[10] { 0x02, 0x03, 0x0C, 0x34, 0x00, 0x00, 0x7C, 0x00, 0x00, 0x00 };
                    break;
                case CCNET_Commands.ENABLE_MONEY_WITH_ESCROW:
                    Command = new byte[10] { 0x02, 0x03, 0x0C, 0x34, 0x00, 0x00, 0x7C, 0x00, 0x00, 0x7C };
                    break;
                case CCNET_Commands.DISABLE_MONEY:
                    Command = new byte[10] { 0x02, 0x03, 0x0C, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    break;
                case CCNET_Commands.RETURN:
                    Command = new byte[4] { 0x02, 0x03, 0x06, 0x36 };
                    break;
            }

            SigningCommand = GetCRC16(Command);
            LastCmd = cmd;
            port.Write(SigningCommand, 0, SigningCommand.Length);
            System.Threading.Thread.Sleep(100);

            return true;
        }
        [DebuggerStepThrough]
        public bool ValidateResponse(byte[] response)
        {
            if (response.Length < 2)
                return false;

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
        public Money GetMoney(byte b)
        {
            switch (b)
            {
                case 0x02:
                    return Money.RUB10;

                case 0x03:
                    return Money.RUB50;

                case 0x04:
                    return Money.RUB100;
                case 0x05:
                    return Money.RUB500;

                case 0x06:
                    return Money.RUB1000;
                case 0x07:
                    return Money.RUB5000;
                default:
                    return Money.RUB0;
            }
        }
        public CCNET_STATUS ReadAnswer()
        {
            CCNET_STATUS ret = CCNET_STATUS.NULL;
            int btoread = port.BytesToRead;

            if (btoread != 0)
            {
                byte[] buf = new byte[btoread];
                int res;

                try
                {
                    res = port.Read(buf, 0, btoread);
                }
                catch (Exception ex)
                {
                    /*Пока нихера не делаем, ибо хуй знает, откуда оно берется...*/
                }

                if (ValidateResponse(buf))
                {
                    switch (buf[3])
                    {
                        //Enable Idle
                        case 0x30: ret = CCNET_STATUS.ILLEGAL_COMMAND; break;
                        case 0x10: ret = CCNET_STATUS.POWER_UP; break;
                        case 0x13: ret = CCNET_STATUS.INITIALIZE; break;
                        case 0x14: ret = CCNET_STATUS.IDLING; break;
                        case 0x19: ret = CCNET_STATUS.DISABLED; break;
                        case 0x15: ret = CCNET_STATUS.ACCEPTING; break;
                        case 0x17: ret = CCNET_STATUS.STACKING; break;
                        case 0x18: ret = CCNET_STATUS.REJECTING; break;
                        case 0x00: ret = CCNET_STATUS.ACK; break;
                        case 0xFF: ret = CCNET_STATUS.NACK; break;
                        case 0x80: ret = CCNET_STATUS.ESCROW;  break;
                        case 0x81: ret = CCNET_STATUS.ACCEPTING; EnteredMoney = GetMoney(buf[4]); break;
                            // купюра возвращена 
                            /*
                            sRETURNED10RUB=  {0x02, 0x03, 0x07, 0x82, 0x02, 0x2E, 0x23};
                            sRETURNED50RUB=  {0x02, 0x03, 0x07, 0x82, 0x03, 0xA7, 0x32};
                            sRETURNED100RUB= {0x02, 0x03, 0x07, 0x82, 0x04, 0x18, 0x46};
                            sRETURNED500RUB= {0x02, 0x03, 0x07, 0x82, 0x05, 0x91, 0x57};
                            sRETURNED1000RUB={0x02, 0x03, 0x07, 0x82, 0x06, 0x0A, 0x65};
                             */
                        //case 0x82: ret = CCNET_STATUS.RETURN; EnteredMoney =  GetMoney(buf[4]); break;
                        case 0x41: ret = CCNET_STATUS.ERROR; LastError = CCNET_ERRORS.DROP_CASSETTE_FULL; break;
                        case 0x42: ret = CCNET_STATUS.ERROR; LastError = CCNET_ERRORS.DROP_CASSETTE_REMOVED; break;
                        case 0x43: ret = CCNET_STATUS.ERROR; LastError = CCNET_ERRORS.JAM_IN_ACCEPTOR; break;
                        case 0x44: ret = CCNET_STATUS.ERROR; LastError = CCNET_ERRORS.JAM_IN_STACKER; break; // jam in stacker
                        case 0x45: ret = CCNET_STATUS.ERROR; LastError = CCNET_ERRORS.CHEATED; break;
                        case 0x46:
                            if (LastCmd == CCNET_Commands.IDENT && buf[2] == 39)
                            {
                                ret = CCNET_STATUS.Version;
                                Version = Encoding.ASCII.GetString(buf);
                            }
                            else
                                ret = CCNET_STATUS.PAUSE; 
                                
                            break;
                        case 0x1C: ret = CCNET_STATUS.REJECTING; break;
                        default:
                            if (LastCmd == CCNET_Commands.IDENT && buf[2] == 39)
                            {
                                ret = CCNET_STATUS.Version;
                                Version = Encoding.ASCII.GetString(buf);
                            }
                            else
                            {
                                string msg = "";
                                for (int i = 0; i < btoread; i++)
                                {
                                    msg += " " + buf[i].ToString("X");
                                }
                                //MessageBox.Show(msg);

                                ret = CCNET_STATUS.ERROR;
                                LastErrorMessage = "HEXERR: " + msg;
                            }
                            break;
                    }
                }
            }

            if (LastCmd == CCNET_Commands.POOL)
                SendCommand(CCNET_Commands.ACK);
            return ret;
        }
        public void Close()
        {
            if (port != null)
                if (port.IsOpen)
                    port.Close();
        }
        public bool IsOpen()
        { return port.IsOpen; }
       
    }
}
