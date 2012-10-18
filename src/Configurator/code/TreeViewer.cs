using System;
using System.Drawing;
using System.Windows.Forms;

namespace Configurator.code
{
    public class TreeViewer
    {
        private Controller controller;
        private TreeView treeView;
        private ContextMenuStrip docMenu;
        private TreeNode NodeMouseClickSelectedNode;

        public TreeViewer(Controller controller)
        {
            this.controller = controller;
        }

        public TreeView Get(Position position)
        {
            this.treeView = new TreeView
            {
                Width = position.Width,
                Height = position.Height,
                Left = position.Left,
                Top = position.Top,
                AllowDrop = true
            };
            this.treeView.DragDrop += this.TreeView1OnDragDrop;
            this.treeView.DragEnter += this.TreeView1OnDragEnter;
            this.treeView.NodeMouseClick += TreeViewOnNodeMouseClick;


            this.InitTreeView();
            this.PopulateTree();

            return this.treeView;
        }

        private void TreeView1OnDragDrop(object sender, DragEventArgs e)
        {
            var nodeToDropIn = this.treeView.GetNodeAt(this.treeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null)
            {
                return;
            }
            if (nodeToDropIn.Level > 1)
            {
                nodeToDropIn = nodeToDropIn.Parent;
            }

            object jobName = e.Data.GetData(typeof(string));
            if (jobName == null)
            {
                return;
            }
            nodeToDropIn.Nodes.Add(jobName.ToString());

            var role = nodeToDropIn.Parent.Text;
            var frequency = Convert.ToInt32(nodeToDropIn.Text);
            this.controller.AddJobToRoleAndFrequency(role, frequency, jobName.ToString());
        }

        private void InitTreeView()
        {
            this.docMenu = new ContextMenuStrip();
            var deleteLabel = new ToolStripMenuItem { Text = "Delete" };
            deleteLabel.Click += DeleteLabelOnClick;
            this.docMenu.Items.AddRange(new ToolStripMenuItem[] { deleteLabel });
            this.treeView.ContextMenuStrip = this.docMenu;
        }

        private void DeleteLabelOnClick(object sender, EventArgs e)
        {

            if (NodeMouseClickSelectedNode == null)
            {
                return;
            }

            if (NodeMouseClickSelectedNode.Level == 2)
            {
                var frequencyNode = NodeMouseClickSelectedNode.Parent;
                var roleNode = frequencyNode.Parent;
                var frequency = Convert.ToInt32(frequencyNode.Text);
                var roleName = roleNode.Text;
                var jobName = NodeMouseClickSelectedNode.Text;
                //Delete Job
                controller.DeleteJobFromRole(jobName, frequency, roleName);
                NodeMouseClickSelectedNode.Remove();
            }
        }

        private void PopulateTree()
        {
            var wi = this.controller.GetWorkItems();
            foreach (var w in wi)
            {
                var treeNode = new TreeNode(w.RoleName);
                foreach (var ts in w.TaskSet)
                {
                    var tn = new TreeNode(ts.Frequency.ToString());
                    foreach (var task in ts.Tasks)
                    {
                        var t = new TreeNode(task.JobName);
                        tn.Nodes.Add(t);
                    }
                    treeNode.Nodes.Add(tn);
                }
                this.treeView.Nodes.Add(treeNode);
            }
        }

        private void TreeView1OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TreeViewOnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            NodeMouseClickSelectedNode = ((TreeView)sender).SelectedNode = e.Node;
        }
    }
}
