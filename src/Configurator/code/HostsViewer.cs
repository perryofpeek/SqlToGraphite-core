using System.Windows.Forms;

using Configurator.code;

namespace Configurator
{
    internal class HostsViewer
    {
         private Controller controller;

         private readonly HostsTreeViewer roleTreeViewer;

        private Panel panel;

        public HostsViewer(Controller controller, HostsTreeViewer roleTreeViewer)
        {
            this.controller = controller;
            this.roleTreeViewer = roleTreeViewer;
        }

        public Panel Get(Position position)
        {
            this.panel = new Panel
                {
                    Width = position.Width,
                    Height = position.Height,
                    Left = position.Left,
                    Top = position.Top     
                };
            this.panel.Controls.Clear();
            var jobLbPos = new Position(0, 200, 0, position.Height);
            var roleViewer = new RoleViewer(controller);
            var lb = roleViewer.Get(jobLbPos);
            var tvPos = new Position(250, 300, 0, position.Height);
            var tv = this.roleTreeViewer.Get(tvPos);
            panel.Controls.Add(lb);
            panel.Controls.Add(tv);
            return panel;
        }
    }
}