using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;

namespace Acceptors
{
    public enum Model { NULL, EBA03, CASHCODE }
    public enum Commands { Reset, GetStatus, StartTake, StopTake, AcceptMoney }
    public enum Result { OK, Error, Null }

    public class Independed
    {
        private string[] availablePorts;  // Список доступных портов
        public Model model;
        public string Version = string.Empty;
        public Commands CurCommand;
        public Result LastResult;
        public string PortNumber = string.Empty;
        public Decimal Amount;
        public int BillCount;
        public bool Error;
        public string ErrorMsg;
        public bool BillAcceptorActivity;
        public int CountR10;
        public int CountR50;
        public int CountR100;
        public int CountR500;
        public int CountR1000;
        public int CountR5000;

        public struct AcceptMoney
        {
            public bool R10;
            public bool R50;
            public bool R100;
            public bool R500;
            public bool R1000;
            public bool R5000;
        }

        EBA03 eba;
        CashCode_CCNET ccnet;
        public AcceptMoney accmoney;

        public Independed()
        {
            List<string> cmports = new List<string>();

            try
            {
                log.AddToLog("\r\n-------------------------------------------------------");
                log.AddToLog("Определение купюроприемника");

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

            log.AddToLog("Обнаружение доступных портов...");
            availablePorts  = System.IO.Ports.SerialPort.GetPortNames();

            for (int i = 0; i < availablePorts.Length; i++)
            {
                if (cmports.Count > 0)
                {
                    string s = cmports.Find(p => p == availablePorts[i]);

                    if (!String.IsNullOrEmpty(s))
                        continue;
                }

                try
                {
                    log.AddToLog("Поиск eba на порту " + availablePorts[i] + "...");
                    eba = new EBA03(availablePorts[i]);
                    log.AddToLog("отправка команды EBA03.EBA03_Commands.VersionRequest");
                    eba.SendCommand(EBA03.EBA03_Commands.VersionRequest);
                    eba.ReadAnswer();

                    if (eba.Version != null)
                        if(eba.Version.IndexOf("ID003") != -1)
                        {
                            model = Model.EBA03; PortNumber = availablePorts[i];
                            log.AddToLog("Ответ получен. Купюроприемник: " + model.ToString());
                            break;
                        }

                    log.AddToLog("не получен ожидаемый ответ...");

                    if (eba != null)
                    {
                        log.AddToLog("Закрытие eba");
                        eba.Close();
                    }

                    log.AddToLog("Поиск CashCode на порту " + availablePorts[i]);
                    ccnet = new CashCode_CCNET(availablePorts[i]);
                    log.AddToLog("отправка команды CashCode_CCNET.CCNET_Commands.POOL");
                    ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.POOL);
                    CashCode_CCNET.CCNET_STATUS ans = ccnet.ReadAnswer();
                    log.AddToLog("получен ответ " + ans);

                    if (ans == CashCode_CCNET.CCNET_STATUS.IDLING)
                    {
                        log.AddToLog("отправка команды CashCode_CCNET.CCNET_Commands.DISABLE_MONEY");
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.DISABLE_MONEY);
                        ans = ccnet.ReadAnswer();
                        log.AddToLog("получен ответ " + ans);
                    }
                    if (ans == CashCode_CCNET.CCNET_STATUS.INITIALIZE || ans == CashCode_CCNET.CCNET_STATUS.DISABLED || ans == CashCode_CCNET.CCNET_STATUS.POWER_UP || ans == CashCode_CCNET.CCNET_STATUS.PAUSE)
                    {
                        log.AddToLog("отправка команды CashCode_CCNET.CCNET_Commands.RESET");
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.RESET);
                        ans = ccnet.ReadAnswer();
                        log.AddToLog("получен ответ " + ans);
                    }
                    //System.Threading.Thread.Sleep(50);
                    //ccnet.ReadAnswer();
                    
                    //ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.ACK);
                    //System.Threading.Thread.Sleep(50);

                    log.AddToLog("отправка команды CashCode_CCNET.CCNET_Commands.IDENT");
                    ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.IDENT);
                   // System.Threading.Thread.Sleep(50);
                    CashCode_CCNET.CCNET_STATUS st = ccnet.ReadAnswer();
                    log.AddToLog("получен ответ " + st.ToString());

                    if (st == CashCode_CCNET.CCNET_STATUS.Version || st == CashCode_CCNET.CCNET_STATUS.ILLEGAL_COMMAND)
                    {
                        if (ccnet.Version != null)
                            if (ccnet.Version.IndexOf("SM") != -1 || ccnet.Version.IndexOf("VU") != -1 || ccnet.Version.IndexOf("FL") != -1)
                            {
                                Match m = Regex.Match(ccnet.Version, @"'(?<val>.*?)\s", RegexOptions.IgnoreCase);

                                if (m.Success)
                                {
                                    Version = m.Groups["val"].ToString();
                                    Version = Regex.Replace(Version, @"[^A-Z^А-Я^0-9^\-^:^;^.^,^\s^?]", string.Empty, RegexOptions.IgnoreCase);
                                }
                            }

                        model = Model.CASHCODE;
                        log.AddToLog("Найден купюроприемник " + model.ToString() + ". Порт " + availablePorts[i]);
                        PortNumber = availablePorts[i];
                        break;
                    }
                    else
                    {
                        log.AddToLog("не получен ожидаемый ответ...");

                        if (ccnet != null)
                        {
                            log.AddToLog("закрытие ccnet...");
                            ccnet.Close();
                        }
                    }
                }
                catch(Exception ex)
                {
                    if (eba != null)
                        eba.Close();
                    if (ccnet != null)
                        ccnet.Close();
                    model = Model.NULL;

                    log.AddToLog("Ошибка при определении купюроприемника. " + ex.Message);
                }
            }

            BillAcceptorActivity = false;
            log.AddToLog("Выход");
        }
        public bool AllowMoneyEnterOnPooling 
        { get; set; }
        public Result SendCommand(Commands cmd)
        {
            Result ret = Result.Null;
            switch (model)
            { 
                case Model.EBA03:
                    ret = SendEBA(cmd);
                    break;
                case Model.CASHCODE:
                    ret = SendCashCode(cmd);
                    break;
                case Model.NULL:
                    ret = Result.Null;
                    Error = true;
                    ErrorMsg = "Port is null";

                    break;
            }
            return ret;

        }
        private Result SendEBA(Commands cmd)
        {
            Result ret = Result.Null;
            EBA03.EBA03_Returns ans;
            if (eba.IsOpen())
            {
                switch (cmd)
                { 
                    case Commands.Reset:
                        eba.SendCommand(EBA03.EBA03_Commands.Reset);
                         ans= eba.ReadAnswer();

                        if (ans == EBA03.EBA03_Returns.Ack)
                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                    case Commands.StartTake:
                        eba.SendCommand(EBA03.EBA03_Commands.EnableMoney);
                         ans = eba.ReadAnswer();

                        if (ans == EBA03.EBA03_Returns.Inhibit1 )
                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                    case Commands.StopTake:
                        eba.SendCommand(EBA03.EBA03_Commands.DisableMoney);
                         ans = eba.ReadAnswer();

                        if (ans == EBA03.EBA03_Returns.Inhibit0)
                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                    case Commands.GetStatus:
                        eba.SendCommand(EBA03.EBA03_Commands.StatusRequest);
                        ans = eba.ReadAnswer();

                        if (ans == EBA03.EBA03_Returns.Inhibit1)
                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                }
                
            }
            return ret;
        }
        private Result SendCashCode(Commands cmd)
        {
            Result ret = Result.Null;
            CashCode_CCNET.CCNET_STATUS ans;
            if (ccnet.IsOpen())
            {
                switch (cmd)
                {
                    case Commands.Reset:
                        ccnet.SendCommand( CashCode_CCNET.CCNET_Commands.RESET );
                        ans = ccnet.ReadAnswer();

                        if (ans ==  CashCode_CCNET.CCNET_STATUS.ACK)
                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                    case Commands.StartTake:
                        AllowMoneyEnterOnPooling = true;
                        
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.ENABLE_MONEY_WITH_ESCROW );
                        ans = ccnet.ReadAnswer();

                        if (ans == CashCode_CCNET.CCNET_STATUS.ACK)
                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                    case Commands.StopTake:
                        AllowMoneyEnterOnPooling = false;
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.DISABLE_MONEY);
                        ans = ccnet.ReadAnswer();

                        if (ans ==  CashCode_CCNET.CCNET_STATUS.ACK)

                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                    case Commands.GetStatus:
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.POOL);
                        ans = ccnet.ReadAnswer();

                        if (ans == CashCode_CCNET.CCNET_STATUS.DISABLED)
                            ret = Result.OK;
                        else
                            ret = Result.Error;

                        break;
                }

            }
            return ret;
        }
        public void Pooling()
        {
            switch (model)
            {
                case Model.EBA03:
                    PoolingEBA();
                    break;
                case Model.CASHCODE:
                    PoolingCashCode();
                    break;
                case Model.NULL:
                    Error = true;
                    ErrorMsg = "Port is null";
                    break;
            }
        }
        public void Flush()
        { 
            Amount = 0 ;
            BillCount= 0 ;
            Error = false ;
            ErrorMsg="";
            CountR10 = 0;
            CountR50 = 0;
            CountR100 = 0;
            CountR1000 = 0;
            CountR500 = 0;
            CountR5000 = 0;
        }
        private void PoolingEBA()
        {
            if (eba.IsOpen())
            {
                BillAcceptorActivity = true;
                Error = false;
                ErrorMsg = "";
                eba.SendCommand(EBA03.EBA03_Commands.StatusRequest);
                EBA03.EBA03_Returns ans = eba.ReadAnswer();

                if (ans == EBA03.EBA03_Returns.Enable || ans == EBA03.EBA03_Returns.Disable)
                    BillAcceptorActivity = false;

                // Если купюра распозната
                if (ans == EBA03.EBA03_Returns.Escrow)
                {
                    // проверка на
                    if (eba.EnteredMoney == EBA03.Money.RUB10 && !accmoney.R10)
                    {
                        eba.SendCommand(EBA03.EBA03_Commands.Return);
                       
                    }
                    else if (eba.EnteredMoney == EBA03.Money.RUB50 && !accmoney.R50)
                    {
                        eba.SendCommand(EBA03.EBA03_Commands.Return);
                       
                    }
                    else if (eba.EnteredMoney == EBA03.Money.RUB100 && !accmoney.R100)
                    {
                        eba.SendCommand(EBA03.EBA03_Commands.Return);
                        
                    }
                    else if (eba.EnteredMoney == EBA03.Money.RUB500 && !accmoney.R500)
                    {
                        eba.SendCommand(EBA03.EBA03_Commands.Return);
                      
                    }
                    else if (eba.EnteredMoney == EBA03.Money.RUB1000 && !accmoney.R1000)
                    {
                        eba.SendCommand(EBA03.EBA03_Commands.Return);
                       
                    }
                    else if (eba.EnteredMoney == EBA03.Money.RUB5000 && !accmoney.R5000)
                    {
                        eba.SendCommand(EBA03.EBA03_Commands.Return);
                    }
                    else
                    {
                        eba.SendCommand(EBA03.EBA03_Commands.Stack1);
                    }
                }
                if (ans == EBA03.EBA03_Returns.VendValid)
                {
                    eba.SendCommand(EBA03.EBA03_Commands.Ack);
                    switch(int.Parse(eba.EnteredMoney.ToString().Replace("RUB", "")))
                    {
                        case 10: CountR10++; break;
                        case 50: CountR50++; break;
                        case 100: CountR100++; break;
                        case 500: CountR500++; break;
                        case 1000: CountR1000++; break;
                        case 5000: CountR5000++; break;
                          
                    }
                    Amount += Decimal.Parse(eba.EnteredMoney.ToString().Replace("RUB", ""));
                    BillCount++;
                    eba.EnteredMoney = EBA03.Money.RUB0;
                }
                if (ans == EBA03.EBA03_Returns.ERROR)
                {
                    Error = true;
                    ErrorMsg = eba.Error.ToString();
                }
                if (ans == EBA03.EBA03_Returns.PowerUpWithBilliAcceptor || ans == EBA03.EBA03_Returns.PowerUpWithBillInStacker)
                {
                    Error = true;
                    ErrorMsg = ans.ToString();
                }

            }
            else
            { Error = true; ErrorMsg = "Port not open."; }
        }
        private void PoolingCashCode()
        {
            if (ccnet.IsOpen())
            {
                BillAcceptorActivity = true;
                Error = false;
                ErrorMsg = "";
                ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.POOL);
                CashCode_CCNET.CCNET_STATUS ans = ccnet.ReadAnswer();

                if (ans == CashCode_CCNET.CCNET_STATUS.IDLING || ans == CashCode_CCNET.CCNET_STATUS.DISABLED)
                    BillAcceptorActivity = false;

                // Если купюра распозната
                if (ans == CashCode_CCNET.CCNET_STATUS.ESCROW && AllowMoneyEnterOnPooling)
                {
                    // проверка на
                    if (ccnet.EnteredMoney == CashCode_CCNET.Money.RUB10 && !accmoney.R10)
                    {
                        ccnet.SendCommand( CashCode_CCNET.CCNET_Commands.RETURN);
                    }
                    else if (ccnet.EnteredMoney == CashCode_CCNET.Money.RUB50 && !accmoney.R50)
                    {
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.RETURN);
                    }
                    else if (ccnet.EnteredMoney == CashCode_CCNET.Money.RUB100 && !accmoney.R100)
                    {
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.RETURN);
                    }
                    else if (ccnet.EnteredMoney == CashCode_CCNET.Money.RUB500 && !accmoney.R500)
                    {
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.RETURN);
                    }
                    else if (ccnet.EnteredMoney == CashCode_CCNET.Money.RUB1000 && !accmoney.R1000)
                    {
                        ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.RETURN);
                    }
                    else 
                        if (ccnet.EnteredMoney == CashCode_CCNET.Money.RUB5000 && !accmoney.R5000)
                        {
                            ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.RETURN);
                        }
                        else
                        {
                            ccnet.SendCommand(CashCode_CCNET.CCNET_Commands.STACK);
                            /*System.Threading.Thread.Sleep(30);

                            if(ccnet.ReadAnswer() == CashCode_CCNET.CCNET_STATUS.ACK)
                            {
                                switch (int.Parse(ccnet.EnteredMoney.ToString().Replace("RUB", "")))
                                {
                                    case 10: CountR10++; break;
                                    case 50: CountR50++; break;
                                    case 100: CountR100++; break;
                                    case 500: CountR500++; break;
                                    case 1000: CountR1000++; break;
                                    case 5000: CountR5000++; break;

                                }
                                Amount += Decimal.Parse(ccnet.EnteredMoney.ToString().Replace("RUB", ""));
                                BillCount++;
                                ccnet.EnteredMoney =  CashCode_CCNET.Money.RUB0;
                            }*/
                        }
                }
                if (ans == CashCode_CCNET.CCNET_STATUS.ACCEPTING)
                {
                    switch (int.Parse(ccnet.EnteredMoney.ToString().Replace("RUB", "")))
                    {
                        case 10: CountR10++; break;
                        case 50: CountR50++; break;
                        case 100: CountR100++; break;
                        case 500: CountR500++; break;
                        case 1000: CountR1000++; break;
                        case 5000: CountR5000++; break;
                    }
                    Amount += Decimal.Parse(ccnet.EnteredMoney.ToString().Replace("RUB", ""));
                    if(ccnet.EnteredMoney != CashCode_CCNET.Money.RUB0)
                        BillCount++;
                    ccnet.EnteredMoney = CashCode_CCNET.Money.RUB0;
                }/*
                if (ans == CashCode_CCNET.CCNET_STATUS.RETURN)
                {
                    switch (int.Parse(ccnet.EnteredMoney.ToString().Replace("RUB", "")))
                    {
                        case 10: CountR10--; break;
                        case 50: CountR50--; break;
                        case 100: CountR100--; break;
                        case 500: CountR500--; break;
                        case 1000: CountR1000--; break;
                        case 5000: CountR5000--; break;

                    }
                    Amount -= Decimal.Parse(ccnet.EnteredMoney.ToString().Replace("RUB", ""));
                    BillCount--;
                    ccnet.EnteredMoney = CashCode_CCNET.Money.RUB0;
                }*/
                if (ans ==  CashCode_CCNET.CCNET_STATUS.ERROR)
                {
                    Error = true;
                    ErrorMsg = ccnet.LastError.ToString() +"-"+ ccnet.LastErrorMessage ;
                }
                /*if (ans == CashCode_CCNET.CCNET_STATUS.NULL)
                {
                    Error = true;
                    ErrorMsg = "Not answer.";
                }*/
            }
            else
            { Error = true; ErrorMsg = "Port not open."; }
        }
    }
}
