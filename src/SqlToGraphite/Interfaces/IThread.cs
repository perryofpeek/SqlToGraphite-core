namespace SqlToGraphite
{
    public interface IThread
    {
        void Start();

        void Abort();
    }
}