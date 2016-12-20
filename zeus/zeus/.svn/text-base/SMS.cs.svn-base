using System;
using System.Collections.Generic;
using System.Text;

namespace SMS
{
    class SMS
    {
        public class CompositeSMS
        {
            //Длина блока TPDU
            public int TPDULength = 0;
            public string FullPDUString = string.Empty;
        }

        public class IncomeSMS
        {
            //Порядковый номер СМС-сообщения на носителе, для его удаления
            public int smsNumber = 0;
            //СМС-центр
            public string TPSCA = string.Empty;
            //Служебный байт
            public string TPMTI = string.Empty;
            //Отправитель
            public string TPOA = string.Empty;
            public string Sender = string.Empty;
            //Поле UDH, если СМС состоит больше, чем из одной части
            public string TPUDH = string.Empty;
            //Текст сообщения
            public string SMSText = string.Empty;
        }

        //СМС-центр
        //По-умолчанию пусть будет 00. Это значит, что номер СМС-центра не указываем.
        //Потом переделаю, если что, вдруг нужно будет указывать...
        private const string SCA = "00";
        //Тип сообщения. Цельное или составное
        //По-умолчанию будет цельное
        private static string PDUType = "01";
        //Уникальный идентификатор части сообщения для тех случаев, 
        //когда сообщение состоит более чем из одной части
        private static string TPMR = "00";
        //Длина номера получателя(количество цифр!), 
        //тип номера получателя (всегда у нас будет 91 - международный)
        //и сам номер получателя в перекодированном виде
        private static string TPDA = string.Empty;
        //Какая-то хуйня...
        private static string TPPID = "00";
        //Кодировка сообщения. Так как всегда будем отправлять в формате UCS2 (чтобы не заморачиваться лишний раз),
        //то он всегда будет 08 (2 байта на символ)
        private const string TPDCS = "08";
        //Длина сообщения. Указывается в байтах.
        private static string TPUDL = "00";
        //UDH - заголовок, который используется только для сообщений, состоящих больше чем из одной части
        private static string UDH = string.Empty;
        //Ну и, собственно, само сообщение
        private static string TPUD = string.Empty;

        //Максимальная длина сообщения
        private static byte maxLength = 70;
        //Количество частей в СМС-сообщении. По умолчанию - 1 (сообщение не больше 70 символов)
        private static byte smsCount = 1;

        static String strAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЬЪЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'-* :;)(.,!=_";
        static String[] ArrayUCSCode = new String[140]{            
            "0410","0411","0412","0413","0414","0415","00A8","0416","0417",
            "0418","0419","041A","041B","041C","041D","041E","041F","0420",
            "0421","0422","0423","0424","0425","0426","0427","0428","0429",
            "042C","042A","042D","042E","042F","0430","0431","0432","0433",
            "0434","0435","00B8","0436","0437","0438","0439","043A","043B",
            "043C","043D","043E","043F","0440","0441","0442","0443","0444",
            "0445","0446","0447","0448","0449","044A","044B","044C","044D",
            "044E","044F","0041","0042","0043","0044","0045","0046","0047",
            "0048","0049","004A","004B","004C","004D","004E","004F","0050",
            "0051","0052","0053","0054","0055","0056","0057","0058","0059",
            "005A","0061","0062","0063","0064","0065","0066","0067","0068",
            "0069","006A","006B","006C","006D","006E","006F","0070","0071",
            "0072","0073","0074","0075","0076","0077","0078","0079","007A",
            "0030","0031","0032","0033","0034","0035","0036","0037","0038",
            "0039","0027","002D","002A","0020","003A","003B","0029","0028",
            "002E","002C","0021","003D","005F"};

        static private String TextToUCS2(String Text)
        {
            Text = Text.Trim();
            Text = Text.Replace(@"\r\n", " ");
            StringBuilder UCS = new StringBuilder(Text.Length);
            Int32 intLetterIndex = 0;

            for (int i = 0; i < Text.Length; i++)
            {
                intLetterIndex = strAlphabet.IndexOf(Text[i]);
                if (intLetterIndex != -1)
                {
                    UCS.Append(ArrayUCSCode[intLetterIndex]);
                }

            }
            return UCS.ToString();
        }

        //Преобразование текста из ANSI в UCS
        static private String ConvertTextToUCS(String InputText)
        {
            UnicodeEncoding ue = new UnicodeEncoding();

            byte[] ucs2 = ue.GetBytes(InputText);

            for (int i = 0; i < ucs2.Length; i += 2)
            {
                byte b = ucs2[i + 1];
                ucs2[i + 1] = ucs2[i];
                ucs2[i] = b;
            }

            return BitConverter.ToString(ucs2).Replace("-", "");
        }
        //Преобразование телефонного номера
        static private String CodePhoneNumber(String PhoneNumber)
        {
            StringBuilder NewNumber = new StringBuilder(PhoneNumber.Length);
            if (PhoneNumber.Length / 2 == PhoneNumber.Length / 2.0) // число четное
            {
                for (int i = 0; i < PhoneNumber.Length / 2; i++)
                {
                    NewNumber.Append(PhoneNumber[2 * i + 1].ToString());
                    NewNumber.Append(PhoneNumber[2 * i].ToString());
                }
            }
            else // номер с нечетным кол-вом символом
            {
                for (int i = 0; i < PhoneNumber.Length / 2; i++)
                {
                    NewNumber.Append(PhoneNumber[2 * i + 1].ToString());
                    NewNumber.Append(PhoneNumber[2 * i].ToString());
                }
                NewNumber.Append("F");
                NewNumber.Append(PhoneNumber[PhoneNumber.Length - 1]);
            }
            return NewNumber.ToString();
        }
        static private String DecodePhoneNumber(String PhoneNumber)
        {
            StringBuilder NewNumber = new StringBuilder(PhoneNumber.Length);

            //Меняем цифры попарно
            for (int i = 0; i < PhoneNumber.Length / 2; i++)
            {
                NewNumber.Append(PhoneNumber[2 * i + 1].ToString());
                NewNumber.Append(PhoneNumber[2 * i].ToString());
            }

            //Если обнаружен символ F - убираем его
            NewNumber = NewNumber.Replace("F", string.Empty);

            return NewNumber.ToString();
        }
        //Формирование СМС-сообщения в удобоваримом для модема формате
        //Короче, всякие преборазования и разделения
        static public List<CompositeSMS> FormSMS(string cellNumber, string smsText)
        {
            //Перекодировка номера телефона получателя
            TPDA = String.Format("{0:X2}", cellNumber.Length) + "91" + CodePhoneNumber(cellNumber);
            //Части сообщения для тех случаев, когда сообщение содержит более одной части
            List<string> messageParts = new List<string>();
            List<CompositeSMS> FullMessageParts = new List<CompositeSMS>();

            string PDUMessage = string.Empty;

            if (smsText.Length > 70)
                maxLength = 67;

            //Определяем количество частей для отправки сообщения "одним целым"
            smsCount = Convert.ToByte(Math.Ceiling(Convert.ToDouble(smsText.Length) / Convert.ToDouble(maxLength)));

            //Так как, блять, добавился блок UDH
            //Если длина сообщения > 70 символов (140 байт) => сообщение состоит из нескольких частей
            //В зависимости от этого в заголовке должно быть прописано 01 (сообщение из одной части) 
            //или 41 (сообщение из нескольких частей)
            if (smsCount > 1)
            {
                PDUType = "41";
            }
            else
            {
                PDUType = "01";
            }

            //Генерация случайного числа
            //Для уникализации СМС-сообщения
            Random r = new Random();
            int R = r.Next(255);

            //Разбиение текста сообщение на части с количеством символов не более 70
            for (int i = 0; i < smsCount; i++)
            {
                if (i != smsCount - 1)
                    messageParts.Add(smsText.Substring(i * maxLength, maxLength));
                else
                    messageParts.Add(smsText.Substring(i * maxLength));
            }

            for (int i = 0; i < messageParts.Count; i++)
            {
                if (i < 10)
                    TPMR = "0" + i.ToString();
                else
                    TPMR = i.ToString();

                if (messageParts.Count > 1)
                    UDH = "05" + "0003" + String.Format("{0:X2}", R) + String.Format("{0:X2}", messageParts.Count) + String.Format("{0:X2}", i + 1);
                else
                    UDH = string.Empty;

                TPUDL = String.Format("{0:X2}", messageParts[i].Length * 2 + UDH.Length / 2);
                TPUD = ConvertTextToUCS(messageParts[i]);
                PDUMessage = SCA + PDUType + TPMR + TPDA + TPPID + TPDCS + TPUDL + UDH + TPUD;
                CompositeSMS cSMS = new CompositeSMS();
                cSMS.TPDULength = PDUType.Length / 2 + TPMR.Length / 2 + TPDA.Length / 2 + TPPID.Length / 2
                    + TPDCS.Length / 2 + TPUDL.Length / 2 + UDH.Length / 2 + TPUD.Length / 2;
                cSMS.FullPDUString = PDUMessage;
                FullMessageParts.Add(cSMS);
            }

            return FullMessageParts;
        }

        //Перекодировка текста из UCS2 в ANSI
        static private String ConvertUCSToAnsi(string InputText)
        {
            StringBuilder ANSIText = new StringBuilder();
            Int32 intLetterIndex = 0;

            int i = 0;

            while (i < InputText.Length)
            {
                intLetterIndex = System.Array.IndexOf(ArrayUCSCode, InputText.Substring(i, 4));

                if (intLetterIndex > -1)
                {
                    ANSIText.Append(strAlphabet.Substring(intLetterIndex, 1));
                }

                i += 4;
            }

            return ANSIText.ToString();
        }
        //Расшифровка входящего СМС-сообщения
        static public IncomeSMS ParseSMS(string FullModemText)
        {
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(FullModemText, @"^CMGL[:] [0-9]*[,][0-9]*[,].*?[,][0-9]*|");

            if (FullModemText.Trim() == string.Empty || !m.Success)
                return null;

            string[] smsParts = FullModemText.Split('|');

            if (smsParts.Length != 2)
                return null;

            IncomeSMS IncSMS = new IncomeSMS();

            m = System.Text.RegularExpressions.Regex.Match(smsParts[0], @"^CMGL[:](?<val>.*?)[,].*$");

            if (m.Success)
                IncSMS.smsNumber = Convert.ToInt32(m.Groups["val"].ToString().Trim());
            else
                return null;

            //Просто введем переменную, которая у нас будет "кареткой", бля, чтобы знать местоположение в строке
            int CurrentIndex = 0;

            //Определяем длину TPSCA
            int SCALength = Convert.ToByte(smsParts[1].Substring(0, 2), 16) * 2 + 2;
            //На всякий случай запомним, вдруг пригодится в будущем
            IncSMS.TPSCA = smsParts[1].Substring(0, SCALength);
            //Байт TPMTI - служебный. Просто пока запомним его
            IncSMS.TPMTI = smsParts[1].Substring(SCALength, 2);
            //Номер отправителя
            //Сначала вычисляется его длина в ЦИФРАХ
            int OALength = Convert.ToByte(smsParts[1].Substring(IncSMS.TPSCA.Length + IncSMS.TPMTI.Length, 2), 16);

            if (OALength / 2 != OALength / 2.0)
                OALength += 3;

            CurrentIndex = IncSMS.TPSCA.Length + IncSMS.TPMTI.Length + 2;
            //Потом он записывается ПОЛНОСТЬЮ вместе с форматом (91 - значит международный формат. По-нашему: федеральный номер)
            IncSMS.TPOA = smsParts[1].Substring(CurrentIndex, OALength);
            CurrentIndex += OALength;
            IncSMS.Sender = DecodePhoneNumber(IncSMS.TPOA.Substring(2));
            //И упираемся в два служебных байта
            //Первый байт нам нах не нужен (идентификатор протокола), поэтому идем мимо него
            CurrentIndex += 2;
            //Второй байт уже поинтереснее... это поле TP-DCS - схема кодирования данных
            //Нас интересует только 08. Потом, если будет необходимо, сделаю перекодирование не только из UCS2 но и из 7-битной кодировки
            string TPDCS = smsParts[1].Substring(CurrentIndex, 2);
            CurrentIndex += 2;

            //Если не 08 - выходим, нахуй...
            if (TPDCS != "08")
                if (TPDCS == "00")
                {
                    //Если 00 - то это 7-ми битная кодировка. Расшифровываем...

                }
                else
                    return null;

            //Теперь нужно выковырять время. ГГММДДЧЧММССВЗ
            //Тут тоже нужно попарно переворачивать... пиздос
            //а хотя нахуй оно нужно... идем мимо...
            CurrentIndex += 14;
            //Следующий байт указывает длину самого, собственно, сообщения
            int smsTextLength = Convert.ToByte(smsParts[1].Substring(CurrentIndex, 2), 16) * 2;
            CurrentIndex += 2;
            IncSMS.SMSText = ConvertUCSToAnsi(smsParts[1].Substring(CurrentIndex, smsTextLength));
            CurrentIndex += smsTextLength;

            return IncSMS;
        }
    }
}