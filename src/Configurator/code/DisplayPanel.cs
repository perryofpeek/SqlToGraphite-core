using System.Windows.Forms;

using SqlToGraphite;

namespace Configurator.code
{
    public class DisplayPanel : BuilderBase
    {
        public DisplayPanel(Panel panel, DefaultJobProperties defaultJobProperties, Controller controller, AssemblyResolver assemblyResolver)
            : base(panel, defaultJobProperties, controller, assemblyResolver)
        {
        }


    }
}