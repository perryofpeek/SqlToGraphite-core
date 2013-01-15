namespace SqlToGraphite
{
    public interface IConfigReader
    {       
        string GetXml();

        string GetHash();
    }
}