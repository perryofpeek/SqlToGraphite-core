using System.Windows.Forms;

namespace Configurator.code
{
    public class Builder
    {
        private readonly Panel panel;

        private readonly DefaultJobProperties defaultJobProperties;

        //public Control.ControlCollection Controls { get; set; }

        public Builder(Panel panel, DefaultJobProperties defaultJobProperties)
        {
            this.panel = panel;
            this.defaultJobProperties = defaultJobProperties;
            //Controls = new Control.ControlCollection(owner);
        }

        private int nextTop;

        public void AddPair(string name, string value)
        {
            var lbl = new Label { Text = name, Top = this.nextTop, Width = this.defaultJobProperties.DefaultLabelWidth };
            var txb = new TextBox
            {
                Text = value,
                Top = this.nextTop,
                Left = this.defaultJobProperties.DefaultLabelWidth + this.defaultJobProperties.DefaultSpace,
                Width = defaultJobProperties.TextWidth               
            };
            panel.Controls.Add(lbl);
            panel.Controls.Add(txb);
            nextTop = nextTop + defaultJobProperties.DefaultHeight;
        }
    }
}