using System;
using System.Drawing;
using System.Windows.Forms;

namespace Configurator.code
{
    public class RoleTreeViewer
    {
        private Controller controller;
        private TreeView treeView;
        private ContextMenuStrip docMenu;
        private TreeNode nodeMouseClickSelectedNode;        
        private const int RoleNodePosition = 1;
        private const int FrequencyNodePosition = 2;
        private const int JobNodePosition = 3;

        public RoleTreeViewer(Controller controller)
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
            this.treeView.MouseDown += treeView_MouseDown;
            this.treeView.AfterLabelEdit += TreeViewOnAfterLabelEdit;
            this.treeView.Scrollable = true;

            this.InitTreeView();
            this.PopulateTree();

            return this.treeView;
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            this.nodeMouseClickSelectedNode = treeView.GetNodeAt(e.X, e.Y);
        }

        private void TreeView1OnDragDrop(object sender, DragEventArgs e)
        {
            var nodeToDropIn = this.treeView.GetNodeAt(this.treeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null)
            {
                return;
            }
            if (nodeToDropIn.Level > FrequencyNodePosition)
            {
                nodeToDropIn = nodeToDropIn.Parent;
            }

            object jobName = e.Data.GetData(typeof(string));
            if (jobName == null)
            {
                return;
            }

            if (nodeToDropIn.Level == FrequencyNodePosition)
            {
                nodeToDropIn.Nodes.Add(jobName.ToString());
                var role = nodeToDropIn.Parent.Text;
                var frequency = Convert.ToInt32(nodeToDropIn.Text);
                this.controller.AddJobToRoleAndFrequency(role, frequency, jobName.ToString());
            }
        }

        private void InitTreeView()
        {
            this.docMenu = new ContextMenuStrip();
            var deleteLabel = new ToolStripMenuItem { Text = "Delete" };
            var addLabel = new ToolStripMenuItem { Text = "Add" };
            deleteLabel.Click += DeleteLabelOnClick;
            addLabel.Click += AddLabelOnClick;
            this.docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel });
            this.treeView.ContextMenuStrip = this.docMenu;
            treeView.LabelEdit = true;
        }

        private void TreeViewOnAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (this.nodeMouseClickSelectedNode.Level == RoleNodePosition)
                    {
                        var roleNode = this.nodeMouseClickSelectedNode;
                        int frequency = 0;
                        if (!int.TryParse(e.Label, out frequency))
                        {
                            e.CancelEdit = true;
                            MessageBox.Show("Invalid value Must be integer", "Node Label Edit");
                            e.Node.BeginEdit();
                        }
                        else
                        {
                            controller.AddRoleFrequency(frequency, roleNode.Text);
                        }
                    }


                    if (this.nodeMouseClickSelectedNode.Level == 0)
                    {
                        var roleNode = e;
                        controller.AddNewRole(roleNode.Label);                        
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show("Invalid - cannot be blank", "Node Label Edit");
                    e.Node.BeginEdit();
                }
            }
        }

        private void AddLabelOnClick(object sender, EventArgs e)
        {
            if (this.nodeMouseClickSelectedNode.Level == RoleNodePosition)
            {                
                this.nodeMouseClickSelectedNode.ExpandAll();
                var treeNode = new TreeNode("120");
                this.nodeMouseClickSelectedNode.Nodes.Add(treeNode);
                treeNode.ExpandAll();
                treeNode.BeginEdit();
                this.nodeMouseClickSelectedNode.ExpandAll();
                this.nodeMouseClickSelectedNode.Parent.ExpandAll();
            }

            if (this.nodeMouseClickSelectedNode.Level == 0)
            {                
                var treeNode = new TreeNode("NewRole");
                this.nodeMouseClickSelectedNode.Nodes.Add(treeNode);
                treeNode.ExpandAll();
                treeNode.BeginEdit();                
            }
        }

        private void DeleteLabelOnClick(object sender, EventArgs e)
        {

            if (this.nodeMouseClickSelectedNode == null)
            {
                return;
            }

            if (this.nodeMouseClickSelectedNode.Level == RoleNodePosition)
            {
                var roleNode = this.nodeMouseClickSelectedNode;
                var roleName = roleNode.Text;
                //Delete Job
                controller.DeleteRole(roleName);
                this.nodeMouseClickSelectedNode.Remove();
            }

            if (this.nodeMouseClickSelectedNode.Level == FrequencyNodePosition)
            {
                var frequencyNode = this.nodeMouseClickSelectedNode;
                var roleNode = frequencyNode.Parent;
                var frequency = Convert.ToInt32(frequencyNode.Text);
                var roleName = roleNode.Text;
                //Delete Job
                controller.DeleteFrequency(frequency, roleName);
                this.nodeMouseClickSelectedNode.Remove();
            }

            if (NodeIsRole())
            {
                var frequencyNode = this.nodeMouseClickSelectedNode.Parent;
                var roleNode = frequencyNode.Parent;
                var frequency = Convert.ToInt32(frequencyNode.Text);
                var roleName = roleNode.Text;
                var jobName = this.nodeMouseClickSelectedNode.Text;
                //Delete Job
                controller.DeleteJobFromRole(jobName, frequency, roleName);
                this.nodeMouseClickSelectedNode.Remove();
            }
        }

        private bool NodeIsRole()
        {
            return this.nodeMouseClickSelectedNode.Level == JobNodePosition;
        }

        private void PopulateTree()
        {
            var rolesNode = new TreeNode("Roles");
            this.treeView.Nodes.Add(rolesNode);
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
                rolesNode.Nodes.Add(treeNode);
            }
        }

        private void TreeView1OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}
