using System;
using System.Threading;
using System.Threading.Tasks;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ServiceBus
{
    // Polls an Azure ServiceBus subscription on a background thread looking for messages
    internal class SubscriptionClientPoller : CancellationTokenSource, IDisposable
    {
        private readonly ISubscriptionClient _subscription;
        private readonly Action<BrokeredMessage> _callback;

        public SubscriptionClientPoller(ISubscriptionClient subscription, Action<BrokeredMessage> callback)
        {
            Assumes.NotNull(subscription);
            Assumes.NotNull(callback);

            _subscription = subscription;
            _callback = callback;
            Task.Run(() => OnTimeElapsed(null), this.Token);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Cancel();
            }
            base.Dispose(disposing);
        }

        private void Sleep()
        {
            Task.Delay(5000, this.Token).ContinueWith(OnTimeElapsed).Start();
        }

        private void OnTimeElapsed(Task task)
        {   
            // Executed on a ThreadPool thread
            
            BrokeredMessage message;
            while ((message = Receive()) != null)
            {
                _callback(message);
            }

            // No more messages, sleep
            Sleep();
        }

        private BrokeredMessage Receive()
        {
            IAsyncResult result = _subscription.BeginReceive((AsyncCallback)null);

            return _subscription.EndReceive(result);
        }
    }
}
