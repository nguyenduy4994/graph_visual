using System.Drawing;

namespace GraphVisual.Drawing
{
    public class Format
    {
        private static StringFormat _StrFormat;

        public static StringFormat StrFormat
        {
            get { return _StrFormat; }
            set { _StrFormat = value; }
        }

        public static dynamic Setting
        {
            get { return Properties.Settings.Default; }
        }

        private static Brush _NodeBackground;

        public static Brush NodeBackground
        {
            get { return _NodeBackground; }
            set { _NodeBackground = value; }
        }

        private static Brush _NodeHoverBackground;

        public static Brush NodeHoverBackground
        {
            get { return _NodeHoverBackground; }
            set { _NodeHoverBackground = value; }
        }

        private static Brush _NodeLabelColor;

        public static Brush NodeLabelColor
        {
            get { return _NodeLabelColor; }
            set { _NodeLabelColor = value; }
        }

        private static Pen _LinkLineColor;

        public static Pen LinkLineColor
        {
            get { return _LinkLineColor; }
            set { _LinkLineColor = value; }
        }

        private static Brush[] _Brushes;

        public static Brush[] Brushes
        {
            get { return _Brushes; }
            set { _Brushes = value; }
        }

        static Format()
        {
            _StrFormat = new StringFormat();
            _StrFormat.LineAlignment = StringAlignment.Center;
            _StrFormat.Alignment = StringAlignment.Center;

            _NodeBackground = new SolidBrush(Format.Setting.NodeColor);
            _NodeHoverBackground = new SolidBrush(Format.Setting.NodeHoverColor);
            _NodeLabelColor = new SolidBrush(Format.Setting.NodeLabelColor);
            _LinkLineColor = new Pen(Format.Setting.LinkLineColor, Format.Setting.LinkLineSize);

            _Brushes = new SolidBrush[] {
                new SolidBrush(Color.Orange),
                new SolidBrush(Color.Green),
                new SolidBrush(Color.Blue),
                new SolidBrush(Color.Red),
                new SolidBrush(Color.Purple),
                new SolidBrush(Color.Silver),
                new SolidBrush(Color.YellowGreen)
            };
        }
    }
}
