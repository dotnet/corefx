using System;

namespace OnYourWayHome.ServiceBus
{
    // Provides configuration to setup the Azure Service Bus
    public interface IAzureServiceBusConfiguration
    {
        string Topic
        {
            get;
        }

        int DeviceId
        {
            get;
        }

        string ServiceNamespace
        {
            get;
        }

        string IssuerName
        {
            get;
        }

        string IssuerSecret
        {
            get;
        }
    }
}
