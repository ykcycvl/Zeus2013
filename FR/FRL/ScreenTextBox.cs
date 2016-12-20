using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IpayboxControls
{

    public class ScrTextBox : TextBox
    {
        private Keyboard p;
        public ScrTextBox()
            : base()
        {

        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.BackColor = Color.IndianRed;
            Point pp = PointToScreen(this.Location);

            p = new Keyboard(new Point(350, 0), this);

            p.Disposed += new EventHandler(p_Disposed);
            p.ShowDialog();
            base.OnMouseClick(e);
        }

        void p_Disposed(object sender, EventArgs e)
        {
            string entered = p.GetText();
            if (entered != null && entered.Length > 0)
                this.Text = entered;
        }
    }
    public class Keyboard : Form
    {

        private int _line = 0;
        private int _countButtons = 0;
        Size _buttonSize = new Size(40, 40);
        private int _xspacing = 5;
        private int _yspacing = 5;
        private int _countButtonsX = 13;
        //private int _countButtonsY = 4;


        private bool _shift;
        private bool _capsLock;
        private Control _control;
        private string _text;


        public Keyboard()
        {
            InitializeComponent();

        }
        public Keyboard(Point location)
            : this()
        {
            StartPosition = FormStartPosition.Manual;
            this.TopMost = true;


            this.Location = location;
        }
        public Keyboard(Point location, Control control)
            : this(location)
        {
            _control = control;
            _control.FindForm().TopMost = false;
        }
        public bool Shift
        {
            get { return _shift; }
            set
            {
                if (CapsLock)
                    return;
                _shift = value;
                RemoveButtons();
                if (_shift)
                    AddButtonsUpper();
                else
                    AddButtonsLower();
            }
        }

        public bool CapsLock
        {
            get { return _capsLock; }
            set
            {
                if (Shift)
                    return;
                _capsLock = value;
                RemoveButtons();
                if (_capsLock)
                    AddButtonsUpper();
                else
                    AddButtonsLower();
            }
        }

        public string GetText()
        {
            return _text;
        }
        private void RemoveButtons()
        {
            /*
            foreach(var control in Controls)
            {
                if(control is Button)
                    Controls.Remove((Button)control);
            }
            Application.DoEvents();*/
            Controls.Clear();
            InitializeComponent();
            bool changed = false;
            if (_shift)
            {
                _shift = false;
                changed = true;
            }
            textBox1.Text = _text;

            if (changed)
                _shift = true;
        }
        private void Initialize()
        {
            _buttonSize = new Size(40, 40);
            _line = 0;
            _countButtons = 0;
        }

        private void AddButtonsUpper()
        {
            Initialize();
            AddButton("1".ToUpper());
            AddButton("2".ToUpper());
            AddButton("3".ToUpper());
            AddButton("4".ToUpper());
            AddButton("5".ToUpper());
            AddButton("6".ToUpper());
            AddButton("7".ToUpper());
            AddButton("8".ToUpper());
            AddButton("9".ToUpper());
            AddButton("0".ToUpper());
            AddButton("-".ToUpper());
            AddButton("=".ToUpper());
            AddButton("+".ToUpper());
            AddButton("q".ToUpper());
            AddButton("w".ToUpper());
            AddButton("e".ToUpper());
            AddButton("r".ToUpper());
            AddButton("t".ToUpper());
            AddButton("y".ToUpper());
            AddButton("u".ToUpper());
            AddButton("i".ToUpper());
            AddButton("o".ToUpper());
            AddButton("p".ToUpper());
            AddButton("[".ToUpper());
            AddButton("]".ToUpper());
            AddButton(@"\".ToUpper());
            AddButton(@"/".ToUpper());
            AddButton("a".ToUpper());
            AddButton("s".ToUpper());
            AddButton("d".ToUpper());
            AddButton("f".ToUpper());
            AddButton("g".ToUpper());
            AddButton("h".ToUpper());
            AddButton("j".ToUpper());
            AddButton("k".ToUpper());
            AddButton("l".ToUpper());
            AddButton(";".ToUpper());
            AddButton("'".ToUpper());
            AddButton("\"".ToUpper());
            AddButton(":".ToUpper());
            AddButton("z".ToUpper());
            AddButton("x".ToUpper());
            AddButton("c".ToUpper());
            AddButton("v".ToUpper());
            AddButton("b".ToUpper());
            AddButton("n".ToUpper());
            AddButton("m".ToUpper());
            AddButton(",".ToUpper());
            AddButton(".".ToUpper());
            AddButton("<".ToUpper());
            AddButton(">".ToUpper());
            AddButton("?".ToUpper());
            AddButton("|".ToUpper());
            _buttonSize = new Size(85, _buttonSize.Height);
            AddButton("Backspace");
            AddButton("Space");
            AddButton("Clear");
            AddButton("CapsLock");
            //AddButton("Shift");
            AddButton("Enter");
        }
        private void AddButtonsLower()
        {
            Initialize();
            AddButton("1");
            AddButton("2");
            AddButton("3");
            AddButton("4");
            AddButton("5");
            AddButton("6");
            AddButton("7");
            AddButton("8");
            AddButton("9");
            AddButton("0");
            AddButton("-");
            AddButton("=");
            AddButton("+");
            AddButton("q");
            AddButton("w");
            AddButton("e");
            AddButton("r");
            AddButton("t");
            AddButton("y");
            AddButton("u");
            AddButton("i");
            AddButton("o");
            AddButton("p");
            AddButton("[");
            AddButton("]");
            AddButton(@"\");
            AddButton(@"/");
            AddButton("a");
            AddButton("s");
            AddButton("d");
            AddButton("f");
            AddButton("g");
            AddButton("h");
            AddButton("j");
            AddButton("k");
            AddButton("l");
            AddButton(";");
            AddButton("'");
            AddButton("\"");
            AddButton(":");
            AddButton("z");
            AddButton("x");
            AddButton("c");
            AddButton("v");
            AddButton("b");
            AddButton("n");
            AddButton("m");
            AddButton(",");
            AddButton(".");
            AddButton("<");
            AddButton(">");
            AddButton("?");
            AddButton("|");
            _buttonSize = new Size(85, _buttonSize.Height);
            AddButton("Backspace");
            AddButton("Space");
            AddButton("Clear");
            AddButton("CapsLock");
            //AddButton("Shift");
            AddButton("Enter");
        }

        private void keyboard_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            AddButtonsLower();

        }

        public void AddButton(string text)
        {
            int tmp = _countButtons >= _countButtonsX ? _countButtons % _countButtonsX : 1;
            if (tmp == 0)
            {
                _line++;
                _countButtons = 0;
            }

            int x = _countButtons * (_buttonSize.Width + _xspacing) + _xspacing;
            int y = _line * (_buttonSize.Height + _yspacing) + _yspacing + 50;

            Button b = new Button();
            b.Font = new Font("Arial", 11, FontStyle.Bold);


            b.Size = _buttonSize;
            b.Location = new Point(x, y);

            if (CapsLock && text == "CapsLock" || Shift && text == "Shift")
            {
                b.ForeColor = Color.Red;
                //b.Font = new Font("Arial", 12, FontStyle.Underline);
            }

            b.Text = text;
            b.Tag = text;
            b.Click += new EventHandler(ButtonClick);

            this.Controls.Add(b);
            _countButtons++;
        }

        void ButtonClick(object sender, EventArgs e)
        {
            Button tmp = (Button)sender;

            if (tmp != null)
            {
                /*   AddButton("Backspace");
            AddButton("Space");
            AddButton("Clear");
            AddButton("CapsLock");
            AddButton("Shift");
            AddButton("Enter");*/
                switch (tmp.Text)
                {
                    case "Backspace":
                        if (_text.Length > 0)
                            _text = _text.Remove(_text.Length - 1);
                        break;
                    case "Space":
                        _text += " ";
                        break;
                    case "Clear":
                        _text = "";
                        break;
                    case "CapsLock":
                        CapsLock = !CapsLock;
                        break;
                    case "Shift":
                        Shift = !Shift;
                        break;
                    case "Enter":
                        if (_control != null)
                        {
                            _control.BackColor = Color.White;
                            _control.FindForm().TopMost = true;
                        }
                        this.Dispose();
                        break;
                    default:
                        _text = _text + tmp.Text;
                        break;

                }
            }




            textBox1.Text = _text;
            Application.DoEvents();

            // throw new NotImplementedException();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (_shift)
                Shift = false;
        }
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(11, 10);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(579, 26);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // keyboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 287);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Keyboard";
            this.Text = "keyboard";
            this.Load += new System.EventHandler(this.keyboard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
    }
}
