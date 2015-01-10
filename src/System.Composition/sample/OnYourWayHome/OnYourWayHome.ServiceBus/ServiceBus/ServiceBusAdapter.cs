using System;
using System.Runtime.Serialization;
using OnYourWayHome.ServiceBus.Serialization;

namespace OnYourWayHome.ServiceBus
{
    public static class ServiceBusAdapter
    {
        public static IServiceBusAdapter Current
        {
            get;
            set;
        }
    }

    public interface IServiceBusAdapter
    {
        IDataContractSerializer CreateJsonSerializer(Type type);

        byte[] ComputeHmacSha256(byte[] secretKey, byte[] data);
    }
}
