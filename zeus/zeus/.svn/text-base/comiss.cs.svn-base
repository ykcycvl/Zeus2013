using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace zeus
{
    public partial class comiss : Form
    {
        public comiss()
        {
            InitializeComponent();
        }

        private void comiss_Load(object sender, EventArgs e)
        {
            bool Is = false;
            this.Location = new Point (150, 150);
            gw.Location = new Point(0, 0);
            gw.Size = this.Size;
            DataTable dt = new DataTable();

            dt.Columns.Add("Сумма платежа");
            dt.Columns.Add("Размер комиссии");
            if (Ipaybox.showComission != null)
            {
                foreach (XmlElement el in Ipaybox.showComission)
                {
                    if (el.Name == "c")
                    {
                        string from = el.GetAttribute("from");
                        string to = el.GetAttribute("to");
                        string fix = el.GetAttribute("fix");
                        string persent = el.GetAttribute("persent");
                        string min = el.GetAttribute("min");
                        string max = el.GetAttribute("max");
                        string summa = "";
                        if(from == "0" && to == "0")
                            summa = "Для любой суммы";
                        else
                            summa = "От " + from + " руб. до " + to + " руб.";
                        string razmer = "";

                        if (!string.IsNullOrEmpty(fix) && fix != "0")
                            razmer = fix + " руб.";
                        else
                            razmer = persent + " %";

                        if (!string.IsNullOrEmpty(min) && min != "0")
                            razmer += ", минимально " + min + " руб.";
                        if (!string.IsNullOrEmpty(max) && max != "0")
                            razmer += ", максимально " + min + " руб.";

                        dt.Rows.Add(new string[2] { summa, razmer });
                        Is = true;
                    }
                }
                if (Is == false)
                    dt.Rows.Add(new string[2] { "Для любой суммы", "Комиссия 0 руб." });
            }
            else
            {
                dt.Rows.Add(new string[2] { "Для любой суммы", "Комиссия 0 руб." });
            }
            gw.DataSource = dt;
            gw.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            gw.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            gw.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < gw.Columns.Count; i++)
            {
                gw.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void gw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
