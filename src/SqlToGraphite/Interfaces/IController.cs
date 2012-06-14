namespace SqlToGraphite
{
    public interface IController
    {
        void Process();

        void Stop();
    }
}