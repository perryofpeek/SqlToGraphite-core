namespace Graphite
{
    using System;

    public class GraphiteMetric : IGraphiteMetric
    {
        public GraphiteMetric(string path, string value, DateTime dotNetTimeStamp)
        {
            this.Path = path;
            this.Value = value;
            this.DotNetTimeStamp = dotNetTimeStamp;
        }

        public GraphiteMetric(string path, int value, DateTime dotNetTimeStamp)
        {
            this.Path = path;
            this.Value = value.ToString();
            this.DotNetTimeStamp = dotNetTimeStamp;
        }

        public GraphiteMetric(string path, long value, DateTime dotNetTimeStamp)
        {
            this.Path = path;
            this.Value = value.ToString();
            this.DotNetTimeStamp = dotNetTimeStamp;
        }

        public GraphiteMetric(string path, decimal value, DateTime dotNetTimeStamp)
        {
            this.Path = path;
            this.Value = value.ToString();
            this.DotNetTimeStamp = dotNetTimeStamp;
        }

        public GraphiteMetric(string path, float value, DateTime dotNetTimeStamp)
        {
            this.Path = path;
            this.Value = value.ToString();
            this.DotNetTimeStamp = dotNetTimeStamp;
        }

        public GraphiteMetric(string path, double value, DateTime dotNetTimeStamp)
        {
            this.Path = path;
            this.Value = value.ToString();
            this.DotNetTimeStamp = dotNetTimeStamp;
        }

        public string Path { get; private set; }

        public string Value { get; private set; }

        public long UnixTimeStamp
        {
            get
            {
                return this.DotNetTimeStamp.ToUnixTime();
            }
        }

        public DateTime DotNetTimeStamp { get; private set; }

        public string ToGraphiteMessage()
        {
            return string.Format("{0} {1} {2}\n", this.Path, this.Value, this.UnixTimeStamp);
        }
    }
}