using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace OnYourWayHome.ServiceBus.Serialization
{
    // Contains methods for serializing to and from streams and bytes 
    internal static class DataContractServices
    {
        public static MemoryStream SerializeAsStream(object graph)
        {
            DataContractSerializer serializer = new DataContractSerializer(graph.GetType());

            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, graph);
            stream.Flush();
            stream.Position = 0;

            return stream;
        }

        public static byte[] SerializeAsBytes(object graph)
        {
            using (MemoryStream stream = SerializeAsStream(graph))
            {
                return stream.ToArray();
            }
        }

        public static object DeserializeFromBytes(Type type, byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return DeserializeFromStream(type, stream);
            }
        }

        public static object DeserializeFromStream(Type type, Stream stream)
        {
            DataContractSerializer serializer = new DataContractSerializer(type);

            return serializer.ReadObject(stream);
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            return (T)DeserializeFromStream(typeof(T), stream);
        }
    }
}
