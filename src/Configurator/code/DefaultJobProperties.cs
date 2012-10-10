namespace Configurator.code
{
    public class DefaultJobProperties
    {
        private readonly int maxWidth;

        public DefaultJobProperties(int maxWidth)
        {
            this.maxWidth = maxWidth;
            DefaultHeight = 25;
            DefaultLabelWidth = 100;
            DefaultSpace = 10;
        }

        public int DefaultLabelWidth { get; set; }

        public int TextWidth
        {
            get
            {
                return maxWidth - (DefaultLabelWidth + DefaultSpace);
            }

            set
            {
                TextWidth = value;
            }
        }

        public int DefaultHeight { get; set; }

        public int DefaultSpace { get; set; }
    }
}