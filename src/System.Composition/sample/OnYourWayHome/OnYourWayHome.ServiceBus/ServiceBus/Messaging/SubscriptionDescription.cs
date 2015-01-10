//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "SubscriptionDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class SubscriptionDescription
    {
        [DataMember(Name = "LockDuration", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
        public TimeSpan? LockDuration { get; set; }

        [DataMember(Name = "RequiresSession", IsRequired = false, Order = 1003, EmitDefaultValue = false)]
        public bool? RequiresSession { get; set; }

        [DataMember(Name = "DefaultMessageTimeToLive", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
        public TimeSpan? DefaultMessageTimeToLive { get; set; }

        [DataMember(Name = "DeadLetteringOnMessageExpiration", IsRequired = false, Order = 1006, EmitDefaultValue = false)]
        public bool? EnableDeadLetteringOnMessageExpiration { get; set; }

        [DataMember(Name = "DeadLetteringOnFilterEvaluationExceptions", IsRequired = false, Order = 1007, EmitDefaultValue = false)]
        public bool? EnableDeadLetteringOnFilterEvaluationExceptions { get; set; }

        [DataMember(Name = "DefaultRuleDescription", IsRequired = false, Order = 1008, EmitDefaultValue = false)]
        public RuleDescription DefaultRuleDescription { get; set; }

        [DataMember(Name = "MessageCount", IsRequired = false, Order = 1009, EmitDefaultValue = false)]
        public long? MessageCount { get; set; }

        [DataMember(Name = "MaxDeliveryCount", IsRequired = false, Order = 1010, EmitDefaultValue = false)]
        public int? MaxDeliveryCount { get; set; }

        [DataMember(Name = "EnableBatchedOperations", IsRequired = false, Order = 1011, EmitDefaultValue = false)]
        public bool? EnableBatchedOperations { get; set; }

        public string TopicPath { get; internal set; }

        public string Name { get; internal set; }

        public Uri Uri { get; private set; }

        internal void UpdateFromEntity(string topicPath, Entity<SubscriptionDescription> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.TopicPath = topicPath;
            this.Name = entity.Name;
            this.Uri = entity.Uri;
        }
    }
}
