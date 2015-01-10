using System;

namespace OnYourWayHome.ServiceBus.Parts
{
    internal class WindowsStoreAzureServiceBusConfiguration : DefaultAzureServiceBusConfiguration
    {
        public WindowsStoreAzureServiceBusConfiguration()
        {
        }

        public override int DeviceId
        {
            get { return 1; }
        }

        public override string Topic
        {
            get { return "Dave"; }
        }
    }
}
