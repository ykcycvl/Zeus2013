using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeus.API
{
    /// <summary>
    /// Интерфейс для Pay
    /// </summary>
    interface IPay
    {
        string ToXML();
    }
     

    /// <summary>
    /// API Класс платеж [Pay]
    /// </summary>
    public class Pay:IPay
    {

        public System.Collections.Specialized.NameValueCollection Extra;
        
        string _txn_id;
        public string Txn_id
        {
            get { return _txn_id; }
            set { _txn_id = value; }
        }
        
        int _prv_id;
        public int Prv_id
        {
            get { return _prv_id; }
            set { _prv_id = value; }
        }

        DateTime _txn_date;
        public DateTime Txn_date
        {
            get { return _txn_date; }
            set { _txn_date = value; }
        }

        Decimal _from_amount;
        public Decimal From_amount
        {
            get { return _from_amount; }
            set { _from_amount = value; }
        }

        Decimal _to_amount;
        public Decimal To_amount
        {
            get { return _to_amount; }
            set { _to_amount = value; }
        }

        public string Account
        {
            get { return Extra["account"]; }
            set
            {
                if (Extra["account"] == null)
                {
                    Extra.Add("account", value);
                }
                else
                    Extra.Set("account", value);
            }
        }

        int _recept;
        public int Recept
        {
            get { return _recept; }
            set { _recept = value; }
        }


        public Pay()
        {
            Extra = new System.Collections.Specialized.NameValueCollection();
        }
        public Pay(string Txn_id, string Account, int Prv_id, DateTime Txn_date, Decimal From_amount, Decimal To_amount, int Recept  )
        {
            Extra = new System.Collections.Specialized.NameValueCollection();
            _txn_id = Txn_id;
            _prv_id = Prv_id;
            _txn_date = Txn_date;
            _from_amount = From_amount;
            _to_amount = To_amount;
            _recept = Recept;
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public virtual string ToXML()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            //doc.Name = "pays";

            System.Xml.XmlElement pay = doc.CreateElement("pay");

            pay.SetAttribute("prv_id", Prv_id.ToString());
            pay.SetAttribute("amount", From_amount.ToString());
            pay.SetAttribute("to-amount", To_amount.ToString());
            pay.SetAttribute("txn_id", Txn_id);
            pay.SetAttribute("receipt", Recept.ToString());
            pay.SetAttribute("txn_date", Txn_date.ToString("yyyyMMdd HH:mm:ss"));

            /*
             foreach (string line in Ipaybox.curPay.Options.Split('|'))
            {
                if (line != "")
                {
                    string[] param = line.Split('-');
                    if (param[0] != null)
                    {
                        XmlElement extra = Ipaybox.pays.CreateElement("extra");
                        if (param[1] == "account")
                            Ipaybox.curPay.account = param[2];
                        extra.SetAttribute("name", param[1]);
                        extra.SetAttribute("value", param[2]);
                        extra.SetAttribute("CRC", Ipaybox.getMd5Hash("n" + param[1] + "v" + param[2]));
                        pay.AppendChild(extra);
                    }
                }
            }
             */
            for (int i = 0; i < Extra.Count; i++)
            { 
                if(!string.IsNullOrEmpty(Extra.GetKey(i)))
                {
                    System.Xml.XmlElement extra = doc.CreateElement("extra");
                    extra.SetAttribute("name", Extra.GetKey(i));
                    extra.SetAttribute("value", Extra[i]);
                    extra.SetAttribute("CRC", Ipaybox.getMd5Hash("n" + Extra.GetKey(i) + "v" + Extra[i]));
                    pay.AppendChild(extra);
                }
            }
            doc.AppendChild(pay);
            return doc.ChildNodes[0].ToString();
        }
    }

}
