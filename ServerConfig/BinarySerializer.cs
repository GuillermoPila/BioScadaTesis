using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ServerConfig
{
    public class BinarySerializer
    {
        public static void Serialize(object obj, string FileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(FileName, FileMode.Create);
            formatter.Serialize(stream, obj);
            stream.Close();
        }
        public static byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            Serialize(obj, stream);
            return stream.ToArray();
        }
        public static void Serialize(object obj, Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }
        public static object Deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
        public static object Clone(object Source)
        {
            MemoryStream stream = new MemoryStream();
            Serialize(Source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return Deserialize(stream);
        }
        public static object Deserialize(string FileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(FileName, FileMode.Open);
            object Result = formatter.Deserialize(stream);
            stream.Close();
            return Result;
        }
        public static object Deserialize(byte[] Data)
        {
            MemoryStream stream = new MemoryStream(Data);
            return Deserialize(stream);
        }
    }
}