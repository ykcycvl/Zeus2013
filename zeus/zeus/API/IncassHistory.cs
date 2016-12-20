using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace zeus.API
{
    public class IncassHistory : List<IncassHistoryEntity>
    {
        string _filename = "history.xml";
        public IncassHistory()
            : base()
        {

       

        }
        public IncassHistory(bool load)
        {
            if (load)
            {
                Load();
            }
        }

        public void Clear60()
        {
            if (this.Count > 60)
            {
                this.Remove(this[60]);
            }
        }
        
        public void Load()
        { 
             if (!File.Exists(_filename))
                return;

            IncassHistory my = (IncassHistory)Noti.Shared.Helper.Serializer.Deserialize(typeof(IncassHistory), File.ReadAllText(_filename));

            foreach (IncassHistoryEntity item in my)
            {
                this.Add(item);
            }
        }
        public void Save()
        {
           File.WriteAllText(_filename,Noti.Shared.Helper.Serializer.Serialize(typeof(IncassHistory), this));
        }
    }

    public class IncassHistoryEntity
    {
        public IncassHistoryEntity(Incass incass)
        {
            this.TerminalId = incass.Terminalid.ToString();
            this.DateStarted = incass.Datestarted;
            this.Amount = incass.Amount;
            this.Id = incass.Id;
            this.BytesSend = incass.Bytessend;
            this.BytesRecieved = incass.Bytesrecieved;

            this.CountBills = incass.CountBill;
            this.CountBill10 = incass.CountBill10;
            this.CountBill50 = incass.CountBill50;
            this.CountBill100 = incass.CountBill100;
            this.CountBill500 = incass.CountBill500;
            this.CountBill1000 = incass.CountBill1000;
            this.CountBill5000 = incass.CountBill5000;



        }
        public IncassHistoryEntity() { }


        public string TerminalId { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateStoped { get; set; }
        public Decimal Amount { get; set; }
        public ulong Id { get; set; }
        public uint CountBills { get; set; }
        private string _md5;
        public string MD5 
        { 
            get
            {
                if(string.IsNullOrEmpty(_md5))
                {
                    _md5 = GetMD5Hash(this.ToString());
                }
                return _md5;
            }
            set { _md5 = value; }
        }
        public uint CountBill10 { get; set; }
        public uint CountBill50 { get; set; }
        public uint CountBill100 { get; set; }
        public uint CountBill500 { get; set; }
        public uint CountBill1000 { get; set; }
        public uint CountBill5000 { get; set; }
        public uint BytesSend { get; set; }
        public uint BytesRecieved { get; set; }

      
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(TerminalId);
            sb.Append(DateStarted);
            sb.Append(DateStoped);
            sb.Append(Amount);
            sb.Append(Id);
            sb.Append(CountBills);
            sb.Append(CountBill10);
            sb.Append(CountBill50);
            sb.Append(CountBill100);
            sb.Append(CountBill500);
            sb.Append(CountBill5000);
            sb.Append(BytesSend);
            sb.Append(BytesRecieved);

            return sb.ToString();
        }

      
       
        private string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }
     
        public bool CheckMD5()
        {
            return string.Equals(MD5, GetMD5Hash(this.ToString()));
        }
    }
}
