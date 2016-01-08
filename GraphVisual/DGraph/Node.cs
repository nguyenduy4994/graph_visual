using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphVisual.Drawing;

namespace GraphVisual.GraphD
{
    public class Node : IDrawable
    {
        // Nhãn của nút (nút số bao nhiêu)
        private string _Label;

        public string Label
        {
            get { return _Label; }
            set { _Label = value; }
        }

        // Vị trí của nút trên bản vẽ
        private Point _Location;

        public Point Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        // Danh sách kề
        private List<Node> _AdjacencyNodes;

        public List<Node> AdjacencyNodes
        {
            get { return _AdjacencyNodes; }
            set { _AdjacencyNodes = value; }
        }

        // Danh sách cạnh kề của nút
        private List<Edge> _AdjacencyEdges;

        public List<Edge> AdjacencyEdges
        {
            get { return _AdjacencyEdges; }
            set { _AdjacencyEdges = value; }
        }

        // Hover
        private bool _IsHover;

        public bool IsHover
        {
            get { return _IsHover; }
            set { _IsHover = value; }
        }

        // Node Color
        private Brush _NodeBrush;

        public Brush NodeBrush
        {
            get { return _NodeBrush; }
            set { _NodeBrush = value; }
        }

        public Node(string pLabel, Point pLocation)
        {
            _AdjacencyNodes = new List<Node>();
            _AdjacencyEdges = new List<Edge>();
            _Label = pLabel;
            _Location = pLocation;
            _IsHover = false;
        }

        public Node()
        {
            _AdjacencyNodes = new List<Node>();
            _AdjacencyEdges = new List<Edge>();
            _IsHover = false;
            _NodeBrush = Format.NodeBackground;
        }

        // method
        public void Draw(Graphics g)
        {
            Rectangle bound = new Rectangle(_Location.X - Format.Setting.NodeHaftSize, _Location.Y - Format.Setting.NodeHaftSize, Format.Setting.NodeSize, Format.Setting.NodeSize);
            
            g.DrawEllipse(Pens.Black, bound);
            if (_IsHover == false)
                g.FillEllipse(_NodeBrush, bound);
            else
                g.FillEllipse(Format.NodeHoverBackground, bound);

            if (Format.Setting.ShowNodeLabel)
                g.DrawString(_Label, Format.Setting.NodeLabelFont, Format.NodeLabelColor, _Location, Format.StrFormat);
        }
    }
}
