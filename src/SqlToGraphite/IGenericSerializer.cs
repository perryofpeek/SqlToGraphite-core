using System;
using System.Text;
using System.Xml;

namespace ConfigSpike.Config
{
    public interface IGenericSerializer
    {
        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>String representation of the serialized object.</returns>
        string Serialize<T>(T obj);

        string Serialize<T>(T obj, Type[] extraTypes);

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="writer">The writer to be used for output in the serialization.</param>
        void Serialize<T>(T obj, XmlWriter writer);

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="writer">The writer to be used for output in the serialization.</param>
        /// <param name="extraTypes"><c>Type</c> array of additional object types to serialize.</param>
        void Serialize<T>(T obj, XmlWriter writer, Type[] extraTypes);

        /// <summary>
        /// Deserializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="reader">The reader used to retrieve the serialized object.</param>
        /// <returns>The deserialized object of type T.</returns>
        T Deserialize<T>(XmlReader reader);

        /// <summary>
        /// Deserializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="reader">The reader used to retrieve the serialized object.</param>
        /// <param name="extraTypes"><c>Type</c> array
        ///           of additional object types to deserialize.</param>
        /// <returns>The deserialized object of type T.</returns>
        T Deserialize<T>(XmlReader reader, Type[] extraTypes);

        /// <summary>
        /// Deserializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="xml">The XML file containing the serialized object.</param>
        /// <returns>The deserialized object of type T.</returns>
        T Deserialize<T>(string xml);

        T Deserialize<T>(string xml, Type[] extraTypes);

        void SaveAs<T>(T obj, string fileName, Encoding encoding, Type[] extraTypes);

        void SaveAs<T>(T obj, string fileName, Type[] extraTypes);

        void SaveAs<T>(T obj, string fileName, Encoding encoding);

        void SaveAs<T>(T obj, string fileName);

        T Open<T>(string fileName, Type[] extraTypes);

        T Open<T>(string fileName);
    }
}