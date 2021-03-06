﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotRas;
using System.IO.Ports;


namespace zeus
{
    public static class _Modem
    {
        //private static bool PortIsOpen = false;
        private static SerialPort port = new SerialPort();
        //Определение параметров порта и попытка открытия порта модема
        private static bool OpenPort(string portName, int writeTimeOut, int readTimeOut, int baudRate,
            Parity parity, int dataBits, StopBits stopBits, Handshake handshake, bool dtrEnable, bool rtsEnable,
            string newLine)
        {
            //Попытка инициализировать порт
            try
            {
                port.PortName = portName;
                port.WriteTimeout = 500;
                port.ReadTimeout = 500;
                port.BaudRate = 115200;
                port.Parity = System.IO.Ports.Parity.None;
                port.DataBits = 8;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.Handshake = System.IO.Ports.Handshake.RequestToSend;
                port.DtrEnable = true;
                port.RtsEnable = true;
                port.NewLine = System.Environment.NewLine;
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                return false;
            }

            //Попытка открытия порта
            try
            {
                port.Open();
            }
            catch
            {                
                return false;
            }

            //Попытка записи в порт
            try
            {
                port.WriteLine("AT");
                System.Threading.Thread.Sleep(100);
                string portans = port.ReadExisting().Trim();

                if (portans.IndexOf("OK") == -1)
                {
                    port.Close();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                port.Close();
                return false;
            }
        }

        private static void ClosePort()
        {
            try
            {
                if (port.IsOpen)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Закрытие порта " + port.PortName);
                    port.Close();
                }
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "БАХ2!");
                throw ex;
            }
        }
        public static bool SendSMS(string cellNumber)
        {
            bool res = false;
            cellNumber = cellNumber.Replace("+", "");

            try
            {
                //Разорвать соединение
                Ipaybox.Modem.Disconnect(Ipaybox.NetModemName);
            }
            catch { }

            try
            {
                OpenPort(Ipaybox.modemPort, 500, 500, 115200, Parity.None, 8, StopBits.One, Handshake.RequestToSend, true, true, Environment.NewLine);

                if (port != null)
                    if (port.IsOpen)
                    {
                        try
                        {
                            string s = string.Empty;

                            List<SMS.SMS.CompositeSMS> CS = SMS.SMS.FormSMS(cellNumber, "ЗЕВС. IMSI=" + Ipaybox.IMSI);

                            for (int i = 0; i < CS.Count; i++)
                            {
                                s = port.ReadExisting();
                                s = string.Empty;

                                //1. Подготовка к отправке
                                port.Write("AT+CMGS=" + Convert.ToString(CS[i].TPDULength) + "\r");
                                //port.WriteLine("AT+CMGS=" + Convert.ToString(CS[i].TPDULength));
                                //2. Задержка, чтобы модем успел переварить то, что ему скормили
                                System.Threading.Thread.Sleep(500);
                                s = port.ReadExisting().Trim();

                                //3. Ну и, собственно, само уже сформированное сообщение
                                s = string.Empty;
                                port.Write(CS[i].FullPDUString + (char)(26));
                                System.Threading.Thread.Sleep(1000);

                                int j = 0;

                                while (s == string.Empty && j < 3)
                                {
                                    s = port.ReadExisting().Trim();
                                    System.Threading.Thread.Sleep(1500);
                                    j++;
                                }

                                if (s.IndexOf("ERROR") != -1 || s == string.Empty)
                                {
                                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "\r\nНе удалось отправить СМС \r\nна номер:" + cellNumber + "\r\n" + res);
                                    return false;
                                }
                            }

                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "\r\nОтправлено СМС на номер: " + cellNumber + "\r\n" + res);
                            return true;                            
                        }
                        catch (Exception ex)
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                            return false;
                        }
                        finally
                        {
                            port.Close();
                            port.Dispose();
                        }
                    }
            }
            catch (Exception ex)
            {
                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                return false;
            }

            return res;
        }
        public static void Init()
        {
            string res;
            string[] ComPorts = SerialPort.GetPortNames();

            for (int i = 0; i < ComPorts.Length; i++)
            {
                try
                {
                    //Разорвать соединение
                    Ipaybox.Modem.Disconnect(Ipaybox.NetModemName);
                }
                catch { }

                try
                {
                    if (OpenPort(ComPorts[i], 500, 500, 115200, Parity.None, 8, StopBits.One, Handshake.RequestToSend, true, true, Environment.NewLine))
                        if (port.IsOpen)
                        {
                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Модем подключен к порту " + port.PortName);
                            Ipaybox.modemPort = ComPorts[i];

                            try
                            {
                                //Определение оператора связи
                                port.WriteLine("AT+COPS?");
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Команда " + "AT+COPS?");
                                System.Threading.Thread.Sleep(500);
                                res = port.ReadExisting().Trim();
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ответ " + res.Trim());

                                Match m = Regex.Match(res, "\"(?<val>.*?)\"");

                                if (m.Success)
                                {
                                    Ipaybox.OpsosName = m.Groups["val"].ToString().Trim();
                                }

                                //Определение IMSI
                                port.WriteLine("AT+CIMI");
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Команда " + "AT+CIMI");
                                System.Threading.Thread.Sleep(1000);
                                res = port.ReadExisting().Trim();
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ответ " + res.Trim());

                                if (res.Trim().IndexOf("ERROR") == -1)
                                {
                                    m = Regex.Match(res, "[0-9]+", RegexOptions.Singleline);

                                    if (m.Success)
                                        Ipaybox.IMSI = m.Groups[0].ToString().Trim();
                                }
                            }
                            catch (Exception ex)
                            {
                                Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                            }
                            finally
                            {
                                port.Close();
                                port.Dispose();
                            }

                            break;
                        }
                }
                catch (Exception ex)
                {
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, ex.Message);
                }
            }
        }
    }

    public class modem
    {
        public string[] Entry;
        DotRas.RasDialer dialer;
        DotRas.RasPhoneBook phonebook;

        public bool Connected;

        public modem()
        {
            try
            {
                dialer = new RasDialer();
                phonebook = new RasPhoneBook();

                dialer.DialCompleted += new EventHandler<DialCompletedEventArgs>(dialer_DialCompleted);
                dialer.StateChanged += new EventHandler<StateChangedEventArgs>(dialer_StateChanged);
                phonebook.Open();

                Entry = new string[phonebook.Entries.Count];

                for (int i = 0; i < phonebook.Entries.Count; i++)
                {
                    Entry[i] = phonebook.Entries[i].Name;
                }
            }
            catch(Exception ex)
            {
                HelperClass.CrashLog.AddCrash(ex);
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Не удалось инициализировать класс работы с GPRS;");
            }
        }
        private void dialer_StateChanged(object obj, StateChangedEventArgs args)
        { 
            if(Ipaybox.Debug)
                Ipaybox.AddToLog(Ipaybox.Logs.Main, "Модем State: "+args.State.ToString()+" Message:"+args.ErrorMessage);
            if (args.State == RasConnectionState.Connected)
                Connected = true;
            else
                Connected = false;
        }
        private void dialer_DialCompleted(object obj,DialCompletedEventArgs args )
        {
            Connected = args.Connected;

            if (Ipaybox.Debug)
            {
                if(Connected)
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Соединение установлено.");
                else
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Соединение НЕ установлено.");
            }
        }
        public void Connect(string entry)
        {
            try
            {
                if (!IsConnected(entry))
                {
                    if (Ipaybox.Debug)
                        Ipaybox.AddToLog(Ipaybox.Logs.Main, "Попытка соединения через " + entry);
                    dialer.PhoneBookPath = phonebook.Path;
                    dialer.EntryName = entry;
                    dialer.DialAsync();  
                }
            }
            catch(Exception ex)
            {
                if (Ipaybox.Debug)
                    Ipaybox.AddToLog(Ipaybox.Logs.Main, "Соединение НЕ установлено.");
                HelperClass.CrashLog.AddCrash(ex);
                Connected = false;
            }
        }
        public void Disconnect(string entry)
        {
            try
            {
                for (int i = 0; i < dialer.GetActiveConnections().Count; i++)
                {
                    if (entry == dialer.GetActiveConnections()[i].EntryName.ToString())
                    {
                        dialer.GetActiveConnections()[i].HangUp();
                    }
                }
            }
            catch
            { }
        }
        public bool IsConnected(string entry)
        {
            try
            {
                for (int i = 0; i < dialer.GetActiveConnections().Count; i++)
                {
                    if (entry == dialer.GetActiveConnections()[i].EntryName.ToString())
                    {
                        if (dialer.GetActiveConnections()[i].EntryName == entry)
                            if (dialer.GetActiveConnections()[i].GetConnectionStatus().ConnectionState == RasConnectionState.Connected)
                            {
                                Ipaybox.Internet = true;
                                return true;
                            }
                            else
                            {
                                Ipaybox.Internet = false;
                                return false;
                            }
                    }

                }
                Ipaybox.Internet = false;
            }
            catch
            { }
            return false;

        }

    }
}
