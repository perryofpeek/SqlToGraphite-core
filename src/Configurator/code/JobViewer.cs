using System.Windows.Forms;

namespace Configurator.code
{
    public class JobViewer
    {
        private readonly Controller controller;

        private ListBox lbJobRoles;

        public JobViewer(Controller controller)
        {
            this.controller = controller;
        }

        private void LbJobRolesOnDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void LbJobRolesOnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if (this.lbJobRoles != null && this.lbJobRoles.SelectedItem != null)
            {
                this.lbJobRoles.DoDragDrop(this.lbJobRoles.SelectedItem, DragDropEffects.Move);    
            }            
        }

        private void DisplayJobs()
        {           
            lbJobRoles.Items.Clear();
            foreach (var job in controller.GetJobs())
            {
                lbJobRoles.Items.Add(job.Name);
            }
        }

        public ListBox Get(Position position)
        {
            this.lbJobRoles = new ListBox
                {
                    Width = position.Width,
                    Height = position.Height,
                    Left = position.Left,
                    Top = position.Top,
                    AllowDrop = true
                };
            this.lbJobRoles.DragOver += this.LbJobRolesOnDragOver;
            this.lbJobRoles.MouseDown += this.LbJobRolesOnMouseDown;
            DisplayJobs();
            return this.lbJobRoles;
        }
    }
}