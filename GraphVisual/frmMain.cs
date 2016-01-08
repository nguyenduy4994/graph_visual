using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GraphVisual.GraphD;
using GraphVisual.Drawing;
using GraphVisual.Algorithm;
using System.Diagnostics;

namespace GraphVisual
{
    public partial class frmMain : Form
    {
        string graphFileName;

        public frmMain()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.logo;

            _MyDocument = new GraphDocument(drawingSpace);
        }

        private GraphDocument _MyDocument;

        public GraphDocument MyDocument
        {
            get { return _MyDocument; }
            set { _MyDocument = value; }
        }

        // Menu open
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "TXT File|*.txt";

            if (od.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                graphFileName = od.FileName;
                _MyDocument.Dispose();
                
                _MyDocument = new GraphDocument(this.drawingSpace);
                _MyDocument.Graph = DGraph.CreateGraphFromFile(od.FileName);

                // Tao vi tri ngau nhien cho nut
                Random rand = new Random();
                for (int i = 0; i < _MyDocument.Graph.Nodes.Count; i++)
                {
                    _MyDocument.Graph.Nodes[i].Location = new Point(rand.Next(Format.Setting.NodeSize, drawingSpace.Width - Format.Setting.NodeSize), rand.Next(Format.Setting.NodeSize, drawingSpace.Height - Format.Setting.NodeSize));
                }

                // ve lai
                ReDraw();
                this.Cursor = Cursors.Default;

                lblGraphInfo.Text = "Number of Nodes: " + _MyDocument.Graph.Nodes.Count.ToString() + " nodes\n\r";
                lblGraphInfo.Text += "Number of Edges: " + _MyDocument.Graph.Edges.Count.ToString() + " edges\n\r";
                txtAlgorithmLog.Text = string.Empty;
            }
        }

        // Ve lai hinh len khung ve
        private void ReDraw()
        {
            drawingSpace.Invalidate();
        }

        private void saveImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            if (sd.ShowDialog() == DialogResult.OK)
            {
                MyDocument.SaveToFile(sd.FileName);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            deleteNodeToolStripMenuItem.Enabled = (MyDocument.HoverNode != null);
        }

        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyDocument.IsAdd = (addNodeToolStripMenuItem.Checked);

        }

        private void createLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyDocument.IsCreateLink = (createLinkToolStripMenuItem.Checked);
        }

        private void girvanNewmanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyDocument == null)
                return;
            GirvanNewman gn = new GirvanNewman();
            this.Cursor = Cursors.WaitCursor;
            gn.Log = txtAlgorithmLog;
            MyDocument.IsOverlap = false;
            Stopwatch w = new Stopwatch();
            w.Start();
            MyDocument.CS = gn.FindCommunityStructure(MyDocument.Graph);
            w.Stop();
            this.Cursor = Cursors.Default;

            lblGraphInfo.Text = "Number of Nodes: " + _MyDocument.Graph.Nodes.Count.ToString() + " nodes\n\r";
            lblGraphInfo.Text += "Number of Edges: " + _MyDocument.Graph.Edges.Count.ToString() + " edges\n\r";
            lblGraphInfo.Text += "-------------------------------\r\n";
            lblGraphInfo.Text += "Girvan Newman algorithm\n\r";
            lblGraphInfo.Text += "Run times: " + (w.ElapsedMilliseconds / 1000.0).ToString() + "s\n\r";
           
            lblGraphInfo.Text += "Using Modularity as quality measure\r\n";
            lblGraphInfo.Text += "Best Q: " + gn.BestQ.ToString("0.000") + "\r\n";
            

            lblGraphInfo.Text += "Number of community: " + MyDocument.CS.Count.ToString() + "\r\n";

            // Định dạng màu và reposition 
            int n = Format.Brushes.Length;
            int numCom = MyDocument.CS.Count;
            int maxCom = 3;

            // số cột để vẽ
            int col = (numCom >= maxCom) ? 3 : numCom;

            // số dòng để vẽ
            int row = (numCom <= maxCom) ? 1 : Convert.ToInt32(Math.Ceiling((double)numCom / maxCom));

            // từ đó tính ra được kích thước
            int width = drawingSpace.Width / col;
            int height = drawingSpace.Height / row;

            Random rand = new Random();

            for (int i = 0; i < numCom; i++)
            {
                int minWidth = width * (i % col);
                int maxWidth = minWidth + width;

                int minHeight = height * (i / col);
                int maxHeight = minHeight + height;

                foreach (Node node in MyDocument.CS[i].Nodes)
                {
                    Node _node = MyDocument.Graph.FindNode(node.Label);
                    _node.NodeBrush = Format.Brushes[i % n];
                    _node.Location = new Point(rand.Next(minWidth, maxWidth), rand.Next(minHeight, maxHeight));
                }
            }

            drawingSpace.Invalidate();
            MessageBox.Show("Girvan Newman algorithm runs complete", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void congaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyDocument == null)
                return;
            Conga cg = new Conga();
            cg.Log = txtAlgorithmLog;

            DialogResult r = MessageBox.Show("Do you want to use VAD?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (r == DialogResult.Yes)
            {
                cg.UseVAD = true;
            }
            else
            {
                cg.UseVAD = false;
            }
            
            this.Cursor = Cursors.WaitCursor;
            MyDocument.IsOverlap = true;
            Stopwatch w = new Stopwatch();
            w.Start();
            MyDocument.CS = cg.FindCommunityStructure(MyDocument.Graph);
            w.Stop();
            this.Cursor = Cursors.Default;
            MyDocument.OverlapNode = new Dictionary<Node, List<DGraph>>();
            int n = Format.Brushes.Length;
            for (int i = 0; i < MyDocument.CS.Count; i++)
            {
                MyDocument.CS[i].NodeBrush = Format.Brushes[i % n];
                foreach (Node node in MyDocument.CS[i].Nodes)
                {
                    Node _node = MyDocument.Graph.FindNode(node.Label, false);
                    if (_node != null)
                        _node.NodeBrush = Format.Brushes[i % n];
                    else
                    {
                        string[] node_label = node.Label.Split('-');
                        _node = MyDocument.Graph.FindNode(node_label[0], false);

                        if (MyDocument.OverlapNode.ContainsKey(_node) == false)
                        {
                            MyDocument.OverlapNode.Add(_node, new List<DGraph>());
                        }

                        if (MyDocument.OverlapNode[_node].Contains(MyDocument.CS[i]) == false)
                            MyDocument.OverlapNode[_node].Add(MyDocument.CS[i]);
                    }
                }
            }

            lblGraphInfo.Text = "Number of Nodes: " + _MyDocument.Graph.Nodes.Count.ToString() + " nodes\n\r";
            lblGraphInfo.Text += "Number of Edges: " + _MyDocument.Graph.Edges.Count.ToString() + " edges\n\r";
            lblGraphInfo.Text += "-------------------------------\r\n";
            lblGraphInfo.Text += "CONGA algorithm\n\r";
            lblGraphInfo.Text += "Run times: " + (w.ElapsedMilliseconds / 1000.0).ToString() + "s\n\r";
            if (cg.UseVAD)
            {
                lblGraphInfo.Text += "Using VAD as quality measure\r\n";
                lblGraphInfo.Text += "Best VAD: " + cg.BestVAD.ToString("0.000") + "\r\n";
            }
            else
            {
                lblGraphInfo.Text += "Using Modularity as quality measure\r\n";
                lblGraphInfo.Text += "Best Q: " + cg.BestM.ToString("0.000") + "\r\n";
            }

            lblGraphInfo.Text += "Number of community: " + MyDocument.CS.Count.ToString() + "\r\n";
            lblGraphInfo.Text += "Number of overlapping node(s): " + MyDocument.OverlapNode.Count.ToString() + "\r\n";

            var list_overlap = MyDocument.OverlapNode.Keys.ToList();
            lblGraphInfo.Text += "   ";
            foreach (Node node in list_overlap)
            {
                lblGraphInfo.Text += node.Label + " ";
            }

            drawingSpace.Invalidate();
            MessageBox.Show("CONGA algorithm runs complete", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to exit?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void girvanNewmanImprovementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyDocument == null)
                return;
            GirvanNewmanImprovement gn = new GirvanNewmanImprovement();
            this.Cursor = Cursors.WaitCursor;
            gn.Log = txtAlgorithmLog;
            MyDocument.IsOverlap = false;
            Stopwatch w = new Stopwatch();
            w.Start();
            MyDocument.CS = gn.FindCommunityStructure(MyDocument.Graph);
            w.Stop();
            this.Cursor = Cursors.Default;

            lblGraphInfo.Text = "Number of Nodes: " + _MyDocument.Graph.Nodes.Count.ToString() + " nodes\n\r";
            lblGraphInfo.Text += "Number of Edges: " + _MyDocument.Graph.Edges.Count.ToString() + " edges\n\r";
            lblGraphInfo.Text += "-------------------------------\r\n";
            lblGraphInfo.Text += "Girvan Newman Improvement algorithm\n\r";
            lblGraphInfo.Text += "Run times: " + (w.ElapsedMilliseconds / 1000.0).ToString() + "s\n\r";

            lblGraphInfo.Text += "Using Modularity as quality measure\r\n";
            lblGraphInfo.Text += "Best Q: " + gn.BestQ.ToString("0.000") + "\r\n";


            lblGraphInfo.Text += "Number of community: " + MyDocument.CS.Count.ToString() + "\r\n";

            // Định dạng màu và reposition 
            int n = Format.Brushes.Length;
            int numCom = MyDocument.CS.Count;
            int maxCom = 3;

            // số cột để vẽ
            int col = (numCom >= maxCom) ? 3 : numCom;

            // số dòng để vẽ
            int row = (numCom <= maxCom) ? 1 : Convert.ToInt32(Math.Ceiling((double)numCom / maxCom));

            // từ đó tính ra được kích thước
            int width = drawingSpace.Width / col;
            int height = drawingSpace.Height / row;

            Random rand = new Random();

            for (int i = 0; i < numCom; i++)
            {
                int minWidth = width * (i % col);
                int maxWidth = minWidth + width;

                int minHeight = height * (i / col);
                int maxHeight = minHeight + height;

                foreach (Node node in MyDocument.CS[i].Nodes)
                {
                    Node _node = MyDocument.Graph.FindNode(node.Label);
                    _node.NodeBrush = Format.Brushes[i % n];
                    _node.Location = new Point(rand.Next(minWidth, maxWidth), rand.Next(minHeight, maxHeight));
                }
            }

            drawingSpace.Invalidate();
            MessageBox.Show("Girvan Newman Improvement algorithm runs complete", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }
    }
}
