//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System.Runtime.Serialization;

    [DataContract(Name = "TrueFilter", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class TrueFilter : SqlFilter
    {
        public TrueFilter()
            : base("1=1")
        {
        }
    }
}
