using System;
using System.Security.Cryptography;
using OnYourWayHome.ServiceBus.Serialization;

namespace OnYourWayHome.ServiceBus
{
    // Windows 8 implementation
    public class PhoneServiceBusAdapter : ServiceBusAdapter
    {
        public PhoneServiceBusAdapter()
        {
        }

        public override IDataContractSerializer CreateJsonSerializer(Type type)
        {
            return new DataContractJsonSerializerAdapter(type);
        }

        public override byte[] ComputeHmacSha256(byte[] secretKey, byte[] data)
        {
            using (HMACSHA256 cryptoProvider = new HMACSHA256(secretKey))
            {
                return cryptoProvider.ComputeHash(data);
            }
        }
    }
}
