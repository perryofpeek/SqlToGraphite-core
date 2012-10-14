namespace SqlToGraphite.Conf
{
    public interface IConfigWriter
    {
        void Save(string data);

        void Save(string data, string path);
    }
}