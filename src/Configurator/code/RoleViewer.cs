using System.Windows.Forms;

namespace Configurator.code
{
    public class RoleViewer
    {
        private readonly Controller controller;

        private ListBox labelRoles;

        public RoleViewer(Controller controller)
        {
            this.controller = controller;
        }

        private void LabelRolesOnDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void LabelRolesOnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if (this.labelRoles != null && this.labelRoles.SelectedItem != null)
            {
                this.labelRoles.DoDragDrop(this.labelRoles.SelectedItem, DragDropEffects.Move);    
            }            
        }

        private void DisplayRoles()
        {           
            this.labelRoles.Items.Clear();
            foreach (var role in controller.GetRoles())
            {
                this.labelRoles.Items.Add(role);
            }
        }

        public ListBox Get(Position position)
        {
            this.labelRoles = new ListBox
                {
                    Width = position.Width,
                    Height = position.Height,
                    Left = position.Left,
                    Top = position.Top,
                    AllowDrop = true
                };
            this.labelRoles.DragOver += this.LabelRolesOnDragOver;
            this.labelRoles.MouseDown += this.LabelRolesOnMouseDown;
            this.DisplayRoles();
            return this.labelRoles;
        }
    }
}