using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SqlToGraphite
{
    /// <summary>
    /// A generic class used to serialize objects.
    /// </summary>
    public class GenericSerializer : IGenericSerializer
    {
        public GenericSerializer(string defaultNamespace)
        {
            this.defaultNamespace = defaultNamespace;
        }

        private string defaultNamespace = "SqlToGraphite_1.0.0.0";

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>String representation of the serialized object.</returns>
        public string Serialize<T>(T obj)
        {
            XmlSerializer xs = null;
            StringWriter sw = null;
            try
            {
                xs = new XmlSerializer(typeof(T), defaultNamespace);
                sw = new StringWriter();
                xs.Serialize(sw, obj);
                sw.Flush();
                return sw.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public string Serialize<T>(T obj, Type[] extraTypes)
        {
            XmlSerializer xs = null;
            StringWriter sw = null;
            try
            {
                xs = new XmlSerializer(typeof(T), extraTypes);
                sw = new StringWriter();
                xs.Serialize(sw, obj);
                sw.Flush();
                return sw.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="writer">The writer to be used for output in the serialization.</param>
        public void Serialize<T>(T obj, XmlWriter writer)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T), defaultNamespace);
            xs.Serialize(writer, obj);
        }

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="writer">The writer to be used for output in the serialization.</param>
        /// <param name="extraTypes"><c>Type</c> array of additional object types to serialize.</param>
        public void Serialize<T>(T obj, XmlWriter writer, Type[] extraTypes)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T), extraTypes);
            xs.Serialize(writer, obj);
        }

        /// <summary>
        /// Deserializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="reader">The reader used to retrieve the serialized object.</param>
        /// <returns>The deserialized object of type T.</returns>
        public T Deserialize<T>(XmlReader reader)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T), defaultNamespace);
            return (T)xs.Deserialize(reader);
        }

        /// <summary>
        /// Deserializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="reader">The reader used to retrieve the serialized object.</param>
        /// <param name="extraTypes"><c>Type</c> array
        ///           of additional object types to deserialize.</param>
        /// <returns>The deserialized object of type T.</returns>
        public T Deserialize<T>(XmlReader reader, Type[] extraTypes)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T), extraTypes);
           // xs.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);
           // xs.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);
           // xs.UnreferencedObject += new UnreferencedObjectEventHandler(Serializer_UnknownObject);
            return (T)xs.Deserialize(reader);
        }

        private void Serializer_UnknownObject(object sender, UnreferencedObjectEventArgs e)
        {
            Console.WriteLine("Unknown Element");
            Console.WriteLine("\t" + e.UnreferencedId + " " + e.UnreferencedObject);
            //Group x = (Group)e.ObjectBeingDeserialized;
            //Console.WriteLine(x.GroupName);
            Console.WriteLine(sender.ToString());
        }
        
        private void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Console.WriteLine("Unknown Element");
            Console.WriteLine("\t" + e.Attr.Name + " " + e.Attr.InnerXml);
            Console.WriteLine("\t LineNumber: " + e.LineNumber);
            Console.WriteLine("\t LinePosition: " + e.LinePosition);

            //Group x = (Group)e.ObjectBeingDeserialized;
            //Console.WriteLine(x.GroupName);
            Console.WriteLine(sender.ToString());
        }

        private void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            Console.WriteLine("Unknown Element");
            Console.WriteLine("\t" + e.Element.Name + " " + e.Element.InnerXml);
            Console.WriteLine("\t LineNumber: " + e.LineNumber);
            Console.WriteLine("\t LinePosition: " + e.LinePosition);

            //Group x = (Group)e.ObjectBeingDeserialized;
            //Console.WriteLine(x.GroupName);
            Console.WriteLine(sender.ToString());
        }

        /// <summary>
        /// Deserializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="xml">The XML file containing the serialized object.</param>
        /// <returns>The deserialized object of type T.</returns>
        public T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer xs = null;
            StringReader sr = null;
            try
            {
                xs = new XmlSerializer(typeof(T), defaultNamespace);
                sr = new StringReader(xml);
                return (T)xs.Deserialize(sr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        public T Deserialize<T>(string xml, Type[] extraTypes)
        {
            if (xml == null || xml == string.Empty)
            {
                return default(T);
            }

            XmlSerializer xs = null;
            StringReader sr = null;
            try
            {
                xs = new XmlSerializer(typeof(T), extraTypes);
                sr = new StringReader(xml);
                return (T)xs.Deserialize(sr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        public void SaveAs<T>(T obj, string fileName, Encoding encoding, Type[] extraTypes)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var di = new DirectoryInfo(Path.GetDirectoryName(fileName));
            if (!di.Exists)
            {
                di.Create();
            }

            var document = new XmlDocument();
            var settings = new XmlWriterSettings { Indent = true, Encoding = encoding, CloseOutput = true, CheckCharacters = false };
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                if (extraTypes != null)
                {
                    this.Serialize<T>(obj, writer, extraTypes);
                }
                else
                {
                    this.Serialize<T>(obj, writer);
                }

                writer.Flush();
                document.Save(writer);
            }
        }

        public void SaveAs<T>(T obj, string fileName, Type[] extraTypes)
        {
            this.SaveAs<T>(obj, fileName, Encoding.UTF8, extraTypes);
        }

        public void SaveAs<T>(T obj, string fileName, Encoding encoding)
        {
            this.SaveAs<T>(obj, fileName, encoding, null);
        }

        public void SaveAs<T>(T obj, string fileName)
        {
            this.SaveAs<T>(obj, fileName, Encoding.UTF8);
        }

        public T Open<T>(string fileName, Type[] extraTypes)
        {
            T obj = default(T);
            if (File.Exists(fileName))
            {
                var settings = new XmlReaderSettings { CloseInput = true, CheckCharacters = false };
                using (var reader = XmlReader.Create(fileName, settings))
                {
                    reader.ReadOuterXml();
                    if (extraTypes != null)
                    {
                        obj = this.Deserialize<T>(reader, extraTypes);
                    }
                    else
                    {
                        obj = this.Deserialize<T>(reader);
                    }
                }
            }

            return obj;
        }

        public T Open<T>(string fileName)
        {
            return this.Open<T>(fileName, null);
        }
    }
}