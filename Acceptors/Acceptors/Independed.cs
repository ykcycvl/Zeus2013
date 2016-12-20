using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Printers
{
    public enum Model { NULL,ICT, VKP80, AV268, CITIZEN, TUP900};
    

   

    public class Indepened 
    {
        public Model PrnModel;

        

        public string port;
        public string ErrorMessage;

        ICT ict ;
        AV286 av;
        VKP80 vkp;
        Citizen cit;
        Tup900 tup;
        /// <summary>
        /// В конструкторе осуществляется поиск устройств на свободных ПОРТАХ
        /// </summary>
        public Indepened()
        {
            Initialize();
        }
        ~Indepened()
        {

            switch (PrnModel)
            {
                case Model.ICT:
                    if (ict == null)
                        if (this.port != null)
                            ict.Close();
                    break;
                 case Model.VKP80:
                    if (vkp == null)
                        if (this.port != null)
                            vkp.Close();
                    break;   
                case Model.AV268:
                    if (av == null)
                        if (this.port != null)
                            av.Close();
                    break;
                case Model.CITIZEN:
                    if (cit == null)
                        if (this.port != null)
                            cit.Close();
                    break;
                case Model.TUP900:
                    if (tup == null)
                        if (this.port != null)
                            tup.Close();
                    break;
            }
        }
        public void Initialize()
        {
            List<string> cmports = new List<string>();

            try
            {
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

            string[] availableCom = System.IO.Ports.SerialPort.GetPortNames();

            for (int i = 0; i < availableCom.Length; i++)
            {
                if (cmports.Count > 0)
                {
                    string s = cmports.Find(p => p == availableCom[i]);

                    if (!String.IsNullOrEmpty(s))
                        continue;
                }
                
                try
                {
                    ict = new ICT(availableCom[i]);

                    if (ict.Test())
                    {
                        port = availableCom[i];
                        PrnModel = Model.ICT;
                        break;
                    }
                    if (ict != null)
                        if (ict.IsOpen())
                            ict.Close();
                }
                catch (Exception ex)
                {
                    if (ict != null)
                        if (ict.IsOpen())
                            ict.Close();
                }
                System.Threading.Thread.Sleep(100);
                try
                {
                    vkp = new VKP80(availableCom[i]);
                    System.Threading.Thread.Sleep(100);
                    if (vkp.GetVersion() == "VKP80")
                    {
                        PrnModel = Model.VKP80;
                        port = availableCom[i];
                        break;
                    }
                    if (vkp != null)
                        if (vkp.IsOpen())
                            vkp.Close();
                }
                catch (Exception ex)
                {
                    if (vkp != null)
                        if (vkp.IsOpen())
                            vkp.Close();

                }
                try
                {
                    av = new AV286(availableCom[i]);
                    System.Threading.Thread.Sleep(100);
                    if (av.Version.IndexOf("AV") != -1)
                    {
                        PrnModel = Model.AV268;
                        port = availableCom[i];
                        break;
                    }
                    if (av != null)
                        if (av.IsOpen())
                            av.Close();
                }
                catch (Exception ex)
                {
                    if (av != null)
                        if (av.IsOpen())
                            av.Close();

                }
                try
                {
                    cit = new Citizen(availableCom[i]);
                    System.Threading.Thread.Sleep(100);
                    cit.Test();
                    if (cit.Version.IndexOf("1000") != -1 || cit.Version.IndexOf("700") != -1)
                    {
                        PrnModel = Model.CITIZEN;
                        port = availableCom[i];
                        break;
                    }
                    if (cit != null)
                        if (cit.IsOpen())
                            cit.Close();
                }
                catch (Exception ex)
                {
                    if (cit != null)
                        if (cit.IsOpen())
                            cit.Close();

                }
                try
                {
                    tup = new Tup900(availableCom[i]);
                    //System.Threading.Thread.Sleep(100);
                    if(tup.Test())
                    {
                        PrnModel = Model.TUP900;
                        port = availableCom[i];
                        break;
                    }
                    if (tup != null)
                        tup.Close();
                }
                catch (Exception ex)
                {
                    if (tup != null)
                        tup.Close();

                }
                      
            }
        }
        /// <summary>
        /// Тестируем принтер чека
        /// </summary>
        /// <returns>Результат теста прошел/не прошел</returns>
        public bool Test()
        {
            bool p;
            switch(PrnModel)
            {
                case  Model.ICT:
                    if (ict == null )
                        if(this.port != null)
                            ict = new ICT(this.port);
                        else
                            Initialize();
                    
                    if (ict == null)
                        return false;

                    
                    return ict.Test();
                case Model.VKP80:
                    if (vkp == null)
                        if (this.port != null)
                            vkp = new VKP80(this.port);
                        else
                            Initialize();

                    if (vkp == null)
                        return false;


                    p = vkp.Test();
                    ErrorMessage = vkp.ErrMsg;
                    return p;
                case Model.AV268:
                    if (av == null)
                        if (this.port != null)
                            av = new AV286(this.port);
                        else
                            Initialize();

                    if (av == null)
                        return false;

                    p = av.Test();
                    ErrorMessage = av.ErrMsg;
                    return p;
                case Model.CITIZEN:
                    if (cit == null)
                        if (this.port != null)
                            cit = new Citizen(this.port);
                        else
                            Initialize();

                    if (cit == null)
                        return false;

                    p = cit.Test();
                    ErrorMessage = cit.ErrMsg;
                    return p;
                case Model.TUP900:
                    if (tup == null)
                        if (this.port != null)
                            tup = new Tup900(this.port);
                        else
                            Initialize();

                    if (tup == null)
                        return false;

                    p = tup.Test();
                    ErrorMessage = tup.ErrMsg;
                    return p;
                    
            }
           
            return false;
        }
        /// <summary>
        /// Печатаем чек
        /// </summary>
        /// <param name="check">Строка для печати</param>
        /// <returns>Напечатался или нет</returns>
        public bool Print(string check)
        {
            switch(PrnModel)
            {
                case  Model.ICT:
                    if (ict == null)
                        if (this.port != null)
                            ict = new ICT(this.port);
                        else
                            Initialize();
                    if (ict == null)
                        return false;
                        
                    return  ict.Print(check);
                case Model.VKP80:
                    if (vkp == null)
                        if (this.port != null)
                            vkp = new VKP80(this.port);
                        else
                            Initialize();
                    if (vkp == null)
                        return false;


                    return vkp.Print(check);

                case Model.AV268:
                    if (av == null)
                        if (this.port != null)
                            av = new  AV286(this.port);
                        else
                            Initialize();
                    if (av == null)
                        return false;

                    return av.Print(check);

                case Model.CITIZEN:
                    if (cit == null)
                        if (this.port != null)
                            cit = new Citizen(this.port);
                        else
                            Initialize();
                    if (cit == null)
                        return false;

                    return cit.Print(check);
                case Model.TUP900:
                    if (tup == null)
                        if (this.port != null)
                            tup = new Tup900(this.port);
                        else
                            Initialize();
                    if (tup == null)
                        return false;

                    return tup.Print(check);


                   
            }
            return false;
        }

        public void Close()
        {
            switch (PrnModel)
            {
                case Model.ICT:

                    ict.Close();
                    break;
                case Model.VKP80:
                    vkp.Close();
                    break;
                case Model.AV268:
                    av.Close();
                    break;
                case Model.CITIZEN:
                    cit.Close();
                    break;
                case Model.TUP900:
                    tup.Close();
                    break;


            }
             
        }

    }
}
