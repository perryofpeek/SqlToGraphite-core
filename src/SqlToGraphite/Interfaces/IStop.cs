namespace SqlToGraphite
{
    public interface IStop
    {
        bool ShouldStop();

        void SetStop(bool value);
    }
}