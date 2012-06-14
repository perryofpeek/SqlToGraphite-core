
namespace SqlToGraphite
{
    public interface ISleep 
    {
        void Sleep(int length);

        void SleepSeconds(int frequency);
    }
}