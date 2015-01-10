using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnYourWayHome.ServiceBus.Messaging
{
    public interface ISubscriptionClient
    {
        IAsyncResult BeginReceive(AsyncCallback callback);
        BrokeredMessage EndReceive(IAsyncResult result);
    }
}
