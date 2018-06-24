using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Soap;


namespace BioScadaServer.Tools
{
    
    public class XmlSerializer
    {

        public static void Serialize(object obj, string FileName)
        {

            // Serializing NameValueCollection object by using SoapFormatter
            SoapFormatter sf = new SoapFormatter();
            Stream strm1 = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sf.Serialize(strm1, obj);
            strm1.Close();
            // End of SOAP Serialization

           

        }

        public static void SerializeG(object obj, string FileName)
        {

            System.Xml.Serialization.XmlSerializer re = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            TextWriter stream = new StreamWriter(FileName);
            re.Serialize(stream, obj);

            stream.Close();
        }
        
        public static void DeSerializeG(object obj, string FileName)
        {

            //System.Xml.Serialization.XmlSerializer re = new System.Xml.Serialization.XmlSerializer(obj.GetType(), 
            //    new Type[] { typeof(Variables.Variable), typeof(Variables.ModbusRegisterVariable), 
            //        typeof(Variables.ModbusVariable), typeof(Variables.ModbusCoilVariable) });

            //re.UnknownNode += serializer_UnknownNode;
            //re.UnknownAttribute += serializer_UnknownAttribute;
            //FileStream fs = new FileStream(FileName, FileMode.Open);

            //BioScada.Experiment e = (BioScada.Experiment)re.Deserialize(fs);

            //fs.Close();
        }

        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private static void serializer_UnknownAttribute
        (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }

      

    }
}
