using System;
using System.Text;
using System.Threading;
using OnYourWayHome.ApplicationModel;
using OnYourWayHome.ApplicationModel.Composition;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ServiceBus.Parts
{
    public class AzureServiceBus : DisposableObject, IAzureServiceBus, IStartupService
    {
        private ITopicClient _topic;
        private ISubscriptionClient _subscription;
        private IDisposable _poller;
        private readonly IAzureEntityFactory _entityFactory;
        private readonly IAzureServiceBusConfiguration _configuration;
        private readonly SynchronizationContext _context;

        public AzureServiceBus(IAzureServiceBusConfiguration configuration, IAzureEntityFactory entityFactory)
        {
            Requires.NotNull(configuration, "configuration");
            Requires.NotNull(entityFactory, "entityFactory");

            _entityFactory = entityFactory;
            _configuration = configuration;
            _context = SynchronizationContext.Current ?? new SynchronizationContext();  // Capture the context so that we can call back to the UI thread

            Initialize();
        }

        private void Initialize()
        {
            _entityFactory.BeginCreateTopic(_configuration.ServiceNamespace, _configuration.IssuerName, _configuration.IssuerSecret, _configuration.Topic,
                (topicResult) =>
                {
                    // Created the topic, now create the subscription
                    _topic = _entityFactory.EndCreateTopic(topicResult);

                    _entityFactory.BeginCreateSubscription(_topic, _configuration.DeviceId.ToString(), GetSqlFilter(_configuration),
                        (subscriptionResult) =>
                        {
                            // Create the subscription, now start polling
                            _subscription = _entityFactory.EndCreateSubscription(subscriptionResult);
                            _poller = _entityFactory.CreatePoller(_subscription, OnMessageReceived);
                        });
                });
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public void Send(BrokeredMessage message)
        {
            Requires.NotNull(message, "message");

            // TODO: Offline cache for when the Service Bus is down
            // and handle when haven't yet connected
            if (_topic != null)
                _topic.BeginSend(message, Callback);
        }

        private void Callback(IAsyncResult result)
        {
            _topic.EndSend(result);
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnMessageReceived(BrokeredMessage message)
        {   // Called back on a ThreadPool thread

            // TODO: Currently filters do not work for me, so need to filter on the client
            object deviceId;
            if (message.Headers.TryGetValue("X-MS-PROP-" + WellKnownProperty.DeviceId, out deviceId))
            {
                if ((string)deviceId == _configuration.DeviceId.ToString())
                    return;
            }

            // Send back to the UI thread
            _context.Post((_) => OnMessageReceived(new MessageReceivedEventArgs(message)), null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _poller.Dispose();
            }
        }

        private static string GetSqlFilter(IAzureServiceBusConfiguration configuration)
        {
            StringBuilder filter = new StringBuilder();

            // Not any messages sent by this device
            filter.AppendFormat("NOT {0} = '{1}'", WellKnownProperty.DeviceId, configuration.DeviceId);


            return filter.ToString();
        }
    }
}
