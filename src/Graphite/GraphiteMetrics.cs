namespace Graphite
{
    using System.Collections.Generic;

    using SqlToGraphiteInterfaces;

    public class GraphiteMetrics
    {
        private readonly IList<IResult> results;

        public GraphiteMetrics(IList<IResult> results)
        {
            this.results = results;
        }

        public string ToGraphiteMessageList()
        {
            var rtn = string.Empty;
            foreach (var result in this.results)
            {
                var graphiteMetric = new GraphiteMetric(result.FullPath, result.Value, result.TimeStamp);
                rtn = string.Concat(rtn , graphiteMetric.ToGraphiteMessage());
            }

            return rtn;
        }
    }
}