using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Acceptors;

namespace zeus.LoginForms
{
    public partial class AcceptorTestFrm : Form
    {
        // Внесенная сумма
        Decimal InsSumm;
        bool needPooling = false;

        public AcceptorTestFrm()
        {
            InitializeComponent();
        }

        private void AcceptorTestFrm_Load(object sender, EventArgs e)
        {
            //Инициализация купюроприемника
            if (Ipaybox.Bill == null)
            {
                Ipaybox.Bill = new Acceptors.Independed();

                // Если модель NULL - купюроприемник не найден.
                // Добавляется запись в лог, отключается таймер и кнопки на форме
                if (Ipaybox.Bill.model == Acceptors.Model.NULL && !Ipaybox.Debug)
                {
                    textBox1.Text += "Не удалось обнаружить купюроприемник. Тест неактивен.\r\n";
                    Pooling.Enabled = false;
                    groupBox1.Enabled = false;
                }
                else
                {
                    Pooling.Enabled = true;
                }
            }

            Ipaybox.Bill.Flush();

            Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StartTake);

            if (res == Acceptors.Result.Error || res == Result.Null)
            {
                label2.Text = "START TAKE FAILURE";
            }

            //Запрет на прием денег
            Ipaybox.Bill.accmoney.R10 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R50 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R100 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R500 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R1000 = !checkBox1.Checked;
            Ipaybox.Bill.AllowMoneyEnterOnPooling = checkBox1.Checked;

            needPooling = true;
        }

        private void Pooling_Tick(object sender, EventArgs e)
        {
            if (needPooling)
            {
                try
                {
                    Ipaybox.Bill.Pooling();

                    if (InsSumm != Ipaybox.Bill.Amount)
                    {
                        // Обработка денег
                        textBox1.Text += Ipaybox.Bill.LastResult.ToString().Trim() + "\r\n";
                        Decimal bill = Ipaybox.Bill.Amount - InsSumm;
                        InsSumm = Ipaybox.Bill.Amount;
                        label1.Text = Ipaybox.Bill.BillCount.ToString();
                        textBox1.Text += "Внесена купюра " + bill.ToString() + "\r\n";
                    }

                    // Обработка ошибок
                    if (Ipaybox.Bill.Error)
                    {
                        Match m = Regex.Match(Ipaybox.Bill.ErrorMsg, @"HEXERR[:](?<val>.*?)");

                        string err = string.Empty;

                        if (m.Success)
                        { 
                            err = m.Groups["val"].Value.ToString().Trim();
                        }

                        if (Ipaybox.Bill.ErrorMsg.IndexOf("2 3 7 82 2 2E 23") >= 0)
                            textBox1.AppendText("Внесена купюра 10. Возврат.\r\n");
                        else
                            if (Ipaybox.Bill.ErrorMsg.IndexOf("2 3 7 82 3 A7 32") >= 0)
                                textBox1.AppendText("Внесена купюра 50. Возврат.\r\n");
                            else
                                if (Ipaybox.Bill.ErrorMsg.IndexOf("2 3 7 82 4 18 46") >= 0)
                                    textBox1.AppendText("Внесена купюра 100. Возврат.\r\n");
                                else
                                    if (Ipaybox.Bill.ErrorMsg.IndexOf("2 3 7 82 5 91 57") >= 0 && Ipaybox.Bill.ErrorMsg.IndexOf("DROP_CASSETTE_REMOVED") < 0)
                                        textBox1.AppendText("Внесена купюра 500. Возврат.\r\n");
                                    else
                                        if (Ipaybox.Bill.ErrorMsg.IndexOf("2 3 7 82 6 A 65") >= 0)
                                            textBox1.AppendText("Внесена купюра 1000. Возврат.\r\n");
                                        else
                                        {
                                            Pooling.Enabled = false;
                                            Pooling.Stop();
                                            Ipaybox.AddToLog(Ipaybox.Logs.Main, "Ошибка купюроприемника при тестировании в СМ\r\n" + Ipaybox.Bill.ErrorMsg);
                                            textBox1.AppendText(Ipaybox.Bill.ErrorMsg + "\r\nОпрос остановлен. Для продолжения нажмите `Сброс` ");

                                            Ipaybox.Bill.BillAcceptorActivity = false;
                                        }
                    }
                    else
                        textBox1.AppendText("OK\r\n");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось обнаружить купюроприемник! " + ex.Message);
                    needPooling = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Pooling.Stop();
            //Сброс?
            Ipaybox.Bill.Flush();
            //Остановка приема купюр
            Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StopTake);
            //Сброс?
            res = Ipaybox.Bill.SendCommand(Commands.Reset);
            textBox1.AppendText("Reset\r\n");
            //Старт приема купюр
            res = Ipaybox.Bill.SendCommand(Commands.StartTake); 
 
            //Обнуление значений
            InsSumm = 0;
            textBox1.Text = string.Empty;
            label1.Text = "Купюр: 0";

            Ipaybox.Bill.accmoney.R10 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R50 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R100 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R500 = !checkBox1.Checked;
            Ipaybox.Bill.accmoney.R1000 = !checkBox1.Checked;
            Ipaybox.Bill.AllowMoneyEnterOnPooling = checkBox1.Checked;
            label2.Text = res.ToString();

            //Запуск таймера
            Pooling.Enabled = true;
            Pooling.Start();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StopTake);
            Ipaybox.Bill.Flush();
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StopTake);
            label2.Text = res.ToString();
            Pooling.Stop();
            textBox1.AppendText("Остановлено\r\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StartTake);
            Ipaybox.Bill.AllowMoneyEnterOnPooling = checkBox1.Checked;
            label2.Text = res.ToString();
            Pooling.Start();
            textBox1.AppendText("Запущено\r\n");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            needPooling = false;

            //Сброс
            Ipaybox.Bill.Flush();
            InsSumm = 0;

            //Остановка приема купюр
            Acceptors.Result res = Ipaybox.Bill.SendCommand(Commands.StopTake);

            //Старт приема купюр
            res = Ipaybox.Bill.SendCommand(Commands.StartTake);

            //С укладкой/без укладки в зависимости от выбранной опции
            Ipaybox.Bill.accmoney.R10 = checkBox1.Checked;
            Ipaybox.Bill.accmoney.R50 = checkBox1.Checked;
            Ipaybox.Bill.accmoney.R100 = checkBox1.Checked;
            Ipaybox.Bill.accmoney.R500 = checkBox1.Checked;
            Ipaybox.Bill.accmoney.R1000 = checkBox1.Checked;
            Ipaybox.Bill.AllowMoneyEnterOnPooling = checkBox1.Checked;

            needPooling = true;
        }
    }
}
