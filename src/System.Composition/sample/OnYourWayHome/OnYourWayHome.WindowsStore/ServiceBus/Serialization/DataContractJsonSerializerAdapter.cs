using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace OnYourWayHome.ServiceBus.Serialization
{
    internal class DataContractJsonSerializerAdapter : IDataContractSerializer
    {
        private readonly DataContractJsonSerializer _underlyingSerializer;

        public DataContractJsonSerializerAdapter(Type type)
        {
            _underlyingSerializer = new DataContractJsonSerializer(type);
        }

        public object ReadObject(Stream stream)
        {
            return _underlyingSerializer.ReadObject(stream);
        }

        public void WriteObject(Stream stream, object graph)
        {
            _underlyingSerializer.WriteObject(stream, graph);
        }
    }
}
