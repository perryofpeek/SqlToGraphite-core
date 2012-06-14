namespace SqlToGraphite.Clients
{
    public interface IKnownDataClients
    {
        bool IsKnown(string client);
    }
}