using System.Collections.Generic;

namespace ConfigSpike.Config
{
    public class Template
    {
        public Template()
        {
            this.WorkItems = new List<WorkItems>();
        }

        public List<WorkItems> WorkItems { get; set; }
    }
}