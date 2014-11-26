using System;
using System.IO;

namespace OnYourWayHome.ServiceBus.Serialization
{
    public interface IDataContractSerializer
    {
        object ReadObject(Stream stream);

        void WriteObject(Stream stream, object graph);
    }
}
