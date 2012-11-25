namespace SqlToGraphite.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("Metadata")]
    [Serializable]
    public class MetadataContainer : List<MetadataBase>
    {
    }
}