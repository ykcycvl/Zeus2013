using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeus.API
{
    public interface IIncass
    {
        string ToXML();
    }
   

    public class Incass
    {
        //private string OldIncassXML;

        private Incass old;

        private Decimal _amount;

        public Decimal Amount
        {
            get { return _amount; }
            set { _amount = value; if (_autocommit && !_txn_started) ToDisk(); }
        }

        private ulong _id;

        public ulong Id
        {
            get { return _id; }
            set { _id = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _terminalid;

        public uint Terminalid
        {
            get { return _terminalid; }
            set { _terminalid = value; ToDisk(); }
        }

        private uint _userid;

        public uint UserID
        {
            get { return _userid; }
            set { _userid = value; ToDisk(); }
        }

        private uint _countPay;

        public uint CountPay
        {
            get { return _countPay; }
            set { _countPay = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _countBill;

        public uint CountBill
        {
            get { return _countBill; }
            set { _countBill = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _countBill10;

        public uint CountBill10
        {
            get { return _countBill10; }
            set { _countBill10 = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _countBill50;

        public uint CountBill50
        {
            get { return _countBill50; }
            set { _countBill50 = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _countBill100;

        public uint CountBill100
        {
            get { return _countBill100; }
            set { _countBill100 = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _countBill500;

        public uint CountBill500
        {
            get { return _countBill500; }
            set { _countBill500 = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _countBill1000;

        public uint CountBill1000
        {
            get { return _countBill1000; }
            set { _countBill1000 = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _countBill5000;

        public uint CountBill5000
        {
            get { return _countBill5000; }
            set { _countBill5000 = value; if (_autocommit && !_txn_started)  ToDisk(); }
        }
        private uint _bytessend;

        public uint Bytessend
        {
            get { return _bytessend; }
            set { _bytessend = value; if (_autocommit && !_txn_started) ToDisk(); }
        }
        private uint _bytesrecieved;

        public uint Bytesrecieved
        {
            get { return _bytesrecieved; }
            set { _bytesrecieved = value; if (_autocommit && !_txn_started)  ToDisk(); }
        }
        private DateTime _datestarted;

        public DateTime Datestarted
        {
            get { return _datestarted; }
            set { _datestarted = value; if (_autocommit && !_txn_started) ToDisk(); }
        }

        private DateTime _datestoped ;


        /// <summary>
        /// Начата ли незафиксированная на диске транзакция
        /// </summary>
        private bool _txn_started;

        /// <summary>
        /// Флаг Автосброса данных на диск
        /// </summary>
        private bool _autocommit;

        public bool AutoCommit
        {
            get { return _autocommit; }
            set { _autocommit = value; }
        }
        /// <summary>
        /// Полный путь до файла;
        /// </summary>
        private string _fullname;

        public string Fullname
        {
            get { return _fullname; }
            set { _fullname = value; }
        }

        public void StartTransaction()
        {
            old = this;
            if (_txn_started == true)
                throw new Exception("Текущая транзакция не закрыта.");
            else
                _txn_started = true;
        }
        public void Commit()
        { 
            // Записываем на диск и закрываем транзакцию.
            ToDisk();

            old = null;
            _txn_started = false;
        }
        public void RollBack()
        {
            this.Id = old.Id;
            this.Terminalid = old.Terminalid;
            this.UserID = old.UserID;
            this.CountBill = old.CountBill;
            this.CountBill10 = old.CountBill10;
            this.CountBill50 = old.CountBill50;
            this.CountBill100 = old.CountBill100;
            this.CountBill500 = old.CountBill500;
            this.CountBill1000 = old.CountBill1000;
            this.CountBill5000 = old.CountBill5000;
            this.CountPay = old.CountPay;
            this.Datestarted = old.Datestarted;
            this.Bytesrecieved = old.Bytesrecieved;
            this.Bytessend = old.Bytessend;

            old = null;
            // Записываем на диск и закрываем транзакцию.
            _txn_started = false;
        }

        private void ToDisk()
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(_fullname);
            System.IO.StreamWriter sw ;
            //if (fi.Exists)
            //    fi.Delete();
            
            sw = fi.CreateText();

            sw.Write(ToXML());
            sw.Close();
        }
        private void FromDisk()
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(_fullname);
            if (fi.Exists)
            {
                System.IO.StreamReader sr = fi.OpenText();

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                try
                {
                    doc.LoadXml(sr.ReadToEnd());
                }
                catch
                {
                    return;
                }
                finally
                {
                    sr.Close();
                }

                _id = ulong.Parse(doc.DocumentElement.GetAttribute("id"));

                foreach (System.Xml.XmlElement el in doc.DocumentElement)
                {
                    /*
                      <count_bill>61</count_bill>
                      <incass-start-date>12.04.2010 13:32:29</incass-start-date>
                      <amount>9700</amount>
                      <bytes_send>52133</bytes_send>
                      <bytes_read>987664</bytes_read>
                      <count_check>68</count_check>
                      <count5000>0</count5000>
                      <count1000>2</count1000>
                      <count500>11</count500>
                      <count100>16</count100>
                      <count50>7</count50>
                      <count10>25</count10>
                     
                     */
                    switch (el.Name)
                    {
                        case "count_bill": _countBill = uint.Parse(el.InnerText); break;
                        case "incass-start-date": _datestarted = DateTime.Parse(el.InnerText); break;
                        case "amount": _amount = Decimal.Parse(el.InnerText); break;
                        case "bytes_send": _bytessend = uint.Parse(el.InnerText); break;
                        case "bytes_read": _bytesrecieved = uint.Parse(el.InnerText); break;
                        case "count_check": _countPay = uint.Parse(el.InnerText); break;
                        case "count5000": _countBill5000 = uint.Parse(el.InnerText); break;
                        case "count1000": _countBill1000 = uint.Parse(el.InnerText); break;
                        case "count500": _countBill500 = uint.Parse(el.InnerText); break;
                        case "count100": _countBill100 = uint.Parse(el.InnerText); break;
                        case "count50": _countBill50 = uint.Parse(el.InnerText); break;
                        case "count10": _countBill10 = uint.Parse(el.InnerText); break;
                    }
                }

            }
        }
        public Incass(string filename)
        {
            _fullname = filename;
            FromDisk();
        }
        
        public string IncassNow()
        {
            if (this._amount == 0)
                throw new Exception("Сумма инкассации равна `0`. Не возможно инкассировать нулевую сумму.");
            _datestoped = DateTime.Now;//.ToString("yyyy.MM.dd HH:mm:ss");
            string outxml = this.ToXML();
            this.Clear();
            return outxml;
        }
        public void Clear()
        {
            this.StartTransaction();
            try
            {
                this.Id = ulong.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                this.Terminalid = 0;
                this.UserID = 1;
                this.CountBill = 0;
                this.CountBill10 = 0;
                this.CountBill50 = 0;
                this.CountBill100 = 0;
                this.CountBill500 = 0;
                this.CountBill1000 = 0;
                this.CountBill5000 = 0;
                this.CountPay = 0;
                this._amount = 0;
                this.Datestarted = DateTime.Now;
                this.Bytesrecieved = 0;
                this.Bytessend = 0;
                this.Commit();
            }
            catch
            { RollBack(); }

            old = null;
            
        }
        public string ToXML()
        {
            /*
              <count_bill>61</count_bill>
              <incass-start-date>12.04.2010 13:32:29</incass-start-date>
              <amount>9700</amount>
              <bytes_send>52133</bytes_send>
              <bytes_read>987664</bytes_read>
              <count_check>68</count_check>
              <count5000>0</count5000>
              <count1000>2</count1000>
              <count500>11</count500>
              <count100>16</count100>
              <count50>7</count50>
              <count10>25</count10>

             sb.Append("<>"); sb.Append(); sb.Append("</>");
             */
            StringBuilder sb = new StringBuilder("<statistic ");

            sb.Append("id=\""); sb.Append(_id); sb.Append("\">");
            sb.Append("\r\n<userid>"); sb.Append(_userid.ToString()); sb.Append("</userid>");
            sb.Append("\r\n<count_bill>"); sb.Append(_countBill); sb.Append("</count_bill>");
            sb.Append("\r\n<incass-start-date>"); sb.Append(_datestarted.ToString("yyyy.MM.dd HH:mm:ss")); sb.Append("</incass-start-date>");
            sb.Append("\r\n<incass-start-date-utc>"); sb.Append(_datestarted.ToUniversalTime().ToString("yyyy.MM.dd HH:mm:ss")); sb.Append("</incass-start-date-utc>");
            if (_datestoped != null)
            {
                sb.Append("\r\n<incass-stop-date>"); sb.Append(_datestoped.ToString("yyyy.MM.dd HH:mm:ss")); sb.Append("</incass-stop-date>");
                sb.Append("\r\n<incass-stop-date-utc>"); sb.Append(_datestoped.ToUniversalTime().ToString("yyyy.MM.dd HH:mm:ss")); sb.Append("</incass-stop-date-utc>");
            }
            sb.Append("\r\n<amount>"); sb.Append(_amount); sb.Append("</amount>");
            sb.Append("\r\n<bytes_send>"); sb.Append(_bytessend); sb.Append("</bytes_send>");
            sb.Append("\r\n<bytes_read>"); sb.Append(_bytesrecieved); sb.Append("</bytes_read>");
            sb.Append("\r\n<count_check>"); sb.Append(_countPay); sb.Append("</count_check>");
            sb.Append("\r\n<count5000>"); sb.Append(_countBill5000); sb.Append("</count5000>");
            sb.Append("\r\n<count1000>"); sb.Append(_countBill1000); sb.Append("</count1000>");
            sb.Append("\r\n<count500>"); sb.Append(_countBill500); sb.Append("</count500>");
            sb.Append("\r\n<count100>"); sb.Append(_countBill100); sb.Append("</count100>");
            sb.Append("\r\n<count50>"); sb.Append(_countBill50); sb.Append("</count50>");
            sb.Append("\r\n<count10>"); sb.Append(_countBill10); sb.Append("</count10>");
            sb.Append("\r\n</statistic>");

            return sb.ToString();
        }

    }
}
