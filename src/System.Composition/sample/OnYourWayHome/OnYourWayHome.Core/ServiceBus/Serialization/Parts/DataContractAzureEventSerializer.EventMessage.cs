using System;
using System.Runtime.Serialization;

namespace OnYourWayHome.ServiceBus.Serialization.Parts
{
    partial class DataContractAzureEventSerializer
    {
        [DataContract]
        public class EventMessage
        {
            [DataMember]
            public string AssemblyQualifiedTypeName;

            [DataMember]
            public byte[] Data;
        }
    }
}
