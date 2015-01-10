//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Filter", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    [KnownType(typeof(SqlFilter))]
    [KnownType(typeof(TrueFilter))]
    [KnownType(typeof(FalseFilter))]
    [KnownType(typeof(CorrelationFilter))]
    public abstract class Filter
    {
        internal Filter()
        {
        }
    }
}