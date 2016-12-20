using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Printers
{
    public class T400
    {
        public System.IO.Ports.SerialPort com = new System.IO.Ports.SerialPort();
        public bool ok;
        public bool ERROR;
        public string ErrMsg;

        public T400(string portName)
        {
            //Определение параметров COM-порта
            com.PortName = portName;
            com.WriteTimeout = 10000;
            com.ReadTimeout = 2000;
            com.BaudRate = 38400;
            com.Parity = System.IO.Ports.Parity.None;
            com.DataBits = 8;
            com.StopBits = System.IO.Ports.StopBits.One;
            com.Handshake = System.IO.Ports.Handshake.RequestToSend;
            com.DtrEnable = false;
            com.RtsEnable = false;
            com.NewLine = System.Environment.NewLine;

            try
            {
                com.Open();

                //Инициализация принтера
                char[] chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x40 });
                com.Write(chars, 0, chars.Length);
            }
            catch (Exception ex)
            {
                if (com.IsOpen)
                    com.Close();

                throw ex;
            }
        }
        public bool Test()
        {
            try
            {
                // Инициализация принтера
                char[] init = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1D, 0x49, 1 });

                com.Write(init, 0, init.Length);
                System.Threading.Thread.Sleep(100);

                byte[] ans = new byte[com.BytesToRead];

                com.Read(ans, 0, com.BytesToRead);

                if (ans.Length > 0)
                {
                    if (ans[0] > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsOpen()
        {
            if (com != null)
                return com.IsOpen;
            else
                return false;
        }
        public bool Print(string str)
        {
            ok = false;

            if (com.IsOpen)
            {
                ok = true;

                try
                {
                    //Установка кодовой страницы для корректного отображения кириллицы
                    char[] chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x74, 17 });
                    com.Write(chars, 0, chars.Length);

                    //Установка размера шрифта
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1D, 0x21, 0x00 });
                    com.Write(chars, 0, chars.Length);

                    //Выбор фонта
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x4D, 1 });
                    com.Write(chars, 0, chars.Length);

                    //Установка межстрочного интервала
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x33, 0x17 });
                    com.Write(chars, 0, chars.Length);

                    byte[] ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);
                    string binary = string.Empty;

                    if (ans.Length > 0)
                        binary = Convert.ToString(ans[0], 2);

                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x61, 0 });
                    com.Write(chars, 0, chars.Length);
                    byte[] toprint = System.Text.Encoding.Convert(Encoding.Default, Encoding.GetEncoding(866), Encoding.Default.GetBytes(str + "\r\n"));
                    com.Write(toprint, 0, toprint.Length);

                    ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);

                    if (ans.Length > 0)
                        binary = Convert.ToString(ans[0], 2);

                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x0A });
                    com.Write(chars, 0, chars.Length);

                    ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);

                    binary = string.Empty;

                    if (ans.Length > 0)
                        for (int i = 0; i < ans.Length; i++)
                        {
                            binary += " " + Convert.ToString(ans[i], 2);
                        }

                    //Обрезка
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1D, 0x56, 66, 0 });
                    com.Write(chars, 0, chars.Length);

                    ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);

                    binary = string.Empty;

                    if (ans.Length > 0)
                        for (int i = 0; i < ans.Length; i++)
                        {
                            binary += " " + Convert.ToString(ans[i], 2);
                        }

                    return ok;
                }
                catch (Exception ex)
                {
                    ok = false;
                    ERROR = true;
                    ErrMsg = ex.Message;
                    return ok;
                }
            }

            return ok;
        }
        public bool PrintFromBytes(byte[] str)
        {
            ok = false;

            if (com.IsOpen)
            {
                ok = true;

                try
                {
                    //Установка кодовой страницы для корректного отображения кириллицы
                    char[] chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x74, 17 });
                    com.Write(chars, 0, chars.Length);

                    /*//Установка размера шрифта
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1D, 0x21, 0x00 });
                    com.Write(chars, 0, chars.Length);

                    //Выбор фонта
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x4D, 49 });
                    com.Write(chars, 0, chars.Length);*/

                    // Выбор режима печати
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x21, 0x01 });
                    com.Write(chars, 0, chars.Length);

                    //Установка межстрочного интервала
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x33, 0x15 });
                    com.Write(chars, 0, chars.Length);

                    byte[] ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);
                    string binary = string.Empty;

                    if (ans.Length > 0)
                        binary = Convert.ToString(ans[0], 2);

                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1B, 0x61, 0 });
                    com.Write(chars, 0, chars.Length);
                    com.Write(str, 0, str.Length);

                    ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);

                    if (ans.Length > 0)
                        binary = Convert.ToString(ans[0], 2);

                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x0A });
                    com.Write(chars, 0, chars.Length);

                    ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);

                    binary = string.Empty;

                    if (ans.Length > 0)
                        for (int i = 0; i < ans.Length; i++)
                        {
                            binary += " " + Convert.ToString(ans[i], 2);
                        }

                    //Обрезка
                    chars = System.Text.Encoding.ASCII.GetChars(new byte[] { 0x1D, 0x56, 66, 0 });
                    com.Write(chars, 0, chars.Length);

                    ans = new byte[com.BytesToRead];
                    com.Read(ans, 0, com.BytesToRead);

                    binary = string.Empty;

                    if (ans.Length > 0)
                        for (int i = 0; i < ans.Length; i++)
                        {
                            binary += " " + Convert.ToString(ans[i], 2);
                        }

                    return ok;
                }
                catch (Exception ex)
                {
                    ok = false;
                    ERROR = true;
                    ErrMsg = ex.Message;
                    return ok;
                }
            }

            return ok;
        }
        public void Close()
        {
            if (com != null)
            {
                if (com.IsOpen)
                    com.Close();
            }
        }
    }
}
