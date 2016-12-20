using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace zeus.HelperClass
{
    class TransparentPanel : Panel
    {
        //Чтобы background не прорисовывался выставим флаг ControlStyles.Opaque: 
        public TransparentPanel()
        {
            SetStyle(ControlStyles.Opaque, true);
        }

        //И добавим к окну, при его создании, стиль WS_EX_TRANSPARENT: 
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x00000020;
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WS_EX_TRANSPARENT;
                return createParams;
            }
        }

        //Теперь, чтобы панель была видна как полупрозрачная, необходимо переопределить ее метод OnPaint
        //и в нем закрашивать нужную облаcть цветом с alpha-составляющей: 
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(1, Color.White)),
             0, 0, Width, Height);
        }

        //При перемещении такой 'прозрачной' панели (например, при установке свойства Control.Location) 
        //можно заметить, что  она, перемещаясь на новое место, остается с тем же background'ом, 
        //что и на прежнем месте. Так происходит потому, что низлежащие окна не перерисовываются 
        //в тех новых координатах, в которых находится наша Panel после перемещения, поэтому их нужно заставить обновиться: 
        protected override void OnMove(EventArgs e)
        {
            if (Parent != null)
                Parent.Invalidate(Bounds, true);
        }
    }

    class CustomControls
    {

    }
}
