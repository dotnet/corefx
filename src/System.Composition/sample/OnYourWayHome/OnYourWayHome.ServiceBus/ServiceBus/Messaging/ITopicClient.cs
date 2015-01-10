using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnYourWayHome.ServiceBus.Messaging
{
    public interface ITopicClient
    {
        IAsyncResult BeginSend(BrokeredMessage message, AsyncCallback callback);

        void EndSend(IAsyncResult result);
    }
}
