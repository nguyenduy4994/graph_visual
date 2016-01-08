using GraphVisual.Drawing;
using GraphVisual.GraphD;
using System.Drawing;
using System.Windows.Forms;

namespace GraphVisual
{
    public partial class DrawingSpace : Panel
    {
        public DrawingSpace()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
        }
    }
}
