using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eagle.Utility
{
    public static class SerializerHelper
    {
        /// <summary>
        ///  Serializes the <paramref name="obj"/> object to its xml representation and
        ///  saves the file to the absolute <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="path">The absolute path on disk to save the serialized xml file.</param>
        /// <param name="obj">The object to be deserialized.</param>        
        public static Task XmlSerialize<T>(string path, T obj) where T : class
        {
            return Task.Run(() =>
                {
                    var ser = new XmlSerializer(typeof(T));
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        ser.Serialize(fs, obj);
                    }
                });
        }

        /// <summary>
        /// Serializes the <paramref name="obj"/> object and returns the xml as string.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>A string containing the serialized xml string.</returns>
        public static string XmlSerialize<T>(T obj) where T : class
        {
            string result;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.Serialize(ms, obj);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        /// <summary>
        /// Deserializes the Stream object <paramref name="stream"/> into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type to deserialized into.</typeparam>
        /// <param name="stream">The stream containing the xml file.</param>
        /// <returns>An object of type T deserialized from the xml representation.</returns>
        public static T XmlDeserialize<T>(Stream stream) where T : class
        {
            if (stream == null) return null;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            return ser.Deserialize(stream) as T;
        }

        /// <summary>
        /// Deserializes the <paramref name="path"/> xml into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type to deserialized into.</typeparam>
        /// <param name="stream">The path to the xml file.</param>
        /// <returns>An object of type T deserialized from the xml representation.</returns>
        public static T XmlDeserialize<T>(string path) where T : class
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (!File.Exists(path)) return null;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return ser.Deserialize(fs) as T;
            }
        }


    }
}
