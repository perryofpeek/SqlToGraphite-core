using System;
using System.Drawing;
using System.Windows.Forms;

namespace Configurator.code
{
    public class RolesViewer
    {
        private Controller controller;

        private readonly TreeViewer treeViewer;

        private Panel panel;

        public RolesViewer(Controller controller, TreeViewer treeViewer)
        {
            this.controller = controller;
            this.treeViewer = treeViewer;
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
            var jobViewer = new JobViewer(controller);
            var lb = jobViewer.Get(jobLbPos);
            var tvPos = new Position(250, 300, 0, position.Height);
            var tv = treeViewer.Get(tvPos);
            panel.Controls.Add(lb);
            panel.Controls.Add(tv);
            return panel;
        }
    }
}