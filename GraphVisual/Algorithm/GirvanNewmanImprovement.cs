using GraphVisual.GraphD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphVisual.Algorithm
{
    class GirvanNewmanImprovement : IAlgorithm
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

        public CommunityStructure FindCommunityStructure(DGraph pGraph)
        {
            // Clone graph này ra để xử lý
            graph = pGraph.Clone();

            // Cộng đồng
            CommunityStructure tempCS = GetCommunityStructure();

            // Số cộng đồng
            int initCount = tempCS.Count;
            int countCommunity = initCount;

            // Q
            _BestQ = 0;
            Q = 0;

            // Tính edge betweenness lần đầu
            CalculateEdgeBetweenness(tempCS);

            // tính thuật toán
            int j = 0;
            while (true)
            {
                while (countCommunity <= initCount)
                {
                    WriteLog("Xóa lần " + j.ToString()); j++;
                    // Xóa cạnh có edge betweenness lớn nhất
                    List<DGraph> communities = RemoveMaxEdgeBetweenness(tempCS); // Xóa cạnh lớn nhất và cho biết community nào có cạnh được xóa

                    // Tính lại Edgebetweenness
                    CalculateEdgeBetweenness(tempCS, communities);

                    // Đếm lại số cộng đồng
                    tempCS = GetCommunityStructure();
                    countCommunity = tempCS.Count;
                }

                initCount = countCommunity;

                // Tính Q
                Q = CalculateModularity(tempCS, pGraph);
                if (Q > _BestQ)
                {
                    _BestQ = Q;
                    Cs = tempCS;
                }

                if (graph.Edges.Count == 0) break;
            }

            return this.Cs;
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

        // Hàm xóa cạnh và trả về danh sách các subgraph có cạnh bị xóa 
        // Nhằm tính lại edge betweenness
        private List<DGraph> RemoveMaxEdgeBetweenness(CommunityStructure pTempCS)
        {
            var maxValue = edgeBetweenness.Max(u => u.Value);
            List<Edge> lstEdge = (from e in edgeBetweenness
                                  where e.Value == maxValue
                                  select e.Key).ToList();

            List<DGraph> lstGraph = new List<DGraph>();

            // Xóa tất cả các cạnh này
            foreach (Edge e in lstEdge)
            {
                // xoa canh trong do thi
                graph.Edges.Remove(e);

                // Xoa canh ke trong nut
                e.NodeA.AdjacencyEdges.Remove(e);
                e.NodeB.AdjacencyEdges.Remove(e);

                // Xoa nut ke
                e.NodeA.AdjacencyNodes.Remove(e.NodeB);
                e.NodeB.AdjacencyNodes.Remove(e.NodeA);

                WriteLog(" - Remove: (" + e.NodeA.Label + ", " + e.NodeB.Label + ")\t" + edgeBetweenness[e].ToString("0.00"));

                // xoa edgebetweenness
                edgeBetweenness.Remove(e);

                // tim cong dong
                foreach (DGraph subgraph in pTempCS)
                {
                    if (subgraph.Nodes.Contains(e.NodeA))
                        lstGraph.Add(subgraph);
                }
            }

            return lstGraph;
        }

        private void CalculateEdgeBetweenness(CommunityStructure pCS, List<DGraph> communites = null)
        {
            if (edgeBetweenness == null)
            {
                edgeBetweenness = new Dictionary<Edge, double>();
                foreach (Edge e in graph.Edges)
                {
                    edgeBetweenness.Add(e, 0);
                }
            }

            if (communites != null)
            {
                foreach (DGraph graph in communites)
                {
                    _CalculateEdgeBetweenness(graph);
                }
            }
            else
            {
                foreach (DGraph subg in pCS)
                {
                    _CalculateEdgeBetweenness(subg);
                }
            }
        }

        private void _CalculateEdgeBetweenness(DGraph subgraph)
        {
            if (subgraph == null) return;
            int n = subgraph.Nodes.Count;
            int MAX = int.MaxValue;

            foreach (Node s in subgraph.Nodes)
            {
                foreach (Edge e in s.AdjacencyEdges)
                {
                    edgeBetweenness[e] = 0;
                }
            }

            foreach (Node s in subgraph.Nodes)
            {
                Queue<Node> Q = new Queue<Node>();
                Stack<Node> S = new Stack<Node>();
                Dictionary<Node, List<Node>> pred = new Dictionary<Node, List<Node>>();

                Dictionary<Node, int> dist = new Dictionary<Node, int>();
                Dictionary<Node, int> sigma = new Dictionary<Node, int>();
                Dictionary<Node, double> delta = new Dictionary<Node, double>();

                // initialization
                foreach (Node d in subgraph.Nodes)
                {
                    dist.Add(d, MAX);
                    sigma.Add(d, 0);
                    delta.Add(d, 0);
                    pred.Add(d, new List<Node>());
                }

                dist[s] = 0;
                sigma[s] = 1;
                Q.Enqueue(s);

                // while
                while (Q.Count != 0)
                {
                    Node v = Q.Dequeue();
                    S.Push(v);

                    // sửa chỗ này lại, không duyệt hết mà chỉ duyệt 1 số thôi
                    foreach (Node w in v.AdjacencyNodes)
                    {
                        if (dist[w] == MAX)
                        {
                            dist[w] = dist[v] + 1;
                            Q.Enqueue(w);
                        }
                        if (dist[w] == dist[v] + 1)
                        {
                            sigma[w] = sigma[w] + sigma[v];
                            pred[w].Add(v);
                        }
                    }
                }

                // accumuation
                while (S.Count != 0)
                {
                    Node w = S.Pop();
                    foreach (Node v in pred[w])
                    {
                        double c = ((double)(sigma[v]) / sigma[w]) * (1.0 + delta[w]);

                        // tim canh
                        Edge e = graph.FindEdge(v, w);
                        edgeBetweenness[e] += c;

                        delta[v] += c;
                    }
                }
            }
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
    }
}
