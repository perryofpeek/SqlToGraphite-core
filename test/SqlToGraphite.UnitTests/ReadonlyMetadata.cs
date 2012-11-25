namespace SqlToGraphite.UnitTests
{
    using System;
    using System.Xml.Serialization;

    [XmlType("Readonly")]
    [Serializable]
    public class ReadonlyMetadata : MetadataBase
    {
    }
}