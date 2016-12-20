using System;
using System.Collections.Generic;
using System.Text;

namespace FR
{
    public class Atol:FiscalRegisters
    {
        FprnM1C.IFprnM45 KKM;
        FiscalSettings FSettings;

        public bool EnableLog { get; set; }
        public string LogFileName { get { return "fr_log.txt"; } }

        private void Log(string text)
        {
            IPayBox.Helper.TextLog.Add.Line(LogFileName, DateTime.Now.ToString() + " " + text);
        }

        public override bool  PaperPresent
        {
            get 
            { 
                 Execute(KKM.GetCurrentMode());
                 return !KKM.OutOfPaper;
            }
        }
        private bool Execute(int Result)
        {
            _errorNumber = Result.ToString();
            _errorText = KKM.ResultDescription;

            Log(string.Format("Результат операции #{0} ({1})", _errorNumber, _errorText));
            // - 3802 - чек открыт, операция невозможна.
            // - 3822 - смена превысила 24 часа;
            // -3828 = смена закрыта операция невозможна.
            if (Result == -3802)
                ClosePayment();

            if (Result == -3822)
                AddDelayedZReport();


            if (Result == 0)
                return true;
            else
                return false;
            
        }
        public override bool PrintText(string text)
        {
            Log("Печатаем текст:" + text);
            KKM.Caption = text;
            if (!Execute(KKM.PrintString()))
                return false;
            Log("Полная обрезка");
            if (!Execute(KKM.FullCut()))
                return false;
            else
                return true;
        }
        private string GetModel(int Model)
        {
            string ModelName = "Не известно.";
            switch (Model)
            {
                case 13: ModelName = "Триум-Ф"; break;
                case 14: ModelName = "ФЕЛИКС-Р Ф"; break;
                case 15: ModelName = "ФЕЛИКС-02К / ЕНВД"; break;
                case 16: ModelName = "МЕРКУРИЙ-140"; break;
                case 17: ModelName = "МЕРКУРИЙ-114.1Ф"; break;
                case 18: ModelName = "ШТРИХ-ФР-Ф"; break;
                case 19: ModelName = "ЭЛВЕС-МИНИ-ФР-Ф"; break;
                case 20: ModelName = "ТОРНАДО"; break;
                case 23: ModelName = "ТОРНАДО-К"; break;
                case 24: ModelName = "ФЕЛИКС-РК // ЕНВД"; break;
                case 25: ModelName = "ШТРИХ-ФР-К"; break;
                case 26: ModelName = "ЭЛВЕС-ФР-К"; break;
                case 27: ModelName = "ФЕЛИКС-3СК"; break;
                case 28: ModelName = "ШТРИХ-МИНИ-ФР-К"; break;
                case 30: ModelName = "FPrint-02K / ЕНВД"; break;
                case 31: ModelName = "FPrint-03K / ЕНВД"; break;
                case 32: ModelName = "FPrint-88K / ЕНВД"; break;
                case 33: ModelName = "BIXOLON-01K"; break;
                case 35: ModelName = "FPrint-5200K / ЕНВД"; break;
                case 37: ModelName = "PayVKP-80K"; break;
                case 38: ModelName = "PayPPU-700K"; break;
                case 39: ModelName = "PayCTS-2000K"; break;
                case 42: ModelName = "Аура-01ФР-KZ"; break;
                case 43: ModelName = "PayVKP-80KZ"; break;
                case 101: ModelName = "POSPrint FP410K"; break;
                case 102: ModelName = "МSTAR-Ф"; break;
                case 103: ModelName = "Мария-301 МТМ Т7"; break;
                case 104: ModelName = "ПРИМ-88ТК"; break;
                case 105: ModelName = "ПРИМ-08ТК"; break;
                case 106: ModelName = "СП101ФР-К"; break;
                case 107: ModelName = "ШТРИХ-КОМБО-ФР-К"; break;
                case 108: ModelName = "ПРИМ-07К"; break;
                case 109: ModelName = "МИНИ-ФП6"; break;
                case 110: ModelName = "ШТРИХ-М-ФР-К"; break;
                case 111: ModelName = "MSTAR-TK.1"; break;
                case 113: ModelName = "ШТРИХ-LIGHT-ФР-К"; break;
            }
            return ModelName;
        }
        private void Initialize()
        { 
            try
            {
                Log("Инициализация");
                FSettings = new FiscalSettings();
                FSettings.SettingsChanged += new FiscalSettings.SettingsChangedEventHandler(FSettings_SettingsChanged);
                FSettings.Event();

                KKM = new FprnM1C.FprnM45Class();
                SetDeviceInfo(GetModel(KKM.Model), "АТОЛ V"+KKM.Version,KKM.UMajorVersion.ToString()+"."+KKM.UMinorVersion.ToString()+"."+KKM.UBuild, "COM"+KKM.PortNumber.ToString(), DeviceType.FiscalRegister);
                KKM.DeviceEnabled = true;

                Log("Получение статуса");
                if (!Execute(KKM.GetStatus()) )
                    throw new Exception("Не удалось найти устройство КММ. \r\n" );

               
                if (KKM.CheckState != 0)
                {
                    Log("Чек не закрыт, закрываем чек");
                    /*if (KKM.CancelCheck() != 0)
                    {
                        Log("Не удалось закрыть чек.");
                        return false;
                    }*/
                    ClosePayment();
                }
                Log("ResetMode()");
                Execute(KKM.ResetMode());
                KKM.TextWrap = 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось создать объект общего драйвера КММ `АТОЛ`.\r\n- " + ex.Message);
            }
        }

        void FSettings_SettingsChanged(object Sender)
        {
            _pkass = FSettings["kassa"];
            _pkkmadm = FSettings["admin"];
            _psysadm = FSettings["sysadmin"];
            _ptech = FSettings["tech"];
        }
       /* ~Atol()
        {
            if (KKM != null)
            {
                if (KKM.ResetMode() == 0)
                    KKM.DeviceEnabled = false;

            }
                    
        }*/
        public Atol()
        {
            FiscalMode = false;
            Initialize();
        }
        public Atol(bool IsFiscalMode)
        {
            FiscalMode = IsFiscalMode;
            Initialize();
        }
        public override bool OpenPayment()
        {
            Log("Открытие чека");
            KKM.CheckType = 1;
            Log("Статус");
            if (KKM.GetStatus() != 0)
                return false;
            Log("Открываем чек");
            if (KKM.OpenCheck() != 0)
                return false;
            _paymentOpened = true;
            return true;
        }
        public override bool ClosePayment()
        {
            Log("Закрытие чека");
            KKM.TypeClose = 0;

            if (!Execute(KKM.CloseCheck()))
                return false;
            Log("ResetMode()");
            if (!Execute(KKM.ResetMode()))
                return false;
            
            //

            _paymentOpened = false;
            return true ;
        }
        public override void GetZReport()
        {
            Log("Снять Z отчет");
            // Z - отчет
            // устанавливаем пароль системного администратора ККМ = 30
            KKM.Password = _psysadm;
            // входим в режим отчетов с гашением
            KKM.Mode = 3;
            Log("Устанавливаем Режим отчета с гашением");
            if (KKM.SetMode() != 0)
                return;
            // снимаем отчет
            Log("Снимаем отчет");
            KKM.ReportType = 1;
            int err = KKM.Report();
            Log("Результат №"+err.ToString());
            if (err != 0)
                return;
            Log("Resetmode()");
            if (KKM.ResetMode() != 0)
                return ;
        }
        public override void GetXReport()
        {
            // X - отчет
            // устанавливаем пароль администратора ККМ =29
            KKM.Password = _pkkmadm;
            // входим в режим отчетов без гашения
            KKM.Mode = 2;
            if (!Execute(KKM.SetMode()) )
                return;
            // снимаем отчет
            KKM.ReportType = 2;
            if (KKM.Report() != 0)
                return;
            if (KKM.ResetMode() != 0)
                return ;
        }
        public override void ShowFiscalSettingsForm()
        {
            FSettings.InitializeForm();
        }
        public override void GetDelayedZReports()
        {
            Log("Отложенные Z отчеты\r\nResetMode()");

            if(!Execute(KKM.ResetMode()))
                return;
            KKM.Password = _psysadm;
            KKM.Mode = 3;
            Log("Установка режима 3");
            if (!Execute(KKM.SetMode()))
                return;
            KKM.StreamFormat = 3;
            KKM.OutboundStream = "B5"; //вход в режим распечатки отчетов из памяти
            KKM.ReportType = 1;
            Log("Вход в режим распечатки отчетов, снятие");
            Execute(KKM.RunCommand());
            
            string answer = KKM.InboundStream; //формат ответа: '550019', где "19" -- количество оставшихся свободных полей в памяти в hex, "00" -- код ошибки
            Log("Релуьтат:"+answer);
            if (answer.Length == 6)
            {
                string p = answer[4] + "" + answer[5];
                _countFreeZReports = int.Parse(p);

                if (answer[2] + "" + answer[3] != "00")
                {
                    Log("Невозможно снять Z в пямять 0х" + answer[2] + "" + answer[3]);
                    //throw new Exception("Невозможно снять Z-отчет в память. Error 0x" + answer[2] + "" + answer[3]);
                    return ;
                }
            }
            
            //собственное здесь идет разбор ответа и анализ того, что Вам надо
            //KKM.Report(); //а вот и собственно снятие отчета в память.
            Log("ReserMode()");
            Execute(KKM.ResetMode()); //выходим из режима снятия отчетов. 
        }
        public override bool AddDelayedZReport()
        {
            //здесь было бы неплохо сделать проверку состояния ККМ
            

            Log("Автомнятие Z отчета\r\n GetStatus()");


            int status = KKM.GetStatus();
            if (status != 0)
            {
                Log("Что то не то с ККМ Status:"+ status);
                return false;
            }
            if (KKM.CheckState != 0)
            {
                Log("Чек не закрыт, закрываем чек");
                /*if (KKM.CancelCheck() != 0)
                {
                    Log("Не удалось закрыть чек.");
                    return false;
                }*/
                ClosePayment();
            }

            KKM.ResetMode();
            KKM.Password = _psysadm;
            KKM.Mode = 3;
            Log("Вход в режим 3");
            if(!Execute(KKM.SetMode()))
                return false;
            KKM.StreamFormat = 3;
            KKM.OutboundStream = "B4"; //вход в режим снятия отчетов в память
            KKM.ReportType = 1;
            Log("Запуск команды");
            if (!Execute(KKM.RunCommand()))
            {
                
                return false;
            }
            
            string answer = KKM.InboundStream; //формат ответа: '550019', где "19" -- количество оставшихся свободных полей в памяти в hex, "00" -- код ошибки
            Log("Ответная строка "+answer);
            //собственное здесь идет разбор ответа и анализ того, что Вам надо
            if (answer.Length == 6)
            {
                string p = answer[4] +""+ answer[5];
                
                if(int.TryParse(p,out _countFreeZReports))
                    if (_countFreeZReports == 0)
                    {
                        _errorNumber = "-99999";
                        _errorText = "Нет свободной памяти для снятия Z-отчета в буфер.";
                        return false;
                    }
                    if (answer[2] + "" + answer[3] != "00")
                    {
                        Log("Невозможно снять в память:" + answer[2] + "" + answer[3]);
                        //throw new Exception("Невозможно снять Z-отчет в память. Error 0x" + answer[2] + "" + answer[3]);
                        return false;

                    }

            }
            else
                return false;

            Log("Снятие (Report())");
            Execute(KKM.Report());
                

            Log("ResetMode()");
            Execute(KKM.ResetMode());
            //     return false; //выходим из режима снятия отчетов. 

            return true;

        }
        public override void GetStatus()
        {
            try 
            {

                /*int err = KKM.GetStatus();

                if (err != 0)
                {
                    _errorNumber = KKM.ResultCode.ToString();
                    _errorText = KKM.ResultDescription;
                }
                */
                Log("Получить состояние");
                int err = KKM.GetCurrentMode();

                Execute(err);
                
                _errorNumber = KKM.ResultCode.ToString();
                _errorText = KKM.ResultDescription;


                if (KKM.OutOfPaper)
                {
                    Log("Закончилась бумага");
                    _errorNumber = "-99999";
                    _errorText = "Закончилась бумага";
                }
                else
                {
                    if (KKM.CheckState == 1)
                    {
                        Log("CheckState == 1 -> закрываем платеж");
                        ClosePayment();
                    }
                }
            }
            catch (Exception ex)
            {
                 Log("Exception\r\n"+ex.ToString());
                _errorNumber = "-99999";
                _errorText = "Ошибка опроса ККМ. \r\n" + ex.Message;
            }
        }
        public override string getKKMnumber()
        {
            try
            {
                KKM.RegisterNumber = 22;
                KKM.GetRegister();
                return KKM.SerialNumber;
            }
            catch 
            {
                return "?";
            }
        }
        public override string getEKLZnumber()
        {
            try
            {
                KKM.RegisterNumber = 28;
                KKM.GetRegister();
                return KKM.SerialNumber;
            }
            catch
            {
                return "?";
            }
        }
        public override bool Buy(Buy buy)
        {
            try
            {
                /*if (buy == null)
                    throw new Exception("Покупка не может быть null");*/
                Log("Формирование покупки");
                
                //if(buy.Name.Trim().Length == 0 || buy.Amount == 0 || buy.Quanity.Trim().Length==0)
                //    throw new Exception("Невозможно зарегистрировать нулевую покупку.");


              
                if (!KKM.DeviceEnabled)
                    KKM.DeviceEnabled = true;
                
                if (KKM.ResultCode != 0)
                {
                    Log("Результирующий код != 0, выход из покупки");
                    return false;
                }

                if (FiscalMode)
                {
                    Log("Установлен фискальный режим");
                   /* if (!OpenPayment())
                    {
                       
                        return false;
                    }*/

                    // Если регистрация производится в фискальном режиме.
                    // Пароль кассира
                    KKM.Password = _pkass;
                    // входим в режим регистрации
                    KKM.Mode = 1;
                    int i = KKM.SetMode();
                    Log("Вход в режим регистрации");
                    if (!Execute(i))
                    {
                        if (_errorNumber == "0" && (i == -3802 || i == -3822))
                        {
                            Log("Смена превысила 24 часа, или незакрыта покупка. Попытка войти в режим");
                            int jj = KKM.SetMode();
                            if (jj != 0)
                                return false;

                        }
                        else
                        {
                            Log("Ошибка №"+_errorNumber+" отменяем покупку");
                            return false;
                        }
                        
                    }

                    KKM.Name = buy.Name;
                    KKM.Price =  System.Convert.ToDouble(buy.Amount);
                    KKM.Quantity = System.Convert.ToDouble(buy.Quanity);
                    KKM.Department = 1;
                    Log("Регистрация покупки");
                    if (!Execute(KKM.Registration()))
                    {
                        //KKM.ResultCode;
                        //KKM.ResultDescription;
                        Log("Покупка не прошла. Выход");
                        return false;
                    }
                    Log("Перенос по словам, доп текст:" + buy.AdditionalText+"");
                  
                    KKM.TextWrap = 1; // Перенос по словам.
                    KKM.Caption = buy.AdditionalText;
                    Log("Печать строки");
                    if (!Execute(KKM.PrintString()))
                     return false;
                    Log("Закрываем покупку");
                    return ClosePayment();
                }
                else
                {
                    Log("Нефискальный режим, просто печать текста:"+buy.AdditionalText);
                    //Не фискальный режим. Просто печатаем текст.
                    KKM.TextWrap = 1; // Перенос по словам.
                    KKM.Caption =  buy.AdditionalText;
                    Log("Печать строки");
                    if (!Execute(KKM.PrintString()))
                    {
                        Log("Печать не прошла. не обрезаем");
                        return false;
                    }
                    Log("Полная обрезка");
                    return Execute(KKM.FullCut());
                }
            }
            catch //(Exception ex)
            {
                //ex = ex;
                return false;
            }
         
        }
        public override bool Buy(Buy[] buys)
        {
            try
            {
                if (buys.Length == 0)
                    throw new Exception("Покупка не может быть null");

                foreach (Buy buy in buys)
                {
                    if (buy.Name.Trim().Length == 0 || buy.Amount == 0 || buy.Quanity.Trim().Length == 0)
                        throw new Exception("Невозможно зарегистрировать нулевую покупку.");
                }



                if (!KKM.DeviceEnabled)
                    KKM.DeviceEnabled = true;
                if (KKM.ResultCode != 0)
                    return false;

                if (FiscalMode)
                {
                    /* if (!OpenPayment())
                     {
                       
                         return false;
                     }*/

                    // Если регистрация производится в фискальном режиме.
                    // Пароль кассира
                    KKM.Password = _pkass;
                    // входим в режим регистрации
                    KKM.Mode = 1;
                    int i = KKM.SetMode();
                    if (!Execute(i))
                    {
                        if (_errorNumber == "0" && (i == -3802 || i == -3822))
                        {
                            int jj = KKM.SetMode();
                            if (jj != 0)
                                return false;

                        }
                        else
                            return false;

                    }
                    foreach (Buy buy in buys)
                    {
                        KKM.Name = buy.Name;
                        KKM.Price = System.Convert.ToDouble(buy.Amount);
                        KKM.Quantity = System.Convert.ToDouble(buy.Quanity);
                        KKM.Department = 1;

                        if (!Execute(KKM.Registration()))
                        {
                            //KKM.ResultCode;
                            //KKM.ResultDescription;
                            return false;
                        }

                        if (!string.IsNullOrEmpty(buy.AdditionalText))
                        {
                            KKM.TextWrap = 1; // Перенос по словам.
                            KKM.Caption = buy.AdditionalText;
                            if (!Execute(KKM.PrintString()))
                                return false;
                        }

                    }
                    

                    return ClosePayment();
                }
                else
                {
                    //Не фискальный режим. Просто печатаем текст.
                    KKM.TextWrap = 1; // Перенос по словам.
                    KKM.Caption = buys[0].AdditionalText;
                    if (!Execute(KKM.PrintString()))
                        return false;

                    return Execute(KKM.FullCut());
                }
            }
            catch //(Exception ex)
            {
                //ex = ex;
                return false;
            }

        }
        public override bool Buy(string what, int quanity, Decimal amount, string text)
        {
            FR.Buy b = new Buy();
            b.Name = what;
            b.Quanity = quanity.ToString();
            b.Amount = amount;
            b.AdditionalText = text;
            return Buy(b);
        }

       
    }
}
