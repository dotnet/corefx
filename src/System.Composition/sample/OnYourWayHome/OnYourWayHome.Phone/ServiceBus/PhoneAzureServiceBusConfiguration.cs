using System;

namespace OnYourWayHome.ServiceBus
{
    public class PhoneAzureServiceBusConfiguration : DefaultAzureServiceBusConfiguration
    {
        public PhoneAzureServiceBusConfiguration()
        {
        }

        public override int DeviceId
        {
            get { return 2; } 
        }

        public override string Topic
        {
            get { return "MyShoppingList"; }
        }
    }
}
