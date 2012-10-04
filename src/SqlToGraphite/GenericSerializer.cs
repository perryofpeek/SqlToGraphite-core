namespace ConfigSpike.Config
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// A generic class used to serialize objects.
    /// </summary>
    public class GenericSerializer
    {
        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>String representation of the serialized object.</returns>
        public static string Serialize<T>(T obj)
        {
            XmlSerializer xs = null;
            StringWriter sw = null;
            try
            {
                xs = new XmlSerializer(typeof(T));
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

        public static string Serialize<T>(T obj, Type[] extraTypes)
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
        public static void Serialize<T>(T obj, XmlWriter writer)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            xs.Serialize(writer, obj);
        }

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="writer">The writer to be used for output in the serialization.</param>
        /// <param name="extraTypes"><c>Type</c> array of additional object types to serialize.</param>
        public static void Serialize<T>(T obj, XmlWriter writer, Type[] extraTypes)
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
        public static T Deserialize<T>(XmlReader reader)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
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
        public static T Deserialize<T>(XmlReader reader, Type[] extraTypes)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T), extraTypes);
            return (T)xs.Deserialize(reader);
        }

        /// <summary>
        /// Deserializes the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="xml">The XML file containing the serialized object.</param>
        /// <returns>The deserialized object of type T.</returns>
        public static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer xs = null;
            StringReader sr = null;
            try
            {
                xs = new XmlSerializer(typeof(T));
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

        public static T Deserialize<T>(string xml, Type[] extraTypes)
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

        public static void SaveAs<T>(T obj, string fileName, Encoding encoding, Type[] extraTypes)
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
                    Serialize<T>(obj, writer, extraTypes);
                }
                else
                {
                    Serialize<T>(obj, writer);
                }
            
                writer.Flush();
                document.Save(writer);
            }
        }
        
        public static void SaveAs<T>(T obj, string fileName, Type[] extraTypes)
        {
            SaveAs<T>(obj, fileName, Encoding.UTF8, extraTypes);
        }
        
        public static void SaveAs<T>(T obj, string fileName, Encoding encoding)
        {
            SaveAs<T>(obj, fileName, encoding, null);
        }
        
        public static void SaveAs<T>(T obj, string fileName)
        {
            SaveAs<T>(obj, fileName, Encoding.UTF8);
        }

        public static T Open<T>(string fileName, Type[] extraTypes)
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
                        obj = Deserialize<T>(reader, extraTypes);
                    }
                    else
                    {
                        obj = Deserialize<T>(reader);
                    }
                }
            }

            return obj;
        }

        public static T Open<T>(string fileName)
        {
            return Open<T>(fileName, null);
        }
    }
}