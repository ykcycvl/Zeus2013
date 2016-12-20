using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeus
{
    public partial class PrepareCheck : Form
    {
        public PrepareCheck()
        {
            InitializeComponent();
        }

        private void PrepareCheck_Load(object sender, EventArgs e)
        {
            this.Size = Ipaybox.Resolution;

            if (Ipaybox.Inches == 17)
                this.Location = new Point(0, 0);

            try
            {
                _cursor.Hide();
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
