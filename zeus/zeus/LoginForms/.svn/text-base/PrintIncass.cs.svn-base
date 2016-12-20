using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeus.LoginForms
{
    public partial class PrintIncass : Form
    {
        public class Инкассация
        {
            public string Терминал { get; set; }
            public DateTime Дата_Инкассации { get; set; }
            public Decimal Сумма { get; set; }
            public string Идентификатор { get; set; }

            public zeus.API.IncassHistoryEntity Содержимое { get; set; }
        }

        public PrintIncass()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        private void PrintIncass_Load(object sender, EventArgs e)
        {
            List<Инкассация> sourse = new List<Инкассация>();
            if (Ipaybox.IncassHistory.Count > 0)
            {
                foreach (var item in Ipaybox.IncassHistory)
                {
                    //Загружать только платежи с этого теремаif (item.TerminalId == Ipaybox.Terminal.terminal_id && item.CheckMD5())
                    //{
                        sourse.Add(new Инкассация()
                        {
                            Дата_Инкассации = item.DateStoped,
                            Содержимое = item,
                            Сумма = item.Amount,
                            Терминал = item.TerminalId,
                            Идентификатор = item.Id.ToString()
                        });
                  /*  }
                    else
                    {
                        MessageBox.Show("Неверный номер терминала или неправильная проверочная сумма");
                    }*/
                }
                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.DataSource = sourse;
            }
        }
        private void PrintCheck()
        {
            string check = "-------КОПИЯ-------\r\n" +
                            "ИНКАССАЦИОННЫЙ ЧЕК\r\n" +
                            "------------------\r\n" +
                            "Терминал: " + Ipaybox.Terminal.terminal_id + "\r\n" +
                            "Адрес: " + Ipaybox.Terminal.trm_adress + "\r\n" +
                            "Дата: " + _selectedIncass.Содержимое.DateStoped + "\r\n" +
                            "------------------\r\n" +
                            "ID:  " + _selectedIncass.Содержимое.Id + "\r\n" +
                            "Кол-во купюр: " + _selectedIncass.Содержимое.CountBills.ToString() + "\r\n" +
                            "Кол-во 10:    " + _selectedIncass.Содержимое.CountBill10.ToString() + "\r\n" +
                            "Кол-во 50:    " + _selectedIncass.Содержимое.CountBill50.ToString() + "\r\n" +
                            "Кол-во 100:   " + _selectedIncass.Содержимое.CountBill100.ToString() + "\r\n" +
                            "Кол-во 500:   " + _selectedIncass.Содержимое.CountBill500.ToString() + "\r\n" +
                            "Кол-во 1000:  " + _selectedIncass.Содержимое.CountBill1000.ToString() + "\r\n" +
                            "Кол-во 5000:  " + _selectedIncass.Содержимое.CountBill5000.ToString() + "\r\n" +
                            "Байт принято: " + _selectedIncass.Содержимое.BytesRecieved.ToString() + "\r\n" +
                            "Байт послано: " + _selectedIncass.Содержимое.BytesSend.ToString() + "\r\n" +
                            "Платежей:     " + 0 + "\r\n" +
                            "------------------\r\n" +
                            "СУММА:        " + _selectedIncass.Содержимое.Amount.ToString() + "\r\n" +
                            "------------------\r\n";

            Ipaybox.PrintString = check;



        }
        private Инкассация _selectedIncass;
        private void Print_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                _selectedIncass = dataGridView1.SelectedRows[0].DataBoundItem as Инкассация;
                PrintCheck();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void up_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Index >0)
            {
                dataGridView1.Rows[dataGridView1.SelectedRows[0].Index - 1].Selected = true; 
            }
        }

        private void down_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Index < dataGridView1.Rows.Count-1)
            {
                dataGridView1.Rows[dataGridView1.SelectedRows[0].Index + 1].Selected = true;
            }
        }
    }
}
