using GraphVisual.Algorithm;
using GraphVisual.GraphD;
using System.Drawing;

namespace GraphVisual.Drawing
{
    public class DrawCommunity
    {
        public static void Draw(CommunityStructure pCS, Graphics g)
        {
            int n = Format.Brushes.Length;
            for (int i = 0; i < pCS.Count; i++)
            {
                foreach (Node node in pCS[i].Nodes)
                {
                    Rectangle bound = new Rectangle(node.Location.X - Format.Setting.NodeHaftSize, node.Location.Y - Format.Setting.NodeHaftSize, Format.Setting.NodeSize, Format.Setting.NodeSize);

                    g.DrawEllipse(Pens.Black, bound);
                    if (node.IsHover == false)
                        g.FillEllipse(Format.Brushes[i % n], bound);
                    else
                        g.FillEllipse(Format.NodeHoverBackground, bound);

                    if (Format.Setting.ShowNodeLabel)
                        g.DrawString(node.Label, Format.Setting.NodeLabelFont, Format.NodeLabelColor, node.Location, Format.StrFormat);
                }
            }
        }
    }
}
