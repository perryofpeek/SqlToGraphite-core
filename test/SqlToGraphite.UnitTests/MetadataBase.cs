namespace SqlToGraphite.UnitTests
{
    using System;
    using System.Xml.Serialization;

    [XmlType("Meta")]
    [XmlInclude(typeof(ReadonlyMetadata))]
    [Serializable]
    public abstract class MetadataBase
    {
    }
}