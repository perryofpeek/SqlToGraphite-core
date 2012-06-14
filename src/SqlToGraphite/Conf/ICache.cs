namespace SqlToGraphite.Conf
{
    public interface ICache
    {
        void ResetCache();

        bool HasExpired();
    }
}