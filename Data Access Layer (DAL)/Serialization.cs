/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-09-27
/// Modified: n/a
/// ---------------------------

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace DAL
{
    /// <summary>
    /// Utility class containing generic methods for binary and xml serialization to and from file.
    /// </summary>
    public class Serialization
    {
        /// <summary>
        /// BinarySerialize any type of object to file.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object.</param>
        /// <param name="filePath">Path to file.</param>
        public static void BinarySerializeToFile<T>(T obj, string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                formatter.Serialize(stream, obj);
        }

        /// <summary>
        /// BinaryDeserialize any files serialized using BinarySerializeToFile&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="filePath">Path to file.</param>
        /// <returns></returns>
        public static T BinaryDeserializeFromFile<T>(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
                return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Serialize any type of object to XML file.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object.</param>
        /// <param name="filePath">Path to XML file.</param>
        public static void XmlSerializeToFile<T>(T obj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(filePath))
                serializer.Serialize(writer, obj);
        }

        /// <summary>
        /// Deserialize any XML file serialized using XmlSerializeToFile&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="filePath">Path to XML file.</param>
        /// <returns>The object containing data read from the XML file.</returns>
        public static T XmlDeserializeFromFile<T>(string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StreamReader(filePath))
                return (T)xmlSerializer.Deserialize(reader);
        }
    }
}
