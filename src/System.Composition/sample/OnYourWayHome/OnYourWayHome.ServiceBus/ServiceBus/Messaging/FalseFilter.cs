//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System.Runtime.Serialization;

    [DataContract(Name = "FalseFilter", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class FalseFilter : SqlFilter
    {
        public FalseFilter()
            : base("1=0")
        {
        }
    }
}
