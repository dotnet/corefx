//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System.Runtime.Serialization;

    [DataContract(Name = "CorrelationFilter", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class CorrelationFilter : Filter
    {
        public CorrelationFilter(string correlationId)
        {
            this.CorrelationId = correlationId;
        }

        [DataMember(Order = 0x10001)]
        public string CorrelationId { get; private set; }
    }
}
