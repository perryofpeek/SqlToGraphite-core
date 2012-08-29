namespace SqlToGraphite.Conf
{
    public interface IConfigWriter
    {
        void Save(string data);
    }
}