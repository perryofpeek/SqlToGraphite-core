using System;
using System.Windows.Forms;
using SqlToGraphite;

namespace Configurator.code
{
    public class Builder : BuilderBase
    {
        public Builder(Panel panel, DefaultJobProperties defaultJobProperties, Controller controller, AssemblyResolver assemblyResolver, DataGridView resultView)
            : base(panel, defaultJobProperties, controller, assemblyResolver, resultView)
        {
        }

        private Panel addJobPannel;

        private AddJobBuilder addJobBuilder;

        private void AddPluginComboBox()
        {
            var lbl = new Label { Text = "Type", Top = this.nextTop, Width = this.defaultJobProperties.DefaultLabelWidth };
            var cb = new ComboBox { Top = this.GetNextTop(), Left = this.defaultJobProperties.DefaultLabelWidth + this.defaultJobProperties.DefaultSpace, Name = "ClientName" };
            var jobTypes = assemblyResolver.GetJobTypes();
            cb.Width = 250;
            cb.SelectedIndexChanged += this.AddPluginIndexChanged;
            foreach (var c in jobTypes)
            {
                cb.Items.Add(c.Key);
            }
            panel.Controls.Add(lbl);
            panel.Controls.Add(cb);
        }

        private void AddPluginIndexChanged(object sender, EventArgs eventArgs)
        {
            var cb = (ComboBox)sender;            
            addJobBuilder.Render(cb.SelectedItem.ToString());            
        }
        
        public void DisplayJobAdd()
        {  
             addJobPannel = new Panel();
            this.AddPluginComboBox();
            this.AddPluginPannel();
            addJobBuilder = new AddJobBuilder(addJobPannel, defaultJobProperties, controller, assemblyResolver, this.resultGrid);
            addJobBuilder.AddedJobEvent += AddJobBuilderOnAddedJobEvent;
        }

        private void AddJobBuilderOnAddedJobEvent(object sender, EventArgs eventArgs)
        {
            this.OnAddedJobEvent(EventArgs.Empty);
        }

        private void AddPluginPannel()
        {           
            this.addJobPannel = this.CreateNewPanel();
            this.panel.Controls.Add(addJobPannel);
        }        
    }
}