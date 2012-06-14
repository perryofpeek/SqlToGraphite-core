namespace SqlToGraphite.Clients
{
    public interface IKnownGraphiteClients
    {
        bool IsKnown(string client);
    }
}