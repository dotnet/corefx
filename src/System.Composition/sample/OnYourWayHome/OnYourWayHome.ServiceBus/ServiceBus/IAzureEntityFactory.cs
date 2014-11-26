using System;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ServiceBus
{
    public interface IAzureEntityFactory
    {
        IAsyncResult BeginCreateTopic(string serviceNamespace, string issuer, string issuerKey, string name, AsyncCallback callback);

        ITopicClient EndCreateTopic(IAsyncResult result);

        IAsyncResult BeginCreateSubscription(ITopicClient topic, string name, string sqlFilter, AsyncCallback callback);

        ISubscriptionClient EndCreateSubscription(IAsyncResult result);

        IDisposable CreatePoller(ISubscriptionClient subscription, Action<BrokeredMessage> callback);
    }
}
