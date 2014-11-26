//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System.Runtime.Serialization;

    [DataContract(Name = "RuleAction", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    [KnownType(typeof(EmptyRuleAction))]
    [KnownType(typeof(SqlRuleAction))]
    public abstract class RuleAction
    {
        internal RuleAction()
        {
        }
    }
}