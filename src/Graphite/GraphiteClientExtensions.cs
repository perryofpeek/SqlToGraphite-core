namespace Graphite
{
    using System;

    public static class GraphiteClientExtensions
    {
        public static void Send(this IGraphiteClient self, string path, int value)
        {
            self.Send(path, value, DateTime.Now);
        }
    }
}