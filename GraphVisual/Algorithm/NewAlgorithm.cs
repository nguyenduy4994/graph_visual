using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphVisual.GraphD;
using System.Windows.Forms;

namespace GraphVisual.Algorithm
{
    public class NewAlgorithm : IAlgorithm
    {
        public CommunityStructure FindCommunityStructure(GraphD.DGraph pGraph)
        {
            return _NewAlgorithm(pGraph);
        }

        double _BestQ, Q;

        public double BestQ
        {
            get { return _BestQ; }
            set { _BestQ = value; }
        }

        private RichTextBox _Log;

        public RichTextBox Log
        {
            get { return _Log; }
            set { _Log = value; }
        }

        private void WriteLog(string log = "")
        {
            _Log.Text += log + "\r\n";
            _Log.Refresh();
        }

        DGraph graph;
        int VisitedNodeCount = 0;

        public CommunityStructure Cs;

        private CommunityStructure _NewAlgorithm(GraphD.DGraph pGraph)
        {
            graph = pGraph.Clone();
            int NodeCount = graph.Nodes.Count();

            CommunityStructure tempCS = FindStructureWithMaxClique(graph);
            while (VisitedNodeCount < NodeCount)
            {
                while (true)
                {
                    VisitNode(graph);
                    if (CheckNewCommunity(graph))
                    {
                        break;
                    }
                }

                // Tính Q
                Q = CalculateModularity(tempCS, pGraph);
                if (Q > _BestQ)
                {
                    _BestQ = Q;
                    Cs = tempCS;
                }
            }

            return Cs;
        }

        private bool CheckNewCommunity(DGraph graph)
        {
            throw new NotImplementedException();
        }

        private void VisitNode(DGraph graph)
        {
            throw new NotImplementedException();
        }

        private void VisitNode()
        {
            throw new NotImplementedException();
        }

        private double CalculateModularity(CommunityStructure pCs, DGraph pOriginalGraph)
        {
            double modularity = 0;
            int numEdge = pOriginalGraph.Edges.Count;
            foreach (DGraph csItem in pCs)
            {
                int l = 0; // tong bac cua dinh trong c, theo do thi moi => suy ra so canh
                int d = 0; // tong bac
                foreach (Node node in csItem.Nodes)
                {
                    l += node.AdjacencyNodes.Count;
                    d += pOriginalGraph.FindNode(node.Label, false).AdjacencyNodes.Count;
                }

                l /= 2;

                modularity += ((double)l / numEdge) - Math.Pow(((double)d / (2 * numEdge)), 2);
            }
            return modularity;
        }

        private CommunityStructure FindStructureWithMaxClique(DGraph graph)
        {
            return null;
        }
    }
}
