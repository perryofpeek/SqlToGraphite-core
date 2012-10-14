using System;
using System.Windows.Forms;

using SqlToGraphite;

namespace Configurator.code
{
    public class AddJobBuilder : BuilderBase
    {
        public AddJobBuilder(Panel panel, DefaultJobProperties defaultJobProperties, Controller controller, AssemblyResolver assemblyResolver)
            : base(panel, defaultJobProperties, controller, assemblyResolver)
        {
        }       
        
        private void AddClientComboBox()
        {
            var lbl = new Label { Text = "ClientName", Top = this.nextTop, Width = this.defaultJobProperties.DefaultLabelWidth };
            var cb = new ComboBox { Top = this.GetNextTop(), Left = this.defaultJobProperties.DefaultLabelWidth + this.defaultJobProperties.DefaultSpace, Name = "ClientName" };
            var clients = controller.GetClientTypes();
            foreach (var c in clients)
            {
                cb.Items.Add(c.ClientName);
            }
            panel.Controls.Add(lbl);
            panel.Controls.Add(cb);
        }

        public void Render(string name)
        {
            panel.Controls.Clear();
            this.nextTop = 0;
            this.AddClientComboBox();
            this.DisplayEmptyJob(name);            
        }
    }
}