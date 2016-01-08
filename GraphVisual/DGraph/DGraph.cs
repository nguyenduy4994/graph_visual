using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GraphVisual.GraphD
{
    public class DGraph : IDrawable
    {
        private Brush _NodeBrush;

        public Brush NodeBrush
        {
            get { return _NodeBrush; }
            set { _NodeBrush = value; }
        }

        // Danh sách đỉnh
        private List<Node> _Nodes;

        public List<Node> Nodes
        {
            get { return _Nodes; }
            set { _Nodes = value; }
        }

        // Danh sach cạnh
        private List<Edge> _Edges;

        public List<Edge> Edges
        {
            get { return _Edges; }
            set { _Edges = value; }
        }

        public DGraph()
        {
            _Nodes = new List<Node>();
            _Edges = new List<Edge>();
        }

        public void Draw(Graphics g)
        {
            // Ve canh
            for (int i = 0; i < _Edges.Count; i++)
                _Edges[i].Draw(g);

            // Ve nut
            for (int i = 0; i < _Nodes.Count; i++)
                _Nodes[i].Draw(g);
        }

        public static DGraph CreateGraphFromFile(string pFilename)
        {
            try
            {
                DGraph graph = new DGraph();
                StreamReader reader = new StreamReader(pFilename);

                while (reader.EndOfStream == false)
                {
                    string[] _nodes = reader.ReadLine().Split(' ');
                    Node nodeA = graph.FindNode(_nodes[0]);
                    Node nodeB = graph.FindNode(_nodes[1]);

                    graph.CreateLink(nodeA, nodeB);
                }

               

                return graph;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Edge CreateLink(Node nodeA, Node nodeB)
        {
            Edge edge = new Edge(nodeA, nodeB);

            if (nodeA.AdjacencyNodes.Contains(nodeB) || nodeB.AdjacencyNodes.Contains(nodeA))
                return null;

            nodeB.AdjacencyNodes.Add(nodeA);
            nodeA.AdjacencyNodes.Add(nodeB);

            nodeA.AdjacencyEdges.Add(edge);
            nodeB.AdjacencyEdges.Add(edge);

            this.Edges.Add(edge);
            return edge;
        }

        public Node FindNode(string pLabel, bool add = true)
        {
            for (int i = 0; i < _Nodes.Count; i++)
            {
                if (_Nodes[i].Label == pLabel) return _Nodes[i];
            }

            if (add == false) return null;

            return CreateNode(pLabel);
        }

        public Node CreateNode(string pLabel)
        {
            Node node = new Node();
            node.Label = pLabel;
            _Nodes.Add(node);
            return node;
        }

        public DGraph Clone()
        {
            DGraph graph = new DGraph();

            foreach (Edge e in Edges)
            {
                Node nodeA = graph.FindNode(e.NodeA.Label);
                nodeA.Location = new Point(e.NodeA.Location.X, e.NodeA.Location.Y);
                Node nodeB = graph.FindNode(e.NodeB.Label);
                nodeB.Location = new Point(e.NodeB.Location.X, e.NodeB.Location.Y);

                graph.CreateLink(nodeA, nodeB);
            }

            return graph;
        }

        public Edge FindEdge(Node pNodeA, Node pNodeB)
        {
            foreach (Edge e in pNodeA.AdjacencyEdges)
            {
                if ((e.NodeA == pNodeA && e.NodeB == pNodeB) || (e.NodeA == pNodeB && e.NodeB == pNodeA))
                    return e;
            }

            return null;
        }

        public Edge FindEdgeExtract(Node pNodeA, Node pNodeB)
        {
            foreach (Edge e in pNodeA.AdjacencyEdges)
            {
                if ((e.NodeA == pNodeA && e.NodeB == pNodeB))
                    return e;
            }

            return null;
        }
    }
}
