using System;
using System.Net;
using OnYourWayHome.AccessControl;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ServiceBus.Parts
{
    // Creates Azure entities
    public class AzureEntityFactory : IAzureEntityFactory
    {
        public AzureEntityFactory(IServiceBusAdapter adapter)
        {
            Requires.NotNull(adapter, "adapter");

            ServiceBusAdapter.Current = adapter;
        }

        public IAsyncResult BeginCreateTopic(string serviceNamespace, string issuerName, string issuerSecret, string name, AsyncCallback callback)
        {
            Requires.NotNullOrEmpty(serviceNamespace, "serviceNamespace");
            Requires.NotNullOrEmpty(issuerName, "issuerName");
            Requires.NotNullOrEmpty(issuerSecret, "issuerSecret");
            Requires.NotNullOrEmpty(name, "name");

            TokenProvider credentials = TokenProvider.CreateSharedSecretTokenProvider(serviceNamespace, issuerName, issuerSecret);

            Func<TopicClient> creator = () => CreateTopic(credentials, name);

            return TopicClient.BeginCreateTopic(name, credentials, callback, creator);
        }

        public ITopicClient EndCreateTopic(IAsyncResult result)
        {
            Func<TopicClient> creator = (Func<TopicClient>)result.AsyncState;

            return TryCreateEntity(() => TopicClient.EndCreateTopic(result), creator);
        }

        public IAsyncResult BeginCreateSubscription(ITopicClient topic, string name, string sqlFilter, AsyncCallback callback)
        {
            Requires.NotNull(topic, "topic");
            Requires.NotNullOrEmpty(name, "name");

            TopicClient client = (TopicClient)topic;
            SqlFilter filter = new SqlFilter(sqlFilter);

            SubscriptionDescription description = new SubscriptionDescription();

            // TODO: Filters aren't actually working at the moment
            //description.DefaultRuleDescription = new RuleDescription(filter);
            description.DefaultRuleDescription = new RuleDescription();

            Func<SubscriptionClient> creator = () => CreateSubscription(client, name);

            return SubscriptionClient.BeginCreateSubscription(client.Path, name, description, client.TokenProvider, callback, creator);
        }

        public ISubscriptionClient EndCreateSubscription(IAsyncResult result)
        {
            Func<SubscriptionClient> creator = (Func<SubscriptionClient>)result.AsyncState;

            return TryCreateEntity(() => SubscriptionClient.EndCreateSubscription(result), creator);
        }

        public IDisposable CreatePoller(ISubscriptionClient subscription, Action<BrokeredMessage> callback)
        {
            Requires.NotNull(subscription, "subscription");
            Requires.NotNull(callback, "callback");

            return new SubscriptionClientPoller(subscription, callback);
        }

        private T TryCreateEntity<T>(Func<T> createOnServer, Func<T> create)
        {
            try
            {
                // Create the entity before we return it in case this the 
                // first time that we've logged in
                return createOnServer();
            }
            catch (HttpWebException ex)
            {
                // Entity was already created
                if (ex.StatusCode == HttpStatusCode.Conflict)
                    return create();

                throw;
            }
        }

        private static SubscriptionClient CreateSubscription(TopicClient topic, string name)
        {
            return new SubscriptionClient(topic.Path, name, topic.TokenProvider);
        }

        private static TopicClient CreateTopic(TokenProvider credentials, string name)
        {
            return new TopicClient(name, credentials);
        }
    }
}
