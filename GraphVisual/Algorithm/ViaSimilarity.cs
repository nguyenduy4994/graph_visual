using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphVisual.GraphD;
using System.Windows.Forms;

namespace GraphVisual.Algorithm
{
    class ViaSimilarity : IAlgorithm
    {
        Dictionary<Edge, double> edgeBetweenness;
        CommunityStructure Cs;
        DGraph graph;

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

        struct Similarity
        {
            public Node node;
            public double similarity;
        }

        public CommunityStructure FindCommunityStructure(DGraph pGraph)
        {
            // Clone graph này ra để xử lý
            graph = pGraph.Clone();

            // Cộng đồng
            CommunityStructure tempCS = GetCommunityStructure();

            // Số cộng đồng
            int initCount = tempCS.Count;
            int countCommunity = initCount;

            // Tinh similarity cua tat ca cac node 
            Dictionary<Node, List<Similarity>> hashMaxSimilarityNode = new Dictionary<Node, List<Similarity>>();

            // mỗi đỉnh là một community
            foreach(Node node in graph.Nodes)
            {
                // tinh similarity 
                if (node.AdjacencyNodes.Count > 0)
                {
                    List<Similarity> lst = new List<Similarity>();

                    double max = double.MinValue;
                    foreach (Node neighborNode in node.AdjacencyNodes)
                    {
                        Similarity s = new Similarity();
                        s.node = neighborNode;
                        s.similarity = ZLZSimilarity(node, neighborNode);

                        lst.Add(s);

                        if(s.similarity > max)
                        {
                            max = s.similarity;
                        }
                    }

                    // xử lý cái list đó trước rồi mới add vào
                    int n = lst.Count;
                    for(int i = 0; i < n; i++)
                    {
                        if(lst[i].similarity < max)
                        {
                            lst.RemoveAt(i);
                            n--;
                            i--;
                        }
                    }
                    
                    if(lst.Count >= 2)
                    {

                    }

                    // add cái list này vào danh sách
                    hashMaxSimilarityNode.Add(node, lst);
                }

                // khoi tao cong dong
                DGraph com = new DGraph();
                com.Nodes.Add(node);
                tempCS.Add(com);
            }

            // chọn nút bất kì 
            Random r = new Random();
            int nodeNumber = r.Next(0, graph.Nodes.Count);
            Node initNode = graph.Nodes[nodeNumber];
            
            return this.Cs;
        }
        private CommunityStructure GetCommunityStructure()
        {
            CommunityStructure cs = new CommunityStructure();

            int count = 0;
            int n = graph.Nodes.Count;
            Dictionary<Node, bool> visited = new Dictionary<Node, bool>();
            for (int i = 0; i < n; i++)
            {
                visited.Add(graph.Nodes[i], false);
            }

            foreach (Node i in graph.Nodes)
            {
                if (visited[i] == false)
                {
                    count++;
                    DGraph subgraph = new DGraph();

                    Queue<Node> Q = new Queue<Node>();
                    visited[i] = true;
                    Q.Enqueue(i);

                    subgraph.Nodes.Add(i);

                    while (Q.Count != 0)
                    {
                        Node v = Q.Dequeue();
                        foreach (Node j in v.AdjacencyNodes)
                        {
                            if (visited[j] == false)
                            {
                                subgraph.Nodes.Add(j);
                                visited[j] = true;
                                Q.Enqueue(j);
                            }
                        }
                    }
                    cs.Add(subgraph);
                }
            }
            return cs;
        }

        private double ZLZSimilarity(Node a, Node b)
        {
            if (!a.AdjacencyNodes.Contains(b)) return 0;

            double z = 0f;
            List<Node> commonNeighbors = FindCommonNeighbors(a, b);

            foreach(Node c in commonNeighbors)
            {
                z += (1.0 / c.AdjacencyNodes.Count);
            }

            return z;
        }

        private List<Node> FindCommonNeighbors(Node a, Node b)
        {
            List<Node> commonNodes = new List<Node>();

            foreach(Node c in a.AdjacencyNodes)
            {
                if (!commonNodes.Contains(c) && b.AdjacencyNodes.Contains(c)) commonNodes.Add(c);
            }

            foreach(Node c in b.AdjacencyNodes)
            {
                if (!commonNodes.Contains(c) && a.AdjacencyNodes.Contains(c)) commonNodes.Add(c);
            }

            return commonNodes;
        }
    }
}
