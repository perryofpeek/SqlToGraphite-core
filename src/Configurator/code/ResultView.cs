using System.Collections.Generic;
using System.Windows.Forms;
using SqlToGraphiteInterfaces;

namespace Configurator.code
{
    public class ResultView
    {
        private int defaultHeight;

        private Panel resultsPanel;

        private int nextTop;

        private int defaultWidth;

        public ResultView(int defaultHeight, int defaultWidth)
        {
            this.defaultHeight = defaultHeight;
            this.defaultWidth = defaultWidth;
            nextTop = 0;
        }

        protected void IncNextTop()
        {
            this.nextTop = this.nextTop + this.defaultHeight;
        }


        public Panel Get(IList<IResult> results, Panel panel)
        {
            resultsPanel = panel;
            this.DisplayResults(results);
            return resultsPanel;
        }

        protected void DisplayResults(IList<IResult> results)
        {
           resultsPanel.Controls.Add(CreateResultHeader());             
            foreach (var result in results)
            {                
                resultsPanel.Controls.Add(this.CreateResultPanel(result));                
            }
        }

        private Panel CreateResultHeader()
        {
            var p = new Panel { Width = this.resultsPanel.Width, Top = nextTop, Height = this.defaultHeight };
            var l = CreateLabel(0,"Name");
            var l1 = CreateLabel(l.Width, "Path");
            var l2 = CreateLabel(l.Width + l1.Width, "Value");
            var l3 = CreateLabel(l.Width + l1.Width + l2.Width, "TimeStamp");
            p.Controls.Add(l);
            p.Controls.Add(l1);
            p.Controls.Add(l2);
            p.Controls.Add(l3);
            IncNextTop();
            return p;
        }

        private Panel CreateResultPanel(IResult result)
        {
            var p = new Panel { Width = this.resultsPanel.Width , Top = nextTop, Height = this.defaultHeight};
            //p.BorderStyle = BorderStyle.Fixed3D;
            var l = CreateLabel(0, result.Name);
            var l1 = CreateLabel(l.Width, result.Path);
            var l2 = CreateLabel(l.Width + l1.Width, result.Value.ToString());
            var l3 = CreateLabel(l.Width + l1.Width + l2.Width, result.TimeStamp.ToString());
            p.Controls.Add(l);
            p.Controls.Add(l1);
            p.Controls.Add(l2);
            p.Controls.Add(l3);
            IncNextTop();
            return p;
        }

        private Label CreateLabel(int left, string text)
        {            
            var label = new Label
            {                
                Left = left,
                Text = text,
                BorderStyle = BorderStyle.FixedSingle,                
                Width = defaultWidth             
            };
            return label;
        }
    }
}