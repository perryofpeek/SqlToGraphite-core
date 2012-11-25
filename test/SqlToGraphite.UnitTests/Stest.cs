namespace SqlToGraphite.UnitTests
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using NUnit.Framework;

    [TestFixture]
    public class Stest
    {
        [Test]
        public void TestLinq2Xml()
        {
            const string Text = @"<Metadata xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                            <Meta xsi:type=""Readonly"" />
                            <Meta xsi:type=""Garbage"" />
                      </Metadata>";

            // Get the "names" of all implementors of MetadataBase
            var types = AppDomain.CurrentDomain.GetAssemblies().ToList()
               .SelectMany(s => s.GetTypes())
                   .Where(p => typeof(MetadataBase).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface)
                   .Where(t => t.GetCustomAttributes(typeof(XmlTypeAttribute), false).Any())
                   .Select(t => t.GetCustomAttributes(typeof(XmlTypeAttribute), false)
                       .Cast<XmlTypeAttribute>().First().TypeName);

            // Create a parser
            var parser = new XmlSerializer(typeof(MetadataBase));

            // Create metadatacontainer to fill
            var metas = new MetadataContainer();
            // Fill it with matching from from the XML
            metas.AddRange((from t in XDocument.Parse(Text).Descendants("Meta")
                            where types.Contains(t.Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance")).Value)
                            select (MetadataBase)parser.Deserialize(t.CreateReader())).ToList());

            // Should be one guy present
            Assert.AreEqual(metas.Count, 1);
        }
    }
}