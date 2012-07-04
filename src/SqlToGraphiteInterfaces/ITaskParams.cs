namespace SqlToGraphite
{
    public interface ITaskParams
    {
        string Path { get; }

        string Sql { get; }

        string ConnectionString { get; }

        string Type { get; }

        string Name { get; }

        string Client { get; }
    }
}