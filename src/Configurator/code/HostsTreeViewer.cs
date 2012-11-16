using System;
using System.Drawing;
using System.Windows.Forms;

using Configurator.code;

namespace Configurator
{
    internal class HostsTreeViewer
    {
        private Controller controller;
        private TreeView treeView;
        private ContextMenuStrip docMenu;
        private TreeNode nodeMouseClickSelectedNode;
        private const int HostnameNodePosition = 1;
        private const int RoleNodePosition = 2;
        //private const int JobNodePosition = 3;

        public event ChangedEventHandler Changed;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public HostsTreeViewer(Controller controller)
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
            var hostname = "";
            object roleName = e.Data.GetData(typeof(string));
            if (roleName == null)
            {
                return;
            }

            if (nodeToDropIn.Level == 1)
            {
                hostname = nodeToDropIn.Text;
            }

            if (nodeToDropIn.Level == 2)
            {
                nodeToDropIn = nodeToDropIn.Parent;
                hostname = nodeToDropIn.Text;
            }


            if ((hostname != string.Empty) && (roleName.ToString() != string.Empty))
            {
                nodeToDropIn.Nodes.Add(roleName.ToString());
                this.controller.AddRoleToHost(hostname, roleName.ToString());
            }

            OnChanged(EventArgs.Empty);
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
                    if (this.nodeMouseClickSelectedNode.Level == HostnameNodePosition)
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
                            OnChanged(EventArgs.Empty);
                        }
                    }

                    if (this.nodeMouseClickSelectedNode.Level == 0)
                    {
                        var hostNode = e;
                        controller.AddNewHost(hostNode.Label);
                        OnChanged(EventArgs.Empty);
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
            if (this.nodeMouseClickSelectedNode.Level == 0)
            {
                var treeNode = new TreeNode("NewHost");
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

            if (this.nodeMouseClickSelectedNode.Level == HostnameNodePosition)
            {
                var hostNameNode = this.nodeMouseClickSelectedNode;
                var hostname = hostNameNode.Text;
                //Delete Job
                controller.DeleteHost(hostname);
                this.nodeMouseClickSelectedNode.Remove();
            }

            if (this.nodeMouseClickSelectedNode.Level == RoleNodePosition)
            {
                var roleNode = this.nodeMouseClickSelectedNode;
                var roleName = roleNode.Text;
                var hostnameNode = roleNode.Parent;
                var hostname = hostnameNode.Text;
                //Delete Job
                controller.DeleteRoleFromHost(hostname, roleName);
                this.nodeMouseClickSelectedNode.Remove();
            }

            //if (NodeIsRole())
            //{
            //    var frequencyNode = this.nodeMouseClickSelectedNode.Parent;
            //    var roleNode = frequencyNode.Parent;
            //    var frequency = Convert.ToInt32(frequencyNode.Text);
            //    var roleName = roleNode.Text;
            //    var jobName = this.nodeMouseClickSelectedNode.Text;
            //    //Delete Job
            //    controller.DeleteJobFromRole(jobName, frequency, roleName);
            //    this.nodeMouseClickSelectedNode.Remove();
            //}
        }

        //private bool NodeIsRole()
        // {
        //     return this.nodeMouseClickSelectedNode.Level == JobNodePosition;
        //}

        private void PopulateTree()
        {
            var rolesNode = new TreeNode("Host");
            this.treeView.Nodes.Add(rolesNode);
            var wi = this.controller.GetHosts();
            foreach (var w in wi)
            {
                var treeNode = new TreeNode(w.Name);
                foreach (var ts in w.Roles)
                {
                    treeNode.Nodes.Add(new TreeNode(ts.Name));
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