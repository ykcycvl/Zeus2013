using System;
using System.Collections.Generic;
using System.Text;

namespace FR
{
    public struct Buy
    {
        public string Name;
        public string Quanity;
        public Decimal Amount;
        public string AdditionalText;
    }
    public enum DeviceType
    { BillValidator, WatchDog, FiscalRegister, Printer, Modem }
    /// <summary>
    /// Общая информация об устройстве
    /// </summary>
    public abstract class DeviceInfo
    {
        protected DeviceType _type;
        protected string _Model;
        protected string _Protocol;
        protected string _Version;
        protected string _COM;

        public string ComPort
        { get { return _COM; } }
        public string Model
        { get {return _Model; }}
        public string Protocol
        { get { return _Protocol;}}
        public string Version
        { get { return _Version;}}

        protected void SetDeviceInfo(string Model, string Protocol, string Version)
        {
            _Model = Model;
            _Protocol = Protocol;
            _Version = Version;
        }
        protected void SetDeviceInfo(string Model, string Protocol, string Version, string Com, DeviceType type)
        {
            _Model = Model;
            _Protocol = Protocol;
            _Version = Version;
            _COM = Com;
            _type = type;

        }


    }
    public enum PasswordMode
    {
        SystemAdministrator, KKMAdministrator, Kassir, Tehnik
    }
    /// <summary>
    /// Интерфейс фискального регистратора
    /// </summary>
    public interface IFiscalRegister
    {
        bool OpenSession();
        bool CloseSession();
        bool OpenPayment();
        bool ClosePayment();
        void GetXReport();
        void GetZReport();
        void GetDelayedZReports();
        bool AddDelayedZReport();
        void SetPassword(PasswordMode mode, string text);
        void GetStatus();
        string getKKMnumber();
        string getEKLZnumber();
        void ShowFiscalSettingsForm();
        /*string ErrorNumber();
        string ErrorMessage();*/

        bool Buy(Buy buy);
        bool Buy(Buy[] buys);
        bool Buy(string what, int quanity, Decimal amount, string text);
    }
    


    /// <summary>
    /// Абстрактная реализация фискального регистратора
    /// </summary>
    public abstract class FiscalRegisters: DeviceInfo , IFiscalRegister
    {
        /// <summary>
        /// Признак фискального режима
        /// </summary>
        bool IsFiscal;

        protected bool _sessionOpened;
        protected bool _paymentOpened;

        protected string _psysadm;
        protected string _pkkmadm;
        protected string _pkass;
        protected string _ptech;

        protected int _countFreeZReports;



        /// <summary>
        /// Номер ошибки
        /// </summary>
        protected string _errorNumber;
        /// <summary>
        /// Описание ошибки
        /// </summary>
        protected string _errorText;

        public virtual string ErrorNumber
        {
            get { return _errorNumber; }
        }
        public virtual string ErrorMessage
        {
            get { return _errorText; }
        }
        public virtual bool PaperPresent
        {
            get { return false; }
        }
        public virtual void GetStatus()
        {
            throw new NotImplementedException("Не реализовано.");
        }
        public virtual string getKKMnumber()
        {
            return "";
        }
        public virtual string getEKLZnumber()
        {
            return "";
        }
        public virtual void SetPassword(PasswordMode mode, string text)
        {
            switch (mode)
            {
                case PasswordMode.KKMAdministrator: _pkkmadm = text; break;
                case PasswordMode.SystemAdministrator: _psysadm = text; break;
                case PasswordMode.Kassir: _pkass = text; break;
                case PasswordMode.Tehnik: _ptech = text; break;
            }
        }
        public virtual void ShowFiscalSettingsForm()
        { throw new NotImplementedException(); }
        /// <summary>
        /// Печать текста.
        /// </summary>
        /// <param name="text">Текст для печати.</param>
        /// <returns>Успех/неудача</returns>
        public virtual bool PrintText(string text)
        {
            return false;
        }
        public virtual bool FiscalMode
        { 
            get { return IsFiscal; }
            set { IsFiscal = value; }
        }
        public virtual bool OpenSession()
        { return false; }
        public virtual bool CloseSession()
        { return false; }
        public virtual bool OpenPayment()
        { return false; }
         public virtual bool ClosePayment()
        { return false; }
        public virtual void GetXReport()
        { throw new Exception("Невозможно снять Х-отчет. Не реализовано."); }
        public virtual void GetZReport()
        { throw new Exception("Невозможно снять Z-отчет. Не реализовано."); }
        public virtual void GetDelayedZReports()
        { throw new Exception("Невозможно снять отложенные Z-отчеты. Не реализовано."); }
        public virtual bool AddDelayedZReport()
        { throw new Exception("Невозможно снять Z-отчеты в память. Не реализовано."); }

        public virtual bool Buy(Buy buy)
        {
            
                return false;
        }
        public virtual bool Buy(Buy[] buys)
        {
            return false;
        }
        public virtual bool Buy(string what, int quanity, Decimal amount, string text)
        { return false; }

    }
}
