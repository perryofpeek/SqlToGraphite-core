using System.IO;
using System.Xml;

namespace SqlToGraphite
{
    public interface IConfigReader
    {       
        string GetXml();
    }
}