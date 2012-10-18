using System;
using System.Windows.Forms;

namespace Configurator.code
{
    public class RolesView
    {
        private readonly Panel panel;

        private readonly Controller controller;

        private ListBox rolesListBox;

        private ListBox freqListBox;

        private ListBox tasksListBox;

        private int selectedFrequency;

        private string selectedRole;

        private int nextTop;

        private int defaultWidth;

        public RolesView(Panel panel, Controller controller)
        {
            this.panel = panel;
            this.controller = controller;
            nextTop = 0;
            defaultWidth = 100;
        }

        public void GetRoles()
        {
            panel.Controls.Clear();
                      
            panel.Controls.Add(CreateLabel("Roles", 0));
            panel.Controls.Add(CreateLabel("Frequency", defaultWidth));
            panel.Controls.Add(CreateLabel("Roles", defaultWidth + defaultWidth));

            var roles = controller.GetRoles();
            rolesListBox = new ListBox
                {
                    Height = this.panel.Height,
                    Top = nextTop,
                    Width = defaultWidth
                };
            rolesListBox.Items.AddRange(roles.ToArray());
            rolesListBox.SelectedIndexChanged += ListboxOnSelectedIndexChanged;
            panel.Controls.Add(rolesListBox);
            freqListBox = new ListBox
            {
                Left = this.rolesListBox.Width,
                Height = this.panel.Height,
                Top = nextTop
            };
            panel.Controls.Add(freqListBox);
            tasksListBox = new ListBox
                {
                    Left = (rolesListBox.Width + this.freqListBox.Width),
                    Height = this.panel.Height,
                    Top = nextTop
                };
            panel.Controls.Add(tasksListBox);
        }

        private Label CreateLabel(string value,int Left)
        {
            var lbl = new Label { Top = 0, Text = value, Left = Left };
            return lbl;
        }

        private void ListboxOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            freqListBox.Items.Clear();
            var lb = (ListBox)sender;
            selectedRole = lb.SelectedItem.ToString();
            var freq = controller.GetTasksFrequencyInRole(selectedRole);
            foreach (var task in freq)
            {
                freqListBox.Items.Add(task);
            }
            freqListBox.SelectedIndexChanged += FreqListBoxOnSelectedIndexChanged;
            tasksListBox.Items.Clear();
        }

        private void FreqListBoxOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            tasksListBox.Items.Clear();
            var lb = (ListBox)sender;
            selectedFrequency = Convert.ToInt32(lb.SelectedItem);
            var freq = controller.GetTasksWithFrequencyInRole(selectedFrequency, selectedRole);
            foreach (var task in freq)
            {
                tasksListBox.Items.Add(task);
            }
        }
    }
}